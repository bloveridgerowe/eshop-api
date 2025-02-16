using Domain.Entities;
using Domain.Repositories;
using Domain.ValueObjects;
using Infrastructure.Persistence.Entities.Customer;
using Infrastructure.Persistence.Mappers;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class CustomerRepository : ICustomerRepository
{
    private readonly ECommerceDbContext _context;

    public CustomerRepository(ECommerceDbContext context)
    {
        _context = context;
    }

    public async Task<Customer?> FindByIdAsync(Guid id)
    {
        CustomerEntity? entity = await _context.Customers
            .Include(c => c.CardDetails)
            .Include(c => c.Address)
            .SingleOrDefaultAsync(c => c.Id == id);

        if (entity == null)
        {
            return null;
        }

        return entity.ToDomain();
    }
    
    private CardDetails SanitizeCardDetails(CardDetails cardDetails)
    {
        String sanitizedCardNumber = $"000011112222{cardDetails.CardNumber[^4..]}";
        String sanitizedExpiry = "01/30";
        String sanitizedCvv = "000";
        
        return new CardDetails(sanitizedCardNumber, sanitizedExpiry, sanitizedCvv);
    }

    public async Task SaveAsync(Customer customer)
    {
        // Retrieve the existing customer entity from the database
        CustomerEntity? existingCustomer = await _context.Customers
            .Include(c => c.CardDetails)
            .Include(c => c.Address)
            .SingleOrDefaultAsync(c => c.Id == customer.Id);

        if (existingCustomer is null)
        {
            // Add a new customer if it doesn't exist
            await _context.Customers.AddAsync(customer.ToPersistence());
        }
        else
        {
            // Update existing customer properties
            existingCustomer.Name = customer.Name;
            
            // Handle address
            if (customer.Address is not null)
            {
                if (existingCustomer.Address is null)
                {
                    // Add new card details if none exist
                    await _context.CustomerAddress.AddAsync(customer.Address.ToPersistence(customer.Id));
                }
                else
                {
                    existingCustomer.Address.FirstLine = customer.Address.FirstLine;
                    existingCustomer.Address.SecondLine = customer.Address.SecondLine;
                    existingCustomer.Address.City = customer.Address.City;
                    existingCustomer.Address.County = customer.Address.County;
                    existingCustomer.Address.PostCode = customer.Address.PostCode;
                }
            }
            else if (existingCustomer.Address is not null)
            {
                // Remove address details if they no longer exist in the domain model
                _context.CustomerAddress.Remove(existingCustomer.Address);
            }
            
            existingCustomer.Email = customer.Email;

            // Handle card details
            if (customer.CardDetails is not null)
            {
                // This is a demonstration project and I don't want to be storing
                // any real card details, on the off-chance someone enters them.
                // So I will invalidate card details here to prevent them being stored.
                customer.UpdateCardDetails(SanitizeCardDetails(customer.CardDetails));
                
                if (existingCustomer.CardDetails is null)
                {
                    // Add new card details if none exist
                    await _context.CustomerCardDetails.AddAsync(customer.CardDetails.ToPersistence(customer.Id));
                }
                else
                {
                    // Update existing card details
                    existingCustomer.CardDetails.CardNumber = customer.CardDetails.CardNumber;
                    existingCustomer.CardDetails.ExpiryDate = customer.CardDetails.ExpiryDate;
                    existingCustomer.CardDetails.Cvv = customer.CardDetails.Cvv;
                }
            }
            else if (existingCustomer.CardDetails is not null)
            {
                // Remove card details if they no longer exist in the domain model
                _context.CustomerCardDetails.Remove(existingCustomer.CardDetails);
            }
        }

        // Changes saved using ECommerceUnitOfWork
    }
}