# Travelogue PK - Magazine Website

A beautiful, responsive ASP.NET Core MVC website for Travelogue PK, showcasing Pakistan's travel magazines and tours.

## Features

- **Fully Responsive Design** - Works seamlessly on desktop, tablet, and mobile devices
- **Modern UI** - Clean, professional design using Tailwind CSS
- **Multiple Pages:**
  - Home Page with hero banner, featured magazines, and sections
  - About Us page
  - Categories/Shop page with filters and product grid
  - Product Detail page with tabs and reviews
  - Contact Us page with form
  - User Profile/Dashboard
  - Checkout page with payment integration UI
- **Reusable Components:**
  - Header navigation with search
  - Footer with links and contact info
  - Login modal
  - Magazine cards
  - Mountain dividers for visual appeal
- **Interactive Elements:**
  - Product image gallery
  - Tabbed content
  - Smooth animations
  - Hover effects

## Technology Stack

- **Backend:** ASP.NET Core MVC 10
- **Frontend:** 
  - Tailwind CSS (via CDN)
  - Font Awesome Icons
  - Google Fonts (Poppins)
  - Vanilla JavaScript
- **Styling:** Custom CSS for special effects

## Project Structure

```
magazine-app/
├── Controllers/
│   ├── HomeController.cs
│   ├── AboutController.cs
│   ├── CategoriesController.cs
│   ├── ProductController.cs
│   ├── ContactController.cs
│   ├── ProfileController.cs
│   └── CheckoutController.cs
├── Views/
│   ├── Shared/
│   │   ├── _Layout.cshtml
│   │   ├── _Header.cshtml
│   │   ├── _Footer.cshtml
│   │   ├── _LoginModal.cshtml
│   │   └── _MagazineCard.cshtml
│   ├── Home/
│   │   └── Index.cshtml
│   ├── About/
│   │   └── Index.cshtml
│   ├── Categories/
│   │   └── Index.cshtml
│   ├── Product/
│   │   └── Detail.cshtml
│   ├── Contact/
│   │   └── Index.cshtml
│   ├── Profile/
│   │   └── Index.cshtml
│   └── Checkout/
│       └── Index.cshtml
├── wwwroot/
│   ├── css/
│   │   ├── site.css
│   │   └── custom.css
│   ├── js/
│   │   └── site.js
│   └── images/
│       └── (place your images here)
├── Models/
├── Program.cs
└── appsettings.json
```

## Getting Started

### Prerequisites

- .NET 10 SDK installed
- Visual Studio 2022 or VS Code
- A modern web browser

### Installation & Running

1. **Clone or navigate to the project directory:**
   ```bash
   cd D:\Project\Magazine\magazine-app\magazine-app
   ```

2. **Restore dependencies:**
   ```bash
   dotnet restore
   ```

3. **Run the application:**
   ```bash
   dotnet run
   ```

4. **Open your browser and navigate to:**
   ```
   https://localhost:5001
   ```
   or
   ```
   http://localhost:5000
   ```

### Adding Images

To add your own images:

1. Place images in the `wwwroot/images/` folder
2. Recommended image files:
   - `logo.png` - Main logo (transparent background)
   - `logo-white.png` - White version for dark backgrounds
   - `hero-bg.jpg` - Home page hero background
   - `magazine-1.jpg` through `magazine-12.jpg` - Magazine covers
   - `about-hero.jpg` - About page hero
   - `contact-hero.jpg` - Contact page hero
   - `categories-hero.jpg` - Categories page hero
   - `video-thumbnail.jpg` - Video section placeholder
   - `pakistan-beauty.jpg` - Pakistan beauty section
   - `storyteller.jpg` - Newsletter section image
   - `user-profile.jpg` - Profile avatar
   - Magazine detail images, etc.

3. If images are missing, the site will use placeholder images from Unsplash or placeholder services.

## Color Scheme

- **Primary Orange:** `#FF6B35`
- **Primary Blue:** `#0C789A`
- **Dark Navy:** `#1B4965`
- **Light Blue:** `#41B3D3`

## Customization

### Changing Colors

Edit the Tailwind configuration in `Views/Shared/_Layout.cshtml`:

```javascript
tailwind.config = {
    theme: {
        extend: {
            colors: {
                'primary-orange': '#FF6B35',  // Change this
                'primary-blue': '#0C789A',    // Change this
                'dark-navy': '#1B4965',       // Change this
                'light-blue': '#41B3D3',      // Change this
            }
        }
    }
}
```

### Modifying Content

- **Page Content:** Edit the respective `.cshtml` files in the `Views` folder
- **Navigation:** Edit `Views/Shared/_Header.cshtml`
- **Footer:** Edit `Views/Shared/_Footer.cshtml`
- **Styling:** Edit `wwwroot/css/custom.css`

## Pages Overview

### 1. Home Page (`/`)
- Hero banner with call-to-action
- Pakistan beauty section
- Featured magazines grid
- Video section
- "How It Works" process
- Newsletter subscription

### 2. About Us (`/About`)
- Hero section with breadcrumb
- "What We Offer" services
- Services diagram
- Video section
- Testimonials carousel

### 3. Categories (`/Categories`)
- Filterable product grid
- Sidebar with category filters
- Price range slider
- Sorting options
- Magazine cards with add-to-cart

### 4. Product Detail (`/Product/Detail`)
- Product image gallery with thumbnails
- Tabbed content (Features, Description, Reviews)
- Rating system
- Add to cart functionality
- Related products
- Newsletter section

### 5. Contact Us (`/Contact`)
- Hero section
- Office information (hours, location, phone)
- Contact form with validation
- Map integration placeholder

### 6. User Profile (`/Profile`)
- Profile banner with avatar
- Contact information sidebar
- Purchased magazines list
- Preview and download buttons

### 7. Checkout (`/Checkout`)
- PayPal express option
- Contact information form
- Billing/shipping details
- Payment method selection
- Order summary sidebar
- Voucher code input

## Browser Support

- Chrome (latest)
- Firefox (latest)
- Safari (latest)
- Edge (latest)

## Notes

- This is a **front-end only implementation** with no backend logic
- Payment integration is UI only (not functional)
- No database connectivity
- All data is static/hardcoded
- Images will fall back to placeholders if not found

## Future Enhancements (Backend Implementation)

To make this fully functional, you would need to add:

- User authentication and authorization
- Database integration (SQL Server/PostgreSQL)
- Product catalog management
- Shopping cart functionality
- Order processing system
- Payment gateway integration (Stripe, PayPal)
- File upload for magazines (PDF)
- Admin panel for content management
- Email service for notifications
- Search functionality

## Support

For issues or questions, please contact the development team.

## License

Copyright © 2025 Travelogue PK. All rights reserved.

