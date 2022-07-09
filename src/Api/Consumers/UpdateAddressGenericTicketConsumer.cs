using Api.Events.UpdateAddress;
using MassTransit;

namespace Api.Consumers;

public class UpdateAddressGenericTicketConsumer : IConsumer<IUpdateAddressInitialized>
{
    private readonly ILogger<UpdateAddressGenericTicketConsumer> logger;

    public UpdateAddressGenericTicketConsumer(ILogger<UpdateAddressGenericTicketConsumer> logger)
    {
        this.logger = logger;
    }

    public async Task Consume(ConsumeContext<IUpdateAddressInitialized> context)
    {
        logger.LogInformation($"==== IUpdateAddressInitialized event received {context.Message.CorrelationId}");
        await Task.Delay(2000);
        // this.UpdateOrderState(context.Message.Order);
        await context.Publish<IGenericTicketCreated>(new
        {
            CorrelationId = context.Message.CorrelationId,
            // Order = context.Message.Order
        });
    }
}