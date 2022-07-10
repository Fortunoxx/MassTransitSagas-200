namespace Api.Application.Events.Order;

using MassTransit;

public interface IProcessOrderDone : CorrelatedBy<Guid>
{ }
