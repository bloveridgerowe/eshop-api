using Domain.Aggregates.Orders;
using Domain.Repositories;

namespace Presentation.Api.BackgroundServices;

public class OrderFulfilmentBackgroundService : BackgroundService
{
    // Background services are singletons so we must inject a service scope factory to retrieve our scoped items
    
    private readonly ILogger<OrderFulfilmentBackgroundService> _logger;
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public OrderFulfilmentBackgroundService(ILogger<OrderFulfilmentBackgroundService> logger, IServiceScopeFactory serviceScopeFactory)
    {
        _logger = logger;
        _serviceScopeFactory = serviceScopeFactory;
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // This is just a demonstration app so there's no one actually fulfilling orders! 
        // This background service is only present to move the order status along automatically
        
        _logger.LogInformation("Order fulfilment background service is starting");
        
        PeriodicTimer timer = new PeriodicTimer(TimeSpan.FromMinutes(30));

        while (!stoppingToken.IsCancellationRequested && await timer.WaitForNextTickAsync(stoppingToken))
        {
            try
            {
                _logger.LogInformation("Fulfilling orders");

                await ShipPendingOrdersAsync();
                await DeliverShippedOrdersAsync();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Order fulfilment background service failed");
            }
        }
    }
    
    private async Task ShipPendingOrdersAsync()
    {

        await using AsyncServiceScope asyncScope = _serviceScopeFactory.CreateAsyncScope();
        IOrderRepository orderRepository = asyncScope.ServiceProvider.GetRequiredService<IOrderRepository>();
        IUnitOfWork unitOfWork = asyncScope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        
        List<Order> pendingOrders = await orderRepository.GetPendingOrdersAsync();
        
        // We don't want to ship orders immediately as it's not realistic!
        
        List<Order> eligiblePendingOrders = pendingOrders.Where(o => o.CreatedAt <= DateTime.UtcNow.AddHours(-1)).ToList();
        
        foreach (Order pendingOrder in eligiblePendingOrders)
        {
            pendingOrder.MarkAsShipped();
            
            await orderRepository.SaveAsync(pendingOrder);
        }
        
        await unitOfWork.CommitAsync();
        
        _logger.LogInformation("Shipped {eligiblePendingOrders} eligible pending orders", eligiblePendingOrders.Count);
    }

    private async Task DeliverShippedOrdersAsync()
    {
        await using AsyncServiceScope asyncScope = _serviceScopeFactory.CreateAsyncScope();
        IOrderRepository orderRepository = asyncScope.ServiceProvider.GetRequiredService<IOrderRepository>();
        IUnitOfWork unitOfWork = asyncScope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        
        List<Order> shippedOrders = await orderRepository.GetShippedOrdersAsync();
        
        // We don't want to deliver orders too quickly as it's not realistic!
        
        List<Order> eligibleShippedOrders = shippedOrders.Where(o => o.CreatedAt <= DateTime.UtcNow.AddDays(-1)).ToList();
        
        foreach (Order shippedOrder in eligibleShippedOrders)
        {
            shippedOrder.MarkAsDelivered();
            
            await orderRepository.SaveAsync(shippedOrder);
        }
        
        await unitOfWork.CommitAsync();
        
        _logger.LogInformation("Delivered {eligibleShippedOrders} eligible shipped orders", eligibleShippedOrders.Count);
    }
}