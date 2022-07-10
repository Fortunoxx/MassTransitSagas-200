namespace Api.Application.Events.QueryProcessState;

using MassTransit;

public interface IStateQueried : CorrelatedBy<Guid>
{
    string Message { get; set; }
}