import { getCart, updateCartItem, removeCartItem } from './api.js';

// Tracks whether the cart popout is open
let isCartOpen = false;

/**
 * Updates the cart badge count in the nav.
 * Shows the total quantity of all items in the cart.
 * Hides the badge if the cart is empty.
 * @param {Object} cart - The cart object returned from the backend.
 */
function updateCartBadge(cart) {
  const badge = document.getElementById('cart-badge');
  if (!badge) return;

  const totalQuantity = cart && cart.totalQuantity ? cart.totalQuantity : 0;

  if (totalQuantity > 0) {
    badge.textContent = totalQuantity;
    badge.classList.remove('hidden');
  } else {
    badge.classList.add('hidden');
  }
}

/**
 * Renders the cart items into the #cart-items element.
 * Displays each item with its title, price, quantity controls,
 * and a remove button. Shows an empty message if the cart has no items.
 * @param {Object} cart - The cart object returned from the backend.
 */
function renderCart(cart) {
  const cartItems = document.getElementById('cart-items');
  const cartTotal = document.getElementById('cart-total');

  if (!cart || !cart.items || cart.items.length === 0) {
    cartItems.innerHTML = '<p>Your cart is empty. Add some books to your cart to checkout!</p>';
    cartTotal.textContent = '0.00';
    return;
  }

  cartItems.innerHTML = cart.items.map((item) => `
    <div class="cart-item" data-id="${item.bookId}" data-quantity="${item.quantity}">
      <div class="cart-item-info">
        <p class="cart-item-title">${item.bookTitle}</p>
        <p class="cart-item-price">$${item.unitPrice.toFixed(2)}</p>
      </div>
      <div class="cart-item-controls">
        <button class="quantity-btn decrease-btn" type="button" data-id="${item.bookId}" data-quantity="${item.quantity}">
          <i class="fa-solid fa-minus"></i>
        </button>
        <span class="cart-item-quantity">${item.quantity}</span>
        <button class="quantity-btn increase-btn" type="button" data-id="${item.bookId}" data-quantity="${item.quantity}">
          <i class="fa-solid fa-plus"></i>
        </button>
        <button class="remove-btn" type="button" data-id="${item.bookId}">
          <i class="fa-solid fa-trash"></i>
        </button>
      </div>
    </div>
  `).join('');

  cartTotal.textContent = cart.subtotal.toFixed(2);
}

/**
 * Updates the state of the checkout button based on the cart contents.
 * @param {Object} cart - The cart object returned from the backend.
 */
function updateCheckoutButtonState(cart) {
  const checkoutBtn = document.getElementById('checkout-btn');
  if (!checkoutBtn) return;

  checkoutBtn.disabled = !cart || !cart.items || cart.items.length === 0;
}

/**
 * Fetches the latest cart from the backend and re-renders it.
 * Keeps the popout visible after updating.
 */
async function refreshCart() {
  try {
    const cart = await getCart();
    renderCart(cart);
    updateCartBadge(cart);
    updateCheckoutButtonState(cart);
    if (isCartOpen) {
      document.getElementById('cart-popout').classList.remove('hidden');
    }
  } catch (err) {
    console.error('Could not refresh cart:', err);
  }
}

/**
 * Opens the cart popout and loads the latest cart contents.
 */
async function openCart() {
  isCartOpen = true;
  sessionStorage.setItem('cartOpen', 'true');
  document.getElementById('cart-popout').classList.remove('hidden');
  await refreshCart();
}

/**
 * Closes the cart popout.
 */
function closeCart() {
  isCartOpen = false;
  sessionStorage.removeItem('cartOpen');
  document.getElementById('cart-popout').classList.add('hidden');
}

/**
 * Initialises the cart popout by attaching event listeners.
 * Uses event delegation on #cart-items so listeners don't need to be
 * re-attached every time the cart re-renders.
 */
function initCart() {
  // Reopen cart if it was open before the page reloaded
  if (sessionStorage.getItem('cartOpen') === 'true') {
    openCart();
  }

  // Load badge count on page load
  refreshCart();
  
  // Use event delegation on the header so the listener survives nav re-renders
  document.getElementById('guest-nav').addEventListener('click', (e) => {
    if (e.target.closest('#cart-btn')) {
      openCart();
    }
  });
  const closeBtn = document.getElementById('cart-close-btn');
  if (closeBtn) {
    closeBtn.addEventListener('click', closeCart);
  }

  const checkoutBtn = document.getElementById('checkout-btn');
  if (checkoutBtn) {
    checkoutBtn.addEventListener('click', () => {
      window.location.href = 'checkout.html';
    });
  }

  // Event delegation, one listener on the parent handles all button clicks
  // inside #cart-items, even after it re-renders
  const cartItems = document.getElementById('cart-items');
  if (cartItems) {
    cartItems.addEventListener('click', async (e) => {
      const decreaseBtn = e.target.closest('.decrease-btn');
      const increaseBtn = e.target.closest('.increase-btn');
      const removeBtn = e.target.closest('.remove-btn');

      if (decreaseBtn) {
        const bookId = decreaseBtn.dataset.id;
        const quantity = parseInt(decreaseBtn.dataset.quantity);
        try {
          if (quantity <= 1) {
            await removeCartItem(bookId);
          } else {
            await updateCartItem(bookId, quantity - 1);
          }
          await refreshCart();
        } catch (err) {
          alert('Could not update cart. Please try again.');
        }
      }

      if (increaseBtn) {
        const bookId = increaseBtn.dataset.id;
        const quantity = parseInt(increaseBtn.dataset.quantity);
        try {
          await updateCartItem(bookId, quantity + 1);
          await refreshCart();
        } catch (err) {
          alert('Could not update cart. Please try again.');
        }
      }

      if (removeBtn) {
        const bookId = removeBtn.dataset.id;
        try {
          await removeCartItem(bookId);
          await refreshCart();
        } catch (err) {
          alert('Could not remove item. Please try again.');
        }
      }
    });
  }
}

initCart();