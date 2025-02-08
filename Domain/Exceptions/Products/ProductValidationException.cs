namespace Domain.Exceptions.Products;

public class ProductValidationException : DomainException
{
    public ProductValidationException(String message) : base(message)
    {
    }
} 