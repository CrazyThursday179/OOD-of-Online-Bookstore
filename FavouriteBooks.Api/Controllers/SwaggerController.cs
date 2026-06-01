using Microsoft.AspNetCore.Mvc;

namespace FavouriteBooks.Api.Controllers;

[Route("swagger")]
public class SwaggerController : ControllerBase
{
    [HttpGet("v1/swagger.json")]
    public IActionResult GetSwaggerDocument()
    {
        var document = new
        {
            openapi = "3.0.1",
            info = new
            {
                title = "Favourite Books Online Bookstore API",
                version = "v1",
                description = "Backend API for catalogue browsing, cart management, checkout, simulated payment, receipt generation, shipment creation, and minimal admin operations."
            },
            paths = new Dictionary<string, object>
            {
                ["/api/catalogue/books"] = new
                {
                    get = new
                    {
                        summary = "Get all books or search/filter the catalogue.",
                        parameters = new object[]
                        {
                            new
                            {
                                name = "search",
                                @in = "query",
                                required = false,
                                schema = new { type = "string" }
                            },
                            new
                            {
                                name = "categoryId",
                                @in = "query",
                                required = false,
                                schema = new { type = "string", format = "uuid" }
                            }
                        }
                    }
                },
                ["/api/catalogue/books/{id}"] = new
                {
                    get = new
                    {
                        summary = "Get a single book by id.",
                        parameters = new object[]
                        {
                            new
                            {
                                name = "id",
                                @in = "path",
                                required = true,
                                schema = new { type = "string", format = "uuid" }
                            }
                        }
                    }
                },
                ["/api/catalogue/categories"] = new
                {
                    get = new
                    {
                        summary = "Get all book categories."
                    }
                },
                ["/api/catalogue/shipment-methods"] = new
                {
                    get = new
                    {
                        summary = "Get all active shipment methods."
                    }
                },
                ["/api/carts"] = new
                {
                    get = new
                    {
                        summary = "Get a cart by customerId or sessionId."
                    },
                    delete = new
                    {
                        summary = "Clear a cart by customerId or sessionId."
                    }
                },
                ["/api/carts/items"] = new
                {
                    post = new
                    {
                        summary = "Add an item to a cart."
                    }
                },
                ["/api/carts/items/{bookId}"] = new
                {
                    put = new
                    {
                        summary = "Update cart item quantity."
                    },
                    delete = new
                    {
                        summary = "Remove a cart item."
                    }
                },
                ["/api/orders/checkout"] = new
                {
                    post = new
                    {
                        summary = "Checkout a cart, create an order, and generate an invoice."
                    }
                },
                ["/api/orders/{orderId}"] = new
                {
                    get = new
                    {
                        summary = "Get an order by id."
                    }
                },
                ["/api/orders/invoice/{invoiceId}"] = new
                {
                    get = new
                    {
                        summary = "Get an invoice by id."
                    }
                },
                ["/api/payments"] = new
                {
                    post = new
                    {
                        summary = "Submit a simulated payment."
                    }
                },
                ["/api/payments/receipts/{receiptId}"] = new
                {
                    get = new
                    {
                        summary = "Get a receipt by id."
                    }
                },
                ["/api/shipments/{shipmentId}"] = new
                {
                    get = new
                    {
                        summary = "Get a shipment by id."
                    }
                },
                ["/api/admin/orders"] = new
                {
                    get = new
                    {
                        summary = "Get all orders for admin use."
                    }
                },
                ["/api/admin/books"] = new
                {
                    post = new
                    {
                        summary = "Create a new book."
                    }
                },
                ["/api/admin/books/{bookId}/stock"] = new
                {
                    patch = new
                    {
                        summary = "Update a book stock quantity."
                    }
                },
                ["/api/admin/shipments/{shipmentId}/status"] = new
                {
                    patch = new
                    {
                        summary = "Update shipment status."
                    }
                }
            }
        };

        return Ok(document);
    }
}
