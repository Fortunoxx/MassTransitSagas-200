namespace Api.Application.Events.UpdateAddress;

using MassTransit;

public interface IGenericTicketCreated : CorrelatedBy<Guid>
{
}
