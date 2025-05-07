using System.Xml.Linq;
using EuroMotors.Domain.Products;
using EuroMotors.Domain.Categories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace EuroMotors.Api.Services;

public class SitemapService
{
    private readonly IProductRepository _productRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IDistributedCache _cache;
    private readonly string _baseUrl;
    private const string CacheKey = "sitemap_xml";
    private static readonly TimeSpan CacheDuration = TimeSpan.FromHours(1);

    public SitemapService(
        IProductRepository productRepository,
        ICategoryRepository categoryRepository,
        IDistributedCache cache,
        IConfiguration configuration)
    {
        _productRepository = productRepository;
        _categoryRepository = categoryRepository;
        _cache = cache;
        _baseUrl = configuration["BaseUrl"] ?? "https://euromotors.ua";
    }

    public async Task<string> GenerateSitemapAsync()
    {
        string? cachedSitemap = await _cache.GetStringAsync(CacheKey);
        if (!string.IsNullOrEmpty(cachedSitemap))
        {
            return cachedSitemap;
        }

        var sitemap = new XDocument(
            new XDeclaration("1.0", "utf-8", null),
            new XElement(XName.Get("urlset", "http://www.sitemaps.org/schemas/sitemap/0.9")));

        AddUrl(sitemap.Root!, _baseUrl, changefreq: "daily", priority: "1.0");

        List<Category> categories = await _categoryRepository.GetAll().ToListAsync();
        foreach (Category category in categories)
        {
            AddUrl(
                sitemap.Root!,
                $"{_baseUrl}/category/{category.Id}",
                changefreq: "weekly",
                priority: "0.8"
            );
        }

        List<Product> products = await _productRepository.GetAll().ToListAsync();
        foreach (Product product in products)
        {
            AddUrl(
                sitemap.Root!,
                $"{_baseUrl}/product/{product.Id}",
                changefreq: "weekly",
                priority: "0.6"
            );
        }

        string sitemapXml = sitemap.ToString();

        await _cache.SetStringAsync(
            CacheKey,
            sitemapXml,
            new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = CacheDuration
            });

        return sitemapXml;
    }

    private void AddUrl(XElement root, string url, string changefreq = "monthly", string priority = "0.5")
    {
        root.Add(
            new XElement("url",
                new XElement("loc", url),
#pragma warning disable CA1305
                new XElement("lastmod", DateTime.UtcNow.ToString("yyyy-MM-dd")),
#pragma warning restore CA1305
                new XElement("changefreq", changefreq),
                new XElement("priority", priority)
            )
        );
    }
} 