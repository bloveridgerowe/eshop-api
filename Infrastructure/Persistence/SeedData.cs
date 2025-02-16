using Domain.Aggregates.Basket;
using Domain.Aggregates.Orders;
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
    private readonly IOrderRepository _orderRepository;
    private readonly IBasketRepository _basketRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly UserManager<ApplicationUser> _userManager;

    private readonly List<Category> _categories = [];
    private readonly List<Product> _products = [];
    private readonly List<Customer> _customers = [];

    public SeedData(
        ICategoryRepository categoryRepository,
        IProductRepository productRepository, 
        ICustomerRepository customerRepository, 
        IOrderRepository orderRepository, 
        IBasketRepository basketRepository,
        IUnitOfWork unitOfWork, 
        UserManager<ApplicationUser> userManager)
    {
        _categoryRepository = categoryRepository;
        _productRepository = productRepository;
        _customerRepository = customerRepository;
        _orderRepository = orderRepository;
        _basketRepository = basketRepository;
        _unitOfWork = unitOfWork;
        _userManager = userManager;
    }

    public async Task SeedAsync()
    {
        await CreateCategoriesAsync();
        await CreateProductsAsync();
        await CreateCustomersAsync();
    }

    private async Task CreateCategoriesAsync()
    {
        _categories.AddRange([
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
        ]);
        
        foreach (Category category in _categories)
        {
            await _categoryRepository.SaveAsync(category);
        }
        
        await _unitOfWork.CommitAsync();
    }

    private async Task CreateProductsAsync()
    {
        // Electronics
        _products.AddRange([
            CreateProduct("Wireless Earbuds Pro", 154.23M, 10, "electronics0", _categories[0]),
            CreateProduct("Smart Watch Series X", 286.50M, 5, "electronics1", _categories[0]),
            CreateProduct("4K Gaming Monitor", 399.99M, 20, "electronics2", _categories[0]),
            CreateProduct("Mechanical Keyboard", 129.99M, 15, "electronics3", _categories[0]),
            CreateProduct("Bluetooth Speaker", 79.99M, 25, "electronics4", _categories[0]),
            CreateProduct("Noise-Canceling Headphones", 299.99M, 12, "electronics5", _categories[0]),
            CreateProduct("Ultra HD Webcam", 159.99M, 8, "electronics6", _categories[0]),
            CreateProduct("Gaming Mouse", 49.99M, 30, "electronics7", _categories[0]),
            CreateProduct("Portable Power Bank", 39.99M, 50, "electronics8", _categories[0]),
            CreateProduct("Smart Home Hub", 89.99M, 18, "electronics9", _categories[0]),
            CreateProduct("Wireless Charging Pad", 29.99M, 40, "electronics10", _categories[0]),
            CreateProduct("Mini Projector", 199.99M, 15, "electronics11", _categories[0]),
            CreateProduct("Smart Doorbell", 149.99M, 25, "electronics12", _categories[0]),
            CreateProduct("USB-C Hub", 45.99M, 35, "electronics13", _categories[0]),
            CreateProduct("Robot Vacuum", 299.99M, 10, "electronics14", _categories[0])
        ]);

        // Home And Living
        _products.AddRange([
            CreateProduct("Modern Desk Lamp", 45.99M, 15, "home0", _categories[1]),
            CreateProduct("Ergonomic Office Chair", 145.99M, 5, "home1", _categories[1]),
            CreateProduct("Minimalist Wall Clock", 35.49M, 10, "home2", _categories[1]),
            CreateProduct("Smart LED Bulb", 25.75M, 20, "home3", _categories[1]),
            CreateProduct("Ceramic Plant Pot", 19.99M, 30, "home4", _categories[1]),
            CreateProduct("Throw Pillow Set", 55.99M, 12, "home5", _categories[1]),
            CreateProduct("Bamboo Organizer", 25.49M, 25, "home6", _categories[1]),
            CreateProduct("Air Purifier", 89.99M, 8, "home7", _categories[1]),
            CreateProduct("Coffee Table", 199.99M, 7, "home8", _categories[1]),
            CreateProduct("Wall Shelf Unit", 115.49M, 6, "home9", _categories[1]),
            CreateProduct("Cozy Throw Blanket", 39.99M, 20, "home10", _categories[1]),
            CreateProduct("Scented Candle Set", 29.99M, 30, "home11", _categories[1]),
            CreateProduct("Storage Ottoman", 79.99M, 15, "home12", _categories[1]),
            CreateProduct("Decorative Mirror", 89.99M, 10, "home13", _categories[1]),
            CreateProduct("Area Rug 5x7", 129.99M, 8, "home14", _categories[1])
        ]);

        // Accessories
        _products.AddRange([
            CreateProduct("Leather Wallet", 29.99M, 20, "accessories0", _categories[2]),
            CreateProduct("Sunglasses", 49.99M, 15, "accessories1", _categories[2]),
            CreateProduct("Canvas Backpack", 65.49M, 10, "accessories2", _categories[2]),
            CreateProduct("Watch Band", 19.99M, 25, "accessories3", _categories[2]),
            CreateProduct("Phone Case", 12.99M, 50, "accessories4", _categories[2]),
            CreateProduct("Designer Belt", 45.99M, 20, "accessories5", _categories[2]),
            CreateProduct("Silver Necklace", 79.99M, 15, "accessories6", _categories[2]),
            CreateProduct("Laptop Sleeve", 25.99M, 30, "accessories7", _categories[2]),
            CreateProduct("Travel Organizer", 34.99M, 25, "accessories8", _categories[2]),
            CreateProduct("Wireless Earbuds Case", 15.99M, 40, "accessories9", _categories[2])
        ]);

        // Fashion
        _products.AddRange([
            CreateProduct("Denim Jacket", 89.99M, 15, "fashion0", _categories[3]),
            CreateProduct("Cotton T-Shirt", 24.99M, 50, "fashion1", _categories[3]),
            CreateProduct("Leather Boots", 159.99M, 10, "fashion2", _categories[3]),
            CreateProduct("Wool Sweater", 79.99M, 20, "fashion3", _categories[3]),
            CreateProduct("Casual Sneakers", 69.99M, 25, "fashion4", _categories[3]),
            CreateProduct("Summer Dress", 49.99M, 30, "fashion5", _categories[3]),
            CreateProduct("Slim Fit Jeans", 59.99M, 35, "fashion6", _categories[3]),
            CreateProduct("Winter Scarf", 29.99M, 40, "fashion7", _categories[3]),
            CreateProduct("Leather Belt", 39.99M, 45, "fashion8", _categories[3]),
            CreateProduct("Athletic Socks", 14.99M, 60, "fashion9", _categories[3])
        ]);

        // Books
        _products.AddRange([
            CreateProduct("Bestseller Novel", 19.99M, 30, "books0", _categories[4]),
            CreateProduct("Cookbook Collection", 34.99M, 20, "books1", _categories[4]),
            CreateProduct("Self-Help Guide", 24.99M, 25, "books2", _categories[4]),
            CreateProduct("Children's Story Book", 14.99M, 40, "books3", _categories[4]),
            CreateProduct("History Encyclopedia", 49.99M, 15, "books4", _categories[4]),
            CreateProduct("Science Fiction Series", 29.99M, 35, "books5", _categories[4]),
            CreateProduct("Art Coffee Table Book", 59.99M, 10, "books6", _categories[4]),
            CreateProduct("Business Strategy Guide", 39.99M, 20, "books7", _categories[4]),
            CreateProduct("Travel Photography Book", 44.99M, 15, "books8", _categories[4]),
            CreateProduct("Classic Literature Set", 89.99M, 10, "books9", _categories[4])
        ]);

        // Sports
        _products.AddRange([
            CreateProduct("Yoga Mat", 29.99M, 40, "sports0", _categories[5]),
            CreateProduct("Resistance Bands Set", 24.99M, 50, "sports1", _categories[5]),
            CreateProduct("Basketball", 19.99M, 30, "sports2", _categories[5]),
            CreateProduct("Tennis Racket", 89.99M, 15, "sports3", _categories[5]),
            CreateProduct("Dumbbell Set", 149.99M, 20, "sports4", _categories[5]),
            CreateProduct("Running Shoes", 129.99M, 25, "sports5", _categories[5]),
            CreateProduct("Sports Water Bottle", 14.99M, 60, "sports6", _categories[5]),
            CreateProduct("Fitness Tracker", 79.99M, 30, "sports7", _categories[5]),
            CreateProduct("Jump Rope", 9.99M, 70, "sports8", _categories[5]),
            CreateProduct("Gym Bag", 39.99M, 35, "sports9", _categories[5])
        ]);

        // Gaming
        _products.AddRange([
            CreateProduct("Gaming Console", 499.99M, 10, "gaming0", _categories[6]),
            CreateProduct("Wireless Controller", 59.99M, 30, "gaming1", _categories[6]),
            CreateProduct("Gaming Headset", 89.99M, 25, "gaming2", _categories[6]),
            CreateProduct("Gaming Chair", 199.99M, 15, "gaming3", _categories[6]),
            CreateProduct("RGB Mouse Pad", 29.99M, 40, "gaming4", _categories[6])
        ]);

        // Outdoor
        _products.AddRange([
            CreateProduct("Camping Tent", 199.99M, 10, "outdoor0", _categories[7]),
            CreateProduct("Hiking Backpack", 89.99M, 20, "outdoor1", _categories[7]),
            CreateProduct("Sleeping Bag", 69.99M, 25, "outdoor2", _categories[7]),
            CreateProduct("Portable Grill", 149.99M, 15, "outdoor3", _categories[7]),
            CreateProduct("LED Lantern", 34.99M, 35, "outdoor4", _categories[7])
        ]);

        // Beauty
        _products.AddRange([
            CreateProduct("Skincare Set", 89.99M, 20, "beauty0", _categories[8]),
            CreateProduct("Hair Dryer", 59.99M, 25, "beauty1", _categories[8]),
            CreateProduct("Makeup Palette", 49.99M, 30, "beauty2", _categories[8]),
            CreateProduct("Electric Razor", 129.99M, 15, "beauty3", _categories[8]),
            CreateProduct("Nail Care Kit", 39.99M, 35, "beauty4", _categories[8])
        ]);

        // Kitchen
        _products.AddRange([
            CreateProduct("Stand Mixer", 299.99M, 10, "kitchen0", _categories[9]),
            CreateProduct("Knife Set", 149.99M, 15, "kitchen1", _categories[9]),
            CreateProduct("Coffee Maker", 89.99M, 20, "kitchen2", _categories[9]),
            CreateProduct("Food Processor", 179.99M, 12, "kitchen3", _categories[9]),
            CreateProduct("Cooking Utensil Set", 49.99M, 30, "kitchen4", _categories[9])
        ]);

        // Toys
        _products.AddRange([
            CreateProduct("Building Blocks Set", 49.99M, 25, "toys0", _categories[10]),
            CreateProduct("Remote Control Car", 89.99M, 15, "toys1", _categories[10]),
            CreateProduct("Educational Robot Kit", 129.99M, 10, "toys2", _categories[10]),
            CreateProduct("Board Game Collection", 39.99M, 30, "toys3", _categories[10]),
            CreateProduct("Plush Animal Set", 24.99M, 40, "toys4", _categories[10]),
            CreateProduct("Art and Craft Kit", 34.99M, 20, "toys5", _categories[10]),
            CreateProduct("Science Experiment Set", 44.99M, 15, "toys6", _categories[10]),
            CreateProduct("Dollhouse", 79.99M, 8, "toys7", _categories[10]),
            CreateProduct("Musical Toy Piano", 59.99M, 12, "toys8", _categories[10]),
            CreateProduct("Action Figure Set", 29.99M, 35, "toys9", _categories[10])
        ]);

        // Art
        _products.AddRange([
            CreateProduct("Professional Paint Set", 79.99M, 20, "art0", _categories[11]),
            CreateProduct("Drawing Tablet", 199.99M, 10, "art1", _categories[11]),
            CreateProduct("Canvas Pack", 34.99M, 30, "art2", _categories[11]),
            CreateProduct("Sketching Pencils Set", 24.99M, 40, "art3", _categories[11]),
            CreateProduct("Easel Stand", 89.99M, 15, "art4", _categories[11]),
            CreateProduct("Pottery Wheel Kit", 299.99M, 5, "art5", _categories[11]),
            CreateProduct("Calligraphy Set", 44.99M, 25, "art6", _categories[11]),
            CreateProduct("Sculpture Tools", 59.99M, 20, "art7", _categories[11]),
            CreateProduct("Art Storage Box", 49.99M, 30, "art8", _categories[11]),
            CreateProduct("Digital Art Software", 149.99M, 15, "art9", _categories[11])
        ]);

        // Pets
        _products.AddRange([
            CreateProduct("Pet Bed Deluxe", 69.99M, 20, "pets0", _categories[12]),
            CreateProduct("Automatic Pet Feeder", 89.99M, 15, "pets1", _categories[12]),
            CreateProduct("Interactive Pet Toy", 29.99M, 35, "pets2", _categories[12]),
            CreateProduct("Pet Carrier", 49.99M, 25, "pets3", _categories[12]),
            CreateProduct("Pet Grooming Kit", 44.99M, 30, "pets4", _categories[12]),
            CreateProduct("Pet Health Supplements", 34.99M, 40, "pets5", _categories[12]),
            CreateProduct("Pet Training Equipment", 59.99M, 20, "pets6", _categories[12]),
            CreateProduct("Pet GPS Tracker", 79.99M, 15, "pets7", _categories[12]),
            CreateProduct("Pet Water Fountain", 39.99M, 25, "pets8", _categories[12]),
            CreateProduct("Pet Clothing Set", 24.99M, 45, "pets9", _categories[12])
        ]);

        // Garden
        _products.AddRange([
            CreateProduct("Garden Tool Set", 79.99M, 20, "garden0", _categories[13]),
            CreateProduct("Smart Irrigation System", 149.99M, 10, "garden1", _categories[13]),
            CreateProduct("Raised Garden Bed Kit", 129.99M, 15, "garden2", _categories[13]),
            CreateProduct("Composting Bin", 89.99M, 25, "garden3", _categories[13]),
            CreateProduct("Garden Furniture Set", 399.99M, 5, "garden4", _categories[13]),
            CreateProduct("Plant Growing Lights", 69.99M, 30, "garden5", _categories[13]),
            CreateProduct("Greenhouse Kit", 299.99M, 8, "garden6", _categories[13]),
            CreateProduct("Garden Decor Set", 59.99M, 35, "garden7", _categories[13]),
            CreateProduct("Pruning Shears Pro", 44.99M, 40, "garden8", _categories[13]),
            CreateProduct("Garden Storage Box", 119.99M, 15, "garden9", _categories[13])
        ]);

        // Music
        _products.AddRange([
            CreateProduct("Digital Piano", 599.99M, 8, "music0", _categories[14]),
            CreateProduct("Acoustic Guitar", 299.99M, 12, "music1", _categories[14]),
            CreateProduct("Electronic Drum Set", 449.99M, 6, "music2", _categories[14]),
            CreateProduct("Professional Microphone", 149.99M, 20, "music3", _categories[14]),
            CreateProduct("Music Production Software", 199.99M, 15, "music4", _categories[14]),
            CreateProduct("Violin Starter Kit", 249.99M, 10, "music5", _categories[14]),
            CreateProduct("DJ Controller", 399.99M, 8, "music6", _categories[14]),
            CreateProduct("Studio Monitors Pair", 349.99M, 10, "music7", _categories[14]),
            CreateProduct("Music Stand Pro", 39.99M, 40, "music8", _categories[14]),
            CreateProduct("Audio Interface", 179.99M, 15, "music9", _categories[14])
        ]);

        foreach (Product product in _products)
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
    
    private async Task CreateCustomersAsync()
    {
        _customers.AddRange([
            new(Guid.Parse("4da5117f-8fdd-4e3b-9ee3-edb8cd5c6701"), "tim.murphy@eshop.com", "Tim Murphy",
                new Address("Apartment 7", "Oakwood Avenue", "Maidstone", "Kent", "MD14 5AB"),
                new CardDetails("0000111122223189", "01/29", "834")),
            new(Guid.Parse("a9fd365d-38f1-4c36-8bc6-66f8e02e9899"), "joe.bloggs@test.com", "Joe Bloggs",
                new Address("Apartment 23", "Winters Road", "Canterbury", "Kent", "AB11 4GD"),
                new CardDetails("0000111122228346", "01/29", "572"))
        ]);

        foreach (Customer customer in _customers)
        {
            // Setup the customer
            
            await _customerRepository.SaveAsync(customer);
        
            IdentityResult result = await _userManager.CreateAsync(customer.ToUser(), "Password123!");
        
            if (!result.Succeeded)
            {
                throw new InvalidOperationException($"Failed to create user {customer.Email}: {String.Join(", ", result.Errors.Select(e => e.Description))}");
            }
            
            // Create pretend order 1
            
            Order order1 = new Order(Guid.NewGuid(), customer.Id, DateTime.UtcNow.AddDays(-7).AddHours(-2).AddMinutes(-12));
            
            order1.AddItems([
                new OrderItem(_products[0].Id, _products[0].Price, 1, _products[0].Name),
                new OrderItem(_products[1].Id, _products[1].Price, 2, _products[1].Name),
                new OrderItem(_products[2].Id, _products[2].Price, 3, _products[2].Name),
            ]);
            
            await _orderRepository.SaveAsync(order1);
            
            // Create pretend order 2
            
            Order order2 = new Order(Guid.NewGuid(), customer.Id, DateTime.UtcNow.AddDays(-3).AddHours(-7).AddMinutes(-45));
            
            order2.AddItems([
                new OrderItem(_products[3].Id, _products[3].Price, 1, _products[3].Name),
                new OrderItem(_products[4].Id, _products[4].Price, 2, _products[4].Name),
                new OrderItem(_products[5].Id, _products[5].Price, 3, _products[5].Name),
            ]);
            
            order2.MarkAsShipped();
            
            await _orderRepository.SaveAsync(order2);
            
            // Create pretend order 3
            
            Order order3 = new Order(Guid.NewGuid(), customer.Id);
            
            order3.AddItems([
                new OrderItem(_products[6].Id, _products[6].Price, 1, _products[6].Name),
                new OrderItem(_products[7].Id, _products[7].Price, 2, _products[7].Name),
                new OrderItem(_products[8].Id, _products[8].Price, 3, _products[8].Name),
            ]);
            
            order3.MarkAsShipped();
            order3.MarkAsDelivered();
            
            await _orderRepository.SaveAsync(order3);
            
            // Add some items to the basket

            Basket basket = new Basket(Guid.NewGuid(), customer.Id);
            
            basket.AddItems([
                new BasketItem(_products[9].Id, 4, 89.99M, _products[9].Name),
                new BasketItem(_products[10].Id, 2, 29.99M, _products[10].Name),
                new BasketItem(_products[11].Id, 1, 199.99M, _products[11].Name),
            ]);
            
            await _basketRepository.SaveAsync(basket);
        }

        await _unitOfWork.CommitAsync();
    }
}
