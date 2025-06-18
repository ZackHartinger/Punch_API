using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Punch_API.Models;
using Punch_API.Models.Users;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;

namespace Punch_API.Models
{
    public partial class PunchDbContext : IdentityDbContext<AppUser, IdentityRole<int>, int>
    {
        public PunchDbContext()
        { }
        
        public PunchDbContext(DbContextOptions<PunchDbContext> options) :
            base(options)
        { }

        public virtual DbSet<Company> Companies { get; set; }

        public virtual DbSet<AppUser> AppUsers { get; set; }

        public virtual DbSet<Invitation> Invations { get; set; }

        public virtual DbSet<EmployeeWorkDay> EmployeeWorkDays { get; set; }

        public virtual DbSet<WorkDayTask> WorkDayTasks { get; set; }

        public virtual DbSet<WorkTask> WorkTasks { get; set; }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            // Company configuration
            builder.Entity<Company>()
                .HasMany(c => c.AppUsers)
                .WithMany(c => c.Companies)
                .UsingEntity("AppUserCompany");
            builder.Entity<Company>()
                .HasMany(c => c.EmployeeWorkDays)
                .WithOne(c => c.Company)
                .HasForeignKey("CompanyId");
            builder.Entity<Company>()
                .HasMany(c => c.WorkTasks)
                .WithOne(c => c.Company)
                .HasForeignKey("WorkTaskId");

            builder.Entity<Company>()
                .HasData(
                    new Company
                    {
                        CompanyId = 1,
                        CompanyName = "Outdoors By Design"
                    }
                );

            //AppUser configuration
            builder.Entity<IdentityUserRole<int>>().HasData(
                    new IdentityUserRole<int>
                    {
                        RoleId = 1,
                        UserId = 1
                    }
                );
            //builder.Entity<AppUser>()
            //    .HasMany(a => a.Companies)
            //    .WithMany(c => c.AppUsers)
            //    .UsingEntity(ac => ac
            //        .HasData(
            //            new { AppUsersId = 1, CompaniesCompanyId = 1 }
            //        ));

            // EmployeeWorkDay configuration
            builder.Entity<EmployeeWorkDay>()
                .HasOne(e => e.User)
                .WithMany(u => u.EmployeeWorkDays)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<EmployeeWorkDay>()
                .HasOne(e => e.Company)
                .WithMany(c => c.EmployeeWorkDays)
                .HasForeignKey(e => e.CompanyId);

            DateTime createdAt = new DateTime(2025, 4, 21);
            builder.Entity<EmployeeWorkDay>()
                .HasData(
                    new EmployeeWorkDay
                    {
                        EmployeeWorkDayId = 1,
                        CustomerName = "Bourne",
                        CreatedAt = createdAt,
                        Date = new DateOnly(2025, 4, 18),
                        StartTime = new TimeOnly(8, 00, 00),
                        EndTime = new TimeOnly(16, 30, 00),
                        LunchTime = new TimeOnly(12, 00, 00),
                        LunchDuration = 30,
                        Mileage = null,
                        TruckName = null,
                        UserId = 1,
                        CompanyId = 1
                    }
                );

            // WorkTask configuration
            builder.Entity<WorkTask>()
                .HasOne(wt => wt.Company)
                .WithMany(wt => wt.WorkTasks)
                .HasForeignKey(wt => wt.CompanyId);

            builder.Entity<WorkTask>()
            .HasData(
               new WorkTask { WorkTaskId = 1, Category = "General Labor", CompanyId = 1, Description = "Demoed landscape" },
               new WorkTask { WorkTaskId = 2, Category = "General Labor", CompanyId = 1, Description = "Installed bark" },
               new WorkTask { WorkTaskId = 3, Category = "General Labor", CompanyId = 1, Description = "Installed top soil" },
               new WorkTask { WorkTaskId = 4, Category = "General Labor", CompanyId = 1, Description = "Cleaned driveway/sidewalks" },
               new WorkTask { WorkTaskId = 5, Category = "Hardscape", CompanyId = 1, Description = "Prepped subbase" },
               new WorkTask { WorkTaskId = 6, Category = "Hardscape", CompanyId = 1, Description = "Set block/pavers/stone" },
               new WorkTask { WorkTaskId = 7, Category = "Hardscape", CompanyId = 1, Description = "Cut block/pavers/stone" },
               new WorkTask { WorkTaskId = 8, Category = "Irrigation", CompanyId = 1, Description = "Dug ditches" },
               new WorkTask { WorkTaskId = 9, Category = "Irrigation", CompanyId = 1, Description = "Plumbed lines" },
               new WorkTask { WorkTaskId = 10, Category = "Irrigation", CompanyId = 1, Description = "Adjusted heads" }
           );

            // WorkDayTask configuration
            builder.Entity<WorkDayTask>()
                .HasOne(wdt => wdt.EmployeeWorkDay)
                .WithMany(wdt => wdt.WorkDayTasks)
                .HasForeignKey(wdt => wdt.EmployeeWorkDayId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<WorkDayTask>()
                .HasData(
                    new WorkDayTask { WorkDayTaskId = 1, EmployeeWorkDayId = 1, WorkTaskId = 1 },
                    new WorkDayTask { WorkDayTaskId = 2, EmployeeWorkDayId = 1, WorkTaskId = 2 },
                    new WorkDayTask { WorkDayTaskId = 3, EmployeeWorkDayId = 1, WorkTaskId = 3 },
                    new WorkDayTask { WorkDayTaskId = 4, EmployeeWorkDayId = 1, WorkTaskId = 4 }
                );

            base.OnModelCreating(builder);
        }

        partial void OnModelCreatingPartial(ModelBuilder builder);
    }
}
