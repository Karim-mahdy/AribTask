using AribTask.Application.ViewModels;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace AribTask.Application.Common.Abstraction.Services
{
    public interface IAccountService
    {
        Task<SignInResult> LoginAsync(string username, string password, bool rememberMe);
        Task<IdentityResult> RegisterAsync(RegisterViewModel model);
        Task LogoutAsync();
    }
}