namespace Api.Application.Events.Order;

using MassTransit;

public interface IOrderDone : CorrelatedBy<Guid>
{
}
