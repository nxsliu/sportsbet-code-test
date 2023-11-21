using MediatR;
using Microsoft.AspNetCore.Mvc;
using RacingFeedApi.Models;
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
    public async Task<ActionResult<string>> CreateRace(RaceCreate raceCreate)
    {
        await _raceService.CreateRace(raceCreate);

        return Ok();
    }
}