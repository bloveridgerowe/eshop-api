using Domain.Aggregates.Basket;
using Domain.Aggregates.Orders;
using Domain.Entities;
using Domain.Exceptions;
using Domain.Exceptions.Customers;
using Domain.Exceptions.Orders;
using Domain.Repositories;
using MediatR;

namespace Application.Commands.Orders;

public class ConvertBasketToOrderCommandHandler : IRequestHandler<ConvertBasketToOrderCommand, ConvertBasketToOrderCommandResponse>
{
    private readonly IBasketRepository _basketRepository;
    private readonly IProductRepository _productRepository;
    private readonly IOrderRepository _orderRepository;
    private readonly ICustomerRepository _customerRepository;
    private readonly IUnitOfWork _unitOfWork;
    
    public ConvertBasketToOrderCommandHandler(
        IBasketRepository basketRepository,
        IProductRepository productRepository,
        IOrderRepository orderRepository,
        ICustomerRepository customerRepository, IUnitOfWork unitOfWork)
    {
        _basketRepository = basketRepository;
        _productRepository = productRepository;
        _orderRepository = orderRepository;
        _customerRepository = customerRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ConvertBasketToOrderCommandResponse> Handle(ConvertBasketToOrderCommand request, CancellationToken cancellationToken)
    {
        ValidateRequest(request);

        Basket? basket = await _basketRepository.FindByIdAsync(request.BasketId);
        if (basket == null)
        {
            throw new ArgumentException($"Basket with ID {request.BasketId} not found.", nameof(request.BasketId));
        }

        if (!basket.Items.Any())
        {
            throw new ArgumentException("Cannot convert an empty basket to an order.", nameof(request.BasketId));
        }

        Customer? customer = await _customerRepository.FindByIdAsync(request.CustomerId);
        if (customer == null)
        {
            throw new ArgumentException($"Customer with ID {request.CustomerId} not found.", nameof(request.CustomerId));
        }

        if (customer.Address is null || customer.CardDetails is null)
        {
            throw new CustomerDetailsNotFoundException(request.CustomerId);
        }

        Order order = new Order(Guid.NewGuid(), request.CustomerId, DateTime.UtcNow);

        foreach (BasketItem basketItem in basket.Items)
        {
            Product? product = await _productRepository.FindByIdAsync(basketItem.ProductId);
            if (product == null)
            {
                throw new ArgumentException($"Product with ID {basketItem.ProductId} not found.", nameof(request.BasketId));
            }

            product.ValidateStock(basketItem.Quantity);

            if (basketItem.Price != product.Price)
            {
                throw new PriceChangeException(product.Id, basketItem.Price, product.Price);
            }

            OrderItem orderItem = new OrderItem(basketItem.ProductId, basketItem.Price, basketItem.Quantity, basketItem.Name);
            order.AddItem(orderItem);
            
            product.RemoveStock(basketItem.Quantity);
            await _productRepository.SaveAsync(product);
        }

        basket.ClearItems();

        await _orderRepository.SaveAsync(order);
        await _basketRepository.SaveAsync(basket);

        await _unitOfWork.CommitAsync();

        return new ConvertBasketToOrderCommandResponse
        {
            OrderId = order.Id
        };
    }
    
    private static void ValidateRequest(ConvertBasketToOrderCommand request)
    {
        if (request.CustomerId == Guid.Empty)
        {
            throw new ArgumentException("Customer ID cannot be empty", nameof(request.CustomerId));
        }

        if (request.BasketId == Guid.Empty)
        {
            throw new ArgumentException("Basket ID cannot be empty", nameof(request.BasketId));
        }
    }
} 