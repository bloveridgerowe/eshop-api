using Application.Mappers;
using Domain.Aggregates.Basket;
using Domain.Repositories;
using MediatR;

namespace Application.Queries.Baskets;

public class GetBasketDetailsQueryHandler : IRequestHandler<GetBasketDetailsQuery, GetBasketDetailsQueryResponse>
{
    private readonly IBasketRepository _basketRepository;
    private readonly IUnitOfWork _unitOfWork;

    public GetBasketDetailsQueryHandler(IBasketRepository basketRepository, IUnitOfWork unitOfWork)
    {
        _basketRepository = basketRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<GetBasketDetailsQueryResponse> Handle(GetBasketDetailsQuery request, CancellationToken cancellationToken)
    {
        // TODO: Add pagination
        
        Basket? basket = await _basketRepository.FindByCustomerIdAsync(request.CustomerId);

        if (basket is null)
        {
            basket = new Basket(Guid.NewGuid(), request.CustomerId);
            await _unitOfWork.CommitAsync();
        }

        GetBasketDetailsQueryResponse response = new GetBasketDetailsQueryResponse
        {
            BasketDetails = basket.ToQueryModel()
        };

        return response;
    }
} 