using IdpCloud.DataProvider.Entity.BaseInfo;
using IdpCloud.DataProvider.Entity.Identity;
using IdpCloud.DataProvider.Entity.Security;
using IdpCloud.DataProvider.Entity.SSO;
using IdpCloud.DataProvider.EntityConfiguration.BaseInfo;
using IdpCloud.DataProvider.EntityConfiguration.Identity;
using IdpCloud.DataProvider.EntityConfiguration.Security;
using IdpCloud.DataProvider.EntityConfiguration.SSO;
using Microsoft.EntityFrameworkCore;
using System;

namespace IdpCloud.DataProvider.DatabaseContext
{
    public class EfCoreContext : DbContext
    {
        public EfCoreContext(DbContextOptions<EfCoreContext> dbContextOptions)
            : base(dbContextOptions)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.LogTo(Console.WriteLine).EnableDetailedErrors().EnableSensitiveDataLogging();
        }

        public static EfCoreContext Create(string connection)
        {
            return new EfCoreContext(new DbContextOptionsBuilder<EfCoreContext>()
                    .UseSqlServer(connection).Options);
        }

        #region Identity DataSets
        public DbSet<User> Users { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }
        public DbSet<UserPermission> UserPermissions { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<Software> Softwares { get; set; }
        public DbSet<SoftwareDetail> SoftwareDetails { get; set; }
        public virtual DbSet<UserSoftware> UserSoftwares { get; set; }
        public virtual DbSet<Organisation> Organisations { get; set; }
        #endregion

        #region BaseInfo DataSets
        public DbSet<Country> Countries { get; set; }
        public DbSet<Language> Languages { get; set; }
        #endregion

        #region SSO DataSets
        public virtual DbSet<UserSession> UserSessions { get; set; }
        public DbSet<JwtSetting> JwtSettings { get; set; }
        #endregion

        public DbSet<Activity> Activities { get; set; }
        public DbSet<ResetPassword> ResetPasswords { get; set; }
        public DbSet<BanList> BanLists { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            #region BaseInfo Shcema
            modelBuilder.ApplyConfiguration(new CountryConfiguration());
            modelBuilder.ApplyConfiguration(new LanguageConfiguration());
            #endregion

            #region Identity Schema
            modelBuilder.ApplyConfiguration(new PermissionConfiguration());
            modelBuilder.ApplyConfiguration(new RoleConfiguration());
            modelBuilder.ApplyConfiguration(new RolePermissionConfiguration());
            modelBuilder.ApplyConfiguration(new SoftwareConfiguration());
            modelBuilder.ApplyConfiguration(new SoftwareDetailConfiguration());
            modelBuilder.ApplyConfiguration(new UserConfiguration());
            modelBuilder.ApplyConfiguration(new UserPermissionConfiguration());
            modelBuilder.ApplyConfiguration(new UserRoleConfiguration());
            modelBuilder.ApplyConfiguration(new UserSoftwareConfiguration());
            modelBuilder.ApplyConfiguration(new OrganisationConfiguration());
            #endregion

            #region SSO Shcema
            modelBuilder.ApplyConfiguration(new UserSessionConfiguration());
            modelBuilder.ApplyConfiguration(new JwtSettingConfiguration());
            #endregion

            modelBuilder.ApplyConfiguration(new ActivityConfiguration());
            modelBuilder.ApplyConfiguration(new ResetPasswordConfiguration());
            modelBuilder.ApplyConfiguration(new BanListConfiguration());
        }
    }
}