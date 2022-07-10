namespace Api.Infrastructure.StateMachine;

using Api.Application.Events.QueryProcessState;
using Api.Application.Events.UpdateAddress;
using Api.Infrastructure.StateMachine.Activities;
using MassTransit;

public class UpdateAddressState
    : SagaStateMachineInstance
{
    public Guid CorrelationId { get; set; } // uid
    // public int CurrentState { get; set; }
    
    public string CurrentState { get; set; }

    public string StatusText { get; set; }

    public DateTime? CreateDate { get; set; }

    // If using Optimistic concurrency, this property is required
    public byte[] RowVersion { get; set; }
}

public class UpdateAddressStateMachine :
    MassTransitStateMachine<UpdateAddressState>
{
    private readonly ILogger<UpdateAddressStateMachine> logger;

    public State Running { get; private set; }
    public State GenericTicketCreated { get; private set; }

    public Event<IUpdateAddressInvoked> UpdateAddressEvent { get; private set; }
    public Event<IGenericTicketCreated> GenericTicketCreatedEvent { get; private set; }
    public Event<IQueryProcessState> QueryProcessStateEvent { get; private set; }

    public UpdateAddressStateMachine(ILogger<UpdateAddressStateMachine> logger)
    {
        this.logger = logger;
        InstanceState(x => x.CurrentState);
        Event(() => QueryProcessStateEvent, x => x.ReadOnly = true);

        Initially(
            When(UpdateAddressEvent)
                .Then(_ => logger.LogInformation("=== Hello from UpdateAddressStateMachine"))
                .Then(context => context.Instance.StatusText = "Started")
                .Activity(x => x.OfType<UpdateAddressInitializeActivity>())
                .TransitionTo(Running)
        );

        During(Running,
            When(GenericTicketCreatedEvent)
                .Then(_ => logger.LogInformation("=== GenericTicket created"))
                .Then(context => context.Instance.StatusText = "Running")
                .TransitionTo(GenericTicketCreated)
        );

        DuringAny(
            When(QueryProcessStateEvent)
                .Then(context => logger.LogInformation($"==== The current State of {context.Saga.CorrelationId} is {context.Saga.CurrentState}"))
                .RespondAsync(context => context.Init<IStateQueried>(new { context.Saga.CorrelationId, Message = $"==== The current State of {context.Saga.CorrelationId} is {context.Saga.CurrentState}" }))
        );
    }
}
