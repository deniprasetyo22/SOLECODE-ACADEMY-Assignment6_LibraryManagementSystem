using Assignment5.Application.DTOs.Account;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment6.Application.Interfaces.IService
{
    public interface IAuthService
    {
        Task<AuthResponse> SignUpAsync(RegisterModel model);
        Task<AuthResponse> LoginAsync(LoginModel model);
        Task<AuthResponse> CreateRoleAsync(string rolename);
        Task<AuthResponse> AssignToRoleAsync(string userName, string rolename);
        string GenerateRefreshToken();
        Task<AuthResponse> LogoutAsync(string username);
    }
}
