using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using RacingFeedApi.Exceptions;
using RacingFeedApi.ViewModels;
using RacingFeedApi.Providers;
using RacingFeedApi.Repositories;
using RacingFeedApi.Services;
using RacingFeedApi.Events;

namespace RacingFeedApi.UnitTests.Services;

public class RaceServiceTests
{
    private readonly IMessagingProvider mockMessagingProvider = Substitute.For<IMessagingProvider>();
    private readonly IRaceRepository mockRaceRepository = Substitute.For<IRaceRepository>();
    private readonly ILogger<RaceService> mockLogger = Substitute.For<ILogger<RaceService>>();
    private readonly RaceService raceService;

    public RaceServiceTests()
    {
        mockMessagingProvider = Substitute.For<IMessagingProvider>();
        mockRaceRepository = Substitute.For<IRaceRepository>();
        mockLogger = Substitute.For<ILogger<RaceService>>();
        raceService = new RaceService(mockMessagingProvider, mockRaceRepository, mockLogger);
    }

    [Fact]
    public async Task Given_DuplicateRaceId_CreateRace_ThrowsError()
    {
        // arrange
        mockRaceRepository.GetRace(Arg.Any<long>()).Returns(Task.FromResult(new Repositories.Entities.Race { ExternalRaceId = 1 }));

        var raceCreate = new RaceUpdate
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
    public async Task Given_ValidRace_CreateRace_IsSuccessful()
    {
        // arrage
        mockRaceRepository.GetRace(Arg.Any<long>()).ReturnsNull();
        mockRaceRepository.GetMaxInternalRaceId().Returns(Task.FromResult<long>(1));

        var raceCreate = new RaceUpdate
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
            r.ExternalRaceId == 1123 &&
            r.InternalRaceId == 2 &&
            r.RaceDetails == "{\"RaceId\":2,\"RaceLocation\":\"Caulfield\",\"Distance\":2400,\"RaceNumber\":5,\"RaceType\":\"Metropolitan\",\"RaceInfo\":\"Carlton Draught Caulfield Cup\",\"TrackCondition\":\"Good(4)\",\"StartTimeUtc\":\"2023-10-20T20:15:00Z\",\"Runners\":[{\"Id\":101,\"Number\":1,\"Barrier\":8,\"Name\":\"BORN A KING\",\"WinPrice\":3.4,\"Jockey\":\"Ben Melham\",\"Trainer\":\"C Maher and D Eustace\"},{\"Id\":167,\"Number\":3,\"Barrier\":5,\"Name\":\"ARAPAHO\",\"WinPrice\":5.7,\"Jockey\":\"Damian Lane\",\"Trainer\":\"T Yoshioka\"}]}"));

            await mockMessagingProvider.Received(1).PublishEvent(Arg.Is<RaceCreated>(r =>
            r.Message == "{\"RaceId\":2,\"RaceLocation\":\"Caulfield\",\"Distance\":2400,\"RaceNumber\":5,\"RaceType\":\"Metropolitan\",\"RaceInfo\":\"Carlton Draught Caulfield Cup\",\"TrackCondition\":\"Good(4)\",\"StartTimeUtc\":\"2023-10-20T20:15:00Z\",\"Runners\":[{\"Id\":101,\"Number\":1,\"Barrier\":8,\"Name\":\"BORN A KING\",\"WinPrice\":3.4,\"Jockey\":\"Ben Melham\",\"Trainer\":\"C Maher and D Eustace\"},{\"Id\":167,\"Number\":3,\"Barrier\":5,\"Name\":\"ARAPAHO\",\"WinPrice\":5.7,\"Jockey\":\"Damian Lane\",\"Trainer\":\"T Yoshioka\"}]}"));
        });
    }

    [Fact]
    public async Task Given_CaughtException_CreateRace_ThrowsError()
    {
        // arrange
        mockRaceRepository.GetRace(Arg.Any<long>()).ReturnsNull();

        var raceCreate = new RaceUpdate
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
        Assert.Equal("An exception occurred while creating race 1123", createResourceException.Message);
    }

    [Fact]
    public async Task Given_InvalidRaceId_UpdateRace_ThrowsError()
    {
        // arrange
        mockRaceRepository.GetRace(Arg.Any<long>()).ReturnsNull();

        var raceUpdate = new RaceUpdate
        {
            RaceId = 1
        };

        // act
        async Task act() => await raceService.UpdateRace(raceUpdate);

        // assert
        var createResourceException = await Assert.ThrowsAsync<ResourceNotFoundException>(act);
        Assert.Equal("RaceId not found", createResourceException.Message);
    }

    [Fact]
    public async Task Given_ValidRace_UpdateRace_IsSuccessful()
    {
        // arrage
        var updateUtc = DateTime.UtcNow;
        mockRaceRepository.GetRace(Arg.Any<long>()).Returns(
            Task.FromResult(new Repositories.Entities.Race { ExternalRaceId = 1123, InternalRaceId = 5, UpdatedUtc = updateUtc }),
            Task.FromResult(new Repositories.Entities.Race { ExternalRaceId = 1123, InternalRaceId = 5, UpdatedUtc = updateUtc })
            );

        var raceUpdate = new RaceUpdate
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
        await raceService.UpdateRace(raceUpdate);

        // assert
        Received.InOrder(async () =>
        {
            await mockRaceRepository.Received(1).UpdateRace(Arg.Is<Repositories.Entities.Race>(r =>
            r.ExternalRaceId == 1123 &&
            r.InternalRaceId == 5 &&
            r.RaceDetails == "{\"RaceId\":5,\"RaceLocation\":\"Caulfield\",\"Distance\":2400,\"RaceNumber\":5,\"RaceType\":\"Metropolitan\",\"RaceInfo\":\"Carlton Draught Caulfield Cup\",\"TrackCondition\":\"Good(4)\",\"StartTimeUtc\":\"2023-10-20T20:15:00Z\",\"Runners\":[{\"Id\":101,\"Number\":1,\"Barrier\":8,\"Name\":\"BORN A KING\",\"WinPrice\":3.4,\"Jockey\":\"Ben Melham\",\"Trainer\":\"C Maher and D Eustace\"},{\"Id\":167,\"Number\":3,\"Barrier\":5,\"Name\":\"ARAPAHO\",\"WinPrice\":5.7,\"Jockey\":\"Damian Lane\",\"Trainer\":\"T Yoshioka\"}]}"));

            await mockMessagingProvider.Received(1).PublishEvent(Arg.Is<RaceUpdated>(r =>
            r.Message == "{\"RaceId\":5,\"RaceLocation\":\"Caulfield\",\"Distance\":2400,\"RaceNumber\":5,\"RaceType\":\"Metropolitan\",\"RaceInfo\":\"Carlton Draught Caulfield Cup\",\"TrackCondition\":\"Good(4)\",\"StartTimeUtc\":\"2023-10-20T20:15:00Z\",\"Runners\":[{\"Id\":101,\"Number\":1,\"Barrier\":8,\"Name\":\"BORN A KING\",\"WinPrice\":3.4,\"Jockey\":\"Ben Melham\",\"Trainer\":\"C Maher and D Eustace\"},{\"Id\":167,\"Number\":3,\"Barrier\":5,\"Name\":\"ARAPAHO\",\"WinPrice\":5.7,\"Jockey\":\"Damian Lane\",\"Trainer\":\"T Yoshioka\"}]}"));
        });
    }

    [Fact]
    public async Task Given_CaughtException_UpdateRace_ThrowsError()
    {
        // arrange
        var updateUtc = DateTime.UtcNow;
        mockRaceRepository.GetRace(Arg.Any<long>()).Returns(
            Task.FromResult(new Repositories.Entities.Race { ExternalRaceId = 1123, UpdatedUtc = updateUtc }),
            Task.FromResult(new Repositories.Entities.Race { ExternalRaceId = 1123, UpdatedUtc = updateUtc })
            );

        var raceUpdate = new RaceUpdate
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
        async Task act() => await raceService.UpdateRace(raceUpdate);

        // assert
        var createResourceException = await Assert.ThrowsAsync<UpdateResourceException>(act);
        Assert.Equal("An exception occurred while updating race 1123", createResourceException.Message);
    }

    [Fact]
    public async Task Given_ConcurrentUpdate_UpdateRace_ThrowsError()
    {
        // arrange
        mockRaceRepository.GetRace(Arg.Any<long>()).Returns(
            Task.FromResult(new Repositories.Entities.Race { ExternalRaceId = 1123, UpdatedUtc = DateTime.UtcNow }),
            Task.FromResult(new Repositories.Entities.Race { ExternalRaceId = 1123, UpdatedUtc = DateTime.UtcNow.AddMicroseconds(1) })
            );

        var raceUpdate = new RaceUpdate
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
        async Task act() => await raceService.UpdateRace(raceUpdate);

        // assert
        var createResourceException = await Assert.ThrowsAsync<UpdateResourceException>(act);
        Assert.Equal("An exception occurred while updating race 1123", createResourceException.Message);
    }

    [Theory]
    [InlineData(1, true)]
    [InlineData(2, false)]
    public async Task Given_RaceId_CheckRaceExists_IsSuccessful(long raceId, bool exists)
    {
        // arrange
        mockRaceRepository.GetRace(1).Returns(Task.FromResult(new Repositories.Entities.Race { ExternalRaceId = 1, UpdatedUtc = DateTime.UtcNow }));
        mockRaceRepository.GetRace(2).ReturnsNull();

        // act
        var result = await raceService.CheckRaceExits(raceId);

        // assert
        Assert.Equal(exists, result);
    }
}