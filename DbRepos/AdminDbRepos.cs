using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Seido.Utilities.SeedGenerator;
using DbModels;
using DbContext;
using Configuration;
using Models.DTO;

namespace DbRepos;

public class AdminDbRepos
{
    private const string _seedSource = "./app-seeds.json";
    private readonly ILogger<AdminDbRepos> _logger;
    private readonly Encryptions _encryptions;
    private readonly MainDbContext _dbContext;
    /*
        public async Task<ResponseItemDto<GstUsrInfoAllDto>> SeedAsync(int nrItems)
        {
            var safeCount = Math.Max(1, nrItems);

            _dbContext.Attraction.RemoveRange(_dbContext.Attraction);

            var attractions = Enumerable.Range(1, safeCount)
                .Select(i => new DbAttraction
                {
                    AttractionId = Guid.NewGuid(),
                    Name = $"Attraction {i}"
                })
                .ToList();

            _dbContext.Attraction.AddRange(attractions);
            await _dbContext.SaveChangesAsync();

            return await InfoAsync();
        }
    */
    public AdminDbRepos(ILogger<AdminDbRepos> logger, Encryptions encryptions, MainDbContext context)
    {
        _logger = logger;
        _encryptions = encryptions;
        _dbContext = context;
    }

    public async Task<ResponseItemDto<GstUsrInfoAllDto>> InfoAsync() => await DbInfo();

    private async Task<ResponseItemDto<GstUsrInfoAllDto>> DbInfo()
    {
        var info = new GstUsrInfoAllDto();
        info.Db = new GstUsrInfoDbDto
        {
            NrSeededAttraction = await _dbContext.Attraction.Where(c => c.Seeded).CountAsync(),
            NrUnseededAttractions = await _dbContext.Attraction.Where(c => !c.Seeded).CountAsync(),
            NrAttractionsWithAddress = 0,

            NrSeededCities = await _dbContext.City.Where(c => c.Seeded).CountAsync(),
            NrUnseededCities = await _dbContext.City.Where(c => !c.Seeded).CountAsync(),

            NrSeededCountries = await _dbContext.Country.Where(c => c.Seeded).CountAsync(),
            NrUnseededCountries = await _dbContext.Country.Where(c => !c.Seeded).CountAsync(),

            NrSeededComments = await _dbContext.Comment.Where(c => c.Seeded).CountAsync(),
            NrUnseededComments = await _dbContext.Comment.Where(c => !c.Seeded).CountAsync(),

            NrSeededUsers = await _dbContext.User.Where(c => c.Seeded).CountAsync(),
            NrUnseededUsers = await _dbContext.User.Where(c => !c.Seeded).CountAsync(),

            NrSeededReviews = await _dbContext.Review.CountAsync(),
            NrUnseededReviews = 0,
        };

        info.Reviews = new List<GstUsrInfoReviewsDto>();
        info.Attractions = new List<GstUsrInfoAttractionsDto>();

        return new ResponseItemDto<GstUsrInfoAllDto>
        {
#if DEBUG
            ConnectionString = _dbContext.dbConnection,
#endif

            Item = info
        };
    }

    public async Task<ResponseItemDto<GstUsrInfoAllDto>> SeedAsync(int nrOfItems)
    {
        //First of all make sure the database is cleared from all seeded data
        await RemoveSeedAsync(true);

        //Create a seeder
        var fn = Path.GetFullPath(_seedSource);
        var seeder = new SeedGenerator(fn);



        //Generate the seeded data
        var countries = seeder.ItemsToList<DbCountry>(nrOfItems);
        var cities = seeder.ItemsToList<DbCity>(nrOfItems);
        var addresses = seeder.ItemsToList<DbAddress>(nrOfItems);
        var attractions = seeder.ItemsToList<DbAttraction>(nrOfItems);
        var users = seeder.ItemsToList<DbUser>(nrOfItems);
        var comments = seeder.ItemsToList<DbComment>(nrOfItems);


        countries.ForEach(c => c.Seeded = true);
        cities.ForEach(c => c.Seeded = true);
        addresses.ForEach(a => a.Seeded = true);
        users.ForEach(u => u.Seeded = true);
        comments.ForEach(c => c.Seeded = true);
        attractions.ForEach(a => a.Seeded = true);
        foreach (var country in countries)
        {
            for (int i = 0; i < 3; i++)
            {
                var city = seeder.FromList(cities);

                country.DbCities.Add(city);
                city.DbCountry = country;
                
            }
        }
        foreach (var address in addresses)
        {
            var city = seeder.FromList(cities);
            

            address.DbCity = city;
            address.DbCountry = city.DbCountry;
        }

        //Note that all other tables are automatically set through FriendDbM Navigation properties
        _dbContext.Country.AddRange(countries);
        _dbContext.City.AddRange(cities);
        _dbContext.Address.AddRange(addresses);
        _dbContext.Attraction.AddRange(attractions);
        _dbContext.Comment.AddRange(comments);
        _dbContext.User.AddRange(users);
        LogChangeTracker();
        await _dbContext.SaveChangesAsync();
        LogChangeTracker();


        return await DbInfo();
    }

    public async Task<ResponseItemDto<GstUsrInfoAllDto>> RemoveSeedAsync(bool seeded)
    {
        _dbContext.Country.RemoveRange(_dbContext.Country.Where(f => f.Seeded == seeded));
        _dbContext.City.RemoveRange(_dbContext.City.Where(f => f.Seeded == seeded));
        _dbContext.Address.RemoveRange(_dbContext.Address.Where(f => f.Seeded == seeded));
        _dbContext.Attraction.RemoveRange(_dbContext.Attraction.Where(f => f.Seeded == seeded));
        _dbContext.User.RemoveRange(_dbContext.User.Where(f => f.Seeded == seeded));
        _dbContext.Comment.RemoveRange(_dbContext.Comment.Where(f => f.Seeded == seeded));
        

        LogChangeTracker();
        await _dbContext.SaveChangesAsync();
        LogChangeTracker();

        return await DbInfo();
    }

    private void LogChangeTracker()
    {
        foreach (var e in _dbContext.ChangeTracker.Entries())
        {
            var id = e.Entity switch
            {

                DbCountry dbCountry => dbCountry.CountryId,
                DbCity dbCity => dbCity.CityId,
                DbAddress dbAddress => dbAddress.AddressId,
                DbAttraction dbAttraction => dbAttraction.AttractionId,
                DbUser dbUser => dbUser.UserId,
                DbComment dbComment => dbComment.CommentId,
                _ => Guid.Empty
            };

            _logger.LogInformation($"{nameof(LogChangeTracker)}: {e.Entity.GetType().Name}: {id} - {e.State}");
        }
    }
}
