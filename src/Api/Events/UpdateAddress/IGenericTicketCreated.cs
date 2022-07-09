namespace Api.Events.UpdateAddress;

using MassTransit;

public interface IGenericTicketCreated : CorrelatedBy<Guid>
{
}
