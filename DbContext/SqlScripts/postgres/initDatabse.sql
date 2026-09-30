-- Connect to the sql-attractions database before running this script.

CREATE OR REPLACE VIEW "vwInfoDb" AS
	SELECT
		(SELECT COUNT(*) FROM "User" WHERE "Seeded" = TRUE) AS "NrSeededUsers",
		(SELECT COUNT(*) FROM "User" WHERE "Seeded" = FALSE) AS "NrUnseededUsers",
		(SELECT COUNT(*) FROM "Attraction" WHERE "Seeded" = TRUE) AS "NrSeededAttraction",
		(SELECT COUNT(*) FROM "Attraction" WHERE "Seeded" = FALSE) AS "NrUnseededAttractions",
		(SELECT COUNT(*) FROM "Address" WHERE "Seeded" = TRUE) AS "NrSeededAddresses",
		(SELECT COUNT(*) FROM "Address" WHERE "Seeded" = FALSE) AS "NrUnseededAddresses",
		(SELECT COUNT(*) FROM "City" WHERE "Seeded" = TRUE) AS "NrSeededCities",
		(SELECT COUNT(*) FROM "City" WHERE "Seeded" = FALSE) AS "NrUnseededCities",
		(SELECT COUNT(*) FROM "Country" WHERE "Seeded" = TRUE) AS "NrSeededCountries",
		(SELECT COUNT(*) FROM "Country" WHERE "Seeded" = FALSE) AS "NrUnseededCountries",
		(SELECT COUNT(*) FROM "Comment" WHERE "Seeded" = TRUE) AS "NrSeededComments",
		(SELECT COUNT(*) FROM "Comment" WHERE "Seeded" = FALSE) AS "NrUnseededComments",
		(SELECT COUNT(*) FROM "Review" WHERE "Seeded" = TRUE) AS "NrSeededReviews",
		(SELECT COUNT(*) FROM "Review" WHERE "Seeded" = FALSE) AS "NrUnseededReviews",
		(SELECT COUNT(*) FROM "Category" WHERE "Seeded" = TRUE) AS "NrSeededCategories",
		(SELECT COUNT(*) FROM "Category" WHERE "Seeded" = FALSE) AS "NrUnseededCategories";

CREATE OR REPLACE VIEW "vwInfoAttractions" AS
	SELECT co."CountryName" AS "Country", ci."CityName" AS "City", COUNT(a."AttractionId") AS "NrAttractions"
	FROM "Attraction" a
	INNER JOIN "Address" ad ON a."DbAddressAddressId" = ad."AddressId"
	INNER JOIN "City" ci ON ad."DbCityCityId" = ci."CityId"
	INNER JOIN "Country" co ON ci."CountryId" = co."CountryId"
	GROUP BY ROLLUP (co."CountryName", ci."CityName");

CREATE OR REPLACE VIEW "vwInfoReviews" AS
	SELECT
		(SELECT COUNT(*) FROM "Review") AS "NrReviews",
		(SELECT COUNT(*) FROM "Review" WHERE "CommentId" IS NOT NULL) AS "NrComments";

CREATE OR REPLACE VIEW "vwInfoCategories" AS
	SELECT c."Name" AS "Category", COUNT(jc."DbAttractionsAttractionId") AS "NrAttractions"
	FROM "Category" c
	LEFT JOIN "DbAttractionDbCategory" jc ON jc."DbCategoriesCategoryId" = c."CategoryId"
	GROUP BY c."Name";

DROP PROCEDURE IF EXISTS "spDeleteAll"(BOOLEAN);
CREATE PROCEDURE "spDeleteAll"(
	IN "seededParam" BOOLEAN DEFAULT TRUE,
	OUT "nrUsersAffected" INTEGER,
	OUT "nrAttractionsAffected" INTEGER,
	OUT "nrAddressesAffected" INTEGER,
	OUT "nrCitiesAffected" INTEGER,
	OUT "nrCountriesAffected" INTEGER,
	OUT "nrCommentsAffected" INTEGER,
	OUT "nrReviewsAffected" INTEGER,
	OUT "nrCategoriesAffected" INTEGER
)
LANGUAGE plpgsql
AS $$
BEGIN
	SELECT COUNT(*) INTO "nrUsersAffected" FROM "User" WHERE "Seeded" = "seededParam";
	SELECT COUNT(*) INTO "nrAttractionsAffected" FROM "Attraction" WHERE "Seeded" = "seededParam";
	SELECT COUNT(*) INTO "nrAddressesAffected" FROM "Address" WHERE "Seeded" = "seededParam";
	SELECT COUNT(*) INTO "nrCitiesAffected" FROM "City" WHERE "Seeded" = "seededParam";
	SELECT COUNT(*) INTO "nrCountriesAffected" FROM "Country" WHERE "Seeded" = "seededParam";
	SELECT COUNT(*) INTO "nrCommentsAffected" FROM "Comment" WHERE "Seeded" = "seededParam";
	SELECT COUNT(*) INTO "nrReviewsAffected" FROM "Review" WHERE "Seeded" = "seededParam";
	SELECT COUNT(*) INTO "nrCategoriesAffected" FROM "Category" WHERE "Seeded" = "seededParam";

	DELETE FROM "DbAttractionDbCategory" jc
	USING "Attraction" a
	WHERE jc."DbAttractionsAttractionId" = a."AttractionId"
	  AND a."Seeded" = "seededParam";

	DELETE FROM "DbAttractionDbCategory" jc
	USING "Category" c
	WHERE jc."DbCategoriesCategoryId" = c."CategoryId"
	  AND c."Seeded" = "seededParam";

	DELETE FROM "Review" WHERE "Seeded" = "seededParam";
	DELETE FROM "Comment" WHERE "Seeded" = "seededParam";
	DELETE FROM "Attraction" WHERE "Seeded" = "seededParam";
	DELETE FROM "Address" WHERE "Seeded" = "seededParam";
	DELETE FROM "City" WHERE "Seeded" = "seededParam";
	DELETE FROM "Country" WHERE "Seeded" = "seededParam";
	DELETE FROM "User" WHERE "Seeded" = "seededParam";
	DELETE FROM "Category" WHERE "Seeded" = "seededParam";

	RAISE NOTICE 'Seeded-data deletion complete. Query "vwInfoDb" for current totals.';
END;
$$;
