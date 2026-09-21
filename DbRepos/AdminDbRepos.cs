using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Seido.Utilities.SeedGenerator;
using DbModels;
using DbContext;
using Configuration;
using Models;
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

            NrSeededCities = await _dbContext.City.Where(c => c.Seeded).CountAsync(),
            NrUnseededCities = await _dbContext.City.Where(c => !c.Seeded).CountAsync(),

            NrSeededCountries = await _dbContext.Country.Where(c => c.Seeded).CountAsync(),
            NrUnseededCountries = await _dbContext.Country.Where(c => !c.Seeded).CountAsync(),

            NrSeededComments = await _dbContext.Comment.Where(c => c.Seeded).CountAsync(),
            NrUnseededComments = await _dbContext.Comment.Where(c => !c.Seeded).CountAsync(),

            NrSeededUsers = await _dbContext.User.Where(c => c.Seeded).CountAsync(),
            NrUnseededUsers = await _dbContext.User.Where(c => !c.Seeded).CountAsync(),

            NrSeededReviews = await _dbContext.Review.Where(c => c.Seeded).CountAsync(),
            NrUnseededReviews = await _dbContext.Review.Where(c => !c.Seeded).CountAsync(),

            NrSeededCategories = await _dbContext.Category.Where(c => c.Seeded).CountAsync(),
            NrUnseededCategories  = await _dbContext.Category.Where(c => !c.Seeded).CountAsync()

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
        var countries = new List<DbCountry>();
        var countryNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var maxCountryAttempts = Math.Max(100, 4 * 100);

        for (var attempt = 0; countries.Count < 4 && attempt < maxCountryAttempts; attempt++)
        {
            var country = seeder.ItemsToList<DbCountry>(1).Single();

            if (countryNames.Add(country.CountryName))
            {
                countries.Add(country);
            }
        }

        if (countries.Count < 4)
        {
            throw new InvalidOperationException(
                $"Could not generate {nrOfItems} unique countries from the seed data. " +
                $"Only {countries.Count} unique countries were found.");
        }

        var cities = seeder.ItemsToList<DbCity>(100);
        var addresses = seeder.ItemsToList<DbAddress>(1000);
        var attractions = seeder.ItemsToList<DbAttraction>(1000);
        var users = seeder.ItemsToList<DbUser>(50);
        var comments = seeder.ItemsToList<DbComment>(2000);
        var categories = seeder.ItemsToList<DbCategory>(5);
        var reviews = new List<DbReview>();

        countries.ForEach(c => c.Seeded = true);
        cities.ForEach(c => c.Seeded = true);
        addresses.ForEach(a => a.Seeded = true);
        users.ForEach(u => u.Seeded = true);
        comments.ForEach(c => c.Seeded = true);
        categories.ForEach(c => c.Seeded = true);
        attractions.ForEach(a => a.Seeded = true);

        foreach (var country in countries)
        {
            for (int i = 0; i < 100; i++)
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

        foreach (var attraction in attractions)
        {
            var address = seeder.FromList(addresses);


            attraction.DbAddress = address; 

            var numberOfCategories = seeder.Next(1, 4);

        for (int i = 0; i < numberOfCategories; i++)
        {
             var category = seeder.FromList(categories);

            if (!attraction.DbCategories.Contains(category))
            {
                attraction.DbCategories.Add(category);
                category.DbAttractions.Add(attraction);
            }
            }   
    
        }

        foreach (var review in reviews)
        {
            var comment = seeder.FromList(comments);

            review.DbComment = comment;
        }


        var availableComments = new List<DbComment>(comments);
        foreach (var attraction in attractions)
        {
            var reviewCount = seeder.Next(0, 21);

            for (int i = 0; i < reviewCount; i++)
            {
                var review = new DbReview
                {
                    ReviewId = Guid.NewGuid(),
                    DbAttraction = attraction,
                    DbUser = seeder.FromList(users),
                    Rating = (ReviewRating)seeder.Next(1, 6),
                    CreatedAt = seeder.DateAndTime(2020, 2026),
                    Seeded = true,
                };

                if (availableComments.Count > 0 && seeder.Bool)
                {
                    var comment = seeder.FromList(availableComments);
                    availableComments.Remove(comment);
                    review.DbComment = comment;
                    comment.DbReview = review;
                }

                attraction.DbReviews.Add(review);
                reviews.Add(review);
            }
        }

        _dbContext.Country.AddRange(countries);
        _dbContext.City.AddRange(cities);
        _dbContext.Address.AddRange(addresses);
        _dbContext.Attraction.AddRange(attractions);
        _dbContext.Comment.AddRange(comments);
        _dbContext.User.AddRange(users);
        _dbContext.Review.AddRange(reviews);
        _dbContext.Category.AddRange(categories);
        //LogChangeTracker();
        await _dbContext.SaveChangesAsync();
        //LogChangeTracker();


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
        _dbContext.Review.RemoveRange(_dbContext.Review.Where(f => f.Seeded == seeded));
        _dbContext.Category.RemoveRange(_dbContext.Category.Where(f => f.Seeded == seeded));
        

        //LogChangeTracker();
        await _dbContext.SaveChangesAsync();
        //LogChangeTracker();

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
                DbReview dbReview => dbReview.ReviewId,
                _ => Guid.Empty
            };

            _logger.LogInformation($"{nameof(LogChangeTracker)}: {e.Entity.GetType().Name}: {id} - {e.State}");
        }
    }
}
