namespace Api.StateMachine;

using Api.Commands;
using Api.Events;
using Api.Events.Order;
using MassTransit;

public class OrderState :
    SagaStateMachineInstance, CorrelatedBy<Guid>
{
    public Guid CorrelationId { get; set; }
    public int CurrentState { get; set; }
    // public string OrderNumber { get; init; }
    public Uri ResponseAddress { get; set; }
    public Guid? ProcessingId { get; internal set; }
    public Guid OrderId { get; internal set; }
    public Guid? RequestId { get; internal set; }
}

// public class ProcessOrderConsumer : IConsumer<ProcessOrder>
// {
//     public async Task Consume(ConsumeContext<ProcessOrder> context)
//     {
//         await context.RespondAsync(new OrderProcessed(context.Message.OrderId, context.Message.ProcessingId));
//     }
// }
public record OrderProcessed(Guid OrderId, Guid ProcessingId);
public record ProcessOrder(Guid OrderId, Guid ProcessingId);
public record OrderCancelled(Guid OrderId, string Reason);

public class OrderStateMachine :
    MassTransitStateMachine<OrderState>
{
    private readonly ILogger<OrderStateMachine> logger;
    private readonly IConfiguration configuration;
    private readonly IBusControl bus;

    public State Submitted { get; private set; }
    public State Cancelled { get; set; }
    // public State Accepted { get; private set; }
    public Request<OrderState, ProcessOrder, OrderProcessed> ProcessOrder { get; set; }
    
    public OrderStateMachine(ILogger<OrderStateMachine> logger) //, IConfiguration configuration)
    {
        this.logger = logger;
        // this.configuration = configuration;
        this.InstanceState(x => x.CurrentState);

        // Event(() => SubmitOrder, x => x.CorrelateById(context => context.Message.OrderId));
        Event(() => SubmitOrderEvent);
        // Event(() => ProcessOrderEvent);
        Event(() => QueryProcessStateEvent);
        Request(() => ProcessOrder, order => order.ProcessingId, config => { config.Timeout = TimeSpan.Zero; });

        Initially(
            When(SubmitOrderEvent)
                // .Then(context => 
                //     context.Saga.ResponseAddress = context.ResponseAddress
                // )
                .Then(_ => logger.LogInformation("=== Hello from OrderStateMachine"))
                // .PublishAsync( c => c.)
                .Then(c => this.SendCommand<IProcessOrder>("queue-1", c))
                // .Then(c => c.Send<IProcessOrder>(c.Saga.ResponseAddress, "test"))
                .TransitionTo(Submitted)
                // .Then(context =>
                // {
                //     context.Saga.CorrelationId = context.Message.CorrelationId;
                //     context.Saga.ProcessingId = Guid.NewGuid();

                //     context.Saga.OrderId = Guid.NewGuid();

                //     context.Saga.RequestId = context.RequestId;
                //     context.Saga.ResponseAddress = context.ResponseAddress;
                // })
                // .Request(ProcessOrder, context => new ProcessOrder(context.Saga.CorrelationId, context.Saga.ProcessingId!.Value))
                // .TransitionTo(ProcessOrder.Pending)
        );

        // During(ProcessOrder.Pending,
        //     When(ProcessOrder.Completed)
        //         .TransitionTo(Submitted)
        //         .ThenAsync(async context =>
        //         {
        //             var endpoint = await context.GetSendEndpoint(context.Saga.ResponseAddress);
        //             await endpoint.Send(context.Saga, r => r.RequestId = context.Saga.RequestId);
        //         }),
        //     When(ProcessOrder.Faulted)
        //         .TransitionTo(Cancelled)
        //         .ThenAsync(async context =>
        //         {
        //             var endpoint = await context.GetSendEndpoint(context.Saga.ResponseAddress);
        //             await endpoint.Send(new OrderCancelled(context.Saga.OrderId, "Faulted"), r => r.RequestId = context.Saga.RequestId);
        //         }),
        //     When(ProcessOrder.TimeoutExpired)
        //         .TransitionTo(Cancelled)
        //         .ThenAsync(async context =>
        //         {
        //             var endpoint = await context.GetSendEndpoint(context.Saga.ResponseAddress);
        //             await endpoint.Send(new OrderCancelled(context.Saga.OrderId, "Time-out"), r => r.RequestId = context.Saga.RequestId);
        //         }));

        During(Submitted, 
            When(SubmitOrderEvent)
                .Then(_ => logger.LogWarning("=== INVALID: already processing...")),
            When(ProcessOrderDoneEvent)
                .Then(_ => logger.LogInformation("=== DONE: transitioning to final."))
                .Finalize()
        );

        DuringAny(
            When(QueryProcessStateEvent)
                .Then(context => logger.LogInformation($"==== The current State of {context.Saga.CorrelationId} is {context.Saga.CurrentState}"))
                .RespondAsync(context => context.Init<IStateQueried>(new { context.Saga.CorrelationId }))
        );


        DuringAny(When(FinalizeEvent).Finalize());

        SetCompletedWhenFinalized();
    }

    public Event<ISubmitOrder> SubmitOrderEvent { get; private set; }
    public Event<IProcessOrderDone> ProcessOrderDoneEvent { get; private set; }
    public Event<IQueryProcessState> QueryProcessStateEvent { get; private set; }
    public Event<IOrderDone> FinalizeEvent { get; private set; }

    private async Task SendCommand<TCommand>(string endpointKey, BehaviorContext<OrderState, CorrelatedBy<Guid>> context)
        where TCommand : class, CorrelatedBy<Guid>
    {
        // var queues2 = configuration.GetSection(QueueSettings.SectionName);
        // var queues = queues2.Get<IEnumerable<QueueSettings>>();
        // // var sendEndpoint = await context.GetSendEndpoint(new Uri(ConfigurationManager.AppSettings[endpointKey]));
        // var sendEndpoint = await context.GetSendEndpoint(new Uri(queues.First(x => x.Name == "queue-1").Address));
        // var sendEndpoint = await context.GetSendEndpoint<TCommand>();
        // var sendEndpoint = await context.GetSendEndpoint(context.Saga.ResponseAddress);
        var sendEndpoint = await context.GetSendEndpoint(new Uri("queue:test-q-01"));

        await sendEndpoint.Send<TCommand>(new
        {
            CorrelationId = context.Data.CorrelationId,
            // Order = context.Data.Order
        });
    }

    public record QueueSettings
    {
        public static string SectionName => "Queues";
        public string Name { get; init; }
        public string Address { get; init; }
    }    
    
    // public class QueueSettings
    // {
    //     public static string SectionName => "Queues";
    //     public string Name { get; set; } = String.Empty;
    //     public string Address { get; set; } = String.Empty;
    // }
}