using Microsoft.AspNetCore.Mvc;
using UrlShortener.Models;
using UrlShortener.Services;

namespace UrlShortener.Controllers;

[ApiController]
[Route("api")]
public class UrlShortenerController : ControllerBase
{
  private readonly UrlShortenerService _service;

  public UrlShortenerController(UrlShortenerService service)
  {
    _service = service;
  }

  [HttpPost("shorten")]
  public async Task<IActionResult> Shorten([FromBody] ShortenUrlRequest request)
  {
    if (string.IsNullOrWhiteSpace(request.Url) || !Uri.TryCreate(request.Url, UriKind.Absolute, out _))
    {
      return BadRequest("Invalid URL provided.");
    }

    UrlMapping mapping = await _service.ShortenUrlAsync(request.Url);
    return Ok(mapping);
  }

  [HttpGet("{shortCode}")]
  public async Task<IActionResult> RedirectToUrl(string shortCode)
  {
    string? originalUrl = await _service.GetOriginalUrlAsync(shortCode);

    if (originalUrl == null)
    {
      return NotFound();
    }

    return Redirect(originalUrl);
  }
}