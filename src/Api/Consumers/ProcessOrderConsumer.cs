using Api.Commands;
using Api.Events.Order;
using MassTransit;

namespace Api.Consumers;

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
        // this.UpdateOrderState(context.Message.Order);
        await context.Publish<IProcessOrderDone>(new
        {
            CorrelationId = context.Message.CorrelationId,
            // Order = context.Message.Order
        });
    }

    // private void UpdateOrderState(Order order) =>
    //    order.Status = Status.Shipped;
}
