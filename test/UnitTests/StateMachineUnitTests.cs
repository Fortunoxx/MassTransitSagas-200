namespace UniTests;

using Xunit;
using Microsoft.Extensions.DependencyInjection;
using MassTransit;
using MassTransit.Testing;
using System;
using FluentAssertions;
using Api.Application.Events.UpdateAddress;
using Api.Application.Events.Order;
using Api.Infrastructure.StateMachine;

public class UnitTest1
{
    [Fact]
    public async void State_machine_should_run_until_final_state_async()
    {
        {
            await using var provider = new ServiceCollection()
                .AddMassTransitTestHarness(
                    cfg =>
                    {
                        cfg.AddSagaStateMachine<OrderStateMachine, OrderState>();
                    }).
                BuildServiceProvider(true);

            var harness = provider.GetTestHarness();
            await harness.Start();
            await harness.Bus.Publish<ISubmitOrder>(new { CorrelationId = Guid.NewGuid() });

            (await harness.Published.Any()).Should().BeTrue();
            (await harness.Consumed.Any()).Should().BeTrue();
            (await harness.Consumed.Any<ISubmitOrder>()).Should().BeTrue();

            var sagaHarness = harness.GetSagaStateMachineHarness<OrderStateMachine, OrderState>();
            (await sagaHarness.Consumed.Any<ISubmitOrder>()).Should().BeTrue();
        }
    }

    [Fact]
    public async void UpdateAddress_State_machine_should_run_until_final_state_async()
    {
        {
            await using var provider = new ServiceCollection()
                .AddMassTransitTestHarness(
                    cfg =>
                    {
                        cfg.AddSagaStateMachine<UpdateAddressStateMachine, UpdateAddressState>();
                    }).
                BuildServiceProvider(true);

            var harness = provider.GetTestHarness();
            await harness.Start();

            var sagaId = Guid.NewGuid();
            await harness.Bus.Publish<IUpdateAddressInvoked>(new { CorrelationId = sagaId });

            (await harness.Published.Any()).Should().BeTrue();
            (await harness.Consumed.Any()).Should().BeTrue();
            (await harness.Consumed.Any<IUpdateAddressInvoked>()).Should().BeTrue();

            var sagaHarness = harness.GetSagaStateMachineHarness<UpdateAddressStateMachine, UpdateAddressState>();
            (await sagaHarness.Consumed.Any<IUpdateAddressInvoked>()).Should().BeTrue();

            (await sagaHarness.Created.Any(x => x.CorrelationId == sagaId)).Should().BeTrue();

            var instance = sagaHarness.Created.ContainsInState(sagaId, sagaHarness.StateMachine, sagaHarness.StateMachine.Running);
            instance.Should().NotBeNull("Saga instance should be found");

            instance.CorrelationId.Should().Be(sagaId);
            (await harness.Published.Any<IUpdateAddressInitialized>()).Should().BeTrue("this event should have been published");
        }
    }
}