
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Punch_API.Models.Users;
using Punch_API.Models;
using Scalar.AspNetCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using System.Configuration;
using Punch_API.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.Cookies;
using Azure;

namespace Punch_API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();

            var connection = String.Empty;
            if (builder.Environment.IsDevelopment())
            {
                builder.Configuration.AddEnvironmentVariables().AddJsonFile("appsettings.Development.json");
                connection = builder.Configuration.GetConnectionString("PunchDbContext");
            }
            else
            {
                connection = Environment.GetEnvironmentVariable("AZURE_SQL_CONNECTIONSTRING");
            }


            builder.Services.AddCors(options =>
            {
                options.AddDefaultPolicy(builder =>
                {
                    builder.SetIsOriginAllowed(origin => new Uri(origin).Host == "punch-time-management.com")
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials()
                    .WithExposedHeaders("Set-Cookie");
                });
            });

            builder.Configuration.GetSection("GmailOptions").Get<GmailOptions>();

            // Seed initial AppUser, per Microsoft's documentation I call both the UseSeeding and UseAsyncSeeding methods to ensure data is seeded
            // both synchronously and asynchronously, the logic for both methods is the same so only the first UseSeeding has comments
            builder.Services.AddDbContext<PunchDbContext>(optionsBuilder =>
                optionsBuilder
                .UseSqlServer(connection)
                .UseSeeding((context, _) =>
                {
                    // Check if a role named "admin" exists in the database
                    var testRole = context.Set<IdentityRole<int>>().FirstOrDefault(r => r.Name == "admin");
                    // If not we add the "admin" role to the db
                    if (testRole == null)
                    {
                        context.Set<IdentityRole<int>>().Add(new IdentityRole<int> { Name = "admin" });
                        context.SaveChanges();
                    }
                    // User to be added
                    var user = new AppUser
                    {
                        FirstName = builder.Configuration["AdminUserOptions:FirstName"],
                        LastName = builder.Configuration["AdminUserOptions:LastName"],
                        Email = builder.Configuration["AdminUserOptions:Email"],
                        UserName = builder.Configuration["AdminUserOptions:UserName"],
                        SecurityStamp = Guid.NewGuid().ToString("D")
                    };
                    // Check if user already exists
                    var testUser = context.Set<AppUser>().FirstOrDefault(u => u.Email == user.Email);
                    // If not, add the user to the database
                    if (testUser == null)
                    {
                        // Create a password hash for the user
                        var hasher = new PasswordHasher<AppUser>();
                        var hashed = hasher.HashPassword(user, builder.Configuration["AdminUserOptions:Password"]);
                        user.PasswordHash = hashed;

                        context.Set<AppUser>().Add(user);
                        context.SaveChanges();
                    }

                    // Seed data for demo login for live project!
                    var company = context.Set<Company>().FirstOrDefault(c => c.CompanyName == "Demo");
                    if (company == null)
                    {
                        context.Set<Company>().Add(new Company { CompanyName = "Demo" });
                        context.SaveChanges();
                    }

                    var demoRole = context.Set<IdentityRole<int>>().FirstOrDefault(r => r.Name == "demo");
                    if (demoRole == null)
                    {
                        context.Set<IdentityRole<int>>().Add(new IdentityRole<int> { Name = "demo" });
                        context.SaveChanges();
                    }

                    var demoUser = new AppUser
                    {
                        FirstName = builder.Configuration["DemoUserOptions:FirstName"],
                        LastName = builder.Configuration["DemoUserOptions:LastName"],
                        Email = builder.Configuration["DemoUserOptions:Email"],
                        UserName = builder.Configuration["DemoUserOptions:UserName"],
                        SecurityStamp = Guid.NewGuid().ToString("D")
                    };
                    var testDemoUser = context.Set<AppUser>().FirstOrDefault(u => u.Email == demoUser.Email);
                    if (testDemoUser == null)
                    {
                        var hasher = new PasswordHasher<AppUser>();
                        var hashed = hasher.HashPassword(user, builder.Configuration["DemoUserOptions:Password"]);
                        user.PasswordHash = hashed;

                        context.Set<AppUser>().Add(demoUser);
                        context.SaveChanges();
                    }
                }

                )
                .UseAsyncSeeding(async (context, _, cancellationToken) =>
                 {
                     var testRole = await context.Set<IdentityRole<int>>().FirstOrDefaultAsync(r => r.Name == "admin");
                     if (testRole == null)
                     {
                         context.Set<IdentityRole<int>>().Add(new IdentityRole<int> { Id = 1, Name = "admin" });
                         await context.SaveChangesAsync();
                     }
                     var user = new AppUser
                     {
                         FirstName = builder.Configuration["AdminUserOptions:FirstName"],
                         LastName = builder.Configuration["AdminUserOptions:LastName"],
                         Email = builder.Configuration["AdminUserOptions:Email"]
                     };
                     var testUser = await context.Set<AppUser>().FirstOrDefaultAsync(u => u.Email == user.Email);
                     if (testUser == null)
                     {
                         var hasher = new PasswordHasher<AppUser>();
                         var hashed = hasher.HashPassword(user, builder.Configuration["AdminUserOptions:Password"]);
                         user.PasswordHash = hashed;

                         context.Set<AppUser>().Add(user);
                         await context.SaveChangesAsync(cancellationToken);
                     }
                 }));

            builder.Services.AddIdentity<AppUser, IdentityRole<int>>()
                .AddUserStore<AppUserStore>()
                .AddEntityFrameworkStores<PunchDbContext>()
                .AddDefaultTokenProviders();

            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/AppUsers/log-in";
                options.Cookie.SameSite = SameSiteMode.None;
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                options.Cookie.IsEssential = true;
                options.Cookie.HttpOnly = true;
                options.Cookie.Extensions.Add("Partitioned");
                options.Cookie.Domain = "punch-time-management.com";
            });

            builder.Services
                .AddAuthentication()
                .AddCookie(options =>
                {
                    options.LoginPath = "/AppUsers/log-in";
                    options.Cookie.SameSite = SameSiteMode.None;
                    options.Cookie.HttpOnly = true;
                    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                    options.Cookie.IsEssential = true;
                    options.Cookie.Extensions.Add("Partitioned");
                    options.Cookie.Domain = "punch-time-management.com";
                });

            builder.Services.AddAuthorization();

            

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                app.MapScalarApiReference();
            }

            
            app.UseHttpsRedirection();

            app.UseCookiePolicy();

            app.UseCors();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
