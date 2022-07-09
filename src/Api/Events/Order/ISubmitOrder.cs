namespace Api.Events.Order;

using MassTransit;

public interface ISubmitOrder : CorrelatedBy<Guid>
{
}
