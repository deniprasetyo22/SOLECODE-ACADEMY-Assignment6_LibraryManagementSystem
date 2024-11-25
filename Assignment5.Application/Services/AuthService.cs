using Assignment5.Application.DTOs.Account;
using Assignment5.Application.Interfaces.IService;
using Assignment5.Domain.Models;
using Assignment6.Application.Interfaces.IService;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Assignment5.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;

        public AuthService(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
        }
        //Sign Up The User
        public async Task<AuthResponse> SignUpAsync(RegisterModel model)
        {
            var userExists = await _userManager.FindByNameAsync(model.Username);
            if (userExists != null) return new AuthResponse
            {
                Status = "Error",
                Message = "User already exists!"
            };

            AppUser user = new AppUser()
            {
                Email = model.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = model.Username
            };
            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded) return new AuthResponse
            {
                Status = "Error",
                Message = "User creation failed! Please check user details and try again."
            };

            // Assign default role "Library User" to the new user
            var roleResult = await AssignToRoleAsync(user.UserName, "Library User");
            if (roleResult.Status != "Success")
            {
                return new AuthResponse
                {
                    Status = "Error",
                    Message = "User created but failed to assign default role."
                };
            }

            return new AuthResponse { Status = "Success", Message = "User created succesfully!" };
        }

        //Login user
        public async Task<AuthResponse> LoginAsync(LoginModel model)
        {
            var user = await _userManager.FindByNameAsync(model.Username);
            if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
            {
                var userRoles = await _userManager.GetRolesAsync(user);
                var authClaims = new List<Claim>()
                {
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                };

                foreach (var userRole in userRoles)
                {
                    authClaims.Add(new Claim(ClaimTypes.Role, userRole.ToString()));
                }

                var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:SigningKey"]));

                var token = new JwtSecurityToken(
                    issuer: _configuration["JWT:Issuer"],
                    audience: _configuration["JWT:Audience"],
                    expires: DateTime.Now.AddDays(3),
                    claims: authClaims,
                    signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256));

                var refreshToken = GenerateRefreshToken();
                user.RefreshToken = refreshToken;
                await _userManager.UpdateAsync(user);

                return new AuthResponse
                {
                    Token = new JwtSecurityTokenHandler().WriteToken(token),
                    TokenExpiresOn = token.ValidTo,
                    Message = "User successfully login!",
                    User = user,
                    RefreshToken = refreshToken,
                    Roles = userRoles.ToList(),
                    Status = "Success"
                };
            }
            return new AuthResponse { Status = "Error", Message = "Password Not valid!" };

        }
        // Create Role
        public async Task<AuthResponse> CreateRoleAsync(string rolename)
        {
            if (!await _roleManager.RoleExistsAsync(rolename))
                await _roleManager.CreateAsync(new IdentityRole(rolename));
            return new AuthResponse { Status = "Success", Message = "Role Created successfully!" };
        }

        // Assign user to role that already created before
        public async Task<AuthResponse> AssignToRoleAsync(string userName, string rolename)
        {
            var user = await _userManager.FindByNameAsync(userName);

            if (await _roleManager.RoleExistsAsync($"{rolename}"))
            {
                await _userManager.AddToRoleAsync(user, rolename);
            }
            return new AuthResponse { Status = "Success", Message = "User created succesfully!" };
        }

        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }

        public async Task<AuthResponse> LogoutAsync(string username)
        {
            var user = await _userManager.FindByNameAsync(username);
            if (user == null)
            {
                return new AuthResponse { Status = "Error", Message = "User not found!" };
            }

            // Invalidate the user's refresh token
            user.RefreshToken = null;
            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                return new AuthResponse { Status = "Success", Message = "User successfully logged out!" };
            }

            return new AuthResponse { Status = "Error", Message = "Logout failed! Please try again." };
        }

    }
}
