/**
 * payment.js — Secure Payment page.
 * Loads the invoice handed over from checkout, lets the customer choose a
 * payment method (Strategy: Credit/Debit, PayPal, Afterpay), validates the
 * method-specific inputs client-side (mirroring the server rules), and submits
 * a simulated payment. A successful payment redirects to the confirmation page;
 * a declined one stays on the page for a retry.
 */
import { getInvoice, submitPayment } from "./api.js";
import {
  money, formatDate, shortId, escapeHtml, paymentStatusLabel, pill,
  setFieldError, showAlert, clearAlert, queryParam, flowStore,
} from "./util.js";

const els = {
  invoiceAlert: document.getElementById("invoiceAlert"),
  summaryCard: document.getElementById("summaryCard"),
  sumOrderId: document.getElementById("sumOrderId"),
  sumInvoiceId: document.getElementById("sumInvoiceId"),
  sumIssued: document.getElementById("sumIssued"),
  sumDue: document.getElementById("sumDue"),
  sumAmount: document.getElementById("sumAmount"),
  sumStatus: document.getElementById("sumStatus"),
  paymentCard: document.getElementById("paymentCard"),
  form: document.getElementById("paymentForm"),
  paymentAlert: document.getElementById("paymentAlert"),
  payBtn: document.getElementById("payBtn"),
};

// The currently loaded invoice; payment is only enabled once it is set.
let loadedInvoice = null;

// Basic email format check (text@text.text), stricter than the server's
// "contains @" rule, so anything the UI accepts the server also accepts.
const EMAIL_PATTERN = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;

const selectedMethod = () => document.querySelector('input[name="method"]:checked')?.value || "";
const selectedSimulation = () => document.querySelector('input[name="sim"]:checked')?.value || "Success";
const value = (id) => document.getElementById(id).value.trim();

/* ---- Invoice loading ---- */

async function loadInvoice(invoiceId) {
  clearAlert(els.invoiceAlert);
  els.summaryCard.hidden = true;
  els.paymentCard.hidden = true;
  loadedInvoice = null;

  try {
    loadedInvoice = await getInvoice(invoiceId);
    renderSummary(loadedInvoice);
  } catch (error) {
    showAlert(els.invoiceAlert, "err", error.message || "Invoice not found.");
  }
}

function renderSummary(invoice) {
  els.sumOrderId.innerHTML =
    `<span title="${escapeHtml(invoice.orderId)}">${escapeHtml(shortId(invoice.orderId))}</span>`;
  els.sumInvoiceId.textContent = invoice.invoiceNumber || "-";
  els.sumIssued.textContent = formatDate(invoice.issuedUtc);
  els.sumDue.textContent = formatDate(invoice.dueUtc);
  els.sumAmount.innerHTML = `<strong>${money(invoice.total)}</strong>`;
  els.sumStatus.innerHTML = pill(paymentStatusLabel(invoice.paymentStatus));
  els.summaryCard.hidden = false;

  if (invoice.paymentStatus === 2) {
    showAlert(els.invoiceAlert, "warn", "This invoice has already been paid.");
    els.paymentCard.hidden = true;
    return;
  }

  els.paymentCard.hidden = false;
}

/* ---- Method field toggling ---- */

function showMethodFields() {
  const method = selectedMethod();
  document.querySelectorAll(".method-fields").forEach((fieldset) => {
    fieldset.hidden = fieldset.dataset.method !== method;
  });
  setFieldError("method", "");
  clearMethodFieldErrors();
}

function clearMethodFieldErrors() {
  ["cardHolderName", "cardNumber", "expiryMonth", "expiryYear", "cvc",
    "payPalEmail", "afterpayEmail", "afterpayMobile"].forEach((id) => setFieldError(id, ""));
}

/* ---- Client-side validation (mirrors PaymentMethod.Validate on the server) ---- */

function validate(method) {
  clearMethodFieldErrors();
  setFieldError("method", "");
  let valid = true;
  const fail = (id, message) => { setFieldError(id, message); valid = false; };

  if (!method) {
    setFieldError("method", "Please select a payment method.");
    return false;
  }

  if (method === "1") {
    if (!value("cardHolderName")) fail("cardHolderName", "Cardholder name is required.");
    const card = value("cardNumber").replace(/\s/g, "");
    if (!card) fail("cardNumber", "Required.");
    else if (card.length < 12 || card.length > 19 || !/^\d+$/.test(card)) {
      fail("cardNumber", "Enter a valid card number.");
    }
    if (!value("expiryMonth")) fail("expiryMonth", "Required.");
    if (!value("expiryYear")) fail("expiryYear", "Required.");
    const cvc = value("cvc");
    if (!cvc) fail("cvc", "Required.");
    else if (cvc.length < 3 || cvc.length > 4 || !/^\d+$/.test(cvc)) {
      fail("cvc", "CVC must contain 3 or 4 digits.");
    }
  } else if (method === "2") {
    const email = value("payPalEmail");
    if (!email) fail("payPalEmail", "PayPal email is required.");
    else if (!EMAIL_PATTERN.test(email)) fail("payPalEmail", "Enter a valid email address (e.g. abc@gmail.com).");
  } else if (method === "3") {
    const email = value("afterpayEmail");
    if (!email) fail("afterpayEmail", "Afterpay email is required.");
    else if (!EMAIL_PATTERN.test(email)) fail("afterpayEmail", "Enter a valid email address (e.g. abc@gmail.com).");
    if (!value("afterpayMobile")) fail("afterpayMobile", "An Afterpay mobile number is required.");
  }

  return valid;
}

function buildRequest(method) {
  const request = {
    invoiceId: loadedInvoice.id,
    paymentMethodType: Number(method),
    simulationMode: selectedSimulation(),
  };
  if (method === "1") {
    request.cardHolderName = value("cardHolderName");
    request.cardNumber = value("cardNumber").replace(/\s/g, "");
    request.expiryMonth = value("expiryMonth");
    request.expiryYear = value("expiryYear");
    request.cvc = value("cvc");
  } else if (method === "2") {
    request.payPalEmail = value("payPalEmail");
  } else if (method === "3") {
    request.afterpayEmail = value("afterpayEmail");
    request.afterpayMobile = value("afterpayMobile");
  }
  return request;
}

/* ---- Submit ---- */

async function submit(event) {
  event.preventDefault();
  clearAlert(els.paymentAlert);

  if (!loadedInvoice) {
    showAlert(els.paymentAlert, "err", "There is no invoice to pay.");
    return;
  }

  const method = selectedMethod();
  if (!validate(method)) {
    showAlert(els.paymentAlert, "err", "Please fix the highlighted fields.");
    return;
  }

  els.payBtn.disabled = true;
  try {
    const response = await submitPayment(buildRequest(method));

    // Declined: resolves successfully but status 3, with no receipt.
    if (response.status === 3) {
      els.payBtn.disabled = false;
      showAlert(els.paymentAlert, "warn", "Payment declined. Please try again.");
      return;
    }

    // Paid: clear the invoice from the store and hand the receipt/shipment
    // ids to the confirmation page.
    flowStore.set({
      invoiceId: null,
      orderId: response.order ? response.order.id : loadedInvoice.orderId,
      receiptId: response.receipt ? response.receipt.id : null,
      shipmentId: response.shipment ? response.shipment.id : null,
    });
    showAlert(els.paymentAlert, "ok", "Payment successful. Redirecting to your receipt...");
    const params = new URLSearchParams();
    if (response.receipt) params.set("receiptId", response.receipt.id);
    if (response.shipment) params.set("shipmentId", response.shipment.id);
    window.location.href = `success.html?${params.toString()}`;
  } catch (error) {
    els.payBtn.disabled = false;
    showAlert(els.paymentAlert, "err", error.message || "Payment failed.");
  }
}

/* ---- Wiring ---- */

document.querySelectorAll('input[name="method"]').forEach((radio) =>
  radio.addEventListener("change", showMethodFields));
els.form.addEventListener("submit", submit);
els.form.addEventListener("reset", () => {
  setTimeout(() => {
    clearMethodFieldErrors();
    setFieldError("method", "");
    clearAlert(els.paymentAlert);
    showMethodFields();
  }, 0);
});

// The invoice id arrives from checkout (URL query ?invoiceId= or the hand-off store).
const presetInvoice = queryParam("invoiceId") || flowStore.get("invoiceId");
if (presetInvoice) {
  loadInvoice(presetInvoice);
} else {
  showAlert(els.invoiceAlert, "warn", "No invoice to pay. Please complete checkout first.");
}
