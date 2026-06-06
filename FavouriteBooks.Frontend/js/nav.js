/**
 * Retrieves the current user object from localStorage.
 * Returns null if no user is logged in, which indicates a guest.
 * @returns {Object|null} The user object or null.
 */
function getCurrentUser() {
  const user = localStorage.getItem('fb_user');
  return user ? JSON.parse(user) : null;
}

/**
 * Determines the active page by checking the current URL pathname.
 * Used to apply the active class to the correct nav link.
 * @returns {string} A key identifying the current page.
 */
function getActivePage() {
  const path = window.location.pathname;
  if (path.includes('catalogue')) return 'catalogue';
  if (path.includes('login')) return 'login';
  if (path.includes('signup')) return 'signup';
  if (path.includes('account')) return 'account';
  if (path.includes('orders')) return 'orders';
  return ''; // TODO: Add admin pages once they're decided
}

/**
 * Returns the navigation links for a given user role.
 * Guest users see catalogue, login, and sign up.
 * Customer users see catalogue, orders, and account.
 * Admin links are left as a placeholder for future implementation.
 * @param {string} role - The user role: 'guest', 'customer', or 'admin'.
 * @returns {Array} Array of link objects with label, href, and key.
 */
function getNavLinks(role) {
  if (role === 'customer') {
    return [
      { label: 'Catalogue', href: 'catalogue.html', key: 'catalogue' },
      { label: 'My Orders', href: 'orders.html', key: 'orders' },
      { label: 'My Account', href: 'account.html', key: 'account' },
    ];
  }

  if (role === 'admin') {
    // TODO: update these links once admin pages are decided
    return [
      { label: 'Catalogue', href: 'catalogue.html', key: 'catalogue' },
    ];
  }

  // Default: guest
  return [
    { label: 'Catalogue', href: 'catalogue.html', key: 'catalogue' },
    { label: 'Login', href: 'login.html', key: 'login' },
    { label: 'Sign Up', href: 'signup.html', key: 'signup' },
  ];
}

/**
 * Clears user data from localStorage and redirects to the catalogue page.
 * Called when the sign out button is clicked.
 */
function handleSignOut() {
  localStorage.removeItem('fb_user');
  window.location.href = 'catalogue.html';
}

/**
 * Builds and inserts the navigation bar into the #main-nav element.
 * Renders different links based on the current user role.
 * Highlights the active page link automatically.
 * Shows the cart button for all users and sign out only for logged in users.
 */
function renderNav() {
  const nav = document.getElementById('main-nav');
  if (!nav) return;

  const user = getCurrentUser();
  const role = user ? user.role : 'guest';
  const activePage = getActivePage();
  const links = getNavLinks(role);

  const linkItems = links.map((link) => {
    const isActive = link.key === activePage ? 'active' : '';
    return `<li><a href="${link.href}" class="${isActive}">${link.label}</a></li>`;
  }).join('');

  // Cart is available to all users including guests
  const cartIcon = `<button id="cart-btn" type="button" aria-label="Open cart">
    <i class="fa-solid fa-cart-shopping"></i>
    <span id="cart-badge" class="cart-badge hidden">0</span>
  </button>`;

  const signOutBtn = role !== 'guest'
    ? `<button id="sign-out-btn" type="button">Sign Out</button>`
    : '';

  nav.innerHTML = `
    <div class="nav-brand-row">
      <div class="nav-brand">
        <span>Favourite Books</span>
      </div>
      <div class="nav-actions">
        ${cartIcon}
        ${signOutBtn}
      </div>
    </div>
    <nav>
      <ul>${linkItems}</ul>
    </nav>
  `;

  // Attach sign out handler after HTML is inserted into the DOM
  const signOutEl = document.getElementById('sign-out-btn');
  if (signOutEl) {
    signOutEl.addEventListener('click', handleSignOut);
  }
}

renderNav();