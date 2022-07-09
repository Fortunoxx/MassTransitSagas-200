using MassTransit;

namespace Api.Events;

public interface IStateQueried : CorrelatedBy<Guid> 
{
    string Message { get; set; }
}