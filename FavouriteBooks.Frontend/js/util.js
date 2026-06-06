/**
 * util.js — small shared presentation helpers for the Payment, Success and
 * Admin pages. Keeps formatting, enum labels, status pills and form helpers in
 * one place so the three page modules don't duplicate them.
 *
 * The backend serialises enums as integers, so the label helpers below map
 * those integers to a human label plus a CSS pill class (see components.css).
 */

/* ---- Formatting ---- */

const AUD = new Intl.NumberFormat("en-AU", { style: "currency", currency: "AUD" });

/**
 * Formats a number as Australian currency, e.g. 94.9 -> "$94.90".
 * @param {number} value - The amount.
 * @returns {string} The formatted currency string.
 */
export function money(value) {
  return AUD.format(Number(value || 0));
}

/**
 * Formats an ISO date string for display, or "-" when missing/invalid.
 * @param {string} iso - The ISO date string.
 * @returns {string} A locale date-time string.
 */
export function formatDate(iso) {
  if (!iso) return "-";
  const parsed = new Date(iso);
  return Number.isNaN(parsed.getTime()) ? "-" : parsed.toLocaleString("en-AU");
}

/**
 * Escapes a value for safe insertion into HTML.
 * @param {*} value - The value to escape.
 * @returns {string} The escaped string.
 */
export function escapeHtml(value) {
  return String(value ?? "")
    .replace(/&/g, "&amp;")
    .replace(/</g, "&lt;")
    .replace(/>/g, "&gt;")
    .replace(/"/g, "&quot;");
}

/**
 * Returns a short, friendly reference for a GUID (display only; the full id is
 * still used internally), e.g. "...-4d5f7db36760" -> "#6760".
 * @param {string} guid - The full GUID.
 * @returns {string} The short reference.
 */
export function shortId(guid) {
  const compact = String(guid || "").replace(/-/g, "");
  return compact ? `#${compact.slice(-4)}` : "-";
}

/**
 * Builds a readable single-line delivery address from an address object.
 * @param {Object} address - The address object.
 * @returns {string} The formatted address, or "-" when empty.
 */
export function formatAddress(address) {
  if (!address) return "-";
  const parts = [
    address.recipientName,
    address.streetLine1,
    address.streetLine2,
    [address.city, address.state, address.postcode].filter(Boolean).join(" "),
    address.country,
  ].filter((part) => part && String(part).trim());
  return parts.length ? parts.join(", ") : "-";
}

/* ---- Enum labels (integers -> {text, cls}) ---- */

const PAYMENT_METHODS = { 1: "Credit / Debit Card", 2: "PayPal", 3: "Afterpay" };

/**
 * Returns the display name for a payment method integer.
 * @param {number} value - The payment method type.
 * @returns {string} The method name.
 */
export function paymentMethodLabel(value) {
  return PAYMENT_METHODS[value] || "-";
}

/**
 * Maps a payment status integer to a label and pill class.
 * @param {number} value - The payment status.
 * @returns {{text: string, cls: string}} Label and CSS class.
 */
export function paymentStatusLabel(value) {
  switch (value) {
    case 1: return { text: "Pending", cls: "pending" };
    case 2: return { text: "Paid", cls: "paid" };
    case 3: return { text: "Declined", cls: "declined" };
    case 4: return { text: "Validation Failed", cls: "failed" };
    default: return { text: "Unknown", cls: "pending" };
  }
}

/**
 * Maps a shipment status integer to a label and pill class.
 * @param {number} value - The shipment status.
 * @returns {{text: string, cls: string}} Label and CSS class.
 */
export function shipmentStatusLabel(value) {
  switch (value) {
    case 1: return { text: "Pending", cls: "pending" };
    case 2: return { text: "Ready for Dispatch", cls: "ready" };
    case 3: return { text: "Dispatched", cls: "dispatched" };
    case 4: return { text: "Delivered", cls: "delivered" };
    default: return { text: "Unknown", cls: "pending" };
  }
}

/**
 * Maps an order status integer to a label and pill class.
 * @param {number} value - The order status.
 * @returns {{text: string, cls: string}} Label and CSS class.
 */
export function orderStatusLabel(value) {
  switch (value) {
    case 1: return { text: "Pending Checkout", cls: "pending" };
    case 2: return { text: "Pending Payment", cls: "pending" };
    case 3: return { text: "Payment Failed", cls: "failed" };
    case 4: return { text: "Paid", cls: "paid" };
    case 5: return { text: "Shipment Created", cls: "ready" };
    case 6: return { text: "Dispatched", cls: "dispatched" };
    case 7: return { text: "Delivered", cls: "delivered" };
    default: return { text: "Unknown", cls: "pending" };
  }
}

/**
 * Builds the HTML for a coloured status pill from a label object.
 * @param {{text: string, cls: string}} label - The label and class.
 * @returns {string} The pill HTML.
 */
export function pill(label) {
  return `<span class="pill ${label.cls}">${escapeHtml(label.text)}</span>`;
}

/* ---- Form + alert helpers ---- */

/**
 * Shows or clears a field-level validation message and red border.
 * @param {string} inputId - The input element id (its error box is `${id}-error`).
 * @param {string} message - The error message, or "" to clear.
 */
export function setFieldError(inputId, message) {
  const input = document.getElementById(inputId);
  const errorBox = document.getElementById(`${inputId}-error`);
  if (input) input.classList.toggle("invalid", Boolean(message));
  if (errorBox) errorBox.textContent = message || "";
}

/**
 * Renders an alert banner (success/warn/error) into the given element.
 * @param {HTMLElement} element - The alert container.
 * @param {string} type - "ok", "warn" or "err".
 * @param {string} message - The message to show.
 */
export function showAlert(element, type, message) {
  if (!element) return;
  element.className = `alert show ${type}`;
  element.innerHTML = `<strong>${escapeHtml(message)}</strong>`;
}

/**
 * Hides and clears an alert banner.
 * @param {HTMLElement} element - The alert container.
 */
export function clearAlert(element) {
  if (!element) return;
  element.className = "alert";
  element.innerHTML = "";
}

/**
 * Reads a query-string parameter from the current URL.
 * @param {string} name - The parameter name.
 * @returns {string|null} The value, or null.
 */
export function queryParam(name) {
  return new URLSearchParams(window.location.search).get(name);
}

/* ---- Cross-page hand-off (localStorage) ---- */

const STORE_KEY = "fb_flow";

/**
 * A tiny localStorage-backed store used to pass ids between pages
 * (e.g. the invoice id from checkout to payment, or receipt/shipment
 * ids from payment to the success page).
 */
export const flowStore = {
  all() {
    try {
      return JSON.parse(localStorage.getItem(STORE_KEY)) || {};
    } catch {
      return {};
    }
  },
  get(key) {
    return this.all()[key];
  },
  set(values) {
    localStorage.setItem(STORE_KEY, JSON.stringify({ ...this.all(), ...values }));
  },
};
