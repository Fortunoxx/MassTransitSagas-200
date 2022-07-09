namespace Api.Controllers;

using Api.Events;
using Api.Events.UpdateAddress;
using Api.Models;
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
        this._logger = logger;
        this.publishEndpoint = publishEndpoint;
        this.client = client;
    }

    [HttpGet("{correlationId}", Name = "GetWeatherForecast2")]
    public async Task<IActionResult> GetWeatherForecast2(Guid correlationId)
    {
        var response = await client.GetResponse<IStateQueried>(new { CorrelationId = correlationId });
        return Ok(response);
    }

    // [HttpGet(Name = "GetWeatherForecast")]
    // public IEnumerable<WeatherForecast> Get()
    // {
    //     // return Enumerable.Range(1, 5).Select(index => new WeatherForecast
    //     // {
    //     //     Date = DateTime.Now.AddDays(index),
    //     //     TemperatureC = Random.Shared.Next(-20, 55),
    //     //     Summary = Summaries[Random.Shared.Next(Summaries.Length)]
    //     // })
    //     // .ToArray();
    // }

    [HttpPost(Name = "Post2")]
    public IActionResult Post([FromBody] OrderModel orderModel)
    {
        if (!ModelState.IsValid)
            return BadRequest();

        this.publishEndpoint.Publish<IUpdateAddressInvoked>(new
        {
            CorrelationId = orderModel.CorrelationId,
        });

        return NoContent();
    }
}
