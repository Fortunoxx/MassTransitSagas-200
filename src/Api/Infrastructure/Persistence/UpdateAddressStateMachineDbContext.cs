namespace Api.Infrastructure.Persistence;

using Api.Infrastructure.StateMachine;
using MassTransit;
using MassTransit.EntityFrameworkCoreIntegration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class UpdateAddressStateMachineDbContext : SagaDbContext
{
    public UpdateAddressStateMachineDbContext(DbContextOptions options)
        : base(options)
    {
    }

    protected override IEnumerable<ISagaClassMap> Configurations
    {
        get { yield return new UpdateAddressStateMap(); }
    }
}

public class UpdateAddressStateMap :
   SagaClassMap<UpdateAddressState>
{
    protected override void Configure(EntityTypeBuilder<UpdateAddressState> entity, ModelBuilder model)
    {
        entity.Property(x => x.CurrentState).HasMaxLength(64);
        entity.Property(x => x.CreateDate);

        // If using Optimistic concurrency, otherwise remove this property
        entity.Property(x => x.RowVersion).IsRowVersion();
    }
}
