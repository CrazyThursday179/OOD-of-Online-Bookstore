using FavouriteBooks.Api.Models;
using FavouriteBooks.Api.Repositories;

namespace FavouriteBooks.Api.Infrastructure;

public class SeedDataService(
    BookRepository bookRepository,
    BookCategoryRepository categoryRepository,
    CartRepository cartRepository,
    CustomerRepository customerRepository,
    AdminRepository adminRepository,
    OrderRepository orderRepository,
    InvoiceRepository invoiceRepository,
    PaymentDetailsRepository paymentDetailsRepository,
    ReceiptRepository receiptRepository,
    ShipmentRepository shipmentRepository,
    ShipmentMethodRepository shipmentMethodRepository)
{
    public async Task SeedAsync()
    {
        if ((await categoryRepository.GetAllAsync()).Count == 0)
        {
            await categoryRepository.SaveAllAsync(DefaultCategories());
        }

        if ((await bookRepository.GetAllAsync()).Count == 0)
        {
            var categories = await categoryRepository.GetAllAsync();
            await bookRepository.SaveAllAsync(DefaultBooks(categories));
        }

        if ((await shipmentMethodRepository.GetAllAsync()).Count == 0)
        {
            await shipmentMethodRepository.SaveAllAsync(DefaultShipmentMethods());
        }

        if ((await customerRepository.GetAllAsync()).Count == 0)
        {
            await customerRepository.SaveAllAsync(DefaultCustomers());
        }

        if ((await adminRepository.GetAllAsync()).Count == 0)
        {
            await adminRepository.SaveAllAsync(DefaultAdmins());
        }

        if ((await cartRepository.GetAllAsync()).Count == 0)
        {
            await cartRepository.SaveAllAsync([]);
        }

        if ((await orderRepository.GetAllAsync()).Count == 0)
        {
            await orderRepository.SaveAllAsync([]);
        }

        if ((await invoiceRepository.GetAllAsync()).Count == 0)
        {
            await invoiceRepository.SaveAllAsync([]);
        }

        if ((await paymentDetailsRepository.GetAllAsync()).Count == 0)
        {
            await paymentDetailsRepository.SaveAllAsync([]);
        }

        if ((await receiptRepository.GetAllAsync()).Count == 0)
        {
            await receiptRepository.SaveAllAsync([]);
        }

        if ((await shipmentRepository.GetAllAsync()).Count == 0)
        {
            await shipmentRepository.SaveAllAsync([]);
        }
    }

    public static async Task InitializeAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var seeder = scope.ServiceProvider.GetRequiredService<SeedDataService>();
        await seeder.SeedAsync();
    }

    private static List<BookCategory> DefaultCategories()
    {
        return
        [
            new BookCategory
            {
                Id = Guid.Parse("97d77d8a-a5ce-49dd-b5ef-82f1676d7a00"),
                Name = "Software Architecture",
                Description = "Books about software architecture and design."
            },
            new BookCategory
            {
                Id = Guid.Parse("f3c8f6a7-626d-476a-bf4e-285516701601"),
                Name = "Programming",
                Description = "General programming and engineering books."
            },
            new BookCategory
            {
                Id = Guid.Parse("cb705433-43ce-4732-9355-4f2558dcf6a9"),
                Name = "Business",
                Description = "Books related to business and entrepreneurship."
            }
        ];
    }

    private static List<Book> DefaultBooks(IReadOnlyList<BookCategory> categories)
    {
        Guid categoryId(string name) => categories.First(category => category.Name == name).Id;

        return
        [
            new Book
            {
                Id = Guid.Parse("3d163b32-52f7-4c5d-a1ef-ec1dad40d00a"),
                Isbn = "9780321125217",
                Title = "Domain-Driven Design",
                Author = "Eric Evans",
                Description = "Tactical and strategic guidance for complex domain modelling.",
                Format = "Paperback",
                Price = 84.95m,
                StockQuantity = 12,
                IsActive = true,
                CategoryIds = [categoryId("Software Architecture"), categoryId("Programming")]
            },
            new Book
            {
                Id = Guid.Parse("348f7825-bd2e-46ea-94fb-c84329d19ec2"),
                Isbn = "9780134494166",
                Title = "Clean Architecture",
                Author = "Robert C. Martin",
                Description = "Practical design principles for layered and maintainable systems.",
                Format = "Paperback",
                Price = 67.50m,
                StockQuantity = 8,
                IsActive = true,
                CategoryIds = [categoryId("Software Architecture")]
            },
            new Book
            {
                Id = Guid.Parse("7bfcf2e0-7d96-4f30-9237-8e8409831c1b"),
                Isbn = "9781617294532",
                Title = "ASP.NET Core in Action",
                Author = "Andrew Lock",
                Description = "A hands-on guide to building ASP.NET Core web applications and APIs.",
                Format = "Paperback",
                Price = 79.90m,
                StockQuantity = 10,
                IsActive = true,
                CategoryIds = [categoryId("Programming")]
            },
            new Book
            {
                Id = Guid.Parse("95f7c0dc-58f3-4265-aa6a-bcd66898ce53"),
                Isbn = "9780241551837",
                Title = "The Lean Startup",
                Author = "Eric Ries",
                Description = "Product and venture management lessons for growing businesses.",
                Format = "Hardcover",
                Price = 29.95m,
                StockQuantity = 15,
                IsActive = true,
                CategoryIds = [categoryId("Business")]
            }
        ];
    }

    private static List<ShipmentMethod> DefaultShipmentMethods()
    {
        return
        [
            new ShipmentMethod
            {
                Id = Guid.Parse("c6d1b7a7-09f5-4d2a-8fd5-12e23db2af30"),
                Name = "Standard Shipping",
                Description = "Budget-friendly shipping across Australia.",
                Cost = 9.95m,
                EstimatedDeliveryDays = 5,
                IsActive = true
            },
            new ShipmentMethod
            {
                Id = Guid.Parse("46727e06-f4ef-4b4f-a5a0-fad49888fd9f"),
                Name = "Express Shipping",
                Description = "Faster shipping for urgent orders.",
                Cost = 16.50m,
                EstimatedDeliveryDays = 2,
                IsActive = true
            }
        ];
    }

    private static List<Customer> DefaultCustomers()
    {
        return
        [
            new Customer
            {
                Id = Guid.Parse("7c9b2d29-ded5-480a-bdad-a96dbf4cc3e2"),
                FirstName = "Ava",
                LastName = "Chen",
                Email = "ava.chen@example.com",
                DefaultAddress = new Address
                {
                    RecipientName = "Ava Chen",
                    StreetLine1 = "25 Glenferrie Road",
                    City = "Hawthorn",
                    State = "VIC",
                    Postcode = "3122",
                    Country = "Australia"
                }
            }
        ];
    }

    private static List<Admin> DefaultAdmins()
    {
        return
        [
            new Admin
            {
                Id = Guid.Parse("15f7e8c3-0e07-4167-84e2-7b4ad1cf0ca3"),
                FirstName = "Mia",
                LastName = "Patel",
                Email = "admin@favouritebooks.test"
            }
        ];
    }
}
