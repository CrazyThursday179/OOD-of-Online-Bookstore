const BASE_URL = "http://localhost:5000/api";

/**
 * Retrieves the session ID from localStorage.
 * Creates and stores a new one if it doesn't exist yet.
 * Used to identify the cart for guest and customer users.
 * @returns {string} The session ID.
 */
export function getSessionId() {
  let id = localStorage.getItem("fb_session_id");
  if (!id) {
    id = "session-" + crypto.randomUUID();
    localStorage.setItem("fb_session_id", id);
  }
  return id;
}

/**
 * Core fetch helper for all API requests.
 * Handles headers, JSON parsing, and error checking in one place.
 * The backend wraps all responses in { isSuccess, message, data }
 * so we check isSuccess rather than relying solely on HTTP status.
 * @param {string} path - The API route to call e.g. /catalogue/books.
 * @param {Object} options - Optional fetch options e.g. method, body.
 * @returns {Promise<any>} The data field from the API response.
 * @throws {Error} If the API returns isSuccess: false.
 */
async function apiFetch(path, options = {}) {
  const res = await fetch(`${BASE_URL}${path}`, {
    headers: { "Content-Type": "application/json" },
    ...options,
  });

  const data = await res.json();

  /* Some endpoints return a plain array or object directly,
    others wrap the response in { isSuccess, message, data }.
    We handle both cases here. */
  if (Array.isArray(data) || (data && data.isSuccess === undefined)) {
    return data;
  }

  if (!data.isSuccess) {
    throw new Error(data.message || "Something went wrong");
  }

  return data.data;
}

/**
 * Fetches all books from the catalogue.
 * Search is handled on the frontend, but categoryId can be passed
 * to filter by genre on the backend.
 * @param {string} search - Optional search term (not used for backend filtering).
 * @param {string} categoryId - Optional category ID to filter by genre.
 * @returns {Promise<Array>} Array of book objects.
 */
export function getBooks(search = "", categoryId = "") {
  const params = new URLSearchParams();
  if (search) params.set("search", search);
  if (categoryId) params.set("categoryId", categoryId);
  return apiFetch(`/catalogue/books?${params}`);
}

/**
 * Fetches all book categories from the backend.
 * Used to populate the genre filter dropdown on the catalogue page.
 * @returns {Promise<Array>} Array of category objects.
 */
export function getCategories() {
  return apiFetch("/catalogue/categories");
}

/**
 * Fetches all available shipment methods.
 * Used to populate the shipment options on the checkout page.
 * @returns {Promise<Array>} Array of shipment method objects.
 */
export function getShipmentMethods() {
  return apiFetch("/catalogue/shipment-methods");
}

/**
 * Fetches the current cart for the active session.
 * @returns {Promise<Object>} The cart object including items and total.
 */
export function getCart() {
  return apiFetch(`/carts?sessionId=${getSessionId()}`);
}

/**
 * Adds a book to the cart for the current session.
 * @param {string} bookId - The ID of the book to add.
 * @param {number} quantity - The quantity to add, defaults to 1.
 * @returns {Promise<Object>} The updated cart object.
 */
export function addToCart(bookId, quantity = 1) {
  return apiFetch("/carts/items", {
    method: "POST",
    body: JSON.stringify({
      sessionId: getSessionId(),
      bookId,
      quantity,
    }),
  });
}

/**
 * Updates the quantity of an existing item in the cart.
 * @param {string} bookId - The ID of the book to update.
 * @param {number} quantity - The new quantity.
 * @returns {Promise<Object>} The updated cart object.
 */
export function updateCartItem(bookId, quantity) {
  return apiFetch(`/carts/items/${bookId}`, {
    method: "PUT",
    body: JSON.stringify({
      sessionId: getSessionId(),
      quantity,
    }),
  });
}

/**
 * Removes a book from the cart entirely.
 * @param {string} bookId - The ID of the book to remove.
 * @returns {Promise<Object>} The updated cart object.
 */
export function removeCartItem(bookId) {
  return apiFetch(`/carts/items/${bookId}?sessionId=${getSessionId()}`, {
    method: "DELETE",
  });
}

/**
 * Submits the checkout form to create an order and invoice.
 * Does not process payment as that is handled separately.
 * After checkout the backend clears the cart automatically.
 * @param {Object} formData - Delivery address and shipment method details.
 * @returns {Promise<Object>} Object containing orderId and invoiceId.
 */
export function checkout(formData) {
  return apiFetch("/orders/checkout", {
    method: "POST",
    body: JSON.stringify({
      sessionId: getSessionId(),
      ...formData,
    }),
  });
}

/* ------------------------------------------------------------------ */
/* Payment, receipt, shipment and admin endpoints (added for the      */
/* Payment, Success and Admin pages). All reuse the shared apiFetch.  */
/* ------------------------------------------------------------------ */

/**
 * Fetches a single invoice by id, used to show the amount owed before payment.
 * @param {string} invoiceId - The invoice id returned by checkout.
 * @returns {Promise<Object>} The invoice object.
 */
export function getInvoice(invoiceId) {
  return apiFetch(`/orders/invoice/${invoiceId}`);
}

/**
 * Fetches a single order by id, used to show the order summary on the receipt.
 * @param {string} orderId - The order id.
 * @returns {Promise<Object>} The order object including items and totals.
 */
export function getOrder(orderId) {
  return apiFetch(`/orders/${orderId}`);
}

/**
 * Fetches the orders placed by a customer, matched on their email.
 * Used by the My Orders page.
 * @param {string} email - The customer's email address.
 * @returns {Promise<Array>} Array of the customer's orders, newest first.
 */
export function getMyOrders(email) {
  return apiFetch(`/orders/mine?email=${encodeURIComponent(email)}`);
}

/**
 * Submits a (simulated) payment for an invoice. A declined payment still
 * resolves successfully with status 3; only hard failures throw.
 * @param {Object} paymentRequest - Invoice id, method, details and simulation mode.
 * @returns {Promise<Object>} The payment response (receipt and shipment on success).
 */
export function submitPayment(paymentRequest) {
  return apiFetch("/payments", {
    method: "POST",
    body: JSON.stringify(paymentRequest),
  });
}

/**
 * Fetches a receipt by id for the order confirmation page.
 * @param {string} receiptId - The receipt id.
 * @returns {Promise<Object>} The receipt object.
 */
export function getReceipt(receiptId) {
  return apiFetch(`/payments/receipts/${receiptId}`);
}

/**
 * Fetches a shipment by id to show its current tracking status.
 * @param {string} shipmentId - The shipment id.
 * @returns {Promise<Object>} The shipment object.
 */
export function getShipment(shipmentId) {
  return apiFetch(`/shipments/${shipmentId}`);
}

/**
 * Fetches all customer orders for the admin fulfilment view.
 * @returns {Promise<Array>} Array of order objects, newest first.
 */
export function getAdminOrders() {
  return apiFetch("/admin/orders");
}

/**
 * Adds a new book to the catalogue (admin only).
 * @param {Object} bookData - ISBN, title, author, price, stock, categories etc.
 * @returns {Promise<Object>} The created book object.
 */
export function addBook(bookData) {
  return apiFetch("/admin/books", {
    method: "POST",
    body: JSON.stringify(bookData),
  });
}

/**
 * Updates the stock quantity of an existing book (admin only).
 * @param {string} bookId - The book id.
 * @param {number} stockQuantity - The new stock quantity.
 * @returns {Promise<Object>} The updated book object.
 */
export function updateBookStock(bookId, stockQuantity) {
  return apiFetch(`/admin/books/${bookId}/stock`, {
    method: "PATCH",
    body: JSON.stringify({ stockQuantity }),
  });
}

/**
 * Removes (deactivates) a book from the catalogue (admin only). The book is
 * hidden from customers but kept for historical order data.
 * @param {string} bookId - The book id.
 * @returns {Promise<Object>} The updated (deactivated) book object.
 */
export function removeBook(bookId) {
  return apiFetch(`/admin/books/${bookId}`, { method: "DELETE" });
}

/**
 * Advances a shipment's fulfilment status (admin only).
 * @param {string} shipmentId - The shipment id.
 * @param {number} status - The new shipment status (2 Ready, 3 Dispatched, 4 Delivered).
 * @returns {Promise<Object>} The updated shipment object.
 */
export function updateShipmentStatus(shipmentId, status) {
  return apiFetch(`/admin/shipments/${shipmentId}/status`, {
    method: "PATCH",
    body: JSON.stringify({ status }),
  });
}
