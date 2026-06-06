/**
 * success.js — Order Confirmation page.
 * Shows the receipt, order summary and shipment tracking for the customer's
 * latest completed order. The ids arrive from the payment redirect (URL query
 * or the hand-off store) — the customer never types them.
 */
import { getReceipt, getOrder, getShipment, getShipmentMethods } from "./api.js";
import {
  money, formatDate, escapeHtml, formatAddress, paymentMethodLabel,
  shipmentStatusLabel, pill, showAlert, clearAlert, queryParam, flowStore, shortId,
} from "./util.js";

const els = {
  loadAlert: document.getElementById("loadAlert"),
  confirmation: document.getElementById("confirmation"),
  // Receipt
  rNumber: document.getElementById("rNumber"),
  rOrderRef: document.getElementById("rOrderRef"),
  rPaid: document.getElementById("rPaid"),
  rMethod: document.getElementById("rMethod"),
  rAmount: document.getElementById("rAmount"),
  // Order
  orderCard: document.getElementById("orderCard"),
  orderItems: document.getElementById("orderItems"),
  oSubtotal: document.getElementById("oSubtotal"),
  oShipping: document.getElementById("oShipping"),
  oTotal: document.getElementById("oTotal"),
  // Shipment
  shipmentCard: document.getElementById("shipmentCard"),
  sStatus: document.getElementById("sStatus"),
  sMethod: document.getElementById("sMethod"),
  sTracking: document.getElementById("sTracking"),
  sCreated: document.getElementById("sCreated"),
  sDispatched: document.getElementById("sDispatched"),
  sDelivered: document.getElementById("sDelivered"),
  sAddress: document.getElementById("sAddress"),
};

// Shipment methods are fetched once and cached so a shipment's method id can be
// shown as a friendly name + delivery estimate (ETA).
let methodMap = null;
async function getMethodMap() {
  if (methodMap) return methodMap;
  methodMap = {};
  try {
    const list = await getShipmentMethods();
    (list || []).forEach((method) => { methodMap[method.id] = method; });
  } catch {
    // Non-fatal: the method name simply shows "-" if this lookup fails.
  }
  return methodMap;
}

function formatMethod(method) {
  if (!method) return "-";
  const days = method.estimatedDeliveryDays;
  const eta = days ? ` · ~${days} business day${days === 1 ? "" : "s"}` : "";
  return `${method.name}${eta}`;
}

/* ---- Load the confirmation for the given ids ---- */

async function showConfirmation(receiptId, shipmentId) {
  clearAlert(els.loadAlert);
  try {
    const receipt = await getReceipt(receiptId);
    els.confirmation.hidden = false;
    renderReceipt(receipt);
    await renderOrder(receipt.orderId);

    if (shipmentId) {
      await renderShipment(shipmentId);
    } else {
      els.shipmentCard.hidden = true;
    }
  } catch (error) {
    els.confirmation.hidden = true;
    showAlert(els.loadAlert, "err", error.message || "Could not load your order.");
  }
}

function renderReceipt(receipt) {
  els.rNumber.textContent = receipt.receiptNumber || "-";
  els.rOrderRef.textContent = shortId(receipt.orderId);
  els.rPaid.textContent = formatDate(receipt.paidUtc);
  els.rMethod.textContent = paymentMethodLabel(receipt.paymentMethodType);
  els.rAmount.innerHTML = `<strong>${money(receipt.amountPaid)}</strong>`;
}

async function renderOrder(orderId) {
  if (!orderId) { els.orderCard.hidden = true; return; }
  try {
    const order = await getOrder(orderId);
    els.orderItems.innerHTML = (order.items || [])
      .map((item) => `
        <tr>
          <td>${escapeHtml(item.bookTitle)}</td>
          <td class="num">${item.quantity}</td>
          <td class="num">${money(item.unitPrice)}</td>
          <td class="num">${money(item.lineTotal)}</td>
        </tr>`)
      .join("");
    els.oSubtotal.textContent = money(order.subtotal);
    els.oShipping.textContent = money(order.shippingCost);
    els.oTotal.textContent = money(order.total);
    els.orderCard.hidden = false;
  } catch {
    els.orderCard.hidden = true;
  }
}

async function renderShipment(shipmentId) {
  try {
    const shipment = await getShipment(shipmentId);
    els.sStatus.innerHTML = pill(shipmentStatusLabel(shipment.status));
    const methods = await getMethodMap();
    els.sMethod.textContent = formatMethod(methods[shipment.shipmentMethodId]);
    els.sTracking.textContent = shipment.trackingCode || "-";
    els.sCreated.textContent = formatDate(shipment.createdUtc);
    els.sDispatched.textContent = formatDate(shipment.dispatchedUtc);
    els.sDelivered.textContent = formatDate(shipment.deliveredUtc);
    els.sAddress.textContent = formatAddress(shipment.deliveryAddress);
    els.shipmentCard.hidden = false;
  } catch {
    els.shipmentCard.hidden = true;
    showAlert(els.loadAlert, "warn", "Receipt loaded, but the shipment could not be found.");
  }
}

/* ---- Wiring ---- */

document.getElementById("printBtn").addEventListener("click", () => window.print());

// Ids arrive from the payment redirect (URL query) or the hand-off store.
const receiptId = queryParam("receiptId") || flowStore.get("receiptId");
const shipmentId = queryParam("shipmentId") || flowStore.get("shipmentId");
if (receiptId) {
  showConfirmation(receiptId, shipmentId);
} else {
  showAlert(els.loadAlert, "warn",
    "No recent order to show. Complete a purchase to see your confirmation here.");
}
