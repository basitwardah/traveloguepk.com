# Magazine App - Backend Setup Guide

## ✅ Backend Implementation Complete!

This document contains all the instructions needed to set up and run the complete backend for your Magazine application built with ASP.NET Core MVC 10.

---

## 📋 What Has Been Implemented

### 1. **Authentication & Authorization (Microsoft Identity)**
- ✅ Custom `ApplicationUser` extending `IdentityUser`
- ✅ Four roles: **SuperAdmin**, **Admin**, **Uploader**, **Customer**
- ✅ Role-based authorization throughout the application
- ✅ Automatic role seeding on startup
- ✅ Cookie-based authentication with secure settings

### 2. **Database Models (Code-First)**
- ✅ **Guide** - Magazine/travelogue model with cover, PDF, metadata
- ✅ **UserActivity** - Tracks user actions with IP and UserAgent
- ✅ **LogEntry** - Application logging system
- ✅ **ApplicationUser** - Extended Identity user with custom properties

### 3. **Services (Dependency Injection)**
- ✅ **GuideService** - CRUD operations for magazines
- ✅ **FileService** - Upload/delete files with validation
- ✅ **ActivityService** - Track user activities
- ✅ **LogService** - Application logging to database
- All services registered with interfaces for testability

### 4. **File Upload System**
- ✅ Cover images: JPG, PNG, WEBP (Max 5MB)
- ✅ PDF files: PDF only (Max 50MB)
- ✅ Unique filenames using GUID
- ✅ Automatic directory creation
- ✅ File validation (size, type, extension)
- Upload paths:
  - `wwwroot/uploads/guides/covers/`
  - `wwwroot/uploads/guides/pdfs/`

### 5. **Controllers**
- ✅ **GuideAdminController** - Magazine management (Create/Edit/Delete)
  - Requires: Admin, SuperAdmin, or Uploader role
- ✅ **GuideController** - Public viewing and reading
  - Index: Public (lists published magazines)
  - Detail: Public (shows cover and summary)
  - Read: Authenticated users only
  - DownloadPdf: Authenticated users only
- ✅ **AccountController** - Registration, Login, Logout

### 6. **Middleware**
- ✅ **RequestLoggingMiddleware** - Captures:
  - IP address
  - User agent
  - Request path and method
  - Duration
  - Automatic logging to database

### 7. **Razor Views**
- ✅ GuideAdmin: Index, Create, Edit, Details
- ✅ Guide: Index, Detail, Read (with PDF viewer)
- ✅ Account: Login, Register, AccessDenied
- All views styled with Tailwind CSS (matching your existing frontend)

### 8. **Data Seeding**
- ✅ Automatic role creation (SuperAdmin, Admin, Uploader, Customer)
- ✅ SuperAdmin user creation from appsettings.json
- ✅ Runs automatically on application startup

---

## 🚀 Setup Instructions

### Step 1: Install Required Tools

```powershell
# Install Entity Framework Core tools globally
dotnet tool install --global dotnet-ef
```

### Step 2: Restore NuGet Packages

```powershell
cd D:\Project\Magazine\magazine-app\magazine-app
dotnet restore
```

### Step 3: Update Database Connection String (Optional)

The default connection string in `appsettings.json` uses **LocalDB**:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=MagazineAppDb;Trusted_Connection=True;MultipleActiveResultSets=true"
}
```

**For SQL Server Express**, change to:
```json
"DefaultConnection": "Server=.\\SQLEXPRESS;Database=MagazineAppDb;Trusted_Connection=True;MultipleActiveResultSets=true"
```

**For Azure SQL**, change to:
```json
"DefaultConnection": "Server=tcp:yourserver.database.windows.net,1433;Initial Catalog=MagazineAppDb;Persist Security Info=False;User ID=yourusername;Password=yourpassword;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"
```

### Step 4: Configure SuperAdmin Credentials

Edit `appsettings.json` to set your SuperAdmin credentials:

```json
"SeedAdmin": {
  "Email": "admin@magazine.com",
  "Password": "Admin@123"
}
```

**Password Requirements:**
- At least 6 characters
- At least 1 uppercase letter
- At least 1 lowercase letter
- At least 1 digit

### Step 5: Create and Apply Database Migration

```powershell
# Create the initial migration
dotnet ef migrations add InitialMagazineBackend

# Apply the migration to create the database
dotnet ef database update
```

### Step 6: Build the Project

```powershell
dotnet build
```

### Step 7: Run the Application

```powershell
dotnet run
```

The application will start at:
- **HTTPS**: https://localhost:5001
- **HTTP**: http://localhost:5000

---

## 👤 Default Users

After running the application for the first time, you'll have:

### SuperAdmin Account
- **Email**: `admin@magazine.com` (or what you set in appsettings.json)
- **Password**: `Admin@123` (or what you set in appsettings.json)
- **Role**: SuperAdmin
- **Can**: Do everything (create, edit, delete magazines, manage users)

### Register Additional Users
Go to: `/Account/Register`

New users are automatically assigned the **Customer** role, which allows them to:
- ✅ View published magazines
- ✅ Read magazines (after login)
- ✅ Download PDFs (after login)
- ❌ Cannot upload or manage magazines

---

## 🔐 Authorization Rules

### Public (No Login Required)
- `/` - Home page
- `/Guide/Index` - List all published magazines
- `/Guide/Detail/{slug}` - View magazine cover and summary

### Authenticated Users (Any Role)
- `/Guide/Read/{slug}` - Read magazine in PDF viewer
- `/Guide/DownloadPdf/{slug}` - Download magazine PDF
- `/Profile/Index` - User dashboard

### Admin, Uploader, SuperAdmin Only
- `/GuideAdmin/Index` - Manage magazines
- `/GuideAdmin/Create` - Upload new magazine
- `/GuideAdmin/Edit/{id}` - Edit magazine
- `/GuideAdmin/Delete/{id}` - Delete magazine
- `/GuideAdmin/Details/{id}` - View magazine details

---

## 📁 Project Structure

```
magazine-app/
├── Controllers/
│   ├── AccountController.cs          # Login, Register, Logout
│   ├── GuideAdminController.cs       # Magazine management
│   ├── GuideController.cs            # Public magazine viewing
│   └── [Existing controllers...]
├── Data/
│   ├── ApplicationDbContext.cs       # EF Core DbContext
│   └── DataSeed.cs                   # Role & admin seeding
├── Middleware/
│   └── RequestLoggingMiddleware.cs   # Activity logging
├── Models/
│   ├── ApplicationUser.cs            # Custom Identity user
│   ├── Guide.cs                      # Magazine model
│   ├── UserActivity.cs               # Activity tracking
│   └── LogEntry.cs                   # Application logs
├── Services/
│   ├── Interfaces/
│   │   ├── IGuideService.cs
│   │   ├── IFileService.cs
│   │   ├── IActivityService.cs
│   │   └── ILogService.cs
│   ├── GuideService.cs
│   ├── FileService.cs
│   ├── ActivityService.cs
│   └── LogService.cs
├── ViewModels/
│   ├── GuideCreateViewModel.cs
│   ├── GuideEditViewModel.cs
│   ├── GuideListViewModel.cs
│   ├── GuideDetailViewModel.cs
│   ├── LoginViewModel.cs
│   └── RegisterViewModel.cs
├── Views/
│   ├── GuideAdmin/
│   │   ├── Index.cshtml
│   │   ├── Create.cshtml
│   │   ├── Edit.cshtml
│   │   └── Details.cshtml
│   ├── Guide/
│   │   ├── Index.cshtml
│   │   ├── Detail.cshtml
│   │   └── Read.cshtml
│   ├── Account/
│   │   ├── Login.cshtml
│   │   ├── Register.cshtml
│   │   └── AccessDenied.cshtml
│   └── [Existing views...]
├── wwwroot/
│   └── uploads/
│       └── guides/
│           ├── covers/    # Magazine covers (auto-created)
│           └── pdfs/      # Magazine PDFs (auto-created)
├── Program.cs             # Configured with Identity, DbContext, Services
├── appsettings.json       # Connection string & seed admin
└── magazine-app.csproj    # NuGet packages added
```

---

## 🎯 Testing the Backend

### 1. **Register a New User**
1. Go to `/Account/Register`
2. Fill in the form
3. New user will be assigned **Customer** role automatically

### 2. **Login as SuperAdmin**
1. Go to `/Account/Login`
2. Use credentials from appsettings.json
3. Default: `admin@magazine.com` / `Admin@123`

### 3. **Upload a Magazine**
1. Login as Admin/SuperAdmin/Uploader
2. Go to `/GuideAdmin/Create`
3. Fill in title, summary
4. Upload cover image (JPG/PNG/WEBP, max 5MB)
5. Upload PDF file (PDF, max 50MB)
6. Check "Publish Immediately" if you want it visible
7. Click "Create Magazine"

### 4. **View Public Magazines**
1. Go to `/Guide/Index` (no login required)
2. You'll see all published magazines
3. Click any magazine to view details

### 5. **Read a Magazine**
1. Login with any account
2. Go to a magazine detail page
3. Click "Read Now"
4. PDF will display in iframe viewer
5. Or click "Download PDF" to save it

---

## 📊 Database Tables

After migration, you'll have these tables:

- `AspNetUsers` - Identity users (with custom fields)
- `AspNetRoles` - Roles (SuperAdmin, Admin, Uploader, Customer)
- `AspNetUserRoles` - User-Role relationships
- `AspNetUserClaims` - User claims
- `AspNetUserLogins` - External logins
- `AspNetUserTokens` - Auth tokens
- `AspNetRoleClaims` - Role claims
- `Guides` - Your magazines
- `UserActivities` - User action logs
- `LogEntries` - Application logs

---

## 🔧 Common Commands

### View Current Migrations
```powershell
dotnet ef migrations list
```

### Remove Last Migration (if needed)
```powershell
dotnet ef migrations remove
```

### Update Database to Specific Migration
```powershell
dotnet ef database update MigrationName
```

### Drop Database (CAUTION: Deletes all data)
```powershell
dotnet ef database drop
```

### Create New Migration (after model changes)
```powershell
dotnet ef migrations add YourMigrationName
dotnet ef database update
```

---

## 🐛 Troubleshooting

### Error: "The ConnectionString property has not been initialized"
- Check that `appsettings.json` has the ConnectionStrings section
- Ensure SQL Server / LocalDB is running

### Error: "Login failed for user"
- For LocalDB, no credentials needed (uses Windows auth)
- For SQL Server, ensure Trusted_Connection=True or provide User ID/Password

### Error: "Cannot find compilation library location for package"
- Run: `dotnet restore`
- Then: `dotnet build`

### Error: "No DbContext was found"
- Ensure `Microsoft.EntityFrameworkCore.Design` package is installed
- Run: `dotnet restore`

### Upload Directory Not Found
- Directories are created automatically by FileService
- If issues persist, manually create:
  - `wwwroot/uploads/guides/covers/`
  - `wwwroot/uploads/guides/pdfs/`

---

## 🔒 Security Notes

1. **Change Default SuperAdmin Password** in production
2. **Use stronger passwords** than the default
3. **Enable HTTPS** in production (already configured)
4. **Update connection string** to use secrets manager in production
5. **File size limits** are enforced (Cover: 5MB, PDF: 50MB)
6. **File type validation** prevents malicious uploads

---

## 📚 API Endpoints Summary

### Public Endpoints
- `GET /Guide/Index` - List magazines
- `GET /Guide/Detail/{slug}` - View magazine detail

### Authenticated Endpoints
- `GET /Guide/Read/{slug}` - Read magazine
- `GET /Guide/DownloadPdf/{slug}` - Download PDF

### Admin/Uploader Endpoints
- `GET /GuideAdmin/Index` - List all magazines (admin view)
- `GET /GuideAdmin/Create` - Show create form
- `POST /GuideAdmin/Create` - Upload magazine
- `GET /GuideAdmin/Edit/{id}` - Show edit form
- `POST /GuideAdmin/Edit/{id}` - Update magazine
- `POST /GuideAdmin/Delete/{id}` - Delete magazine
- `GET /GuideAdmin/Details/{id}` - View details

### Account Endpoints
- `GET /Account/Register` - Show registration form
- `POST /Account/Register` - Register new user
- `GET /Account/Login` - Show login form
- `POST /Account/Login` - Login user
- `POST /Account/Logout` - Logout user
- `GET /Account/AccessDenied` - Access denied page

---

## ✨ Features Implemented

✅ Microsoft Identity with custom user model
✅ Role-based authorization (4 roles)
✅ File upload with validation
✅ PDF viewer in browser
✅ PDF download functionality
✅ Activity logging (IP, UserAgent, Actions)
✅ Application logging to database
✅ Automatic slug generation from titles
✅ Unique slug handling
✅ Publish/Draft status for magazines
✅ Request logging middleware
✅ Auto-creation of SuperAdmin
✅ Responsive views with Tailwind CSS
✅ Form validation (client and server)
✅ Success/Error messages with TempData
✅ Secure file paths
✅ Delete old files on update/delete

---

## 📞 Support

If you encounter any issues:
1. Check the browser console for JavaScript errors
2. Check the terminal for ASP.NET Core errors
3. Verify database connection string
4. Ensure all migrations are applied
5. Check file permissions for upload directories

---

## 🎉 You're Ready!

Your Magazine App backend is fully implemented and ready to use!

**Next Steps:**
1. Run the migration commands
2. Start the application
3. Login as SuperAdmin
4. Upload your first magazine
5. Register test users
6. Test the complete flow

**Happy Coding! 🚀**

