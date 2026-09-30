using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Seido.Utilities.SeedGenerator;
using MySqlConnector;
using Npgsql;
using System.Data;
using System.Data.Common;

using DbModels;
using DbContext;
using Configuration;
using Models;
using Models.DTO;
using Microsoft.Data.SqlClient;

namespace DbRepos;

public class AdminDbRepos
{
    #region Fields
    private const string _seedSource = "./app-seeds.json";
    private readonly ILogger<AdminDbRepos> _logger;
    private readonly Encryptions _encryptions;
    private readonly MainDbContext _dbContext;
    #endregion

    #region Constructor
    public AdminDbRepos(ILogger<AdminDbRepos> logger, Encryptions encryptions, MainDbContext context)
    {
        _logger = logger;
        _encryptions = encryptions;
        _dbContext = context;
    }
    #endregion

    #region Database Information
    public async Task<ResponseItemDto<GstUsrInfoAllDto>> InfoAsync() => await DbInfo();

    public async Task<ResponseItemDto<GstUsrInfoAllDto>> DbInfo()
    {
        var connection = _dbContext.Database.GetDbConnection();
        if (connection.State != ConnectionState.Open)
            await connection.OpenAsync();

        GstUsrInfoDbDto dbInfo = null;
        using (var command = connection.CreateCommand())
        {
            command.CommandText = "SELECT * FROM dbo.vwInfoDb";
            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                dbInfo = new GstUsrInfoDbDto
                {
                    NrSeededUsers = Convert.ToInt32(reader["NrSeededUsers"]),
                    NrUnseededUsers = Convert.ToInt32(reader["NrUnseededUsers"]),
                    NrSeededAttraction = Convert.ToInt32(reader["NrSeededAttraction"]),
                    NrUnseededAttractions = Convert.ToInt32(reader["NrUnseededAttractions"]),
                    NrSeededAddresses = Convert.ToInt32(reader["NrSeededAddresses"]),
                    NrUnseededAddresses = Convert.ToInt32(reader["NrUnseededAddresses"]),
                    NrSeededCities = Convert.ToInt32(reader["NrSeededCities"]),
                    NrUnseededCities = Convert.ToInt32(reader["NrUnseededCities"]),
                    NrSeededCountries = Convert.ToInt32(reader["NrSeededCountries"]),
                    NrUnseededCountries = Convert.ToInt32(reader["NrUnseededCountries"]),
                    NrSeededComments = Convert.ToInt32(reader["NrSeededComments"]),
                    NrUnseededComments = Convert.ToInt32(reader["NrUnseededComments"]),
                    NrSeededReviews = Convert.ToInt32(reader["NrSeededReviews"]),
                    NrUnseededReviews = Convert.ToInt32(reader["NrUnseededReviews"]),
                    NrSeededCategories = Convert.ToInt32(reader["NrSeededCategories"]),
                    NrUnseededCategories = Convert.ToInt32(reader["NrUnseededCategories"])
                };
            }
        }

        var attractions = new List<GstUsrInfoAttractionsDto>();
        using (var command = connection.CreateCommand())
        {
            command.CommandText = "SELECT * FROM dbo.vwInfoAttractions";
            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                attractions.Add(new GstUsrInfoAttractionsDto
                {
                    Country = reader["Country"] as string,
                    City = reader["City"] as string,
                    NrAttractions = Convert.ToInt32(reader["NrAttractions"])
                });
            }
        }

        var reviews = new List<GstUsrInfoReviewsDto>();
        using (var command = connection.CreateCommand())
        {
            command.CommandText = "SELECT * FROM dbo.vwInfoReviews";
            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                reviews.Add(new GstUsrInfoReviewsDto
                {
                    NrReviews = Convert.ToInt32(reader["NrReviews"]),
                    NrComments = Convert.ToInt32(reader["NrComments"])
                });
            }
        }

        var info = new GstUsrInfoAllDto
        {
            Db = dbInfo,
            Reviews = reviews,
            Attractions = attractions
        };

        return new ResponseItemDto<GstUsrInfoAllDto>
        {
#if DEBUG
            ConnectionString = _dbContext.dbConnection,
#endif
            Item = info
        };
    }
    #endregion

    #region Seed Data
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

        var cities = new List<DbCity>();

        foreach (var country in countries)
        {
            var cityNamesForCountry = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var citiesForCountry = new List<DbCity>();
            var maxCityAttempts = Math.Max(100, 25 * 10);

            for (var attempt = 0; citiesForCountry.Count < 25 && attempt < maxCityAttempts; attempt++)
            {
                var city = new DbCity();
                city.DbCountry = country; 
                city.Seed(seeder);         

                if (cityNamesForCountry.Add(city.CityName))
                {
                    city.Seeded = true;
                    country.DbCities.Add(city);
                    citiesForCountry.Add(city);
                }
            }

            if (citiesForCountry.Count < 25)
            {
                throw new InvalidOperationException(
                    $"Could not generate 25 unique cities for country '{country.CountryName}'. " +
                    $"Only {citiesForCountry.Count} unique cities were found.");
            }

            cities.AddRange(citiesForCountry);
        }


        var addresses = seeder.ItemsToList<DbAddress>(1000);
        var attractions = seeder.ItemsToList<DbAttraction>(1000);
        var users = seeder.ItemsToList<DbUser>(500);
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
    #endregion

    #region Remove Seed Data
    public async Task<ResponseItemDto<GstUsrInfoAllDto>> RemoveSeedAsync(bool seeded)
    {
        var connection = _dbContext.Database.GetDbConnection();
        using var command = connection.CreateCommand();
        command.CommandType = CommandType.StoredProcedure;
        command.CommandText = "dbo.spDeleteAll";

        var parameters = new List<SqlParameter>
    {
        new SqlParameter("seededParam", seeded),
        new SqlParameter("nrUsersAffected", SqlDbType.Int) { Direction = ParameterDirection.Output },
        new SqlParameter("nrAttractionsAffected", SqlDbType.Int) { Direction = ParameterDirection.Output },
        new SqlParameter("nrAddressesAffected", SqlDbType.Int) { Direction = ParameterDirection.Output },
        new SqlParameter("nrCitiesAffected", SqlDbType.Int) { Direction = ParameterDirection.Output },
        new SqlParameter("nrCountriesAffected", SqlDbType.Int) { Direction = ParameterDirection.Output },
        new SqlParameter("nrCommentsAffected", SqlDbType.Int) { Direction = ParameterDirection.Output },
        new SqlParameter("nrReviewsAffected", SqlDbType.Int) { Direction = ParameterDirection.Output },
        new SqlParameter("nrCategoriesAffected", SqlDbType.Int) { Direction = ParameterDirection.Output }
    };
        command.Parameters.AddRange(parameters.ToArray());

        if (connection.State != ConnectionState.Open)
            await connection.OpenAsync();
        using var reader = await command.ExecuteReaderAsync();
        await reader.CloseAsync();

        return await DbInfo();
    }
    #endregion

    #region Change Tracking Diagnostics
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
    #endregion
}
