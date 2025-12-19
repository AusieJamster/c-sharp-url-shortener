using NSubstitute;
using UrlShortener.Models;
using UrlShortener.Repositories;
using UrlShortener.Services;

namespace UrlShortener.Tests;

public class UrlShortenerServiceTests
{
    [Fact]
    public async Task ShortenUrlAsync_ShouldSaveToRepository_AndReturnMapping()
    {
        // Arrange
        IUrlRepository mockRepo = Substitute.For<IUrlRepository>();
        UrlShortenerService service = new UrlShortenerService(mockRepo);
        string originalUrl = "https://example.com";

        // Act
        UrlMapping result = await service.ShortenUrlAsync(originalUrl);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(originalUrl, result.OriginalUrl);
        Assert.Equal(8, result.ShortCode.Length);

        // Verify Repository was called
        await mockRepo.Received(1).SaveUrlMappingAsync(Arg.Is<UrlMapping>(x => 
            x.OriginalUrl == originalUrl && 
            x.ShortCode == result.ShortCode
        ));
    }

    [Fact]
    public async Task GetOriginalUrlAsync_ShouldReturnUrl_WhenExists()
    {
        // Arrange
        IUrlRepository mockRepo = Substitute.For<IUrlRepository>();
        string shortCode = "12345678";
        string expectedUrl = "https://example.com";

        mockRepo.GetOriginalUrlAsync(shortCode).Returns(expectedUrl);

        UrlShortenerService service = new UrlShortenerService(mockRepo);

        // Act
        string? result = await service.GetOriginalUrlAsync(shortCode);

        // Assert
        Assert.Equal(expectedUrl, result);
    }
}