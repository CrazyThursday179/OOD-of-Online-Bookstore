/**
 * Builds and inserts the cart popout HTML into the document body.
 * Called on any page that needs the cart popout.
 */
function renderCartPopout() {
  const popout = document.createElement('div');
  popout.id = 'cart-popout';
  popout.className = 'cart-popout hidden';

  popout.innerHTML = `
    <div class="cart-header">
      <h2>Your Cart</h2>
      <button id="cart-close-btn" type="button" aria-label="Close cart">
        <i class="fas fa-times"></i>
      </button>
    </div>
    <div id="cart-items"></div>
    <div class="cart-footer">
      <p>Total Price: $<span id="cart-total">0.00</span></p>
      <button id="checkout-btn" type="button" disabled>Checkout</button> 
    </div>
  `;

  document.body.appendChild(popout);
}

function updateCheckoutButtonState() {
  const checkoutBtn = document.getElementById('checkout-btn');
  const cartItems = document.getElementById('cart-items');
  const hasItems = cartItems && cartItems.children.length > 0;
  checkoutBtn.disabled = !hasItems;
}

renderCartPopout();
updateCheckoutButtonState();