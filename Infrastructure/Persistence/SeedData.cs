using Domain.Entities;
using Domain.Repositories;
using Domain.ValueObjects;
using Infrastructure.Persistence.Entities.Users;
using Infrastructure.Persistence.Mappers;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Persistence;

public class SeedData
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IProductRepository _productRepository;
    private readonly ICustomerRepository _customerRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly UserManager<ApplicationUser> _userManager;

    private static readonly Category[] Categories =
    [
        new(Guid.NewGuid(), "Electronics"),
        new(Guid.NewGuid(), "Home And Living"),
        new(Guid.NewGuid(), "Accessories"),
        new(Guid.NewGuid(), "Fashion"),
        new(Guid.NewGuid(), "Books"),
        new(Guid.NewGuid(), "Sports"),
        new(Guid.NewGuid(), "Gaming"),
        new(Guid.NewGuid(), "Outdoor"),
        new(Guid.NewGuid(), "Beauty"),
        new(Guid.NewGuid(), "Kitchen"),
        new(Guid.NewGuid(), "Toys"),
        new(Guid.NewGuid(), "Art"),
        new(Guid.NewGuid(), "Pets"),
        new(Guid.NewGuid(), "Garden"),
        new(Guid.NewGuid(), "Music")
    ];

    public SeedData(ICategoryRepository categoryRepository, IProductRepository productRepository, UserManager<ApplicationUser> userManager, ICustomerRepository customerRepository, IUnitOfWork unitOfWork)
    {
        _categoryRepository = categoryRepository;
        _productRepository = productRepository;
        _userManager = userManager;
        _customerRepository = customerRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task SeedAsync()
    {
        await CreateCustomerAsync();
        await CreateCategoriesAsync();
        await CreateProductsAsync();
    }

    private async Task CreateCategoriesAsync()
    {
        foreach (Category category in Categories)
        {
            await _categoryRepository.SaveAsync(category);
        }
        
        await _unitOfWork.CommitAsync();
    }

    private async Task CreateProductsAsync()
    {
        List<Product> products = [];
        
        // Electronics
        products.AddRange([
            CreateProduct("Wireless Earbuds Pro", 154.23M, 10, "electronics0", Categories[0]),
            CreateProduct("Smart Watch Series X", 286.50M, 5, "electronics1", Categories[0]),
            CreateProduct("4K Gaming Monitor", 399.99M, 20, "electronics2", Categories[0]),
            CreateProduct("Mechanical Keyboard", 129.99M, 15, "electronics3", Categories[0]),
            CreateProduct("Bluetooth Speaker", 79.99M, 25, "electronics4", Categories[0]),
            CreateProduct("Noise-Canceling Headphones", 299.99M, 12, "electronics5", Categories[0]),
            CreateProduct("Ultra HD Webcam", 159.99M, 8, "electronics6", Categories[0]),
            CreateProduct("Gaming Mouse", 49.99M, 30, "electronics7", Categories[0]),
            CreateProduct("Portable Power Bank", 39.99M, 50, "electronics8", Categories[0]),
            CreateProduct("Smart Home Hub", 89.99M, 18, "electronics9", Categories[0]),
            CreateProduct("Wireless Charging Pad", 29.99M, 40, "electronics10", Categories[0]),
            CreateProduct("Mini Projector", 199.99M, 15, "electronics11", Categories[0]),
            CreateProduct("Smart Doorbell", 149.99M, 25, "electronics12", Categories[0]),
            CreateProduct("USB-C Hub", 45.99M, 35, "electronics13", Categories[0]),
            CreateProduct("Robot Vacuum", 299.99M, 10, "electronics14", Categories[0])
        ]);

        // Home And Living
        products.AddRange([
            CreateProduct("Modern Desk Lamp", 45.99M, 15, "home0", Categories[1]),
            CreateProduct("Ergonomic Office Chair", 145.99M, 5, "home1", Categories[1]),
            CreateProduct("Minimalist Wall Clock", 35.49M, 10, "home2", Categories[1]),
            CreateProduct("Smart LED Bulb", 25.75M, 20, "home3", Categories[1]),
            CreateProduct("Ceramic Plant Pot", 19.99M, 30, "home4", Categories[1]),
            CreateProduct("Throw Pillow Set", 55.99M, 12, "home5", Categories[1]),
            CreateProduct("Bamboo Organizer", 25.49M, 25, "home6", Categories[1]),
            CreateProduct("Air Purifier", 89.99M, 8, "home7", Categories[1]),
            CreateProduct("Coffee Table", 199.99M, 7, "home8", Categories[1]),
            CreateProduct("Wall Shelf Unit", 115.49M, 6, "home9", Categories[1]),
            CreateProduct("Cozy Throw Blanket", 39.99M, 20, "home10", Categories[1]),
            CreateProduct("Scented Candle Set", 29.99M, 30, "home11", Categories[1]),
            CreateProduct("Storage Ottoman", 79.99M, 15, "home12", Categories[1]),
            CreateProduct("Decorative Mirror", 89.99M, 10, "home13", Categories[1]),
            CreateProduct("Area Rug 5x7", 129.99M, 8, "home14", Categories[1])
        ]);

        // Accessories
        products.AddRange([
            CreateProduct("Leather Wallet", 29.99M, 20, "accessories0", Categories[2]),
            CreateProduct("Sunglasses", 49.99M, 15, "accessories1", Categories[2]),
            CreateProduct("Canvas Backpack", 65.49M, 10, "accessories2", Categories[2]),
            CreateProduct("Watch Band", 19.99M, 25, "accessories3", Categories[2]),
            CreateProduct("Phone Case", 12.99M, 50, "accessories4", Categories[2]),
            CreateProduct("Designer Belt", 45.99M, 20, "accessories5", Categories[2]),
            CreateProduct("Silver Necklace", 79.99M, 15, "accessories6", Categories[2]),
            CreateProduct("Laptop Sleeve", 25.99M, 30, "accessories7", Categories[2]),
            CreateProduct("Travel Organizer", 34.99M, 25, "accessories8", Categories[2]),
            CreateProduct("Wireless Earbuds Case", 15.99M, 40, "accessories9", Categories[2])
        ]);

        // Fashion
        products.AddRange([
            CreateProduct("Denim Jacket", 89.99M, 15, "fashion0", Categories[3]),
            CreateProduct("Cotton T-Shirt", 24.99M, 50, "fashion1", Categories[3]),
            CreateProduct("Leather Boots", 159.99M, 10, "fashion2", Categories[3]),
            CreateProduct("Wool Sweater", 79.99M, 20, "fashion3", Categories[3]),
            CreateProduct("Casual Sneakers", 69.99M, 25, "fashion4", Categories[3]),
            CreateProduct("Summer Dress", 49.99M, 30, "fashion5", Categories[3]),
            CreateProduct("Slim Fit Jeans", 59.99M, 35, "fashion6", Categories[3]),
            CreateProduct("Winter Scarf", 29.99M, 40, "fashion7", Categories[3]),
            CreateProduct("Leather Belt", 39.99M, 45, "fashion8", Categories[3]),
            CreateProduct("Athletic Socks", 14.99M, 60, "fashion9", Categories[3])
        ]);

        // Books
        products.AddRange([
            CreateProduct("Bestseller Novel", 19.99M, 30, "books0", Categories[4]),
            CreateProduct("Cookbook Collection", 34.99M, 20, "books1", Categories[4]),
            CreateProduct("Self-Help Guide", 24.99M, 25, "books2", Categories[4]),
            CreateProduct("Children's Story Book", 14.99M, 40, "books3", Categories[4]),
            CreateProduct("History Encyclopedia", 49.99M, 15, "books4", Categories[4]),
            CreateProduct("Science Fiction Series", 29.99M, 35, "books5", Categories[4]),
            CreateProduct("Art Coffee Table Book", 59.99M, 10, "books6", Categories[4]),
            CreateProduct("Business Strategy Guide", 39.99M, 20, "books7", Categories[4]),
            CreateProduct("Travel Photography Book", 44.99M, 15, "books8", Categories[4]),
            CreateProduct("Classic Literature Set", 89.99M, 10, "books9", Categories[4])
        ]);

        // Sports
        products.AddRange([
            CreateProduct("Yoga Mat", 29.99M, 40, "sports0", Categories[5]),
            CreateProduct("Resistance Bands Set", 24.99M, 50, "sports1", Categories[5]),
            CreateProduct("Basketball", 19.99M, 30, "sports2", Categories[5]),
            CreateProduct("Tennis Racket", 89.99M, 15, "sports3", Categories[5]),
            CreateProduct("Dumbbell Set", 149.99M, 20, "sports4", Categories[5]),
            CreateProduct("Running Shoes", 129.99M, 25, "sports5", Categories[5]),
            CreateProduct("Sports Water Bottle", 14.99M, 60, "sports6", Categories[5]),
            CreateProduct("Fitness Tracker", 79.99M, 30, "sports7", Categories[5]),
            CreateProduct("Jump Rope", 9.99M, 70, "sports8", Categories[5]),
            CreateProduct("Gym Bag", 39.99M, 35, "sports9", Categories[5])
        ]);

        // Gaming
        products.AddRange([
            CreateProduct("Gaming Console", 499.99M, 10, "gaming0", Categories[6]),
            CreateProduct("Wireless Controller", 59.99M, 30, "gaming1", Categories[6]),
            CreateProduct("Gaming Headset", 89.99M, 25, "gaming2", Categories[6]),
            CreateProduct("Gaming Chair", 199.99M, 15, "gaming3", Categories[6]),
            CreateProduct("RGB Mouse Pad", 29.99M, 40, "gaming4", Categories[6])
        ]);

        // Outdoor
        products.AddRange([
            CreateProduct("Camping Tent", 199.99M, 10, "outdoor0", Categories[7]),
            CreateProduct("Hiking Backpack", 89.99M, 20, "outdoor1", Categories[7]),
            CreateProduct("Sleeping Bag", 69.99M, 25, "outdoor2", Categories[7]),
            CreateProduct("Portable Grill", 149.99M, 15, "outdoor3", Categories[7]),
            CreateProduct("LED Lantern", 34.99M, 35, "outdoor4", Categories[7])
        ]);

        // Beauty
        products.AddRange([
            CreateProduct("Skincare Set", 89.99M, 20, "beauty0", Categories[8]),
            CreateProduct("Hair Dryer", 59.99M, 25, "beauty1", Categories[8]),
            CreateProduct("Makeup Palette", 49.99M, 30, "beauty2", Categories[8]),
            CreateProduct("Electric Razor", 129.99M, 15, "beauty3", Categories[8]),
            CreateProduct("Nail Care Kit", 39.99M, 35, "beauty4", Categories[8])
        ]);

        // Kitchen
        products.AddRange([
            CreateProduct("Stand Mixer", 299.99M, 10, "kitchen0", Categories[9]),
            CreateProduct("Knife Set", 149.99M, 15, "kitchen1", Categories[9]),
            CreateProduct("Coffee Maker", 89.99M, 20, "kitchen2", Categories[9]),
            CreateProduct("Food Processor", 179.99M, 12, "kitchen3", Categories[9]),
            CreateProduct("Cooking Utensil Set", 49.99M, 30, "kitchen4", Categories[9])
        ]);

        // Toys
        products.AddRange([
            CreateProduct("Building Blocks Set", 49.99M, 25, "toys0", Categories[10]),
            CreateProduct("Remote Control Car", 89.99M, 15, "toys1", Categories[10]),
            CreateProduct("Educational Robot Kit", 129.99M, 10, "toys2", Categories[10]),
            CreateProduct("Board Game Collection", 39.99M, 30, "toys3", Categories[10]),
            CreateProduct("Plush Animal Set", 24.99M, 40, "toys4", Categories[10]),
            CreateProduct("Art and Craft Kit", 34.99M, 20, "toys5", Categories[10]),
            CreateProduct("Science Experiment Set", 44.99M, 15, "toys6", Categories[10]),
            CreateProduct("Dollhouse", 79.99M, 8, "toys7", Categories[10]),
            CreateProduct("Musical Toy Piano", 59.99M, 12, "toys8", Categories[10]),
            CreateProduct("Action Figure Set", 29.99M, 35, "toys9", Categories[10])
        ]);

        // Art
        products.AddRange([
            CreateProduct("Professional Paint Set", 79.99M, 20, "art0", Categories[11]),
            CreateProduct("Drawing Tablet", 199.99M, 10, "art1", Categories[11]),
            CreateProduct("Canvas Pack", 34.99M, 30, "art2", Categories[11]),
            CreateProduct("Sketching Pencils Set", 24.99M, 40, "art3", Categories[11]),
            CreateProduct("Easel Stand", 89.99M, 15, "art4", Categories[11]),
            CreateProduct("Pottery Wheel Kit", 299.99M, 5, "art5", Categories[11]),
            CreateProduct("Calligraphy Set", 44.99M, 25, "art6", Categories[11]),
            CreateProduct("Sculpture Tools", 59.99M, 20, "art7", Categories[11]),
            CreateProduct("Art Storage Box", 49.99M, 30, "art8", Categories[11]),
            CreateProduct("Digital Art Software", 149.99M, 15, "art9", Categories[11])
        ]);

        // Pets
        products.AddRange([
            CreateProduct("Pet Bed Deluxe", 69.99M, 20, "pets0", Categories[12]),
            CreateProduct("Automatic Pet Feeder", 89.99M, 15, "pets1", Categories[12]),
            CreateProduct("Interactive Pet Toy", 29.99M, 35, "pets2", Categories[12]),
            CreateProduct("Pet Carrier", 49.99M, 25, "pets3", Categories[12]),
            CreateProduct("Pet Grooming Kit", 44.99M, 30, "pets4", Categories[12]),
            CreateProduct("Pet Health Supplements", 34.99M, 40, "pets5", Categories[12]),
            CreateProduct("Pet Training Equipment", 59.99M, 20, "pets6", Categories[12]),
            CreateProduct("Pet GPS Tracker", 79.99M, 15, "pets7", Categories[12]),
            CreateProduct("Pet Water Fountain", 39.99M, 25, "pets8", Categories[12]),
            CreateProduct("Pet Clothing Set", 24.99M, 45, "pets9", Categories[12])
        ]);

        // Garden
        products.AddRange([
            CreateProduct("Garden Tool Set", 79.99M, 20, "garden0", Categories[13]),
            CreateProduct("Smart Irrigation System", 149.99M, 10, "garden1", Categories[13]),
            CreateProduct("Raised Garden Bed Kit", 129.99M, 15, "garden2", Categories[13]),
            CreateProduct("Composting Bin", 89.99M, 25, "garden3", Categories[13]),
            CreateProduct("Garden Furniture Set", 399.99M, 5, "garden4", Categories[13]),
            CreateProduct("Plant Growing Lights", 69.99M, 30, "garden5", Categories[13]),
            CreateProduct("Greenhouse Kit", 299.99M, 8, "garden6", Categories[13]),
            CreateProduct("Garden Decor Set", 59.99M, 35, "garden7", Categories[13]),
            CreateProduct("Pruning Shears Pro", 44.99M, 40, "garden8", Categories[13]),
            CreateProduct("Garden Storage Box", 119.99M, 15, "garden9", Categories[13])
        ]);

        // Music
        products.AddRange([
            CreateProduct("Digital Piano", 599.99M, 8, "music0", Categories[14]),
            CreateProduct("Acoustic Guitar", 299.99M, 12, "music1", Categories[14]),
            CreateProduct("Electronic Drum Set", 449.99M, 6, "music2", Categories[14]),
            CreateProduct("Professional Microphone", 149.99M, 20, "music3", Categories[14]),
            CreateProduct("Music Production Software", 199.99M, 15, "music4", Categories[14]),
            CreateProduct("Violin Starter Kit", 249.99M, 10, "music5", Categories[14]),
            CreateProduct("DJ Controller", 399.99M, 8, "music6", Categories[14]),
            CreateProduct("Studio Monitors Pair", 349.99M, 10, "music7", Categories[14]),
            CreateProduct("Music Stand Pro", 39.99M, 40, "music8", Categories[14]),
            CreateProduct("Audio Interface", 179.99M, 15, "music9", Categories[14])
        ]);

        foreach (Product product in products)
        {
            await _productRepository.SaveAsync(product);
        }
        
        await _unitOfWork.CommitAsync();
    }

    private Product CreateProduct(String name, Decimal price, Int32 stock, String imageId, Category category)
    {
        return new Product(
            Guid.NewGuid(),
            name,
            "Lorem ipsum odor amet, consectetuer adipiscing elit. Mollis taciti parturient nulla erat dictum ad. Laoreet nec tristique ipsum magna natoque adipiscing mollis, sodales laoreet. Class nascetur inceptos nec aliquet hac pretium facilisis. Platea ac eleifend eget; taciti sagittis mi fermentum. Tincidunt ex feugiat; tortor placerat nec bibendum. Efficitur libero morbi lacinia donec; vestibulum condimentum habitasse. Nullam vulputate etiam cubilia nulla aliquet. Quis magnis elit maecenas convallis himenaeos.\n\nRisus facilisis auctor pellentesque sed posuere. Fames facilisis nibh nulla varius massa ad felis. Leo tincidunt vehicula morbi ante natoque dictumst turpis tristique. Faucibus integer dis non mollis aenean neque conubia? Gravida lorem suspendisse cursus vivamus cubilia rhoncus pharetra laoreet? Neque sem fermentum auctor platea cursus? Adipiscing odio erat cras vitae maecenas nostra netus. Dignissim fames consectetur eu phasellus eros vehicula purus ligula potenti. Eleifend lectus vivamus consequat dignissim himenaeos tristique; habitasse mattis.\n\nHac senectus vestibulum tellus iaculis curae viverra orci. Varius arcu placerat porttitor senectus nec bibendum suspendisse sociosqu. Dictumst fringilla duis massa elementum semper. Eros efficitur finibus placerat, nam platea natoque. Class aliquam duis etiam; ullamcorper habitasse odio eleifend. Viverra magnis ac gravida maximus fusce urna imperdiet massa ultricies. Semper id sollicitudin molestie lobortis finibus sed sollicitudin dignissim ornare.",
            stock % 2 == 0,
            $"https://picsum.photos/seed/{imageId}/300/300",
            price,
            stock,
            [category.Name]
        );
    }
    
    private async Task CreateCustomerAsync()
    {
        Customer customer = new(Guid.Parse("a9fd365d-38f1-4c36-8bc6-66f8e02e9899"), "joe.bloggs@test.com", "Joe Bloggs", new Address("Flat 23", "Winters Road", "Canterbury", "Kent", "AB11 4GD"), new CardDetails("0000111122223333", "01/29", "123"));
        
        await _customerRepository.SaveAsync(customer);
        
        IdentityResult result = await _userManager.CreateAsync(customer.ToUser(), "Password123!");
        
        if (!result.Succeeded)
        {
            throw new InvalidOperationException($"Failed to create user {customer.Email}: {String.Join(", ", result.Errors.Select(e => e.Description))}");
        }
    }
}
