using EuroMotors.Infrastructure.Sitemap;
using Microsoft.AspNetCore.Mvc;

namespace EuroMotors.Api.Controllers.Sitemap;

[ApiController]
[Route("sitemap.xml")]
public class SitemapController : ControllerBase
{
    private readonly SitemapService _sitemapService;

    public SitemapController(SitemapService sitemapService)
    {
        _sitemapService = sitemapService;
    }

    [HttpGet]
    public async Task<ContentResult> GetSitemap()
    {
        string sitemap = await _sitemapService.GenerateSitemapAsync();
        return Content(sitemap, "application/xml");
    }
}
