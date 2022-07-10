namespace Api.StateMachine.Activities;

using System.Threading.Tasks;
using Api.Events.UpdateAddress;
using MassTransit;

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