using FavouriteBooks.Api.Infrastructure;
using FavouriteBooks.Api.Repositories;
using FavouriteBooks.Api.Services;
using FavouriteBooks.Api.Services.Payments;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddSingleton<JsonDataStore>();
builder.Services.AddSingleton<SeedDataService>();

builder.Services.AddSingleton<BookRepository>();
builder.Services.AddSingleton<BookCategoryRepository>();
builder.Services.AddSingleton<CartRepository>();
builder.Services.AddSingleton<CustomerRepository>();
builder.Services.AddSingleton<AdminRepository>();
builder.Services.AddSingleton<OrderRepository>();
builder.Services.AddSingleton<InvoiceRepository>();
builder.Services.AddSingleton<PaymentDetailsRepository>();
builder.Services.AddSingleton<ReceiptRepository>();
builder.Services.AddSingleton<ShipmentRepository>();
builder.Services.AddSingleton<ShipmentMethodRepository>();

builder.Services.AddScoped<ICatalogueService, CatalogueService>();
builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddScoped<IShipmentService, ShipmentService>();
builder.Services.AddScoped<IAdminService, AdminService>();

builder.Services.AddScoped<CreditDebitPayment>();
builder.Services.AddScoped<PayPalPayment>();
builder.Services.AddScoped<AfterpayPayment>();
builder.Services.AddScoped<PaymentStrategyFactory>();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://127.0.0.1:5500", "http://localhost:5500")
            .AllowAnyHeader().AllowAnyMethod();
    });
});

var app = builder.Build();

await SeedDataService.InitializeAsync(app.Services);

app.UseHttpsRedirection();
app.UseCors();
app.MapControllers();

app.Run();
