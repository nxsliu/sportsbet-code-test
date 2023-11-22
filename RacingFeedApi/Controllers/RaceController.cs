using Microsoft.AspNetCore.Mvc;
using RacingFeedApi.ViewModels;
using RacingFeedApi.Services;

[ApiController]
[Route("api/[controller]")]
public class RaceController : ControllerBase
{
    private readonly IRaceService _raceService;

    public RaceController(IRaceService raceService)
    {
        _raceService = raceService;
    }

    [HttpPost]
    [Consumes("application/xml")]
    public async Task<ActionResult<string>> UpdateRace(RaceUpdate race)
    {
        //throw new NotImplementedException();

        if (await _raceService.CheckRaceExits(race.RaceId))
        {
            await _raceService.UpdateRace(race);
        }
        else
        {
            await _raceService.CreateRace(race);
        }

        return Ok();
    }
}