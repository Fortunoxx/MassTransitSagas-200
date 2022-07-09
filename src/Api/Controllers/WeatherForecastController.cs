namespace Api.Controllers;

using Api.Events;
using Api.Events.Order;
using Api.Models;
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

        this.publishEndpoint.Publish<IQueryProcessState>(new
        {
            CorrelationId = correlationId,
        });

        return Ok();
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

    [HttpPost(Name = "Post")]
    public IActionResult Post([FromBody] OrderModel orderModel)
    {
        if (!ModelState.IsValid)
            return BadRequest();

        this.publishEndpoint.Publish<ISubmitOrder>(new
        {
            CorrelationId = orderModel.CorrelationId,
            OrderNumber = orderModel.OrderNumber
        });

        return NoContent();
    }

    // [HttpPost(Name = "PostProcess")]
    // [Route("[controller]/api/Process")]
    // public IActionResult PostProcess([FromBody] OrderModel orderModel)
    // {
    //     if(!ModelState.IsValid)
    //         return BadRequest();

    //     this.publishEndpoint.Publish<IProcessOrder>(new { 
    //         CorrelationId = orderModel.CorrelationId, 
    //         // OrderNumber = orderModel.OrderNumber
    //     });

    //     return NoContent();
    // }
}
