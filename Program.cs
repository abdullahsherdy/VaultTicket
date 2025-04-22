using API.Data;
using API.Helpers;
using API.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using System.Text.Json.Serialization;

namespace API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services
            /// to avoid json cycle
            /// can done via two methods 
            /// in ok(Response, options)
            /// or make it general for all controllers 
            builder.Services.AddControllers()
              .AddJsonOptions(opt =>
              {
                  opt.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
              });

            builder.Services.AddEndpointsApiExplorer();

            // Register DbContext
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            // Register DI services
            builder.Services.AddScoped<IRandomNumberGeneratorService, RandomNumberGeneratorService>();
            builder.Services.AddScoped<ITicketService, TicketService>();
            builder.Services.AddScoped<JwtService>();
            builder.Services.AddScoped<PasswordHasher>();
            builder.Services.AddScoped<TokenService>();
            builder.Services.AddSingleton<AESEncryptionService>();


            // JWT Config
            var jwtKey = builder.Configuration["Jwt:Key"];
            var jwtIssuer = builder.Configuration["Jwt:Issuer"];

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = false,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = jwtIssuer,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
                    };
                });

            builder.Services.AddAuthorization(); // ⬅️ This was missing

            // Swagger + JWT Auth
            builder.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new() { Title = "Event Booking API", Version = "v1" });

                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Enter 'Bearer {your token}'"
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
            });

            var app = builder.Build();

            //  Correct Middleware order is CRITICAL
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseRouting(); // ⬅️ This must come BEFORE Auth Middlewares

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers(); // Map controllers after routing

            app.Run();
        }
    }
}
