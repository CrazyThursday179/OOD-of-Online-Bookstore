# Favourite Books Online Bookstore Backend

This repository contains the `ASP.NET Core Web API` backend for the `Favourite Books Online Bookstore System` used in `SWE30003 Assignment 3`.

The main implemented flow is:

`Browse Catalogue -> Manage Shopping Cart -> Checkout / Create Order -> Generate Invoice -> Simulate Payment -> Generate Receipt -> Create Shipment Record`

## Tech Stack

- `ASP.NET Core Web API`
- `.NET 9`
- JSON file persistence
- Offline-friendly Swagger-compatible route documentation

## Repository Layout

```text
.
├─ FavouriteBooks.Api
│  ├─ Common
│  ├─ Controllers
│  ├─ Data
│  ├─ DTOs
│  ├─ Infrastructure
│  ├─ Models
│  ├─ Properties
│  ├─ Repositories
│  ├─ Services
│  ├─ FavouriteBooks.Api.csproj
│  └─ Program.cs
├─ NuGet.Config
└─ README.md
```

## What Each Main Folder Is For

- `Controllers`
  - HTTP API entry points. Front-end teammates can use these files to see route grouping.

- `DTOs`
  - Request and response shapes. Front-end teammates should use these as the main reference for payload structure.

- `Data`
  - JSON persistence files. Useful when checking whether cart, order, payment, receipt, and shipment data was actually saved.

- `Models`
  - Domain models such as `Book`, `ShoppingCart`, `Order`, `Invoice`, and `Shipment`.

- `Services`
  - Business workflow coordination. Good for understanding lifecycle rules, but front-end teammates usually do not need to modify this layer.

- `Repositories` and `Infrastructure`
  - JSON storage access and app startup helpers.

## Domain Behaviour Highlights

To better reflect Ass2 object responsibilities, the domain models contain behaviour such as:

- `Book`
  - `IsAvailable()`
  - `ReduceStock()`
  - `SetStockQuantity()`

- `ShoppingCart`
  - `IsEmpty()`
  - `AddItem()`
  - `RemoveItem()`
  - `UpdateQuantity()`
  - `CalculateTotal()`
  - `Clear()`

- `Address`
  - `IsValid()`
  - `GetValidationErrors()`

- `Invoice`
  - `MarkAsPaid()`
  - `MarkAsDeclined()`
  - `MarkAsValidationFailed()`

- `Shipment`
  - `MarkDispatched()`
  - `MarkDelivered()`

- `Order`
  - local state methods only, while major workflow coordination stays in `OrderService`

## Build And Run

Recommended environment:

- `Windows`
- `.NET SDK 9.0`
- `Visual Studio 2022` or `VS Code + C# Dev Kit`

From the repository root:

```powershell
$env:APPDATA="$PWD\\.appdata"
dotnet restore FavouriteBooks.Api/FavouriteBooks.Api.csproj --configfile NuGet.Config
dotnet build FavouriteBooks.Api/FavouriteBooks.Api.csproj --no-restore
dotnet run --no-launch-profile --project FavouriteBooks.Api/FavouriteBooks.Api.csproj
```

If the app starts normally, it will show a localhost URL such as `http://localhost:5000`.

## Route Documentation

The app exposes a Swagger-compatible JSON route:

- `GET /swagger/v1/swagger.json`

This is not an interactive Swagger UI page. It is a route-description document that can be:

- opened in the browser to confirm the API is running
- imported into Postman
- imported into Swagger UI tooling
- used as a quick route reference

## Persistent Data

All persistent data is stored in:

- `FavouriteBooks.Api/Data`

Main files:

- `books.json`
- `categories.json`
- `carts.json`
- `customers.json`
- `orders.json`
- `invoices.json`
- `payments.json`
- `receipts.json`
- `shipments.json`
- `shipmentMethods.json`
- `admins.json`

## Seed Data IDs

Useful built-in IDs:

- Demo customer: `7c9b2d29-ded5-480a-bdad-a96dbf4cc3e2`
- Standard shipping: `c6d1b7a7-09f5-4d2a-8fd5-12e23db2af30`
- Express shipping: `46727e06-f4ef-4b4f-a5a0-fad49888fd9f`
- `Domain-Driven Design`: `3d163b32-52f7-4c5d-a1ef-ec1dad40d00a`
- `Clean Architecture`: `348f7825-bd2e-46ea-94fb-c84329d19ec2`

## API Summary

### Catalogue

- `GET /api/catalogue/books`
- `GET /api/catalogue/books?search=architecture`
- `GET /api/catalogue/books?categoryId={categoryId}`
- `GET /api/catalogue/books/{bookId}`
- `GET /api/catalogue/categories`
- `GET /api/catalogue/shipment-methods`

### Shopping Cart

- `GET /api/carts?customerId={guid}`
- `GET /api/carts?sessionId=guest-session-001`
- `POST /api/carts/items`
- `PUT /api/carts/items/{bookId}`
- `DELETE /api/carts/items/{bookId}?customerId={guid}`
- `DELETE /api/carts/items/{bookId}?sessionId=guest-session-001`
- `DELETE /api/carts?customerId={guid}`
- `DELETE /api/carts?sessionId=guest-session-001`

### Orders And Invoice

- `POST /api/orders/checkout`
- `GET /api/orders/{orderId}`
- `GET /api/orders/invoice/{invoiceId}`

### Payment, Receipt, Shipment

- `POST /api/payments`
- `GET /api/payments/receipts/{receiptId}`
- `GET /api/shipments/{shipmentId}`

### Minimal Admin

- `GET /api/admin/orders`
- `POST /api/admin/books`
- `PATCH /api/admin/books/{bookId}/stock`
- `PATCH /api/admin/shipments/{shipmentId}/status`

## Front-End Integration Notes

### Start With Guest Flow First

There is currently no authentication or login system. The easiest integration path is:

- use `guest checkout`
- identify the cart with `sessionId`
- keep the `sessionId` stable in local storage or similar client-side storage

For now, front-end teammates should treat `sessionId` as the main user identity for shopping/cart flow.

### Cart Ownership Rule

A cart is identified by either:

- `customerId`
- `sessionId`

Guest flow should use `sessionId`.

Registered-customer flow can be added later using `customerId`, but it is not the best first integration target.

### Checkout Rule

`POST /api/orders/checkout` does not complete payment.

It creates:

- `Order`
- `Invoice`

After checkout, front-end must store:

- `orderId`
- `invoiceId`

### Payment Rule

Payment is a separate step:

- call `POST /api/payments`
- successful payment creates:
  - `Receipt`
  - `Shipment`

After successful payment, front-end should store:

- `receiptId`
- `shipmentId`

### Cart Clearing Rule

After successful checkout, the cart is cleared on the backend.

Front-end should not expect the old cart contents to remain after checkout.

### Response Shape

Many API responses use a `Result` wrapper with fields such as:

- `isSuccess`
- `message`
- `errors`
- `data`

Front-end should not rely only on HTTP status codes. It should also read `message` and `errors` and show them in the UI when appropriate.

## Example Payloads

### Add Item To Cart

```json
{
  "customerId": null,
  "sessionId": "guest-session-001",
  "bookId": "3d163b32-52f7-4c5d-a1ef-ec1dad40d00a",
  "quantity": 1
}
```

### Guest Checkout

```json
{
  "customerId": null,
  "sessionId": "guest-session-001",
  "guestEmail": "guest@example.com",
  "shipmentMethodId": "c6d1b7a7-09f5-4d2a-8fd5-12e23db2af30",
  "deliveryAddress": {
    "recipientName": "Guest Buyer",
    "streetLine1": "100 Swanston Street",
    "streetLine2": "",
    "city": "Melbourne",
    "state": "VIC",
    "postcode": "3000",
    "country": "Australia"
  }
}
```

### Simulated Payment

`paymentMethodType` values:

- `1` = `CreditDebit`
- `2` = `PayPal`
- `3` = `Afterpay`

`simulationMode` values:

- `Success`
- `Declined`

Example:

```json
{
  "invoiceId": "PUT-INVOICE-ID-HERE",
  "paymentMethodType": 2,
  "payPalEmail": "guest@example.com",
  "simulationMode": "Success"
}
```

### Shipment Status

Shipment status values:

- `1` = `Pending`
- `2` = `ReadyForDispatch`
- `3` = `Dispatched`
- `4` = `Delivered`

## Recommended Front-End Work Order

Front-end teammates should build in this order:

1. `Catalogue page`
   - fetch books
   - search books
   - filter by category

2. `Cart page`
   - add item
   - get cart by `sessionId`
   - update quantity
   - remove item

3. `Checkout page`
   - address form
   - shipment method selection
   - submit checkout

4. `Payment page`
   - choose payment method
   - submit simulated payment
   - support both `Success` and `Declined` demo paths

5. `Receipt / Success / Shipment page`
   - show returned receipt details
   - show shipment status

6. `Minimal admin page`
   - list orders
   - update shipment status

## Suggested Front-End Folder Structure

If a separate front-end app is added later, a clean structure would be:

```text
FavouriteBooks.Frontend/
├─ src/
│  ├─ api/
│  ├─ components/
│  ├─ pages/
│  ├─ hooks/
│  ├─ types/
│  └─ utils/
└─ package.json
```

Recommended API files:

- `api/http.ts`
- `api/catalogueApi.ts`
- `api/cartApi.ts`
- `api/ordersApi.ts`
- `api/paymentsApi.ts`
- `api/shipmentsApi.ts`
- `api/adminApi.ts`

Recommended type files:

- `types/book.ts`
- `types/category.ts`
- `types/cart.ts`
- `types/order.ts`
- `types/invoice.ts`
- `types/payment.ts`
- `types/receipt.ts`
- `types/shipment.ts`

## Suggested Test Flow

For a simple end-to-end verification:

1. `GET /api/catalogue/books`
2. `POST /api/carts/items`
3. `GET /api/carts?sessionId=guest-session-001`
4. `POST /api/orders/checkout`
5. `POST /api/payments` with `simulationMode = "Declined"`
6. `POST /api/payments` with `simulationMode = "Success"`
7. `GET /api/payments/receipts/{receiptId}`
8. `GET /api/shipments/{shipmentId}`
9. `PATCH /api/admin/shipments/{shipmentId}/status`

## Known Limitations

- Payment is simulated only.
- No real database is used.
- No real authentication or login flow is implemented.
- No real email/notification service is implemented.
- Sales statistics/reporting are not implemented in this backend version.
- `CORS` is not configured yet. If the front-end runs on a different localhost port, browser-based API calls may fail until CORS is added.

## Practical Suggestions For Front-End Teammates

- Start with `guest flow`, not registered-customer flow.
- Treat `DTOs` as the primary field-reference layer.
- Save and reuse `sessionId`, `orderId`, `invoiceId`, `receiptId`, and `shipmentId`.
- Use the JSON files in `Data` to verify whether actions were really persisted.
- Build the success path first, then add failure-state UI for quantity validation, invalid address, and declined payment.
