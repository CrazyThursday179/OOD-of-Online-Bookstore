import { getCart, getShipmentMethods, checkout } from './api.js';

/**
 * Retrieves the current user from localStorage.
 * Returns null if no user is logged in (guest).
 * @returns {Object|null} The user object or null.
 */
function getCurrentUser() {
  const user = localStorage.getItem('fb_user');
  return user ? JSON.parse(user) : null;
}

/**
 * Renders the order summary from the current cart.
 * Displays each item with title, quantity and price, plus the total.
 * @param {Object} cart - The cart object returned from the backend.
 */
function renderOrderSummary(cart) {
  const summaryItems = document.getElementById('summary-items');
  const summaryTotal = document.getElementById('summary-total');

  if (!cart || !cart.items || cart.items.length === 0) {
    summaryItems.innerHTML = '<p>Your cart is empty.</p>';
    return;
  }

  summaryItems.innerHTML = cart.items.map((item) => `
    <div class="summary-item">
      <span class="summary-item-title">${item.bookTitle}</span>
      <span class="summary-item-qty">x${item.quantity}</span>
      <span class="summary-item-price">$${item.lineTotal.toFixed(2)}</span>
    </div>
  `).join('');

  summaryTotal.textContent = `Total: $${cart.subtotal.toFixed(2)}`;
}

/**
 * Renders the shipment method options fetched from the backend.
 * Each option is displayed as a radio button with name and price.
 * @param {Array} methods - Array of shipment method objects.
 */
function renderShipmentMethods(methods) {
  const container = document.getElementById('shipment-options');

  container.innerHTML = methods.map((method, index) => `
    <label class="shipment-option">
      <input type="radio" name="shipment" value="${method.id}" ${index === 0 ? 'checked' : ''}>
      ${method.name} - $${method.cost.toFixed(2)}
    </label>
  `).join('');
}

/**
 * Sets up the page based on whether the user is a guest or customer.
 * Guests see the email field and no saved detail options.
 * Customers see saved details and saved address checkboxes,
 * and the email field is pre-filled and hidden.
 * @param {Object|null} user - The current user or null for guests.
 */
function setupUserFields(user) {
  const savedDetailsToggle = document.getElementById('saved-details-toggle');
  const savedAddressToggle = document.getElementById('saved-address-toggle');
  const emailField = document.getElementById('email-field');

  if (user && user.role === 'customer') {
    // Show saved details and address checkboxes for customers
    savedDetailsToggle.classList.remove('hidden');
    savedAddressToggle.classList.remove('hidden');
    emailField.classList.remove('hidden');

    // Saved details checkbox fills in personal info AND email
    document.getElementById('use-saved-details').addEventListener('change', (e) => {
      if (e.target.checked) {
        document.getElementById('first-name').value = user.firstName || '';
        document.getElementById('last-name').value = user.lastName || '';
        document.getElementById('email').value = user.email || '';
      } else {
        document.getElementById('first-name').value = '';
        document.getElementById('last-name').value = '';
        document.getElementById('email').value = '';
      }
    });

    // Saved address checkbox fills in delivery fields
    document.getElementById('use-saved-address').addEventListener('change', (e) => {
      const saved = user.savedAddress;
      if (e.target.checked && saved) {
        document.getElementById('street-line-1').value = saved.streetLine1 || '';
        document.getElementById('street-line-2').value = saved.streetLine2 || '';
        document.getElementById('city').value = saved.city || '';
        document.getElementById('state').value = saved.state || '';
        document.getElementById('postcode').value = saved.postcode || '';
      } else {
        document.getElementById('street-line-1').value = '';
        document.getElementById('street-line-2').value = '';
        document.getElementById('city').value = '';
        document.getElementById('state').value = '';
        document.getElementById('postcode').value = '';
      }
    });
  }
}

/**
 * Validates the checkout form fields before submitting.
 * Returns an error message string if validation fails, or null if valid.
 * @returns {string|null} Error message or null.
 */
function validateForm() {
  const firstName = document.getElementById('first-name').value.trim();
  const lastName = document.getElementById('last-name').value.trim();
  const email = document.getElementById('email').value.trim();
  const streetLine1 = document.getElementById('street-line-1').value.trim();
  const city = document.getElementById('city').value.trim();
  const state = document.getElementById('state').value.trim();
  const postcode = document.getElementById('postcode').value.trim();
  const user = getCurrentUser();

  // Name validation
  if (!firstName || firstName.length < 2) return 'First name must be at least 2 characters.';
  if (!lastName || lastName.length < 2) return 'Last name must be at least 2 characters.';
  if (!/^[a-zA-Z\s'-]+$/.test(firstName)) return 'First name can only contain letters.';
  if (!/^[a-zA-Z\s'-]+$/.test(lastName)) return 'Last name can only contain letters.';

  // Email validation
  if (!user) {
    if (!email) return 'Email is required.';
    if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(email)) return 'Please enter a valid email address.';
  } else {
    if (!email) return 'Email is required.';
    if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(email)) return 'Please enter a valid email address.';
  }

  // Address validation
  if (!streetLine1 || streetLine1.length < 5) return 'Please enter a valid street address.';
  if (!city || city.length < 2) return 'Please enter a valid city.';
  if (!state || state.length < 2) return 'Please enter a valid state.';
  if (!/^\d{4}$/.test(postcode)) return 'Postcode must be 4 digits.';

  return null;
}

/**
 * Handles the proceed to payment button click.
 * Validates the form, builds the checkout payload, submits it to the
 * backend, then stores the returned orderId and invoiceId in localStorage
 * before navigating to the payment page.
 */
async function handleCheckout() {
  const errorEl = document.getElementById('checkout-error');
  errorEl.classList.add('hidden');

  const validationError = validateForm();
  if (validationError) {
    errorEl.textContent = validationError;
    errorEl.classList.remove('hidden');
    return;
  }

  const user = getCurrentUser();
  const shipmentMethodId = document.querySelector('input[name="shipment"]:checked').value;

  const firstName = document.getElementById('first-name').value.trim();
  const lastName = document.getElementById('last-name').value.trim();
  const recipientName = `${firstName} ${lastName}`;

  const payload = {
    customerId: user ? user.id : null,
    guestEmail: user ? null : document.getElementById('email').value.trim(),
    shipmentMethodId,
    deliveryAddress: {
      recipientName,
      streetLine1: document.getElementById('street-line-1').value.trim(),
      streetLine2: document.getElementById('street-line-2').value.trim(),
      city: document.getElementById('city').value.trim(),
      state: document.getElementById('state').value.trim(),
      postcode: document.getElementById('postcode').value.trim(),
      country: 'Australia',
    },
  };

  try {
    const result = await checkout(payload);

    // Store order and invoice IDs for the payment page
    localStorage.setItem('fb_order_id', result.orderId);
    localStorage.setItem('fb_invoice_id', result.invoiceId);

    window.location.href = 'payment.html';
  } catch (err) {
    errorEl.textContent = err.message || 'Checkout failed. Please try again.';
    errorEl.classList.remove('hidden');
  }
}

/**
 * Initialises the checkout page.
 * Fetches cart and shipment methods, renders them, and sets up
 * the proceed to payment button handler.
 */
async function init() {
  const user = getCurrentUser();

  try {
    const cart = await getCart();
    renderOrderSummary(cart);
  } catch (err) {
    document.getElementById('summary-items').innerHTML =
      '<p>Could not load cart. Please try again.</p>';
  }

  try {
    const methods = await getShipmentMethods();
    renderShipmentMethods(methods);
  } catch (err) {
    document.getElementById('shipment-options').innerHTML =
      '<p>Could not load shipment methods. Please try again.</p>';
  }

  setupUserFields(user);

  document.getElementById('proceed-to-payment-btn')
    .addEventListener('click', handleCheckout);
}

init();