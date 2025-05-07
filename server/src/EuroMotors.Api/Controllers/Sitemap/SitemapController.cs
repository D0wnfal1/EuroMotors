using EuroMotors.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace EuroMotors.Api.Controllers.Sitemap;

[ApiController]
[Route("[controller]")]
public class SitemapController : ControllerBase
{
    private readonly SitemapService _sitemapService;

    public SitemapController(SitemapService sitemapService)
    {
        _sitemapService = sitemapService;
    }

    [HttpGet("sitemap.xml")]
    public async Task<ContentResult> GetSitemap()
    {
        var sitemap = await _sitemapService.GenerateSitemapAsync();
        return Content(sitemap, "application/xml");
    }
}