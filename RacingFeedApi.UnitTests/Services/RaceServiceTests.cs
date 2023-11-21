using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using RacingFeedApi.Exceptions;
using RacingFeedApi.ViewModels;
using RacingFeedApi.Providers;
using RacingFeedApi.Repositories;
using RacingFeedApi.Services;
using RacingFeedApi.Events;
using NSubstitute.ExceptionExtensions;

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
            r.RaceDetails == "{\"RaceId\":1123,\"RaceLocation\":\"Caulfield\",\"Distance\":2400,\"RaceNumber\":5,\"RaceType\":\"Metropolitan\",\"RaceInfo\":\"Carlton Draught Caulfield Cup\",\"TrackCondition\":\"Good(4)\",\"StartTimeUtc\":\"2023-10-20T20:15:00Z\",\"Runners\":[{\"Id\":101,\"Number\":1,\"Barrier\":8,\"Name\":\"BORN A KING\",\"WinPrice\":3.4,\"Jockey\":\"Ben Melham\",\"Trainer\":\"C Maher and D Eustace\"},{\"Id\":167,\"Number\":3,\"Barrier\":5,\"Name\":\"ARAPAHO\",\"WinPrice\":5.7,\"Jockey\":\"Damian Lane\",\"Trainer\":\"T Yoshioka\"}]}"));

            await mockMessagingProvider.Received(1).PublishEvent(Arg.Is<RaceCreated>(r =>
            r.Message == "{\"RaceId\":1123,\"RaceLocation\":\"Caulfield\",\"Distance\":2400,\"RaceNumber\":5,\"RaceType\":\"Metropolitan\",\"RaceInfo\":\"Carlton Draught Caulfield Cup\",\"TrackCondition\":\"Good(4)\",\"StartTimeUtc\":\"2023-10-20T20:15:00Z\",\"Runners\":[{\"Id\":101,\"Number\":1,\"Barrier\":8,\"Name\":\"BORN A KING\",\"WinPrice\":3.4,\"Jockey\":\"Ben Melham\",\"Trainer\":\"C Maher and D Eustace\"},{\"Id\":167,\"Number\":3,\"Barrier\":5,\"Name\":\"ARAPAHO\",\"WinPrice\":5.7,\"Jockey\":\"Damian Lane\",\"Trainer\":\"T Yoshioka\"}]}"));
        });
    }

    [Fact]
    public async Task GivenCaughtExceptionCreateRaceThrowsError()
    {
        // arrange
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
            CreationTime = 1697656815
        };

        // act
        async Task act() => await raceService.CreateRace(raceCreate);

        // assert
        var createResourceException = await Assert.ThrowsAsync<CreateResourceException>(act);
        Assert.Equal("An exception occurred while creating this race", createResourceException.Message);
    }
}