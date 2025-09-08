using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Interface;
using infrastructure.DataContext;
using infrastructure.Identity;
using infrastructure.Service;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace infrastructure.DepandancyInjection
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config) {
            services.AddDbContext<AppDbContext>(
                opt => opt.UseSqlServer(config.GetConnectionString("DefaultConnection"))
                );
            services.AddIdentity<AppUser,AppRole>(x =>
            {
                x.Password.RequiredLength = 8;
                x.User.RequireUniqueEmail = true;  
            })
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders();
            services.AddScoped<IAuthService, AuthService>();
            var Key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["jwt:key"]!));
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(
                opt => {
                    opt.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = config["jwt:Issuer"],
                        ValidAudience = config["jwt:Audience"],
                        IssuerSigningKey   = Key
                    };
                }
                );
            return services;
        }
    }
}
