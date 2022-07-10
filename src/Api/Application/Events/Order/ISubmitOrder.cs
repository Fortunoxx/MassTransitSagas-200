namespace Api.Application.Events.Order;

using MassTransit;

public interface ISubmitOrder : CorrelatedBy<Guid>
{
}
