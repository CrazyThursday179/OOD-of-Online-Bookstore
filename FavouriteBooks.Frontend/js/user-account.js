/**
 * Retrieves the current user from localStorage.
 * Redirects to login if no user is found.
 * @returns {Object|null} The current user object or null.
 */
function getCurrentUser() {
  const user = localStorage.getItem('fb_user');
  return user ? JSON.parse(user) : null;
}

/**
 * Retrieves all registered users from localStorage.
 * @returns {Array} Array of registered user objects.
 */
function getRegisteredUsers() {
  const users = localStorage.getItem('fb_registered_users');
  return users ? JSON.parse(users) : [];
}

/**
 * Saves the updated users array back to localStorage.
 * Also updates the current session user if their details changed.
 * @param {Array} users - The updated array of registered users.
 * @param {Object} updatedUser - The updated current user object.
 */
function saveUsers(users, updatedUser) {
  localStorage.setItem('fb_registered_users', JSON.stringify(users));
  // Update the current session user without the password
  const { password: _, ...safeUser } = updatedUser;
  localStorage.setItem('fb_user', JSON.stringify(safeUser));
}

/**
 * Renders the account information section with the current user's details.
 * Pre-fills the delivery address form if a default address is saved.
 * @param {Object} user - The current user object.
 */
function renderAccountInfo(user) {
  document.getElementById('user-name').textContent =
    `${user.firstName} ${user.lastName}`;
  document.getElementById('user-email').textContent = user.email;

  // Pre-fill address form if a default address exists
  if (user.defaultAddress) {
    const addr = user.defaultAddress;
    document.getElementById('street-line-1').value = addr.streetLine1 || '';
    document.getElementById('street-line-2').value = addr.streetLine2 || '';
    document.getElementById('city').value = addr.city || '';
    document.getElementById('state').value = addr.state || '';
    document.getElementById('postcode').value = addr.postcode || '';
  }
}

/**
 * Validates the delivery address form fields.
 * Returns an error message string if validation fails, or null if valid.
 * @returns {string|null} Error message or null.
 */
function validateAddress() {
  const streetLine1 = document.getElementById('street-line-1').value.trim();
  const city = document.getElementById('city').value.trim();
  const state = document.getElementById('state').value.trim();
  const postcode = document.getElementById('postcode').value.trim();

  if (!streetLine1 || streetLine1.length < 5) return 'Please enter a valid street address.';
  if (!city || city.length < 2) return 'Please enter a valid city.';
  if (!state || state.length < 2) return 'Please enter a valid state.';
  if (!/^\d{4}$/.test(postcode)) return 'Postcode must be 4 digits.';

  return null;
}

/**
 * Handles saving the delivery address form.
 * Validates the fields, updates the user's defaultAddress in localStorage,
 * and shows a confirmation message.
 * @param {Event} e - The form submit event.
 * @param {Object} user - The current user object.
 */
function handleSaveAddress(e, user) {
  e.preventDefault();

  const errorEl = document.getElementById('account-error');
  errorEl.classList.add('hidden');

  const validationError = validateAddress();
  if (validationError) {
    errorEl.textContent = validationError;
    errorEl.classList.remove('hidden');
    return;
  }

  const updatedAddress = {
    recipientName: `${user.firstName} ${user.lastName}`,
    streetLine1: document.getElementById('street-line-1').value.trim(),
    streetLine2: document.getElementById('street-line-2').value.trim(),
    city: document.getElementById('city').value.trim(),
    state: document.getElementById('state').value.trim(),
    postcode: document.getElementById('postcode').value.trim(),
    country: 'Australia',
  };

  const users = getRegisteredUsers();
  const userIndex = users.findIndex((u) => u.id === user.id);

  if (userIndex !== -1) {
    users[userIndex].defaultAddress = updatedAddress;
    saveUsers(users, users[userIndex]);
  }

  alert('Address saved successfully!');
}

/**
 * Handles editing the account email.
 * Prompts the user for a new email, validates it, and updates localStorage.
 * @param {Object} user - The current user object.
 */
function handleEditEmail(user) {
  const newEmail = prompt('Enter your new email address:');
  if (!newEmail) return;

  if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(newEmail.trim())) {
    alert('Please enter a valid email address.');
    return;
  }

  const users = getRegisteredUsers();
  const userIndex = users.findIndex((u) => u.id === user.id);

  if (userIndex !== -1) {
    users[userIndex].email = newEmail.trim();
    saveUsers(users, users[userIndex]);
    document.getElementById('user-email').textContent = newEmail.trim();
    alert('Email updated successfully!');
  }
}

/**
 * Handles changing the account password.
 * Prompts the user for their current password, then a new password,
 * validates both and updates localStorage.
 * @param {Object} user - The current user object.
 */
function handleChangePassword(user) {
  const users = getRegisteredUsers();
  const fullUser = users.find((u) => u.id === user.id);

  const currentPassword = prompt('Enter your current password:');
  if (!currentPassword) return;

  if (fullUser.password !== currentPassword) {
    alert('Incorrect current password.');
    return;
  }

  const newPassword = prompt('Enter your new password (min 8 characters, 1 uppercase, 1 number):');
  if (!newPassword) return;

  if (newPassword.length < 8) {
    alert('Password must be at least 8 characters.');
    return;
  }
  if (!/[A-Z]/.test(newPassword)) {
    alert('Password must contain at least one uppercase letter.');
    return;
  }
  if (!/[0-9]/.test(newPassword)) {
    alert('Password must contain at least one number.');
    return;
  }

  const userIndex = users.findIndex((u) => u.id === user.id);
  users[userIndex].password = newPassword;
  saveUsers(users, users[userIndex]);
  alert('Password changed successfully!');
}

/**
 * Handles deleting the account.
 * Asks for confirmation, removes the user from localStorage,
 * clears the session and redirects to the catalogue.
 * @param {Object} user - The current user object.
 */
function handleDeleteAccount(user) {
  const confirmed = confirm(
    'Are you sure you want to delete your account? This cannot be undone.'
  );
  if (!confirmed) return;

  const users = getRegisteredUsers();
  const updatedUsers = users.filter((u) => u.id !== user.id);
  localStorage.setItem('fb_registered_users', JSON.stringify(updatedUsers));
  localStorage.removeItem('fb_user');

  alert('Your account has been deleted.');
  window.location.href = 'catalogue.html';
}

/**
 * Initialises the account page.
 * Redirects to login if no user is logged in or if the user is an admin.
 * Renders account info and sets up all button handlers.
 */
function init() {
  const user = getCurrentUser();

  // Redirect to login if not logged in or if admin
  if (!user || user.role !== 'customer') {
    window.location.href = 'login.html';
    return;
  }

  renderAccountInfo(user);

  document.getElementById('delivery-form')
    .addEventListener('submit', (e) => handleSaveAddress(e, user));

  document.getElementById('edit-email-btn')
    .addEventListener('click', () => handleEditEmail(user));

  document.getElementById('change-password-btn')
    .addEventListener('click', () => handleChangePassword(user));

  document.getElementById('delete-account-btn')
    .addEventListener('click', () => handleDeleteAccount(user));
}

init();