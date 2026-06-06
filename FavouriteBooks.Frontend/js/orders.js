/**
 * orders.js — My Orders page.
 * Lists the signed-in customer's past orders with status and delivery details,
 * a "Pay now" link for any order still awaiting payment, and the order's
 * current status (which reflects dispatch/delivery once staff fulfil it).
 */
import { getMyOrders } from "./api.js";
import {
  money, formatDate, escapeHtml, formatAddress, orderStatusLabel, pill, shortId,
  showAlert, clearAlert,
} from "./util.js";

const els = {
  alert: document.getElementById("ordersAlert"),
  list: document.getElementById("ordersList"),
};

/**
 * Returns the signed-in user from localStorage, or null for a guest.
 * @returns {Object|null} The user object or null.
 */
function getCurrentUser() {
  const user = localStorage.getItem("fb_user");
  return user ? JSON.parse(user) : null;
}

function orderCard(order) {
  const items = (order.items || [])
    .map((item) => `
      <tr>
        <td>${escapeHtml(item.bookTitle)}</td>
        <td class="num">${item.quantity}</td>
        <td class="num">${money(item.lineTotal)}</td>
      </tr>`)
    .join("");

  // Pending payment (2) or a previously failed payment (3) can still be paid.
  const canPay = order.status === 2 || order.status === 3;
  const payAction = canPay
    ? `<div class="actions">
         <a class="pay-now" href="payment.html?invoiceId=${order.invoiceId}">Pay now</a>
       </div>`
    : "";

  return `
    <div class="card order-card">
      <div class="card-head-row">
        <h2>Order ${escapeHtml(shortId(order.id))}</h2>
        ${pill(orderStatusLabel(order.status))}
      </div>
      <p class="muted">Placed ${formatDate(order.createdUtc)} &middot; Total ${money(order.total)}</p>
      <table>
        <thead><tr><th>Title</th><th class="num">Qty</th><th class="num">Line total</th></tr></thead>
        <tbody>${items}</tbody>
      </table>
      <p><strong>Deliver to:</strong> ${escapeHtml(formatAddress(order.deliveryAddress))}</p>
      ${payAction}
    </div>`;
}

async function init() {
  clearAlert(els.alert);

  const user = getCurrentUser();
  if (!user) {
    showAlert(els.alert, "warn", "Please log in to view your orders.");
    return;
  }

  try {
    const orders = await getMyOrders(user.email);
    if (!orders.length) {
      showAlert(els.alert, "warn", "You have not placed any orders yet.");
      return;
    }
    els.list.innerHTML = orders.map(orderCard).join("");
  } catch (error) {
    showAlert(els.alert, "err", error.message || "Could not load your orders.");
  }
}

init();
