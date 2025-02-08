using Domain.Exceptions.Categories;

namespace Domain.Entities;

public class Category : Entity
{
    private const Int32 MaxNameLength = 50;
    
    public String Name { get; private set; }

    public Category(Guid id, String name)
        : base(id)
    {
        ValidateName(name);
        Name = name;
    }

    public void UpdateName(String newName)
    {
        ValidateName(newName);
        Name = newName;
    }
    
    private static void ValidateName(String name)
    {
        if (String.IsNullOrWhiteSpace(name))
        {
            throw new CategoryValidationException("Category name cannot be empty");
        }
        
        if (name.Length > MaxNameLength)
        {
            throw new CategoryValidationException($"Category name cannot be longer than {MaxNameLength} characters");
        }
        
        if (!char.IsLetter(name[0]))
        {
            throw new CategoryValidationException("Category name must start with a letter");
        }
    }
}