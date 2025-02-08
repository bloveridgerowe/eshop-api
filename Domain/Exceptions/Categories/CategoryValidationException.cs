namespace Domain.Exceptions.Categories;

public class CategoryValidationException : DomainException
{
    public CategoryValidationException(String message) : base(message)
    {
    }
} 