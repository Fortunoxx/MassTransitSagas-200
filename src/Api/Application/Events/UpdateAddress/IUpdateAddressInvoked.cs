namespace Api.Application.Events.UpdateAddress;

using MassTransit;

public interface IUpdateAddressInvoked : CorrelatedBy<Guid>
{
}
