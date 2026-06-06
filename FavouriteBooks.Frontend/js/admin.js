/**
 * admin.js — Admin Dashboard.
 * Two areas:
 *   1. Manage Catalogue — add books, view the catalogue, adjust stock.
 *   2. Orders & Fulfilment — review orders and advance shipment status.
 */
import {
  getCategories, getBooks, getAdminOrders, getShipment,
  addBook, updateBookStock, updateShipmentStatus, removeBook,
} from "./api.js";
import {
  money, formatDate, escapeHtml, formatAddress, orderStatusLabel,
  shipmentStatusLabel, pill, setFieldError, showAlert, clearAlert, shortId,
} from "./util.js";

const $ = (id) => document.getElementById(id);
let categories = [];

/* ---- Tabs ---- */

function activateTab(name) {
  $("tab-catalogue").classList.toggle("active", name === "catalogue");
  $("tab-orders").classList.toggle("active", name === "orders");
  $("panel-catalogue").classList.toggle("active", name === "catalogue");
  $("panel-orders").classList.toggle("active", name === "orders");
  if (name === "orders") loadOrders();
}

/* ---- Catalogue: categories + add book ---- */

async function loadCategories() {
  const box = $("categoryChecks");
  try {
    categories = await getCategories();
  } catch {
    categories = [];
  }
  if (!categories.length) {
    box.textContent = "No categories available.";
    return;
  }
  box.classList.remove("hint");
  box.innerHTML = categories
    .map((c) => `<label><input type="checkbox" value="${c.id}"> ${escapeHtml(c.name)}</label>`)
    .join("");
}

function validateBook() {
  let valid = true;
  const fail = (id, message) => { setFieldError(id, message); valid = false; };
  const value = (id) => $(id).value.trim();
  ["isbn", "title", "author", "price", "stockQuantity"].forEach((id) => setFieldError(id, ""));

  if (!value("isbn")) fail("isbn", "ISBN is required.");
  if (!value("title")) fail("title", "Title is required.");
  if (!value("author")) fail("author", "Author is required.");

  const price = value("price");
  if (price === "" || Number(price) < 0 || Number.isNaN(Number(price))) {
    fail("price", "Price must be zero or greater.");
  }
  const stock = value("stockQuantity");
  if (stock === "" || Number(stock) < 0 || !Number.isInteger(Number(stock))) {
    fail("stockQuantity", "Stock must be a whole number, zero or greater.");
  }
  return valid;
}

async function handleAddBook(event) {
  event.preventDefault();
  const alertBox = $("addBookAlert");
  clearAlert(alertBox);

  if (!validateBook()) {
    showAlert(alertBox, "err", "Please fix the highlighted fields.");
    return;
  }

  const value = (id) => $(id).value.trim();
  const request = {
    isbn: value("isbn"),
    title: value("title"),
    author: value("author"),
    description: value("description"),
    format: value("format"),
    price: Number(value("price")),
    stockQuantity: Number(value("stockQuantity")),
    categoryIds: Array.from(document.querySelectorAll("#categoryChecks input:checked")).map((cb) => cb.value),
  };

  $("addBookBtn").disabled = true;
  try {
    const book = await addBook(request);
    showAlert(alertBox, "ok", `Added "${book.title}" to the catalogue.`);
    $("addBookForm").reset();
    loadBooks();
  } catch (error) {
    showAlert(alertBox, "err", error.message || "Could not add book.");
  } finally {
    $("addBookBtn").disabled = false;
  }
}

/* ---- Catalogue: list + inline stock editing ---- */

async function loadBooks() {
  const body = $("booksBody");
  let books = [];
  try {
    books = await getBooks();
  } catch (error) {
    body.innerHTML = `<tr><td colspan="5" class="empty-state">${escapeHtml(error.message)}</td></tr>`;
    return;
  }

  if (!books.length) {
    body.innerHTML = '<tr><td colspan="5" class="empty-state">No books in the catalogue yet.</td></tr>';
    return;
  }

  body.innerHTML = books
    .map((book) => `
      <tr data-book-id="${book.id}">
        <td>${escapeHtml(book.title)}</td>
        <td>${escapeHtml(book.author)}</td>
        <td class="mono">${escapeHtml(book.isbn)}</td>
        <td class="num">${money(book.price)}</td>
        <td>
          <div class="stock-cell">
            <input type="number" class="stock-input" min="0" step="1" value="${book.stockQuantity}">
            <button type="button" class="secondary save-stock">Save</button>
            <button type="button" class="danger remove-book">Remove</button>
          </div>
        </td>
      </tr>`)
    .join("");

  document.querySelectorAll("#booksBody .save-stock").forEach((btn) =>
    btn.addEventListener("click", (e) => saveStock(e.target.closest("tr"))));
  document.querySelectorAll("#booksBody .remove-book").forEach((btn) =>
    btn.addEventListener("click", (e) => removeBookRow(e.target.closest("tr"))));
}

async function removeBookRow(row) {
  const bookId = row.dataset.bookId;
  const title = row.querySelector("td")?.textContent || "this book";
  const alertBox = $("stockAlert");
  clearAlert(alertBox);

  if (!window.confirm(`Remove "${title}" from the catalogue? Customers will no longer see it.`)) {
    return;
  }

  try {
    await removeBook(bookId);
    showAlert(alertBox, "ok", `"${title}" removed from the catalogue.`);
    loadBooks();
  } catch (error) {
    showAlert(alertBox, "err", error.message || "Could not remove book.");
  }
}

async function saveStock(row) {
  const bookId = row.dataset.bookId;
  const input = row.querySelector(".stock-input");
  const alertBox = $("stockAlert");
  clearAlert(alertBox);

  const quantity = Number(input.value);
  if (input.value === "" || quantity < 0 || !Number.isInteger(quantity)) {
    showAlert(alertBox, "err", "Stock must be a whole number, zero or greater.");
    input.classList.add("invalid");
    return;
  }
  input.classList.remove("invalid");

  try {
    const book = await updateBookStock(bookId, quantity);
    showAlert(alertBox, "ok", `Stock updated to ${book.stockQuantity}.`);
  } catch (error) {
    showAlert(alertBox, "err", error.message || "Could not update stock.");
  }
}

/* ---- Orders & Fulfilment ---- */

async function loadOrders() {
  const body = $("ordersBody");
  const alertBox = $("ordersAlert");
  clearAlert(alertBox);
  body.innerHTML = '<tr><td colspan="5" class="empty-state">Loading orders...</td></tr>';

  let orders = [];
  try {
    orders = await getAdminOrders();
  } catch (error) {
    body.innerHTML = `<tr><td colspan="5" class="empty-state">${escapeHtml(error.message)}</td></tr>`;
    return;
  }

  if (!orders.length) {
    body.innerHTML = '<tr><td colspan="5" class="empty-state">No orders have been placed yet.</td></tr>';
    return;
  }

  body.innerHTML = orders.map(orderRow).join("");

  orders.forEach((order) => {
    if (order.shipmentId) refreshShipmentCell(order.shipmentId);
  });

  document.querySelectorAll("#ordersBody .update-shipment").forEach((btn) =>
    btn.addEventListener("click", (e) => updateShipment(e.target.closest("[data-shipment-id]"))));
  document.querySelectorAll("#ordersBody .detail-toggle").forEach((btn) =>
    btn.addEventListener("click", () => {
      const detail = $(`detail-${btn.dataset.order}`);
      const open = !detail.hidden;
      detail.hidden = open;
      btn.textContent = open ? "Details" : "Hide";
    }));
}

function orderRow(order) {
  // Orders carry the buyer's name in the delivery address (set at checkout for
  // both guests and signed-in customers), so show that rather than "Guest".
  const name = order.deliveryAddress && order.deliveryAddress.recipientName;
  const customer = name
    ? `${escapeHtml(name)}${order.guestEmail ? `<br><span class="hint">${escapeHtml(order.guestEmail)}</span>` : ""}`
    : `Guest${order.guestEmail ? ` &middot; ${escapeHtml(order.guestEmail)}` : ""}`;

  const fulfilment = order.shipmentId
    ? `<div data-shipment-id="${order.shipmentId}">
         <div class="shipment-status muted">Loading...</div>
         <div class="shipment-control">
           <select class="shipment-select">
             <option value="2">Ready for Dispatch</option>
             <option value="3">Dispatched</option>
             <option value="4">Delivered</option>
           </select>
           <button type="button" class="secondary update-shipment">Update</button>
         </div>
       </div>`
    : '<span class="muted">No shipment (awaiting payment)</span>';

  const items = (order.items || [])
    .map((item) => `
      <tr>
        <td>${escapeHtml(item.bookTitle)}</td>
        <td class="num">${item.quantity}</td>
        <td class="num">${money(item.unitPrice)}</td>
        <td class="num">${money(item.lineTotal)}</td>
      </tr>`)
    .join("");

  return `
    <tr>
      <td>${formatDate(order.createdUtc)}</td>
      <td>${customer}
        <div><button type="button" class="detail-toggle" data-order="${order.id}">Details</button></div>
      </td>
      <td class="num">${money(order.total)}</td>
      <td>${pill(orderStatusLabel(order.status))}</td>
      <td>${fulfilment}</td>
    </tr>
    <tr class="detail-row" id="detail-${order.id}" hidden>
      <td colspan="5">
        <strong>Items</strong>
        <table>
          <thead><tr><th>Title</th><th class="num">Qty</th><th class="num">Unit</th><th class="num">Line</th></tr></thead>
          <tbody>${items || '<tr><td colspan="4" class="empty-state">No items.</td></tr>'}</tbody>
        </table>
        <p><strong>Deliver to:</strong> ${escapeHtml(formatAddress(order.deliveryAddress))}</p>
        <p class="hint">Order reference ${escapeHtml(shortId(order.id))}</p>
      </td>
    </tr>`;
}

async function refreshShipmentCell(shipmentId) {
  const cell = document.querySelector(`[data-shipment-id="${shipmentId}"]`);
  if (!cell) return;
  const statusBox = cell.querySelector(".shipment-status");
  try {
    const shipment = await getShipment(shipmentId);
    statusBox.innerHTML = `Current: ${pill(shipmentStatusLabel(shipment.status))}`;
    const select = cell.querySelector(".shipment-select");
    if (select) select.value = String(shipment.status);
  } catch {
    statusBox.textContent = "Shipment unavailable";
  }
}

async function updateShipment(cell) {
  const shipmentId = cell.dataset.shipmentId;
  const select = cell.querySelector(".shipment-select");
  const alertBox = $("ordersAlert");
  clearAlert(alertBox);

  try {
    const shipment = await updateShipmentStatus(shipmentId, Number(select.value));
    // Reload so both the shipment status and the order status column update.
    await loadOrders();
    showAlert(alertBox, "ok", `Shipment moved to "${shipmentStatusLabel(shipment.status).text}".`);
  } catch (error) {
    showAlert(alertBox, "err", error.message || "Could not update shipment.");
  }
}

/* ---- Wiring ---- */

$("tab-catalogue").addEventListener("click", () => activateTab("catalogue"));
$("tab-orders").addEventListener("click", () => activateTab("orders"));
$("addBookForm").addEventListener("submit", handleAddBook);
$("addBookForm").addEventListener("reset", () => {
  setTimeout(() => {
    ["isbn", "title", "author", "price", "stockQuantity"].forEach((id) => setFieldError(id, ""));
    clearAlert($("addBookAlert"));
  }, 0);
});
$("refreshBooksBtn").addEventListener("click", loadBooks);
$("refreshOrdersBtn").addEventListener("click", loadOrders);

// Initial load (Catalogue tab is active by default).
loadCategories();
loadBooks();
