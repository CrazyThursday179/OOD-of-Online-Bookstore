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