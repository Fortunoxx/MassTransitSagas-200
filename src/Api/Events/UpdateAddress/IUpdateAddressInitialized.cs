namespace Api.Events.UpdateAddress;

using MassTransit;

public interface IUpdateAddressInitialized : CorrelatedBy<Guid>
{
}
