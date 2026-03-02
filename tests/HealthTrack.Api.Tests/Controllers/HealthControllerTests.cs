using System.Net;
using System.Text.Json;
using FluentAssertions;

namespace HealthTrack.Api.Tests.Controllers;

public class HealthControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public HealthControllerTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetHealth_ShouldReturnOk()
    {
        // Act
        var response = await _client.GetAsync("/api/v1/health");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetHealth_ShouldReturnHealthyStatus()
    {
        // Act
        var response = await _client.GetAsync("/api/v1/health");
        var content = await response.Content.ReadAsStringAsync();
        var json = JsonDocument.Parse(content);

        // Assert
        json.RootElement.GetProperty("status").GetString().Should().Be("Healthy");
    }

    [Fact]
    public async Task GetHealth_ShouldReturnServiceName()
    {
        // Act
        var response = await _client.GetAsync("/api/v1/health");
        var content = await response.Content.ReadAsStringAsync();
        var json = JsonDocument.Parse(content);

        // Assert
        json.RootElement.GetProperty("service").GetString().Should().Be("HealthTrack API");
    }

    [Fact]
    public async Task GetHealth_ShouldReturnVersion()
    {
        // Act
        var response = await _client.GetAsync("/api/v1/health");
        var content = await response.Content.ReadAsStringAsync();
        var json = JsonDocument.Parse(content);

        // Assert
        json.RootElement.GetProperty("version").GetString().Should().Be("1.0.0");
    }

    [Fact]
    public async Task GetHealth_ShouldReturnTimestamp()
    {
        // Act
        var response = await _client.GetAsync("/api/v1/health");
        var content = await response.Content.ReadAsStringAsync();
        var json = JsonDocument.Parse(content);

        // Assert
        json.RootElement.TryGetProperty("timestamp", out _).Should().BeTrue();
    }
}
