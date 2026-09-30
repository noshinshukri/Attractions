USE [sql-attractions];
GO

-- View: datao (seeded/unseeded per tabell)
CREATE OR ALTER VIEW dbo.vwInfoDb AS
    SELECT
        (SELECT COUNT(*) FROM dbo.[User] WHERE Seeded = 1) AS NrSeededUsers,
        (SELECT COUNT(*) FROM dbo.[User] WHERE Seeded = 0) AS NrUnseededUsers,

        (SELECT COUNT(*) FROM dbo.Attraction WHERE Seeded = 1) AS NrSeededAttraction,
        (SELECT COUNT(*) FROM dbo.Attraction WHERE Seeded = 0) AS NrUnseededAttractions,

        (SELECT COUNT(*) FROM dbo.Address WHERE Seeded = 1) AS NrSeededAddresses,
        (SELECT COUNT(*) FROM dbo.Address WHERE Seeded = 0) AS NrUnseededAddresses,

        (SELECT COUNT(*) FROM dbo.City WHERE Seeded = 1) AS NrSeededCities,
        (SELECT COUNT(*) FROM dbo.City WHERE Seeded = 0) AS NrUnseededCities,

        (SELECT COUNT(*) FROM dbo.Country WHERE Seeded = 1) AS NrSeededCountries,
        (SELECT COUNT(*) FROM dbo.Country WHERE Seeded = 0) AS NrUnseededCountries,

        (SELECT COUNT(*) FROM dbo.Comment WHERE Seeded = 1) AS NrSeededComments,
        (SELECT COUNT(*) FROM dbo.Comment WHERE Seeded = 0) AS NrUnseededComments,

        (SELECT COUNT(*) FROM dbo.Review WHERE Seeded = 1) AS NrSeededReviews,
        (SELECT COUNT(*) FROM dbo.Review WHERE Seeded = 0) AS NrUnseededReviews,

        (SELECT COUNT(*) FROM dbo.Category WHERE Seeded = 1) AS NrSeededCategories,
        (SELECT COUNT(*) FROM dbo.Category WHERE Seeded = 0) AS NrUnseededCategories;
GO

CREATE OR ALTER VIEW dbo.vwInfoAttractions AS
    SELECT co.CountryName AS Country, ci.CityName AS City, COUNT(a.AttractionId) AS NrAttractions
    FROM dbo.Attraction a
    INNER JOIN dbo.Address ad ON a.DbAddressAddressId = ad.AddressId
    INNER JOIN dbo.City ci ON ad.DbCityCityId = ci.CityId
    INNER JOIN dbo.Country co ON ci.CountryId = co.CountryId
    GROUP BY co.CountryName, ci.CityName WITH ROLLUP;
GO


CREATE OR ALTER VIEW dbo.vwInfoReviews AS
    SELECT
        (SELECT COUNT(*) FROM dbo.Review) AS NrReviews,
        (SELECT COUNT(*) FROM dbo.Review WHERE CommentId IS NOT NULL) AS NrComments;
GO

CREATE OR ALTER VIEW dbo.vwInfoCategories AS
    SELECT c.Name AS Category, COUNT(jc.DbAttractionsAttractionId) AS NrAttractions
    FROM dbo.Category c
    LEFT JOIN dbo.DbAttractionDbCategory jc ON jc.DbCategoriesCategoryId = c.CategoryId
    GROUP BY c.Name;
GO
-- Stored procedure: remove seed-data 
CREATE OR ALTER PROC dbo.spDeleteAll
    @seededParam BIT = 1,

    @nrUsersAffected INT OUTPUT,
    @nrAttractionsAffected INT OUTPUT,
    @nrAddressesAffected INT OUTPUT,
    @nrCitiesAffected INT OUTPUT,
    @nrCountriesAffected INT OUTPUT,
    @nrCommentsAffected INT OUTPUT,
    @nrReviewsAffected INT OUTPUT,
    @nrCategoriesAffected INT OUTPUT

    AS
    SET NOCOUNT ON;

    SELECT @nrUsersAffected       = COUNT(*) FROM dbo.[User]     WHERE Seeded = @seededParam;
    SELECT @nrAttractionsAffected = COUNT(*) FROM dbo.Attraction WHERE Seeded = @seededParam;
    SELECT @nrAddressesAffected   = COUNT(*) FROM dbo.Address    WHERE Seeded = @seededParam;
    SELECT @nrCitiesAffected      = COUNT(*) FROM dbo.City       WHERE Seeded = @seededParam;
    SELECT @nrCountriesAffected   = COUNT(*) FROM dbo.Country    WHERE Seeded = @seededParam;
    SELECT @nrCommentsAffected    = COUNT(*) FROM dbo.Comment    WHERE Seeded = @seededParam;
    SELECT @nrReviewsAffected     = COUNT(*) FROM dbo.Review     WHERE Seeded = @seededParam;
    SELECT @nrCategoriesAffected  = COUNT(*) FROM dbo.Category   WHERE Seeded = @seededParam;


    DELETE jc FROM dbo.DbAttractionDbCategory jc
        INNER JOIN dbo.Attraction a ON jc.DbAttractionsAttractionId = a.AttractionId
        WHERE a.Seeded = @seededParam;

    DELETE jc FROM dbo.DbAttractionDbCategory jc
        INNER JOIN dbo.Category c ON jc.DbCategoriesCategoryId = c.CategoryId
        WHERE c.Seeded = @seededParam;

    DELETE FROM dbo.Review WHERE Seeded = @seededParam;
    DELETE FROM dbo.Comment WHERE Seeded = @seededParam;
    DELETE FROM dbo.Attraction WHERE Seeded = @seededParam;
    DELETE FROM dbo.Address WHERE Seeded = @seededParam;
    DELETE FROM dbo.City WHERE Seeded = @seededParam;
    DELETE FROM dbo.Country WHERE Seeded = @seededParam;
    DELETE FROM dbo.[User] WHERE Seeded = @seededParam;
    DELETE FROM dbo.Category WHERE Seeded = @seededParam;

    SELECT * FROM dbo.vwInfoDb;
GO