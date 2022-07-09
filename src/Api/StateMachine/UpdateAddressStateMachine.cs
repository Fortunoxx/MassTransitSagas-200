namespace Api.StateMachine;

using System.Threading.Tasks;
using Api.Events;
using Api.Events.UpdateAddress;
using MassTransit;

public class UpdateAddressState
    : SagaStateMachineInstance
{
    public Guid CorrelationId { get; set; } // uid
    // public int CurrentState { get; set; }
    public string CurrentState { get; set; }
}

public class UpdateAddressStateMachine :
    MassTransitStateMachine<UpdateAddressState>
{
    private readonly ILogger<UpdateAddressStateMachine> logger;

    public State Initialized { get; private set; }
    public State GenericTicketCreated { get; private set; }

    public Event<IUpdateAddressInvoked> UpdateAddressEvent { get; private set; }
    public Event<IQueryProcessState> QueryProcessStateEvent { get; private set; }
    public Event<IGenericTicketCreated> GenericTicketCreatedEvent { get; private set; }

    public UpdateAddressStateMachine(ILogger<UpdateAddressStateMachine> logger)
    {
        this.logger = logger;
        this.InstanceState(x => x.CurrentState);

        Initially(
            When(UpdateAddressEvent)
                .Then(_ => logger.LogInformation("=== Hello from UpdateAddressStateMachine"))
                .Activity(x => x.OfType<UpdateAddressInitializeActivity>())
                .TransitionTo(Initialized)
        );

        During(Initialized, 
            When(GenericTicketCreatedEvent)
                .Then(_ => logger.LogInformation("=== GenericTicket created"))
                .TransitionTo(GenericTicketCreated)
        );

        DuringAny(
            When(QueryProcessStateEvent)
                .Then(context => logger.LogInformation($"==== The current State of {context.Saga.CorrelationId} is {context.Saga.CurrentState}"))
                .RespondAsync(context => context.Init<IStateQueried>(new { context.Saga.CorrelationId, Message =  $"==== The current State of {context.Saga.CorrelationId} is {context.Saga.CurrentState}"}))
        );
    }
}

public class UpdateAddressInitializeActivity : IStateMachineActivity<UpdateAddressState, IUpdateAddressInvoked>
{
    readonly ConsumeContext _context;
    private readonly ILogger<UpdateAddressInitializeActivity> logger;

    public UpdateAddressInitializeActivity(ConsumeContext context, ILogger<UpdateAddressInitializeActivity> logger)
    {
        _context = context;
        this.logger = logger;
    }

    public void Accept(StateMachineVisitor visitor)
    {
        visitor.Visit(this);
    }

    public async Task Execute(BehaviorContext<UpdateAddressState, IUpdateAddressInvoked> context, IBehavior<UpdateAddressState, IUpdateAddressInvoked> next)
    {
        // do the activity thing
        logger.LogInformation($"==== UpdateAddressInitializeActivity {context.Message.CorrelationId} doing stuff for 10 seconds...");
        await Task.Delay(10000);
        logger.LogInformation($"==== UpdateAddressInitializeActivity {context.Message.CorrelationId} done");

        await _context.Publish<IUpdateAddressInitialized>(new { CorrelationId = context.Saga.CorrelationId }).ConfigureAwait(false);

        // call the next activity in the behavior
        await next.Execute(context).ConfigureAwait(false);
    }

    public Task Faulted<TException>(BehaviorExceptionContext<UpdateAddressState, IUpdateAddressInvoked, TException> context, IBehavior<UpdateAddressState, IUpdateAddressInvoked> next) where TException : Exception
    {
        return next.Faulted(context);
    }

    public void Probe(ProbeContext context)
    {
        context.CreateScope("update-address-initialize-intiated");
    }
}