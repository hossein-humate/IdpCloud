using IdpCloud.DataProvider.DatabaseContext;
using IdpCloud.DataProvider.Entity.Enum;
using IdpCloud.DataProvider.Entity.Security;
using IdpCloud.ServiceProvider.EntityService.Security;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace IdpCloud.Tests.InMemory.Security
{
    /// <summary>
    /// Testing all functionality related to Activity Entity that would generate sql queries by EF Core Package
    /// </summary>
    public class ActivityRepositoryTests : TestBase
    {
        private readonly IActivityRepository _activityRepository;
        private readonly EfCoreContext _efDbContext;
        private const string _ip = "10.10.10.10";
        private const ActivityType _activityType = ActivityType.RequestResetPasswordByEmail;

        public ActivityRepositoryTests()
        {
            _efDbContext = new EfCoreContext(GetMockContextOptions());
            _efDbContext.Database.EnsureDeleted();
            _efDbContext.Database.EnsureCreated();
            _activityRepository = new ActivityRepository(_efDbContext);
        }

        /// <summary>
        /// Three times testing with Count = 5, Count = 7 and Count = 10
        /// <para>
        /// Arrange: Add 5,7,10 Activities for the IP Address of 10.10.10.10 with Type of RequestResetPasswordByEmail
        /// </para>
        /// <para>
        /// Act: <see cref="ActivityRepository.CountLastHourOfIpAsync(string, ActivityType, CancellationToken)"/>
        /// returns Counts of activites for the given IP address
        /// </para>
        /// Assert: Check Act resultCount is Equal input value count then the test Passed
        /// </summary>
        /// <param name="count">represent how many times an activity from same Ip Address should add in Arragne part and then same count should return from Act</param>
        /// <returns> A <see cref="Task"/> reprsenting as async opertation</returns>
        [Theory]
        [InlineData(5)]
        [InlineData(7)]
        [InlineData(10)]
        public async Task CountLastHourOfIp_ShouldReturnCount_WhenThoseActivitiesOfTheSameIPAddedInLast1Hour(int count)
        {
            //Arrange
            for (int i = 0; i < count; i++)
            {
                await _activityRepository.AddAsync(new Activity
                {
                    Ip = _ip,
                    UserAgent = "TestBrowser",
                    Type = _activityType,
                    Decription = "Test"
                });
            }
            await _efDbContext.SaveChangesAsync();

            //Act
            var resultCount = await _activityRepository.CountLastHourOfIpAsync(_ip, _activityType, default);

            //Assert
            Assert.Equal(count, resultCount);
        }

        /// <summary>
        /// Asserts <see cref="ActivityRepository.AddAsync(Activity, CancellationToken)"/>
        /// adds the activity record</summary> when given <see cref="Activity"/>
        /// <returns> A <see cref="Task"/> reprsenting as async opertation</returns>
        [Fact]
        public async Task AddAsync_ShouldAddAndReturnActivity_WhenGivenActivity()
        {
            //Arrange
            var activity = new Activity
            {
                Ip = _ip,
                UserAgent = "TestBrowser",
                Type = _activityType,
                Decription = "Test"
            };

            //Act
            var activityResult = await _activityRepository.AddAsync(activity, default);

            //Assert
            Assert.StrictEqual(activityResult, activity);
        }
    }
}
