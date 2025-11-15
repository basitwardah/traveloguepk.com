using Entities.Models;
using Microsoft.AspNetCore.Identity;

namespace Repository.Interfaces
{
    public interface IAccountRepository
    {
        // User Management
        Task<ApplicationUser?> GetUserByIdAsync(string userId);
        Task<ApplicationUser?> GetUserByEmailAsync(string email);
        Task<ApplicationUser?> GetUserByUsernameAsync(string username);
        Task<IEnumerable<ApplicationUser>> GetAllUsersAsync();
        Task<IEnumerable<ApplicationUser>> GetActiveUsersAsync();
        Task<IdentityResult> CreateUserAsync(ApplicationUser user, string password);
        Task<IdentityResult> UpdateUserAsync(ApplicationUser user);
        Task<IdentityResult> DeleteUserAsync(ApplicationUser user);
        
        // Authentication
        Task<SignInResult> SignInAsync(string email, string password, bool rememberMe, bool lockoutOnFailure);
        Task SignOutAsync();
        Task<bool> CheckPasswordAsync(ApplicationUser user, string password);
        Task<IdentityResult> ChangePasswordAsync(ApplicationUser user, string currentPassword, string newPassword);
        Task<IdentityResult> ResetPasswordAsync(ApplicationUser user, string token, string newPassword);
        Task<string> GeneratePasswordResetTokenAsync(ApplicationUser user);
        Task<string> GenerateEmailConfirmationTokenAsync(ApplicationUser user);
        Task<IdentityResult> ConfirmEmailAsync(ApplicationUser user, string token);
        
        // Role Management
        Task<IdentityResult> AddToRoleAsync(ApplicationUser user, string role);
        Task<IdentityResult> RemoveFromRoleAsync(ApplicationUser user, string role);
        Task<IList<string>> GetUserRolesAsync(ApplicationUser user);
        Task<bool> IsInRoleAsync(ApplicationUser user, string role);
        Task<IEnumerable<ApplicationUser>> GetUsersInRoleAsync(string roleName);
        
        // Claims
        Task<IList<System.Security.Claims.Claim>> GetClaimsAsync(ApplicationUser user);
        Task<IdentityResult> AddClaimAsync(ApplicationUser user, System.Security.Claims.Claim claim);
        Task<IdentityResult> RemoveClaimAsync(ApplicationUser user, System.Security.Claims.Claim claim);
        
        // User Status
        Task<bool> UpdateLastLoginAsync(string userId);
        Task<bool> SetUserActiveStatusAsync(string userId, bool isActive);
        Task<int> GetTotalUsersCountAsync();
        Task<int> GetActiveUsersCountAsync();
        
        // Lockout
        Task<IdentityResult> SetLockoutEnabledAsync(ApplicationUser user, bool enabled);
        Task<IdentityResult> SetLockoutEndDateAsync(ApplicationUser user, DateTimeOffset? lockoutEnd);
        Task<int> GetAccessFailedCountAsync(ApplicationUser user);
        Task<IdentityResult> ResetAccessFailedCountAsync(ApplicationUser user);
    }
}

