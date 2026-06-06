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
 * Saves the updated users array back to localStorage.
 * @param {Array} users - The updated array of registered users.
 */
function saveRegisteredUsers(users) {
  localStorage.setItem('fb_registered_users', JSON.stringify(users));
}

/**
 * Validates the signup form fields.
 * Checks name length, email format, password strength,
 * and that the passwords match.
 * Returns an error message string if validation fails, or null if valid.
 * @returns {string|null} Error message or null.
 */
function validateForm() {
  const firstName = document.getElementById('first-name').value.trim();
  const lastName = document.getElementById('last-name').value.trim();
  const email = document.getElementById('email').value.trim();
  const password = document.getElementById('password').value.trim();
  const confirmPassword = document.getElementById('confirm-password').value.trim();

  if (!firstName || firstName.length < 2) return 'First name must be at least 2 characters.';
  if (!/^[a-zA-Z\s'-]+$/.test(firstName)) return 'First name can only contain letters.';
  if (!lastName || lastName.length < 2) return 'Last name must be at least 2 characters.';
  if (!/^[a-zA-Z\s'-]+$/.test(lastName)) return 'Last name can only contain letters.';
  if (!email) return 'Email is required.';
  if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(email)) return 'Please enter a valid email address.';
  if (!password || password.length < 8) return 'Password must be at least 8 characters.';
  if (!/[A-Z]/.test(password)) return 'Password must contain at least one uppercase letter.';
  if (!/[0-9]/.test(password)) return 'Password must contain at least one number.';
  if (password !== confirmPassword) return 'Passwords do not match.';

  return null;
}

/**
 * Handles the signup form submission.
 * Validates the form, checks the email is not already registered,
 * creates a new user object and saves it to localStorage,
 * then redirects to the login page.
 * @param {Event} e - The form submit event.
 */
function handleSignup(e) {
  e.preventDefault();

  const errorEl = document.getElementById('signup-error');
  errorEl.classList.add('hidden');

  const validationError = validateForm();
  if (validationError) {
    errorEl.textContent = validationError;
    errorEl.classList.remove('hidden');
    return;
  }

  const firstName = document.getElementById('first-name').value.trim();
  const lastName = document.getElementById('last-name').value.trim();
  const email = document.getElementById('email').value.trim();
  const password = document.getElementById('password').value.trim();

  const users = getRegisteredUsers();

  // Check if email is already registered
  if (users.find((u) => u.email === email)) {
    errorEl.textContent = 'An account with this email already exists.';
    errorEl.classList.remove('hidden');
    return;
  }

  // Create new user object
  const newUser = {
    id: crypto.randomUUID(),
    role: 'customer',
    firstName,
    lastName,
    email,
    password,
    defaultAddress: null,
  };

  users.push(newUser);
  saveRegisteredUsers(users);

  // Notify user of successful signup
  alert('Account created successfully! Please log in.');
  window.location.href = 'login.html';

  // Redirect to login page after successful signup
  window.location.href = 'login.html';
}

/**
 * Initialises the signup page by attaching the form submit handler.
 */
function init() {
  document.getElementById('signup-form')
    .addEventListener('submit', handleSignup);
}

init();