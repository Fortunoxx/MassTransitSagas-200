namespace Api.Application.Commands;

using MassTransit;

public interface IProcessOrder : CorrelatedBy<Guid>
{
}