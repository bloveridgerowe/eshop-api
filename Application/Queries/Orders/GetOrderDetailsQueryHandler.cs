using Application.Mappers;
using Domain.Aggregates.Orders;
using Domain.Repositories;
using MediatR;

namespace Application.Queries.Orders;

public class GetOrderDetailsQueryHandler : IRequestHandler<GetOrderDetailsQuery, GetOrderDetailsQueryResponse>
{
    private readonly IOrderRepository _orderRepository;

    public GetOrderDetailsQueryHandler(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<GetOrderDetailsQueryResponse> Handle(GetOrderDetailsQuery request, CancellationToken cancellationToken)
    {
        Order? order = await _orderRepository.FindByIdAsync(request.OrderId);
        
        if (order == null || order.CustomerId != request.CustomerId)
        {
            return GetOrderDetailsQueryResponse.Empty;
        }

        GetOrderDetailsQueryResponse response = new GetOrderDetailsQueryResponse
        {
            OrderDetails = order.ToQueryModel()
        };

        return response;
    }
} 