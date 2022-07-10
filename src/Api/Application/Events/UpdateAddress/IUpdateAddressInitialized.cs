namespace Api.Application.Events.UpdateAddress;

using MassTransit;

public interface IUpdateAddressInitialized : CorrelatedBy<Guid>
{
}
