/**
 * Builds and inserts the footer into the #main-footer element.
 * The footer is consistent across all pages and includes the brand,
 * quick links, and copyright notice.
 */
function renderFooter() {
  const footer = document.getElementById('main-footer');
  if (!footer) return;

  footer.innerHTML = `
    <div class="footer-content">

      <div class="footer-brand">
        <i class="fa-solid fa-book-open"></i>
        <span>Favourite Books</span>
      </div>

      <div class="footer-links">
        <h3>Join our Community!</h3>
        <ul>
          <li><a href="login.html">Sign In</a></li>
          <li><a href="signup.html">Sign Up</a></li>
        </ul>
      </div>

      <div class="footer-copyright">
        <p>&copy; 2026 Favourite Books. All rights reserved.</p>
      </div>

    </div>
  `;
}

renderFooter();