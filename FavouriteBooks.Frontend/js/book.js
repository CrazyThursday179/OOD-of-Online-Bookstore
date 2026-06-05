import { getBooks, addToCart, getCategories } from './api.js';

/**
 * Retrieves the book ID from the URL query parameters.
 * The catalogue page passes the ID as ?id=bookId when navigating here.
 * @returns {string|null} The book ID or null if not found.
 */
function getBookIdFromUrl() {
  const params = new URLSearchParams(window.location.search);
  return params.get('id');
}

/**
 * Renders the book details into the page elements.
 * @param {Object} book - The book object returned from the backend.
 */
function renderBook(book) {
  document.title = `${book.title} - Favourite Books`;

  // Show real image if available, otherwise keep the icon placeholder
  if (book.coverImageUrl) {
    const icon = document.getElementById('book-cover-icon');
    const img = document.createElement('img');
    img.src = book.coverImageUrl;
    img.alt = `Cover of ${book.title}`;
    img.id = 'book-cover';
    icon.replaceWith(img);
  }

  document.getElementById('book-title').textContent = book.title;
  document.getElementById('book-author').textContent = `by ${book.author}`;
  document.getElementById('book-description').textContent = book.description || 'No description available.';
  document.getElementById('book-genre').textContent = `Genre: ${book.categoryNames ? book.categoryNames.join(', ') : 'Uncategorised'}`;
  document.getElementById('book-stock').textContent = book.stockQuantity > 0
    ? `In Stock: ${book.stockQuantity} available`
    : 'Out of Stock';
  document.getElementById('book-price').textContent = `$${book.price.toFixed(2)}`;

  const addBtn = document.getElementById('add-to-cart-btn');
  if (book.stockQuantity <= 0) {
    addBtn.disabled = true;
    addBtn.textContent = 'Out of Stock';
  }
}

/**
 * Initialises the book detail page.
 * Fetches the book by ID from the URL, renders it, and sets up
 * the add to cart button handler.
 */
async function init() {
  // Back button navigates to catalogue
  document.getElementById('back-btn').addEventListener('click', () => {
    window.location.href = 'catalogue.html';
  });

  const bookId = getBookIdFromUrl();

  if (!bookId) {
    document.querySelector('main').innerHTML = '<p>Book not found.</p>';
    return;
  }

  try {
    // Fetch books and categories together
    const [books, categories] = await Promise.all([getBooks(), getCategories()]);
    const book = books.find((b) => b.id === bookId);

    if (!book) {
      document.querySelector('main').innerHTML = '<p>Book not found.</p>';
      return;
    }

    // Match category IDs to their names
    const categoryNames = book.categoryIds.map((id) => {
      const category = categories.find((c) => c.id === id);
      return category ? category.name : 'Uncategorised';
    });

    book.categoryNames = categoryNames;
    renderBook(book);
  } catch (err) {
    document.querySelector('main').innerHTML =
      '<p>Could not load book details. Please try again later.</p>';
  }

  const addBtn = document.getElementById('add-to-cart-btn');
  addBtn.addEventListener('click', async () => {
    const quantity = parseInt(document.getElementById('quantity-select').value);
    try {
      await addToCart(bookId, quantity);
      addBtn.innerHTML = '<i class="fa-solid fa-check"></i> Added!';
      setTimeout(() => {
        addBtn.innerHTML = '<i class="fa-solid fa-cart-plus"></i> Add to Cart';
      }, 1000);
    } catch (err) {
      alert('Could not add to cart. Please try again.');
    }
  });
}

init();