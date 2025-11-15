using System.ComponentModel.DataAnnotations;

namespace magazine_app.ViewModels
{
    public class ManageUserViewModel
    {
        public string Id { get; set; } = string.Empty;
        
        [Display(Name = "Full Name")]
        public string FullName { get; set; } = string.Empty;
        
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;
        
        [Display(Name = "Is Subscribed")]
        public bool IsSubscribed { get; set; }
        
        [Display(Name = "Subscription Plan")]
        public string? SubscriptionPlan { get; set; }
        
        [Display(Name = "Subscription Start Date")]
        public DateTime? SubscriptionStartDate { get; set; }
        
        [Display(Name = "Subscription End Date")]
        public DateTime? SubscriptionEndDate { get; set; }
        
        [Display(Name = "Account Active")]
        public bool IsActive { get; set; }
        
        public List<string> Roles { get; set; } = new();
    }
    
    public class CreateEmployeeViewModel
    {
        [Required(ErrorMessage = "Full Name is required")]
        [Display(Name = "Full Name")]
        public string FullName { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Password is required")]
        [StringLength(100, MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Role is required")]
        [Display(Name = "Role")]
        public string Role { get; set; } = "Uploader";
    }
}

