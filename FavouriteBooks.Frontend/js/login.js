/**
 * Retrieves all registered users from localStorage.
 * Returns an empty array if none exist.
 * @returns {Array} Array of registered user objects.
 */
function getRegisteredUsers() {
  const users = localStorage.getItem('fb_registered_users');
  return users ? JSON.parse(users) : [];
}

/**
 * Validates the login form fields.
 * Returns an error message string if validation fails, or null if valid.
 * @returns {string|null} Error message or null.
 */
function validateForm() {
  const email = document.getElementById('email').value.trim();
  const password = document.getElementById('password').value.trim();

  if (!email) return 'Email is required.';
  if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(email)) return 'Please enter a valid email address.';
  if (!password) return 'Password is required.';

  return null;
}

// Known admin email from admins.json
const ADMIN_EMAIL = 'admin@favouritebooks.test';
const ADMIN_PASSWORD = 'admin123';

/**
 * Handles the login form submission.
 * First checks if the email matches the admin account.
 * Then checks registered customers in localStorage.
 * Redirects to the appropriate page based on role.
 * @param {Event} e - The form submit event.
 */
function handleLogin(e) {
  e.preventDefault();

  const errorEl = document.getElementById('login-error');
  errorEl.classList.add('hidden');

  const validationError = validateForm();
  if (validationError) {
    errorEl.textContent = validationError;
    errorEl.classList.remove('hidden');
    return;
  }

  const email = document.getElementById('email').value.trim();
  const password = document.getElementById('password').value.trim();

  // Check if admin
  if (email === ADMIN_EMAIL && password === ADMIN_PASSWORD) {
    localStorage.setItem('fb_user', JSON.stringify({
      id: '15f7e8c3-0e07-4167-84e2-7b4ad1cf0ca3',
      role: 'admin',
      firstName: 'Mia',
      lastName: 'Patel',
      email: ADMIN_EMAIL,
    }));
    window.location.href = 'admin.html';
    return;
  }

  // Check registered customers
  const users = getRegisteredUsers();
  const user = users.find((u) => u.email === email && u.password === password);

  if (!user) {
    errorEl.textContent = 'Incorrect email or password. Please try again.';
    errorEl.classList.remove('hidden');
    return;
  }

  // Store user without password
  const { password: _, ...safeUser } = user;
  localStorage.setItem('fb_user', JSON.stringify(safeUser));

  window.location.href = 'catalogue.html';
}

/**
 * Initialises the login page by attaching the form submit handler.
 */
function init() {
  document.getElementById('login-form')
    .addEventListener('submit', handleLogin);
}

init();