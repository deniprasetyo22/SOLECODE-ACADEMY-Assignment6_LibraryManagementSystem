using Assignment5.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Assignment5.Application.DTOs.Account
{
    public class AuthResponse
    {
        public string? Status { get; set; }
        public string? Message { get; set; }
        public string Token { get; set; }
        public DateTime? TokenExpiresOn { get; set; }
        public List<string> Roles { get; set; }

        //tambahan
        public AppUser? User { get; set; }

        [JsonIgnore]
        public string RefreshToken { get; set; }
        public DateTime RefreshTokenExpiration { get; set; }
    }
}
