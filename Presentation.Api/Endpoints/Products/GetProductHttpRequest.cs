using Microsoft.AspNetCore.Mvc;

namespace Presentation.Api.Endpoints.Products;

public class GetProductHttpRequest
{
    [FromRoute] public Guid ProductId { get; set; }
}