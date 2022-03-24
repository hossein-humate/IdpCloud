using DataProvider.EntityConfiguration.BaseInfo;
using DataProvider.EntityConfiguration.Identity;
using DataProvider.EntityConfiguration.Log;
using DataProvider.EntityConfiguration.SSO;
using Entity.BaseInfo;
using Entity.Identity;
using Entity.Log;
using Entity.SSO;
using Microsoft.EntityFrameworkCore;

namespace DataProvider.DatabaseContext
{
    public class EfCoreContext : DbContext
    {
        public EfCoreContext(DbContextOptions<EfCoreContext> dbContextOptions)
            : base(dbContextOptions)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
           
        }

        public static EfCoreContext Create(string connection)
        {
            return new EfCoreContext(new DbContextOptionsBuilder<EfCoreContext>()
                    .UseSqlServer(connection).Options);
        }

        #region Identity DataSets
        public DbSet<Address> Addresses { get; set; }
        public DbSet<Person> Persons { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }
        public DbSet<UserPermission> UserPermissions { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<SocialNetwork> SocialNetworks { get; set; }
        public DbSet<Visitor> Visitors { get; set; }
        public DbSet<Software> Softwares { get; set; }
        public DbSet<UserSoftware> UserSoftwares { get; set; }
        #endregion

        #region BaseInfo DataSets
        public DbSet<Currency> Currencies { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<Language> Languages { get; set; }
        public DbSet<MasterDetail> MasterDetails { get; set; }
        #endregion

        #region Log DataSets
        public DbSet<ServerActivity> ServerActivities { get; set; }
        #endregion

        #region SSO DataSets
        public DbSet<UserSession> UserSessions { get; set; }
        #endregion

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            #region BaseInfo Shcema
            modelBuilder.ApplyConfiguration(new CurrencyConfiguration());
            modelBuilder.ApplyConfiguration(new CityConfiguration());
            modelBuilder.ApplyConfiguration(new CountryConfiguration());
            modelBuilder.ApplyConfiguration(new LanguageConfiguration());
            modelBuilder.ApplyConfiguration(new MasterDetailConfiguration());
            #endregion

            #region Identity Schema
            modelBuilder.ApplyConfiguration(new AddressConfiguration());
            modelBuilder.ApplyConfiguration(new PersonConfiguration());
            modelBuilder.ApplyConfiguration(new PermissionConfiguration());
            modelBuilder.ApplyConfiguration(new RoleConfiguration());
            modelBuilder.ApplyConfiguration(new RolePermissionConfiguration());
            modelBuilder.ApplyConfiguration(new SoftwareConfiguration());
            modelBuilder.ApplyConfiguration(new UserConfiguration());
            modelBuilder.ApplyConfiguration(new UserPermissionConfiguration());
            modelBuilder.ApplyConfiguration(new UserRoleConfiguration());
            modelBuilder.ApplyConfiguration(new UserSoftwareConfiguration());
            modelBuilder.ApplyConfiguration(new VisitorConfiguration());
            #endregion

            #region Log Shcema
            modelBuilder.ApplyConfiguration(new ServerActivityConfiguration());
            #endregion

            #region Log Shcema
            modelBuilder.ApplyConfiguration(new UserSessionConfiguration());
            #endregion
        }
    }
}