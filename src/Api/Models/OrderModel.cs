namespace Api.Models;

public record OrderModel
{
    public Guid CorrelationId { get; init; }
    public string OrderNumber { get; init; }
}