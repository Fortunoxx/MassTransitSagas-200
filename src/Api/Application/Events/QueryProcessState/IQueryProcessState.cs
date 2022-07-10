namespace Api.Application.Events.QueryProcessState;

using MassTransit;

public interface IQueryProcessState : CorrelatedBy<Guid>
{
}