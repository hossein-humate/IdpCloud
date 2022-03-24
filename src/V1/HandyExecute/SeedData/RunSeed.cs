using DataProvider.DatabaseContext;
using HandyExecute.SeedData.Seed;

namespace HandyExecute.SeedData
{
    public class RunSeed
    {
        //private string ConnectionString = "Password=1;Persist Security Info=True;User ID=sa;Initial Catalog=Humate;Data Source=.;MultipleActiveResultSets=True;App=EntityFramework";
        private const string ConnectionString = "Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=HumateStaging;Data Source=.;MultipleActiveResultSets=True;App=EntityFramework";
        public static void Run()
        {
            var db = EfCoreContext.Create(ConnectionString);
            db.Countries.AddRange(CountrySeed.Get());
            db.Cities.AddRange(CitySeed.Get());
            db.Currencies.AddRange(CurrencySeed.Get());
            db.Languages.AddRange(LanguageSeed.Get());
            db.MasterDetails.AddRange(MasterDetailSeed.Get());
            db.Permissions.AddRange(PermissionSeed.Get());
            db.Roles.AddRange(RoleSeed.Get());
            db.Softwares.AddRange(SoftwareSeed.Get());
            db.Users.AddRange(UserSeed.Get());
            db.SaveChanges();
        }
    }
}
