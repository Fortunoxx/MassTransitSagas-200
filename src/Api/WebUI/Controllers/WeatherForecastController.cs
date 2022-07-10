namespace Api.WebUI.Controllers;

using Api.Application.Events.Order;
using Api.Application.Events.QueryProcessState;
using Api.WebUI.Models;
using MassTransit;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    private readonly ILogger<WeatherForecastController> _logger;
    private readonly IPublishEndpoint publishEndpoint;

    public WeatherForecastController(ILogger<WeatherForecastController> logger, IPublishEndpoint publishEndpoint)
    {
        _logger = logger;
        this.publishEndpoint = publishEndpoint;
    }

    [HttpGet(Name = "GetWeatherForecast")]
    public IActionResult Get([FromQuery] Guid correlationId)
    {
        if (!ModelState.IsValid)
            return BadRequest();

        publishEndpoint.Publish<IQueryProcessState>(new
        {
            CorrelationId = correlationId,
        });

        return Ok();
    }

    [HttpPost(Name = "Post")]
    public IActionResult Post([FromBody] OrderModel orderModel)
    {
        if (!ModelState.IsValid)
            return BadRequest();

        publishEndpoint.Publish<ISubmitOrder>(new
        {
            orderModel.CorrelationId,
            orderModel.OrderNumber
        });

        return NoContent();
    }
}
