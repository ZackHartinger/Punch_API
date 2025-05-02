
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Punch_API.Models.Users;
using Punch_API.Models;
using Scalar.AspNetCore;

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
                connection = Environment.GetEnvironmentVariable("PunchDbContext");
            }


            builder.Services.AddCors(options =>
            {
                options.AddDefaultPolicy(builder =>
                {
                    builder.SetIsOriginAllowed(origin => new Uri(origin).Host == "localhost")
                    .AllowAnyHeader()
                    .AllowAnyMethod();
                });
            });

            builder.Services.AddDbContext<PunchDbContext>(optionsBuilder =>
                optionsBuilder
                .UseSqlServer(connection)
                .UseSeeding((context, _) =>
                {
                    var testRole = context.Set<IdentityRole<int>>().FirstOrDefault(r => r.Name == "admin");
                    if (testRole == null)
                    {
                        context.Set<IdentityRole<int>>().Add(new IdentityRole<int> { Name = "admin" });
                        context.SaveChanges();
                    }
                    var user = new AppUser
                    {
                        FirstName = "Zack",
                        LastName = "Hartinger",
                        Email = "zackhrtngr@gmail.com"
                    };
                    var testUser = context.Set<AppUser>().FirstOrDefault(u => u.Email == user.Email);
                    if (testUser == null)
                    {
                        var hasher = new PasswordHasher<AppUser>();
                        var hashed = hasher.HashPassword(user, "Hahaha64!");
                        user.PasswordHash = hashed;

                        context.Set<AppUser>().Add(user);
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
                         Id = 1,
                         FirstName = "Zack",
                         LastName = "Hartinger",
                         Email = "zackhrtngr@gmail.com"
                     };
                     var testUser = await context.Set<AppUser>().FirstOrDefaultAsync(u => u.Email == user.Email);
                     if (testUser == null)
                     {
                         var hasher = new PasswordHasher<AppUser>();
                         var hashed = hasher.HashPassword(user, "Hahaha64!");
                         user.PasswordHash = hashed;

                         context.Set<AppUser>().Add(user);
                         await context.SaveChangesAsync(cancellationToken);
                     }
                 }));

            builder.Services.AddIdentity<AppUser, IdentityRole<int>>()
                .AddUserStore<AppUserStore>()
                .AddEntityFrameworkStores<PunchDbContext>()
                .AddDefaultTokenProviders();

            builder.Services
                .AddAuthentication()
                .AddCookie(options => 
                    options.LoginPath = "/AppUsers/log-in");

            builder.Services.AddAuthorization();


            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                app.MapScalarApiReference();
            }

            app.UseHttpsRedirection();

            app.UseCors();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseRouting();

            app.MapControllers();

            app.Run();
        }
    }
}
