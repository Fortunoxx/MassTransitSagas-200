namespace Api.Events.UpdateAddress;

using MassTransit;

public interface IUpdateAddressInvoked : CorrelatedBy<Guid>
{
}
