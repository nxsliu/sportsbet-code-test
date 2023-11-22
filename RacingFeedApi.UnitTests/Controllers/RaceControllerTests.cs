using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using RacingFeedApi.Services;
using RacingFeedApi.ViewModels;

namespace RacingFeedApi.UnitTests.Controllers;

public class RaceControllerTests
{
    private readonly IRaceService mockRaceService = Substitute.For<IRaceService>();

    private readonly RaceController raceController;

    public RaceControllerTests()
    {
        raceController = new RaceController(mockRaceService);
    }

    [Fact]
    public async Task Given_RaceIdExists_UpdateRace_UpdatesRace()
    {
        // arrange
        mockRaceService.CheckRaceExits(Arg.Any<long>()).Returns(Task.FromResult(true));

        var race = new RaceUpdate
        {
            RaceId = 1
        };

        // act
        var result = await raceController.UpdateRace(race);

        // assert
        Assert.IsType<OkResult>(result.Result);
        await mockRaceService.Received(1).UpdateRace(Arg.Any<RaceUpdate>());
        await mockRaceService.DidNotReceive().CreateRace(Arg.Any<RaceUpdate>());
    }

    [Fact]
    public async Task Given_RaceIdNotExist_UpdateRace_CreatesRace()
    {
        // arrange
        mockRaceService.CheckRaceExits(Arg.Any<long>()).Returns(Task.FromResult(false));

        var race = new RaceUpdate
        {
            RaceId = 1
        };

        // act
        var result = await raceController.UpdateRace(race);

        // assert
        Assert.IsType<OkResult>(result.Result);
        await mockRaceService.Received(1).CreateRace(Arg.Any<RaceUpdate>());
        await mockRaceService.DidNotReceive().UpdateRace(Arg.Any<RaceUpdate>());
    }
}