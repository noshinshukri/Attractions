-- Connect to the sql-attractions database before running this script.

DROP PROCEDURE IF EXISTS "spDeleteAll"(BOOLEAN);

DROP VIEW IF EXISTS "vwInfoDb";
DROP VIEW IF EXISTS "vwInfoAttractions";
DROP VIEW IF EXISTS "vwInfoReviews";
DROP VIEW IF EXISTS "vwInfoCategories";

DROP TABLE IF EXISTS "DbAttractionDbCategory";
DROP TABLE IF EXISTS "Review";
DROP TABLE IF EXISTS "Comment";
DROP TABLE IF EXISTS "Attraction";
DROP TABLE IF EXISTS "Address";
DROP TABLE IF EXISTS "City";
DROP TABLE IF EXISTS "Country";
DROP TABLE IF EXISTS "User";
DROP TABLE IF EXISTS "Category";
DROP TABLE IF EXISTS "__EFMigrationsHistory";
