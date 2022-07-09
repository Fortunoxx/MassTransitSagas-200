using MassTransit;

namespace Api.Events;

public interface IQueryProcessState : CorrelatedBy<Guid>
{
}