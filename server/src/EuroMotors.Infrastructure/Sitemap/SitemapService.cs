using System.Xml.Linq;
using EuroMotors.Domain.Categories;
using EuroMotors.Domain.Products;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace EuroMotors.Infrastructure.Sitemap;

public class SitemapService
{
    private readonly IProductRepository _productRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly string _baseUrl;
    private readonly string _namespaceUri;

    public SitemapService(
        IProductRepository productRepository,
        ICategoryRepository categoryRepository,
        IOptions<AppSettings> appSettings,
        IOptions<SitemapOptions> sitemapOptions)
    {
        _productRepository = productRepository;
        _categoryRepository = categoryRepository;
        _baseUrl = appSettings.Value.BaseUrl;
        _namespaceUri = sitemapOptions.Value.NamespaceUri;
    }

    public async Task<string> GenerateSitemapAsync()
    {
        var sitemap = new XDocument(
            new XDeclaration("1.0", "utf-8", null),
            new XElement(XName.Get("urlset", _namespaceUri)));

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

        return sitemap.ToString();
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
