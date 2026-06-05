import { getBooks, getCategories, addToCart } from './api.js';

// Keeps a copy of all books so we can filter locally
// without making a new API call every time the user types or changes genre
let allBooks = [];

/**
 * Renders an array of books into the #book-list element.
 * Each book displays its cover, title, author, price, and an add to cart button.
 * Clicking a card navigates to the book detail page.
 * Clicking add to cart adds the book to the current session's cart.
 * @param {Array} books - Array of book objects to display.
 */
function renderBooks(books) {
  const bookList = document.getElementById('book-list');

  if (books.length === 0) {
    bookList.innerHTML = '<p>No books found.</p>';
    return;
  }

  bookList.innerHTML = books.map((book) => `
    <div class="book-card" data-id="${book.id}">
      ${book.coverImageUrl 
        ? `<img src="${book.coverImageUrl}" alt="Cover of ${book.title}">` 
        : `<i class="fa-solid fa-book fa-3x"></i>`
    }
      <h3>${book.title}</h3>
      <p class="book-author">${book.author}</p>
      <p class="book-price">$${book.price.toFixed(2)}</p>
      <button class="add-to-cart-btn" data-id="${book.id}">
        <i class="fa-solid fa-cart-plus"></i> Add to Cart
      </button>
    </div>
  `).join('');

  // Attach click handler to each card for navigating to book detail page
  document.querySelectorAll('.book-card').forEach((card) => {
    card.addEventListener('click', (e) => {
      // Ignore clicks on the add to cart button so it doesn't also navigate
      if (e.target.closest('.add-to-cart-btn')) return;
      window.location.href = `book.html?id=${card.dataset.id}`;
    });
  });

  // Attach click handler to each add to cart button
  document.querySelectorAll('.add-to-cart-btn').forEach((btn) => {
    btn.addEventListener('click', async (e) => {
      e.stopPropagation();
      try {
        await addToCart(btn.dataset.id);
        // Brief visual feedback so the user knows the item was added
        btn.textContent = 'Added!';
        setTimeout(() => {
          btn.innerHTML = '<i class="fa-solid fa-cart-plus"></i> Add to Cart';
        }, 1000);
      } catch (err) {
        alert('Could not add to cart. Please try again.');
      }
    });
  });
}

/**
 * Fetches categories from the backend and populates the genre filter dropdown.
 * Falls back silently if categories cannot be loaded.
 */
async function populateGenreFilter() {
  try {
    const categories = await getCategories();
    const select = document.getElementById('genre-filter');

    categories.forEach((category) => {
      const option = document.createElement('option');
      option.value = category.id;
      option.textContent = category.name;
      select.appendChild(option);
    });
  } catch (err) {
    console.error('Could not load categories:', err);
  }
}

/**
 * Filters the local allBooks array by search term and selected genre.
 * Runs entirely on the frontend to avoid unnecessary API calls.
 * Matches search term against both title and author fields.
 */
function filterBooks() {
  const searchTerm = document.getElementById('search-input').value.toLowerCase();
  const categoryId = document.getElementById('genre-filter').value;

  const filtered = allBooks.filter((book) => {
    const matchesSearch = book.title.toLowerCase().includes(searchTerm)
      || book.author.toLowerCase().includes(searchTerm);
    const matchesCategory = categoryId === '' || book.categoryIds.includes(categoryId);
    return matchesSearch && matchesCategory;
  });

  renderBooks(filtered);
}

/**
 * Initialises the catalogue page.
 * Fetches all books and categories, renders them, and sets up event listeners
 * for the search input and genre filter dropdown.
 */
async function init() {
  try {
    allBooks = await getBooks();
    renderBooks(allBooks);
  } catch (err) {
    document.getElementById('book-list').innerHTML =
      '<p>Could not load books. Please try again later.</p>';
  }

  await populateGenreFilter();

  document.getElementById('search-input')
    .addEventListener('input', filterBooks);

  document.getElementById('genre-filter')
    .addEventListener('change', filterBooks);
}

init();