namespace Api.Application.Consumers;

using Api.Application.Events.UpdateAddress;
using MassTransit;

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

        await context.Publish<IGenericTicketCreated>(new
        {
            context.Message.CorrelationId,
        });
    }
}