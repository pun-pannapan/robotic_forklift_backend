using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Robotic.Forklift.Application.Interfaces;
using Robotic.Forklift.Application.Mappings;
using Robotic.Forklift.Application.Services;
using Robotic.Forklift.Infrastructure.Data;
using Robotic.Forklift.Infrastructure.Services;
using System.Text;

namespace Robotic.Forklift.Infrastructure
{
    public static class InfrastructureExtensions
    {
        public static IServiceCollection AddForkliftInfrastructure(this IServiceCollection services, IConfiguration cfg)
        {
            services.AddDbContext<AppDbContext>(opts =>
            opts.UseSqlServer(cfg.GetConnectionString("DefaultConnection")));
            services.AddScoped<IAppDbContext>(sp => sp.GetRequiredService<AppDbContext>());

            services.AddScoped<IJwtTokenService, JwtTokenService>();
            services.AddScoped<ICommandParser, CommandParser>();

            services.AddAutoMapper(typeof(MappingProfile));
            services.AddMediatR(cfgM => cfgM.RegisterServicesFromAssembly(typeof(MappingProfile).Assembly));

            var secret = cfg["Jwt:Secret"]!;
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
            services.AddAuthentication(authService =>
            {
                authService.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                authService.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(jwtService =>
            {
                jwtService.RequireHttpsMetadata = false;
                jwtService.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = key,
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                };
            });

            services.AddAuthorization();

            services.AddCors(options =>
            {
                options.AddPolicy("DevelopmentPolicy", policy =>
                {
                    policy.AllowAnyOrigin()
                          .AllowAnyHeader()
                          .AllowAnyMethod();
                });
            });

            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(swaggerService =>
            {
                swaggerService.SwaggerDoc("v1", new OpenApiInfo { Title = "Forklift API", Version = "v1" });
                var securityScheme = new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "JWT Authorization header using the Bearer scheme",
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                };
                swaggerService.AddSecurityDefinition("Bearer", securityScheme);
                swaggerService.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    { new OpenApiSecurityScheme { Reference = new OpenApiReference {
                            Type = ReferenceType.SecurityScheme, Id = "Bearer" } },
                            Array.Empty<string>()
                    }});
            });
            return services;
        }
    }
}
