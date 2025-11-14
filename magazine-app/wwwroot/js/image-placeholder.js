// Image Placeholder Helper
// Creates inline SVG placeholders when images fail to load

function createPlaceholder(color, text) {
    const svg = `
        <svg xmlns="http://www.w3.org/2000/svg" width="300" height="400" viewBox="0 0 300 400">
            <rect width="300" height="400" fill="${color}"/>
            <text x="50%" y="50%" font-family="Arial, sans-serif" font-size="24" fill="white" text-anchor="middle" dominant-baseline="middle">
                ${text}
            </text>
        </svg>
    `;
    return 'data:image/svg+xml;base64,' + btoa(svg);
}

// Replace failed magazine images with colored placeholders
document.addEventListener('DOMContentLoaded', function() {
    const images = document.querySelectorAll('img[data-fallback-color]');
    images.forEach(img => {
        img.onerror = function() {
            const color = this.getAttribute('data-fallback-color') || '#FF6B35';
            const text = this.getAttribute('data-fallback-text') || 'Magazine';
            this.src = createPlaceholder(color, text);
            this.onerror = null; // Prevent infinite loop
        };
    });
});

// Helper for logo fallback
function logoFallback(element) {
    element.style.display = 'none';
    const fallbackText = element.nextElementSibling;
    if (fallbackText) {
        fallbackText.style.display = 'block';
    }
}

