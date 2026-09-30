USE [sql-attractions];
--GO

-- remove stored procedures
DROP PROCEDURE IF EXISTS dbo.spDeleteAll
GO

-- remove views
DROP VIEW IF EXISTS [dbo].[vwInfoDb]
DROP VIEW IF EXISTS [dbo].[vwInfoAttractions]
DROP VIEW IF EXISTS [dbo].[vwInfoReviews]
GO

-- Drop tables in the right order to avoid FK conflicts
DROP TABLE IF EXISTS dbo.DbAttractionDbCategory;
DROP TABLE IF EXISTS dbo.Review;
DROP TABLE IF EXISTS dbo.Comment;
DROP TABLE IF EXISTS dbo.Attraction;
DROP TABLE IF EXISTS dbo.Address;
DROP TABLE IF EXISTS dbo.City;
DROP TABLE IF EXISTS dbo.Country;
DROP TABLE IF EXISTS dbo.[User];
DROP TABLE IF EXISTS dbo.Category;
DROP TABLE IF EXISTS __EFMigrationsHistory;
GO