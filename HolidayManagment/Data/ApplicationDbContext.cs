using Microsoft.EntityFrameworkCore;
using HolidayManagment;
using HolidayManagment.Models;
using System.Collections.Generic;
using System.Reflection.Emit;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace HolidayManagment.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {

            public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
                : base(options)
            {
            }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<IdentityUserLogin<string>>(entity =>
            {
                entity.HasKey(e => e.UserId);  
            });

            builder.Entity<IdentityUserRole<string>>(entity =>
            {
                entity.HasKey(e => new { e.UserId, e.RoleId });  
            });

            builder.Entity<IdentityUserClaim<string>>(entity =>
            {
                entity.HasKey(e => e.Id);  
            });

            builder.Entity<IdentityRoleClaim<string>>(entity =>
            {
                entity.HasKey(e => e.Id);  
            });

            builder.Entity<IdentityUserToken<string>>(entity =>
            {
                entity.HasKey(e => new { e.UserId, e.LoginProvider, e.Name });  
            });
        }


        public DbSet<Company> Companies { get; set; }
            public DbSet<Employee> Employees { get; set; }
            public DbSet<LeaveType> LeaveTypes { get; set; }
            public DbSet<Leave> Leaves { get; set; }

           

        }
}
