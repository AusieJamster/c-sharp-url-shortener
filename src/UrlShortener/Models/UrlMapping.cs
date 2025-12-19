namespace UrlShortener.Models;

public record UrlMapping(string ShortCode, string OriginalUrl);

public record ShortenUrlRequest(string Url);