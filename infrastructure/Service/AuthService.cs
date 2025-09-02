using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Application.DTO.AUTH;
using Application.DTO.ServiceResponse;
using Application.Interface;
using infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace infrastructure.Service
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IConfiguration _config;
        public AuthService(UserManager<AppUser> userManager, IConfiguration config)
        {
            _userManager = userManager;
            _config = config;
        }
        public async Task<string> LoginAsync(LoginDto model)
        {
           var user = await _userManager.FindByEmailAsync(model.Email);
            if (user != null && await _userManager.CheckPasswordAsync(user,model.Password)) {
                var claims = new[]
                {
                    new Claim(JwtRegisteredClaimNames.Sub,user.Email),
                    new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString()),
                };
                var Key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["jwt:Key"]));
                var creds = new SigningCredentials(Key, SecurityAlgorithms.HmacSha256);
                var token = new JwtSecurityToken(
                     issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: model.RememberMe ? DateTime.Now.AddDays(2) : DateTime.Now.AddMinutes(5),
                signingCredentials: creds
                    );
                return new JwtSecurityTokenHandler().WriteToken(token);
            }
            throw new Exception("Invalid login attempt");
        }

        public async Task<UserRegisterDTO> RegisterAsync(RegisterDTO model)
        {
            var User = new AppUser
            {
                UserName = model.UserName,
                Email = model.Email,
                FullName = model.FullName,
            };
            var result = await _userManager.CreateAsync(User,model.Password);
            if (!result.Succeeded)
            {
                throw new Exception(string.Join(", " ,result.Errors.Select(x=>x.Description)));
            }
            return new UserRegisterDTO { 
            Id = User.Id,
            Name = model.UserName,
            };
        }
        
    }
}
