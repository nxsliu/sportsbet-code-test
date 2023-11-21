using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using System.Net;
using Xunit;
using System.Text;
using MediatR;
using RacingFeedApi.Events;
using RacingFeedApi.IntegrationTests.Fakes;
using Microsoft.Extensions.DependencyInjection;

namespace RacingFeedApi.IntegrationTests;

public class RaceTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    
    public RaceTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task GivenValidRequestCreateRaceReturnsSuccess()
    {
        // arrange
        var client = _factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureTestServices(services =>
            {
                services.AddScoped<INotificationHandler<RaceCreated>, FakeRaceCreatedHandler>();
            });
        }).CreateClient();

        // act
        var stringContent = new StringContent("<RaceCreate><RaceId>1123</RaceId></RaceCreate>", Encoding.UTF8, "application/xml");
        var response = await client.PostAsync("api/race", stringContent);

        // assert
        response.EnsureSuccessStatusCode();
    }
}