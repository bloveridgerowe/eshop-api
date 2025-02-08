using Domain.Aggregates.Basket;
using Domain.Repositories;
using MediatR;

namespace Application.Commands.Baskets;

public class ClearBasketCommandHandler : IRequestHandler<ClearBasketCommand>
{
    private readonly IBasketRepository _basketRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ClearBasketCommandHandler(IBasketRepository basketRepository, IUnitOfWork unitOfWork)
    {
        _basketRepository = basketRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(ClearBasketCommand request, CancellationToken cancellationToken)
    {
        ValidateRequest(request);
        
        Basket? basket = await _basketRepository.FindByCustomerIdAsync(request.CustomerId);

        if (basket is null)
        {
            basket = new Basket(Guid.NewGuid(), request.CustomerId);
            await _unitOfWork.CommitAsync();
        }
        
        basket.ClearItems();

        await _basketRepository.SaveAsync(basket);
        await _unitOfWork.CommitAsync();
    }

    private void ValidateRequest(ClearBasketCommand request)
    {
        if (request.CustomerId == Guid.Empty)
        {
            throw new ArgumentException("Customer ID cannot be empty", nameof(request.CustomerId));
        }
    }
}