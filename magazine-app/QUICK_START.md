# 🚀 QUICK START GUIDE

## Magazine App Backend - Get Running in 5 Minutes!

---

## ⚡ STEP-BY-STEP SETUP

### 1️⃣ Install EF Core Tools (One-time only)
```powershell
dotnet tool install --global dotnet-ef
```

### 2️⃣ Create Database Migration
```powershell
dotnet ef migrations add InitialMagazineBackend
```

### 3️⃣ Create & Update Database
```powershell
dotnet ef database update
```

### 4️⃣ Run the Application
```powershell
dotnet run
```

### 5️⃣ Open in Browser
```
https://localhost:5001
```

---

## 🎯 FIRST ACTIONS

### 1. Login as SuperAdmin
- Go to: `/Account/Login`
- Email: `admin@magazine.com`
- Password: `Admin@123`

### 2. Upload Your First Magazine
- Go to: `/GuideAdmin/Create`
- Fill in title and summary
- Upload cover image (JPG/PNG/WEBP, max 5MB)
- Upload PDF file (PDF, max 50MB)
- Check "Publish Immediately"
- Click "Create Magazine"

### 3. View Public Magazine List
- Logout or open incognito
- Go to: `/Guide/Index`
- See your published magazine!

### 4. Register a Test Customer
- Go to: `/Account/Register`
- Fill in registration form
- Login and try reading magazines

---

## 📊 QUICK REFERENCE

### Key URLs
| Purpose | URL | Auth Required |
|---------|-----|---------------|
| Home | `/` | No |
| Magazine Library | `/Guide/Index` | No |
| Magazine Detail | `/Guide/Detail/{slug}` | No |
| Read Magazine | `/Guide/Read/{slug}` | Yes |
| Download PDF | `/Guide/DownloadPdf/{slug}` | Yes |
| Admin Dashboard | `/GuideAdmin/Index` | Admin+ |
| Upload Magazine | `/GuideAdmin/Create` | Admin+ |
| Login | `/Account/Login` | No |
| Register | `/Account/Register` | No |

### Default Roles
| Role | Can Do |
|------|--------|
| **Customer** | Read & download magazines (after login) |
| **Uploader** | Upload & manage magazines |
| **Admin** | Upload & manage magazines |
| **SuperAdmin** | Everything + user management |

---

## 🔧 COMMON ISSUES

### Issue: "dotnet ef not found"
**Solution**: Install EF tools
```powershell
dotnet tool install --global dotnet-ef
```

### Issue: "Cannot connect to database"
**Solution**: Check SQL Server is running
- LocalDB: Already installed with Visual Studio
- SQL Express: Start SQL Server service

### Issue: "Login failed"
**Solution**: Using LocalDB? No password needed! It uses Windows Authentication.

### Issue: "Upload directory not found"
**Solution**: Already created! Check `wwwroot/uploads/guides/`

---

## ✅ VERIFICATION CHECKLIST

After setup, verify these work:

- [ ] Application runs without errors
- [ ] Can access home page
- [ ] Can login as SuperAdmin
- [ ] Can upload a magazine
- [ ] Can view magazine in public list
- [ ] Can logout
- [ ] Can register new user
- [ ] Can login as new user
- [ ] Can read magazine after login
- [ ] Can download PDF

---

## 📁 PROJECT STRUCTURE

```
magazine-app/
├── Controllers/
│   ├── GuideAdminController.cs  ← Admin: Upload/Edit/Delete
│   ├── GuideController.cs       ← Public: View/Read magazines
│   └── AccountController.cs     ← Auth: Login/Register
├── Models/
│   ├── Guide.cs                 ← Magazine model
│   ├── ApplicationUser.cs       ← Custom user
│   ├── UserActivity.cs          ← Activity logs
│   └── LogEntry.cs              ← App logs
├── Services/
│   ├── GuideService.cs          ← Magazine CRUD
│   ├── FileService.cs           ← File upload/delete
│   ├── ActivityService.cs       ← Track activities
│   └── LogService.cs            ← Application logging
├── Views/
│   ├── GuideAdmin/              ← Admin views
│   ├── Guide/                   ← Public views
│   └── Account/                 ← Auth views
└── wwwroot/
    └── uploads/guides/
        ├── covers/              ← Magazine covers
        └── pdfs/                ← Magazine PDFs
```

---

## 🎓 LEARNING RESOURCES

### For More Details:
- **BACKEND_SETUP.md** - Complete setup guide with troubleshooting
- **BACKEND_COMPLETE.md** - Full implementation summary
- **PROJECT_SUMMARY.md** - Original frontend documentation

### Test Workflow:
1. Login as SuperAdmin
2. Upload 2-3 magazines
3. Logout
4. View magazines (public)
5. Try to read → Gets redirected to login
6. Register new account
7. Login as new user
8. Now can read and download

---

## 🎉 YOU'RE READY!

**That's it!** Your backend is fully functional.

**Questions?** Check `BACKEND_SETUP.md` for detailed documentation.

**Happy Coding! 🚀**

