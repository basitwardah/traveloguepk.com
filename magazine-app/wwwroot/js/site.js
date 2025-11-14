// Travelogue PK - Site JavaScript

// Initialize on page load
document.addEventListener('DOMContentLoaded', function() {
    console.log('Travelogue PK website loaded successfully!');
    
    // Initialize AOS (Animate on Scroll) if available
    if (typeof AOS !== 'undefined') {
        AOS.init({
            duration: 800,
            once: true
        });
    }
});

// Smooth scroll to top
function scrollToTop() {
    window.scrollTo({
        top: 0,
        behavior: 'smooth'
    });
}

// Newsletter subscription
function subscribeNewsletter(email) {
    // This would integrate with a backend API
    console.log('Subscribing email:', email);
    alert('Thank you for subscribing to our newsletter!');
}

// Add product to wishlist
function addToWishlist(productId) {
    console.log('Adding to wishlist:', productId);
    alert('Product added to wishlist!');
}

// Quick view product
function quickViewProduct(productId) {
    console.log('Quick view product:', productId);
    // Would open a modal with product details
}

// Filter products
function filterProducts(category) {
    console.log('Filtering by category:', category);
    // Would filter the product grid
}

// Sort products
function sortProducts(sortBy) {
    console.log('Sorting by:', sortBy);
    // Would sort the product grid
}

// Update cart quantity
function updateCartQuantity(itemId, quantity) {
    console.log('Updating cart item:', itemId, 'to quantity:', quantity);
    // Would update cart in backend
}

// Remove from cart
function removeFromCart(itemId) {
    console.log('Removing from cart:', itemId);
    // Would remove item from cart
}

// Apply voucher code
function applyVoucher(code) {
    console.log('Applying voucher:', code);
    // Would validate and apply voucher code
}

// Sticky header on scroll
let lastScroll = 0;
window.addEventListener('scroll', function() {
    const header = document.querySelector('header');
    const currentScroll = window.pageYOffset;
    
    if (currentScroll <= 0) {
        header.classList.remove('scroll-up');
        return;
    }
    
    if (currentScroll > lastScroll && !header.classList.contains('scroll-down')) {
        // Scroll Down
        header.classList.remove('scroll-up');
        header.classList.add('scroll-down');
    } else if (currentScroll < lastScroll && header.classList.contains('scroll-down')) {
        // Scroll Up
        header.classList.remove('scroll-down');
        header.classList.add('scroll-up');
    }
    lastScroll = currentScroll;
});

// Lazy load images
if ('IntersectionObserver' in window) {
    const imageObserver = new IntersectionObserver((entries, observer) => {
        entries.forEach(entry => {
            if (entry.isIntersecting) {
                const img = entry.target;
                img.src = img.dataset.src;
                img.classList.remove('lazy');
                imageObserver.unobserve(img);
            }
        });
    });

    document.querySelectorAll('img.lazy').forEach(img => {
        imageObserver.observe(img);
    });
}

// Back to top button
const backToTopButton = document.getElementById('back-to-top');
if (backToTopButton) {
    window.addEventListener('scroll', () => {
        if (window.pageYOffset > 300) {
            backToTopButton.classList.remove('hidden');
        } else {
            backToTopButton.classList.add('hidden');
        }
    });

    backToTopButton.addEventListener('click', scrollToTop);
}

// Form validation
function validateContactForm(form) {
    const firstName = form.querySelector('[name="firstName"]');
    const email = form.querySelector('[name="email"]');
    const message = form.querySelector('[name="message"]');
    
    let isValid = true;
    
    if (!firstName || firstName.value.trim() === '') {
        alert('Please enter your first name');
        isValid = false;
    }
    
    if (!email || !isValidEmail(email.value)) {
        alert('Please enter a valid email address');
        isValid = false;
    }
    
    if (!message || message.value.trim() === '') {
        alert('Please enter your message');
        isValid = false;
    }
    
    return isValid;
}

function isValidEmail(email) {
    const re = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    return re.test(email);
}

// Initialize tooltips (if using Bootstrap tooltips)
if (typeof bootstrap !== 'undefined') {
    const tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'));
    tooltipTriggerList.map(function (tooltipTriggerEl) {
        return new bootstrap.Tooltip(tooltipTriggerEl);
    });
}

// Product quantity controls
document.querySelectorAll('.quantity-minus').forEach(button => {
    button.addEventListener('click', function() {
        const input = this.nextElementSibling;
        const currentValue = parseInt(input.value);
        if (currentValue > 1) {
            input.value = currentValue - 1;
        }
    });
});

document.querySelectorAll('.quantity-plus').forEach(button => {
    button.addEventListener('click', function() {
        const input = this.previousElementSibling;
        const currentValue = parseInt(input.value);
        input.value = currentValue + 1;
    });
});

// Cookie consent (if needed)
function acceptCookies() {
    localStorage.setItem('cookieConsent', 'accepted');
    document.getElementById('cookie-banner')?.classList.add('hidden');
}

// Check cookie consent on load
if (localStorage.getItem('cookieConsent') !== 'accepted') {
    document.getElementById('cookie-banner')?.classList.remove('hidden');
}

// Print functionality
function printPage() {
    window.print();
}

// Share functionality
function sharePage(platform) {
    const url = encodeURIComponent(window.location.href);
    const title = encodeURIComponent(document.title);
    
    let shareUrl = '';
    
    switch(platform) {
        case 'facebook':
            shareUrl = `https://www.facebook.com/sharer/sharer.php?u=${url}`;
            break;
        case 'twitter':
            shareUrl = `https://twitter.com/intent/tweet?url=${url}&text=${title}`;
            break;
        case 'linkedin':
            shareUrl = `https://www.linkedin.com/shareArticle?mini=true&url=${url}&title=${title}`;
            break;
        case 'whatsapp':
            shareUrl = `https://wa.me/?text=${title}%20${url}`;
            break;
    }
    
    if (shareUrl) {
        window.open(shareUrl, '_blank', 'width=600,height=400');
    }
}

console.log('🎉 Welcome to Travelogue PK - Let\'s Make Your Travel Story!');
