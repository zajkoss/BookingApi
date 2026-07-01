using System.Net;
using System.Net.Http.Json;
using BookingApi.Commands;
using FluentAssertions;

namespace BookingApi.Tests.IntegrationTests;

public class ResourceControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public ResourceControllerTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task CreateResource_WithValidData_ReturnsCreated()
    {
        var command = new CreateResourceCommand("Room A", "Conference room", 10);

        var response = await _client.PostAsJsonAsync("/api/v1/resources", command);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }
}