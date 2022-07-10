namespace Api.WebUI.Controllers;

using Api.Application.Events.QueryProcessState;
using Api.Application.Events.UpdateAddress;
using Api.WebUI.Models;
using MassTransit;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController2 : ControllerBase
{
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    private readonly ILogger<WeatherForecastController2> _logger;
    private readonly IPublishEndpoint publishEndpoint;
    private readonly IRequestClient<IQueryProcessState> client;

    public WeatherForecastController2(ILogger<WeatherForecastController2> logger
        , IPublishEndpoint publishEndpoint
        , IRequestClient<IQueryProcessState> client)
    {
        _logger = logger;
        this.publishEndpoint = publishEndpoint;
        this.client = client;
    }

    [HttpGet("{correlationId}", Name = "GetWeatherForecast2")]
    public async Task<IActionResult> GetWeatherForecast2(Guid correlationId)
    {
        var response = await client.GetResponse<IStateQueried>(new { CorrelationId = correlationId });
        return Ok(response);
    }

    [HttpPost(Name = "Post2")]
    public IActionResult Post([FromBody] OrderModel orderModel)
    {
        if (!ModelState.IsValid)
            return BadRequest();

        publishEndpoint.Publish<IUpdateAddressInvoked>(new
        {
            orderModel.CorrelationId,
        });

        return NoContent();
    }
}
