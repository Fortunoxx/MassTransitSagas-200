using MassTransit;

namespace Api.Commands;

public interface IProcessOrder : CorrelatedBy<Guid>
{
}