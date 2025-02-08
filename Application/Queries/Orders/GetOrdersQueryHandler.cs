using Application.Mappers;
using Domain.Aggregates.Orders;
using Domain.Repositories;
using MediatR;

namespace Application.Queries.Orders;

public class GetOrdersQueryHandler : IRequestHandler<GetOrdersQuery, GetOrdersQueryResponse>
{
    private readonly IOrderRepository _orderRepository;

    public GetOrdersQueryHandler(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<GetOrdersQueryResponse> Handle(GetOrdersQuery request, CancellationToken cancellationToken)
    {
        List<Order> orders = await _orderRepository.GetAllAsync(request.CustomerId);


        GetOrdersQueryResponse response = new GetOrdersQueryResponse
        {
            Orders = orders.Select(order => order.ToCommandQueryModel()).ToList()
        };

        return response;
    }
} 