namespace Api.Application.Consumers;

using Api.Application.Commands;
using Api.Application.Events.Order;
using MassTransit;

public class ProcessOrderConsumer : IConsumer<IProcessOrder>
{
    private ILogger<ProcessOrderConsumer> logger;

    public ProcessOrderConsumer(ILogger<ProcessOrderConsumer> logger)
        => this.logger = logger;

    public async Task Consume(ConsumeContext<IProcessOrder> context)
    {
        logger.LogInformation($"==== IProcessOrder command received {context.Message.CorrelationId} doing stuff for 10 seconds...");
        await Task.Delay(10000);
        logger.LogInformation($"==== IProcessOrder command received {context.Message.CorrelationId} done");

        await context.Publish<IProcessOrderDone>(new
        {
            context.Message.CorrelationId,
        });
    }
}
