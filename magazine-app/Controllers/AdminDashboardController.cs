using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Entities.Models;
using Repository.Data;
using magazine_app.ViewModels;
using magazine_app.Services.Interfaces;

namespace magazine_app.Controllers
{
    [Authorize(Roles = "Admin,SuperAdmin")]
    public class AdminDashboardController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IGuideService _guideService;
        private readonly IActivityService _activityService;

        public AdminDashboardController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            IGuideService guideService,
            IActivityService activityService)
        {
            _context = context;
            _userManager = userManager;
            _guideService = guideService;
            _activityService = activityService;
        }

        // GET: AdminDashboard
        public async Task<IActionResult> Index()
        {
            var allUsers = _userManager.Users.ToList();
            var totalUsers = allUsers.Count;
            var subscribedUsers = allUsers.Count(u => u.IsSubscribed && u.SubscriptionEndDate > DateTime.UtcNow);
            var nonSubscribedUsers = totalUsers - subscribedUsers;
            
            var employees = new List<ApplicationUser>();
            foreach (var user in allUsers)
            {
                var roles = await _userManager.GetRolesAsync(user);
                if (roles.Any(r => r == "Admin" || r == "SuperAdmin" || r == "Uploader"))
                {
                    employees.Add(user);
                }
            }
            
            var allGuides = await _context.Guides.ToListAsync();
            var totalMagazines = allGuides.Count;
            var publishedMagazines = allGuides.Count(g => g.IsPublished);
            var freeMagazines = allGuides.Count(g => g.CurrentPrice == 0);
            
            var recentUsers = allUsers
                .OrderByDescending(u => u.CreatedAt)
                .Take(5)
                .ToList();
            
            var userViewModels = new List<UserListItemViewModel>();
            foreach (var user in recentUsers)
            {
                var roles = await _userManager.GetRolesAsync(user);
                userViewModels.Add(new UserListItemViewModel
                {
                    Id = user.Id,
                    FullName = user.FullName ?? "N/A",
                    Email = user.Email ?? "N/A",
                    IsSubscribed = user.IsSubscribed,
                    SubscriptionEndDate = user.SubscriptionEndDate,
                    SubscriptionPlan = user.SubscriptionPlan,
                    CreatedAt = user.CreatedAt,
                    Roles = roles.ToList(),
                    HasActiveSubscription = user.HasActiveSubscription
                });
            }
            
            var recentMagazines = await _guideService.GetAllGuidesAsync();
            var recentActivities = await _context.UserActivities
                .Include(a => a.User)
                .Include(a => a.Guide)
                .OrderByDescending(a => a.Timestamp)
                .Take(10)
                .Select(a => new ActivityViewModel
                {
                    UserName = a.User != null ? (a.User.FullName ?? a.User.Email) : "Unknown",
                    Action = a.Action,
                    Timestamp = a.Timestamp,
                    GuideTitle = a.Guide != null ? a.Guide.Title : null
                })
                .ToListAsync();
            
            var viewModel = new AdminDashboardViewModel
            {
                TotalUsers = totalUsers,
                SubscribedUsers = subscribedUsers,
                NonSubscribedUsers = nonSubscribedUsers,
                TotalEmployees = employees.Count,
                TotalMagazines = totalMagazines,
                PublishedMagazines = publishedMagazines,
                UnpublishedMagazines = totalMagazines - publishedMagazines,
                FreeMagazines = freeMagazines,
                PaidMagazines = totalMagazines - freeMagazines,
                RecentUsers = userViewModels,
                RecentMagazines = recentMagazines.Take(5).ToList(),
                RecentActivities = recentActivities
            };
            
            return View(viewModel);
        }

        // GET: AdminDashboard/Users
        public async Task<IActionResult> Users(string filter = "all")
        {
            var allUsers = _userManager.Users.ToList();
            
            var filteredUsers = filter switch
            {
                "subscribed" => allUsers.Where(u => u.HasActiveSubscription).ToList(),
                "non-subscribed" => allUsers.Where(u => !u.HasActiveSubscription).ToList(),
                "employees" => new List<ApplicationUser>(),
                _ => allUsers
            };
            
            if (filter == "employees")
            {
                foreach (var user in allUsers)
                {
                    var roles = await _userManager.GetRolesAsync(user);
                    if (roles.Any(r => r == "Admin" || r == "SuperAdmin" || r == "Uploader"))
                    {
                        filteredUsers.Add(user);
                    }
                }
            }
            
            var userViewModels = new List<UserListItemViewModel>();
            foreach (var user in filteredUsers)
            {
                var roles = await _userManager.GetRolesAsync(user);
                userViewModels.Add(new UserListItemViewModel
                {
                    Id = user.Id,
                    FullName = user.FullName ?? "N/A",
                    Email = user.Email ?? "N/A",
                    IsSubscribed = user.IsSubscribed,
                    SubscriptionEndDate = user.SubscriptionEndDate,
                    SubscriptionPlan = user.SubscriptionPlan,
                    CreatedAt = user.CreatedAt,
                    Roles = roles.ToList(),
                    HasActiveSubscription = user.HasActiveSubscription
                });
            }
            
            ViewBag.Filter = filter;
            return View(userViewModels);
        }

        // GET: AdminDashboard/ManageUser/id
        public async Task<IActionResult> ManageUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            
            var roles = await _userManager.GetRolesAsync(user);
            
            var viewModel = new ManageUserViewModel
            {
                Id = user.Id,
                FullName = user.FullName ?? "",
                Email = user.Email ?? "",
                IsSubscribed = user.IsSubscribed,
                SubscriptionPlan = user.SubscriptionPlan,
                SubscriptionStartDate = user.SubscriptionStartDate,
                SubscriptionEndDate = user.SubscriptionEndDate,
                IsActive = user.IsActive,
                Roles = roles.ToList()
            };
            
            return View(viewModel);
        }

        // POST: AdminDashboard/ExpireSubscription/id
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ExpireSubscription(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            
            user.IsSubscribed = false;
            user.SubscriptionEndDate = DateTime.UtcNow;
            
            await _userManager.UpdateAsync(user);
            
            TempData["Success"] = "Subscription expired successfully!";
            return RedirectToAction(nameof(ManageUser), new { id });
        }

        // POST: AdminDashboard/ActivateSubscription/id
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ActivateSubscription(string id, string plan = "Monthly")
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            
            user.IsSubscribed = true;
            user.SubscriptionPlan = plan;
            user.SubscriptionStartDate = DateTime.UtcNow;
            user.SubscriptionEndDate = plan switch
            {
                "Monthly" => DateTime.UtcNow.AddMonths(1),
                "Yearly" => DateTime.UtcNow.AddYears(1),
                "Lifetime" => DateTime.UtcNow.AddYears(100),
                _ => DateTime.UtcNow.AddMonths(1)
            };
            
            await _userManager.UpdateAsync(user);
            
            TempData["Success"] = $"Subscription activated successfully! ({plan})";
            return RedirectToAction(nameof(ManageUser), new { id });
        }

        // GET: AdminDashboard/CreateEmployee
        public IActionResult CreateEmployee()
        {
            return View();
        }

        // POST: AdminDashboard/CreateEmployee
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateEmployee(CreateEmployeeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            
            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                FullName = model.FullName,
                CreatedAt = DateTime.UtcNow,
                IsActive = true,
                IsSubscribed = true, // Employees get automatic subscription
                SubscriptionPlan = "Lifetime",
                SubscriptionStartDate = DateTime.UtcNow,
                SubscriptionEndDate = DateTime.UtcNow.AddYears(100)
            };
            
            var result = await _userManager.CreateAsync(user, model.Password);
            
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, model.Role);
                
                TempData["Success"] = $"Employee created successfully with role: {model.Role}";
                return RedirectToAction(nameof(Users), new { filter = "employees" });
            }
            
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
            
            return View(model);
        }

        // POST: AdminDashboard/DeleteUser/id
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            
            var result = await _userManager.DeleteAsync(user);
            
            if (result.Succeeded)
            {
                TempData["Success"] = "User deleted successfully!";
            }
            else
            {
                TempData["Error"] = "Failed to delete user.";
            }
            
            return RedirectToAction(nameof(Users));
        }
    }
}

