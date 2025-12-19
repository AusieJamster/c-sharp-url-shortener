using UrlShortener.Models;
using UrlShortener.Repositories;

namespace UrlShortener.Services;

public class UrlShortenerService
{
  private readonly IUrlRepository _repository;

  public UrlShortenerService(IUrlRepository repository)
  {
    _repository = repository;
  }

  public async Task<UrlMapping> ShortenUrlAsync(string originalUrl)
  {
    string shortCode = Guid.NewGuid().ToString().Substring(0, 8);
    UrlMapping mapping = new UrlMapping(shortCode, originalUrl);

    await _repository.SaveUrlMappingAsync(mapping);

    return mapping;
  }

  public async Task<string?> GetOriginalUrlAsync(string shortCode)
  {
    return await _repository.GetOriginalUrlAsync(shortCode);
  }
}