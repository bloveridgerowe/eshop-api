using Domain.Exceptions.Products;

namespace Domain.Entities;

public class Product : Entity
{
    private const Int32 MaxNameLength = 200;
    private const Decimal MinPrice = 0.01m;
    
    public String Name { get; private set; }
    public String Description { get; private set; }
    public Decimal Price { get; private set; }
    public Int32 Stock { get; private set; }
    public Boolean Featured { get; private set; }
    public String ImageUrl { get; private set; }
    public List<String> Categories { get; private init; }

    public Product(Guid id, String name, String description, Boolean featured, String imageUrl, Decimal price, Int32 stock, List<String> categories)
        : base(id)
    {
        ValidateName(name);
        ValidateDescription(description);
        ValidatePrice(price);
        ValidateInitialStock(stock);
        ValidateImageUrl(imageUrl);
        ValidateCategories(categories);
        
        Name = name;
        Description = description;
        Featured = featured;
        Price = price;
        ImageUrl = imageUrl;
        Stock = stock;
        Categories = [..categories];
    }

    public void SetFeatured(Boolean featured)
    {
        Featured = featured;
    }
    
    public void Rename(String name)
    {
        ValidateName(name);
        Name = name;
    }
    
    public void ValidateStock(Int32 quantity)
    {
        if (Stock < quantity)
        {
            throw new InsufficientStockException(Name, quantity, Stock);
        }
    }
    
    public void UpdatePrice(Decimal price)
    {
        ValidatePrice(price);
        Price = price;
    }

    public void SetStock(Int32 quantity)
    {
        ValidatePositiveQuantity(quantity);
        Stock = quantity;
    }

    public void SetImage(String imageUrl)
    {
        ValidateImageUrl(imageUrl);
        ImageUrl = imageUrl;
    }

    public void AddStock(Int32 quantity)
    {
        ValidatePositiveQuantity(quantity);
        
        Int32 newStock = Stock + quantity;
        if (newStock < 0)
        {
            throw new InvalidOperationException("Adding stock would exceed maximum stock level");
        }
        
        Stock = newStock;
    }

    public void RemoveStock(Int32 quantity)
    {
        ValidatePositiveQuantity(quantity);
        
        if (Stock < quantity)
        {
            throw new InsufficientStockException(Name, quantity, Stock);
        }

        Stock -= quantity;
    }
    
    private static void ValidateName(String name)
    {
        if (String.IsNullOrWhiteSpace(name))
        {
            throw new ProductValidationException("Product name cannot be empty");
        }
        
        if (name.Length > MaxNameLength)
        {
            throw new ProductValidationException($"Product name cannot be longer than {MaxNameLength} characters");
        }
    }
    
    private static void ValidateDescription(String description)
    {
        if (String.IsNullOrWhiteSpace(description))
        {
            throw new ProductValidationException("Product description cannot be empty");
        }
        
        // if (description.Length > MaxNameLength)
        // {
        //     throw new ProductValidationException($"Product description cannot be longer than {MaxNameLength} characters");
        // }
    }
    
    private static void ValidatePrice(Decimal price)
    {
        if (price < MinPrice)
        {
            throw new ProductValidationException($"Price must be at least {MinPrice:C}");
        }
    }
    
    private static void ValidateInitialStock(Int32 stock)
    {
        if (stock < 0)
        {
            throw new ProductValidationException("Stock cannot be negative");
        }
    }
    
    private static void ValidateImageUrl(String imageUrl)
    {
        if (String.IsNullOrWhiteSpace(imageUrl))
        {
            throw new ProductValidationException("Image URL cannot be empty");
        }
        
        if (!Uri.IsWellFormedUriString(imageUrl, UriKind.Absolute))
        {
            throw new ProductValidationException("Invalid image URL format");
        }
    }
    
    private static void ValidateCategories(List<String> categories)
    {
        if (categories == null || !categories.Any())
        {
            throw new ProductValidationException("Product must have at least one category");
        }
        
        if (categories.Any(String.IsNullOrWhiteSpace))
        {
            throw new ProductValidationException("Categories cannot contain empty values");
        }
    }
    
    private static void ValidatePositiveQuantity(Int32 quantity)
    {
        if (quantity < 0)
        {
            throw new ProductValidationException("Quantity cannot be negative");
        }
    }
}