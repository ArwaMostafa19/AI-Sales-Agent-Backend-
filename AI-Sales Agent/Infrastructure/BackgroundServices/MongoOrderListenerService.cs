using AI_Sales_Agent.Domain.Mongo;
using AI_Sales_Agent.Features.DashboardManagement.RefreshDashboardInsights;
using AI_Sales_Agent.Infrastructure.Mongo;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace AI_Sales_Agent.Infrastructure.BackgroundServices;

public class MongoOrderListenerService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<MongoOrderListenerService> _logger;

    public MongoOrderListenerService(
        IServiceScopeFactory scopeFactory,
        ILogger<MongoOrderListenerService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var insertOnlyPipeline = new EmptyPipelineDefinition<ChangeStreamDocument<OrderDocument>>()
            .Match(change => change.OperationType == ChangeStreamOperationType.Insert);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<IMongoDbContext>();
                var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

                using var cursor = await context.Orders.WatchAsync(
                    insertOnlyPipeline,
                    cancellationToken: stoppingToken);

                await cursor.ForEachAsync(async change =>
                {
                    if (change.FullDocument is not null)
                    {
                        await mediator.Send(
                            new RefreshDashboardInsightsCommand(change.FullDocument),
                            stoppingToken);
                    }
                }, stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "MongoDB order change stream failed and will be restarted");
            }
        }
    }
}
