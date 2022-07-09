namespace Api.Events.Order;

using MassTransit;

public interface IOrderDone : CorrelatedBy<Guid>
{
}
