using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using RacingFeedApi.Exceptions;
using RacingFeedApi.Models;
using RacingFeedApi.Providers;
using RacingFeedApi.Repositories;
using RacingFeedApi.Services;

namespace RacingFeedApi.UnitTests;

public class RaceServiceTests
{
    IMessagingProvider mockMessagingProvider = Substitute.For<IMessagingProvider>();
    IRaceRepository mockRaceRepository = Substitute.For<IRaceRepository>();
    ILogger<RaceService> mockLogger = Substitute.For<ILogger<RaceService>>();
    RaceService raceService;

    public RaceServiceTests()
    {
        mockMessagingProvider = Substitute.For<IMessagingProvider>();
        mockRaceRepository = Substitute.For<IRaceRepository>();
        mockLogger = Substitute.For<ILogger<RaceService>>();
        raceService = new RaceService(mockMessagingProvider, mockRaceRepository, mockLogger);
    }

    [Fact]
    public async Task GivenDuplicateRaceIdCreateRaceThrowsError()
    {
        // arrange
        mockRaceRepository.GetRace(Arg.Any<long>()).Returns(Task.FromResult(new Repositories.Entities.Race{RaceId = 1}));
        
        var raceCreate = new RaceCreate
        {
            RaceId = 1
        };

        // act
        async Task act() => await raceService.CreateRace(raceCreate);

        // assert
        var createResourceException = await Assert.ThrowsAsync<CreateResourceException>(act);
        Assert.Equal("RaceId already exists", createResourceException.Message);
    }

    [Fact]
    public async Task GivenValidRaceCreateRaceReturnsSuccess()
    {
        // arrage
        mockRaceRepository.GetRace(Arg.Any<long>()).ReturnsNull();

        var raceCreate = new RaceCreate
        {
            MeetingId = 219120,
            RaceId = 1123,
            RaceLocation = "Caulfield",
            RaceDistance = 2400,
            RaceNo = 5,
            RaceType = "Metropolitan",
            RaceInfo = "Carlton Draught Caulfield Cup",
            TrackCondition = "Good(4)",
            Source = "RacingServicesProvider",
            PriceType = "Win",
            PoolSize = 227,
            StartTime = 1697832900,
            CreationTime = 1697656815,
            Runners = new List<Runner> 
            {
                new Runner 
                {
                    Id = 101,
                    TabNo = 1,
                    Barrier = 8,
                    Name = "BORN A KING",
                    Price = 3.4m,
                    Jockey = "Ben Melham",
                    Trainer = "C Maher and D Eustace"
                },
                new Runner 
                {
                    Id = 167,
                    TabNo = 3,
                    Barrier = 5,
                    Name = "ARAPAHO",
                    Price = 5.7m,
                    Jockey = "Damian Lane",
                    Trainer = "T Yoshioka"
                }
            }
        };

        // act
        await raceService.CreateRace(raceCreate);

        // assert
        Received.InOrder(async () =>
        {
            await mockRaceRepository.Received(1).InsertRace(Arg.Is<Repositories.Entities.Race>(r => 
            r.RaceId == 1123 && 
            r.RaceLocation == "Caulfield" &&
            r.Distance == 2400 &&
            r.RaceNumber == 5 &&
            r.RaceType == "Metropolitan" &&
            r.RaceInfo == "Carlton Draught Caulfield Cup" &&
            r.TrackCondition == "Good(4)" &&
            r.StartTimeUtc.ToString() == "20/10/2023 8:15:00 pm" &&
            r.Runners.Count() == 2 &&
            r.Runners[0].Id == 101 &&
            r.Runners[0].Number == 1 &&
            r.Runners[0].Barrier == 8 &&
            r.Runners[0].Name == "BORN A KING" &&
            r.Runners[0].WinPrice == 3.4m &&
            r.Runners[0].Jockey == "Ben Melham" &&
            r.Runners[0].Trainer == "C Maher and D Eustace" &&
            r.Runners[1].Id == 167 &&
            r.Runners[1].Number == 3 &&
            r.Runners[1].Barrier == 5 &&
            r.Runners[1].Name == "ARAPAHO" &&
            r.Runners[1].WinPrice == 5.7m &&
            r.Runners[1].Jockey == "Damian Lane" &&
            r.Runners[1].Trainer == "T Yoshioka"));
        });
    }
}