using System.Reflection;
using Api.Application.Consumers;
using MassTransit;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddMassTransit(x =>
{
    // x.AddSagaStateMachine<OrderStateMachine, OrderState>().InMemoryRepository()
    x.SetInMemorySagaRepositoryProvider();
    var entryAssembly = Assembly.GetEntryAssembly();

    // x.AddConsumers(entryAssembly);
    x.AddConsumer<ProcessOrderConsumer>().Endpoint(e => e.Name = "test-q-01");
    x.AddConsumersFromNamespaceContaining<UpdateAddressGenericTicketConsumer>();
    x.AddSagaStateMachines(entryAssembly);
    x.AddSagas(entryAssembly);
    x.AddActivities(entryAssembly);
    // really?
    // x.AddSagaStateMachine<OrderStateMachine, OrderState>().InMemoryRepository();

    x.UsingInMemory((context, cfg) =>
    {
        cfg.ConfigureEndpoints(context);
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
