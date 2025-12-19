using UrlShortener.Models;

namespace UrlShortener.Repositories;

public interface IUrlRepository
{
  Task SaveUrlMappingAsync(UrlMapping mapping);
  Task<string?> GetOriginalUrlAsync(string shortCode);
}
