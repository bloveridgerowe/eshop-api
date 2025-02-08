using Domain.Aggregates.Basket;
using Domain.Entities;
using Domain.Exceptions.Products;
using Domain.Repositories;
using MediatR;

namespace Application.Commands.Baskets;

public class UpdateBasketDetailsCommandHandler : IRequestHandler<UpdateBasketDetailsCommand>
{
    private readonly IBasketRepository _basketRepository;
    private readonly IProductRepository _productRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateBasketDetailsCommandHandler(IBasketRepository basketRepository, IProductRepository productRepository, IUnitOfWork unitOfWork)
    {
        _basketRepository = basketRepository;
        _productRepository = productRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(UpdateBasketDetailsCommand request, CancellationToken cancellationToken)
    {
        ValidateRequest(request);

        Basket? basket = await _basketRepository.FindByCustomerIdAsync(request.CustomerId);

        if (basket is null)
        {
            basket = new Basket(Guid.NewGuid(), request.CustomerId);
            await _unitOfWork.CommitAsync();
        }

        foreach (UpdateBasketItemDetails itemDetails in request.Items)
        {
            Product? product = await _productRepository.FindByIdAsync(itemDetails.ProductId);

            if (product is null)
            {
                throw new ProductNotFoundException(itemDetails.ProductId);
            }
            
            BasketItem? existingItem = basket.Items.SingleOrDefault(i => i.ProductId == itemDetails.ProductId);

            if (existingItem is not null)
            {
                if (itemDetails.Quantity > 0)
                {
                    if (itemDetails.Quantity > product.Stock)
                    {
                        throw new InsufficientStockException(product.Name, itemDetails.Quantity, product.Stock);
                    }
                    
                    basket.UpdateItemQuantity(itemDetails.ProductId, itemDetails.Quantity);
                    basket.UpdateItemPrice(itemDetails.ProductId, product.Price); // update price if it's changed since adding
                }
                else
                {
                    basket.RemoveItem(itemDetails.ProductId);
                }
            }
            else if (itemDetails.Quantity > 0)
            {
                BasketItem newItem = new BasketItem(itemDetails.ProductId, itemDetails.Quantity, product.Price, product.Name);
                basket.AddItem(newItem);
            }
        }

        await _basketRepository.SaveAsync(basket);
        await _unitOfWork.CommitAsync();
    }
    
    private static void ValidateRequest(UpdateBasketDetailsCommand request)
    {
        if (request.CustomerId == Guid.Empty)
        {
            throw new ArgumentException("Basket ID cannot be empty", nameof(request.CustomerId));
        }

        foreach (UpdateBasketItemDetails item in request.Items)
        {
            if (item.ProductId == Guid.Empty)
            {
                throw new ArgumentException("Product ID cannot be empty", nameof(request.Items));
            }

            if (item.Quantity < 0)
            {
                throw new ArgumentException("Quantity cannot be negative", nameof(request.Items));
            }
        }
    }
} 