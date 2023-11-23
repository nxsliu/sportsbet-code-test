using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using System.Net;
using Xunit;
using System.Text;
using MediatR;
using RacingFeedApi.Events;
using RacingFeedApi.IntegrationTests.Fakes;
using Microsoft.Extensions.DependencyInjection;
using RacingFeedApi.Repositories;

namespace RacingFeedApi.IntegrationTests;

public class RaceTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    
    public RaceTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Given_ValidCreateRequest_UpdateRace_ReturnsSuccess()
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
        var xmlString = "<RaceUpdate xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\"><MeetingID>219120</MeetingID><RaceId>1123</RaceId><RaceLocation>Caulfield</RaceLocation><RaceDistance>2400</RaceDistance><RaceNo>5</RaceNo><RaceType>Metropolitan</RaceType><RaceInfo>Carlton Draught Caulfield Cup</RaceInfo><TrackCondition>Good(4)</TrackCondition><Source>RacingServicesProvider</Source><PriceType>Win</PriceType><PoolSize>227</PoolSize><StartTime>1697832900</StartTime><CreationTime>1697656815</CreationTime><Runners><Runner Id=\"101\" TabNo=\"1\" Barrier=\"8\" Name=\"BORN A KING\" Price=\"3.4\" Jockey=\"Ben Melham\" Trainer=\"C Maher and D Eustace\"/><Runner Id=\"104\" TabNo=\"2\" Barrier=\"4\" Name=\"AUSTRATA\" Price=\"13.3\" Jockey=\"Mark Zahra\" Trainer=\"A and S Freedman\"/><Runner Id=\"167\" TabNo=\"3\" Barrier=\"5\" Name=\"ARAPAHO\" Price=\"5.7\" Jockey=\"Damian Lane\" Trainer=\"T Yoshioka\"/><Runner Id=\"256\" TabNo=\"4\" Barrier=\"6\" Name=\"OCEANIC FLASH\" Price=\"77.6\" Jockey=\"Blake Shinn\" Trainer=\"C J Waller\"/><Runner Id=\"176\" TabNo=\"5\" Barrier=\"3\" Name=\"TINNIE WINNIE\" Price=\"13.8\" Jockey=\"James Mcdonald\" Trainer=\"C J Waller\"/><Runner Id=\"146\" TabNo=\"6\" Barrier=\"1\" Name=\"DICK WHITTINGTON\" Price=\"29.8\" Jockey=\"Jamie Spencer\" Trainer=\"Simon and E Crisford\"/><Runner Id=\"197\" TabNo=\"7\" Barrier=\"2\" Name=\"KING OF CLUBS\" Price=\"7.7\" Jockey=\"Harry Coffey\" Trainer=\"G M Begg\"/><Runner Id=\"567\" TabNo=\"8\" Barrier=\"7\" Name=\"COLOUR SERGEANT\" Price=\"4\" Jockey=\"Craig Williams\" Trainer=\"C J Waller\"/></Runners></RaceUpdate>";
        var stringContent = new StringContent(xmlString, Encoding.UTF8, "application/xml");
        var response = await client.PostAsync("api/race", stringContent);

        // assert
        response.EnsureSuccessStatusCode();
        Assert.Equal(1123, FakeDataStore.Races[1123].RaceId);
        var jsonString = "{\"RaceId\":1123,\"RaceLocation\":\"Caulfield\",\"Distance\":2400,\"RaceNumber\":5,\"RaceType\":\"Metropolitan\",\"RaceInfo\":\"Carlton Draught Caulfield Cup\",\"TrackCondition\":\"Good(4)\",\"StartTimeUtc\":\"2023-10-20T20:15:00Z\",\"Runners\":[{\"Id\":101,\"Number\":1,\"Barrier\":8,\"Name\":\"BORN A KING\",\"WinPrice\":3.4,\"Jockey\":\"Ben Melham\",\"Trainer\":\"C Maher and D Eustace\"},{\"Id\":104,\"Number\":2,\"Barrier\":4,\"Name\":\"AUSTRATA\",\"WinPrice\":13.3,\"Jockey\":\"Mark Zahra\",\"Trainer\":\"A and S Freedman\"},{\"Id\":167,\"Number\":3,\"Barrier\":5,\"Name\":\"ARAPAHO\",\"WinPrice\":5.7,\"Jockey\":\"Damian Lane\",\"Trainer\":\"T Yoshioka\"},{\"Id\":256,\"Number\":4,\"Barrier\":6,\"Name\":\"OCEANIC FLASH\",\"WinPrice\":77.6,\"Jockey\":\"Blake Shinn\",\"Trainer\":\"C J Waller\"},{\"Id\":176,\"Number\":5,\"Barrier\":3,\"Name\":\"TINNIE WINNIE\",\"WinPrice\":13.8,\"Jockey\":\"James Mcdonald\",\"Trainer\":\"C J Waller\"},{\"Id\":146,\"Number\":6,\"Barrier\":1,\"Name\":\"DICK WHITTINGTON\",\"WinPrice\":29.8,\"Jockey\":\"Jamie Spencer\",\"Trainer\":\"Simon and E Crisford\"},{\"Id\":197,\"Number\":7,\"Barrier\":2,\"Name\":\"KING OF CLUBS\",\"WinPrice\":7.7,\"Jockey\":\"Harry Coffey\",\"Trainer\":\"G M Begg\"},{\"Id\":567,\"Number\":8,\"Barrier\":7,\"Name\":\"COLOUR SERGEANT\",\"WinPrice\":4,\"Jockey\":\"Craig Williams\",\"Trainer\":\"C J Waller\"}]}";
        Assert.Equal(jsonString, FakeDataStore.Races[1123].RaceDetails);
        Assert.Single(MessagesHandledHelper.RaceCreatedMessagedHandled);
        Assert.Equal(jsonString, MessagesHandledHelper.RaceCreatedMessagedHandled.First().Message);
    }

    [Fact]
    public async Task Given_ValidUpdateRequest_UpdateRace_ReturnsSuccess()
    {
        // arrange
        var updatedUtc = DateTime.UtcNow.AddHours(-1);
        var existingRace = new Repositories.Entities.Race
        {
            RaceId = 1111,
            RaceDetails = "{\"RaceId\":1111,\"RaceLocation\":\"Caulfield\",\"Distance\":2400,\"RaceNumber\":5,\"RaceType\":\"Metropolitan\",\"RaceInfo\":\"Carlton Draught Caulfield Cup\",\"TrackCondition\":\"Good(4)\",\"StartTimeUtc\":\"2023-10-20T20:15:00Z\",\"Runners\":[{\"Id\":101,\"Number\":1,\"Barrier\":8,\"Name\":\"BORN A KING\",\"WinPrice\":3.4,\"Jockey\":\"Ben Melham\",\"Trainer\":\"C Maher and D Eustace\"},{\"Id\":104,\"Number\":2,\"Barrier\":4,\"Name\":\"AUSTRATA\",\"WinPrice\":13.3,\"Jockey\":\"Mark Zahra\",\"Trainer\":\"A and S Freedman\"},{\"Id\":167,\"Number\":3,\"Barrier\":5,\"Name\":\"ARAPAHO\",\"WinPrice\":5.7,\"Jockey\":\"Damian Lane\",\"Trainer\":\"T Yoshioka\"},{\"Id\":256,\"Number\":4,\"Barrier\":6,\"Name\":\"OCEANIC FLASH\",\"WinPrice\":77.6,\"Jockey\":\"Blake Shinn\",\"Trainer\":\"C J Waller\"},{\"Id\":176,\"Number\":5,\"Barrier\":3,\"Name\":\"TINNIE WINNIE\",\"WinPrice\":13.8,\"Jockey\":\"James Mcdonald\",\"Trainer\":\"C J Waller\"},{\"Id\":146,\"Number\":6,\"Barrier\":1,\"Name\":\"DICK WHITTINGTON\",\"WinPrice\":29.8,\"Jockey\":\"Jamie Spencer\",\"Trainer\":\"Simon and E Crisford\"},{\"Id\":197,\"Number\":7,\"Barrier\":2,\"Name\":\"KING OF CLUBS\",\"WinPrice\":7.7,\"Jockey\":\"Harry Coffey\",\"Trainer\":\"G M Begg\"},{\"Id\":567,\"Number\":8,\"Barrier\":7,\"Name\":\"COLOUR SERGEANT\",\"WinPrice\":4,\"Jockey\":\"Craig Williams\",\"Trainer\":\"C J Waller\"}]}",
            UpdatedUtc = updatedUtc
        };
        FakeDataStore.Races.Add(existingRace.RaceId, existingRace);

        var client = _factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureTestServices(services =>
            {
                services.AddScoped<INotificationHandler<RaceUpdated>, FakeRaceUpdatedHandler>();
            });
        }).CreateClient();

        // act
        var xmlString = "<RaceUpdate xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\"><MeetingID>219120</MeetingID><RaceId>1111</RaceId><RaceLocation>Malvern</RaceLocation><RaceDistance>5000</RaceDistance><RaceNo>10</RaceNo><RaceType>Country</RaceType><RaceInfo>Mushroom Cup</RaceInfo><TrackCondition>Bad(8)</TrackCondition><Source>RacingServicesProvider</Source><PriceType>Win</PriceType><PoolSize>227</PoolSize><StartTime>1697832900</StartTime><CreationTime>1697656815</CreationTime><Runners><Runner Id=\"222\" TabNo=\"5\" Barrier=\"12\" Name=\"FRANCE\" Price=\"5.5\" Jockey=\"Mark Andersen\" Trainer=\"L J Hoooker\"/></Runners></RaceUpdate>";
        var stringContent = new StringContent(xmlString, Encoding.UTF8, "application/xml");
        var response = await client.PostAsync("api/race", stringContent);

        // assert
        response.EnsureSuccessStatusCode();
        Assert.Equal(1111, FakeDataStore.Races[1111].RaceId);
        var jsonString = "{\"RaceId\":1111,\"RaceLocation\":\"Malvern\",\"Distance\":5000,\"RaceNumber\":10,\"RaceType\":\"Country\",\"RaceInfo\":\"Mushroom Cup\",\"TrackCondition\":\"Bad(8)\",\"StartTimeUtc\":\"2023-10-20T20:15:00Z\",\"Runners\":[{\"Id\":222,\"Number\":5,\"Barrier\":12,\"Name\":\"FRANCE\",\"WinPrice\":5.5,\"Jockey\":\"Mark Andersen\",\"Trainer\":\"L J Hoooker\"}]}";
        Assert.Equal(jsonString, FakeDataStore.Races[1111].RaceDetails);
        Assert.NotEqual(updatedUtc, FakeDataStore.Races[1111].UpdatedUtc);
        Assert.Single(MessagesHandledHelper.RaceUpdatedMessagedHandled);
        Assert.Equal(jsonString, MessagesHandledHelper.RaceUpdatedMessagedHandled.First().Message);
    }
}