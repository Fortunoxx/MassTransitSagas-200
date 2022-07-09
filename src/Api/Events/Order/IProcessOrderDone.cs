namespace Api.Events.Order;

using MassTransit;

public interface IProcessOrderDone : CorrelatedBy<Guid>
{}
