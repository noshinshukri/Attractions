USE `sql-attractions`;

CREATE OR REPLACE VIEW `vwInfoDb` AS
	SELECT
		(SELECT COUNT(*) FROM `User` WHERE `Seeded` = TRUE) AS `NrSeededUsers`,
		(SELECT COUNT(*) FROM `User` WHERE `Seeded` = FALSE) AS `NrUnseededUsers`,
		(SELECT COUNT(*) FROM `Attraction` WHERE `Seeded` = TRUE) AS `NrSeededAttraction`,
		(SELECT COUNT(*) FROM `Attraction` WHERE `Seeded` = FALSE) AS `NrUnseededAttractions`,
		(SELECT COUNT(*) FROM `Address` WHERE `Seeded` = TRUE) AS `NrSeededAddresses`,
		(SELECT COUNT(*) FROM `Address` WHERE `Seeded` = FALSE) AS `NrUnseededAddresses`,
		(SELECT COUNT(*) FROM `City` WHERE `Seeded` = TRUE) AS `NrSeededCities`,
		(SELECT COUNT(*) FROM `City` WHERE `Seeded` = FALSE) AS `NrUnseededCities`,
		(SELECT COUNT(*) FROM `Country` WHERE `Seeded` = TRUE) AS `NrSeededCountries`,
		(SELECT COUNT(*) FROM `Country` WHERE `Seeded` = FALSE) AS `NrUnseededCountries`,
		(SELECT COUNT(*) FROM `Comment` WHERE `Seeded` = TRUE) AS `NrSeededComments`,
		(SELECT COUNT(*) FROM `Comment` WHERE `Seeded` = FALSE) AS `NrUnseededComments`,
		(SELECT COUNT(*) FROM `Review` WHERE `Seeded` = TRUE) AS `NrSeededReviews`,
		(SELECT COUNT(*) FROM `Review` WHERE `Seeded` = FALSE) AS `NrUnseededReviews`,
		(SELECT COUNT(*) FROM `Category` WHERE `Seeded` = TRUE) AS `NrSeededCategories`,
		(SELECT COUNT(*) FROM `Category` WHERE `Seeded` = FALSE) AS `NrUnseededCategories`;

CREATE OR REPLACE VIEW `vwInfoAttractions` AS
	SELECT co.`CountryName` AS `Country`, ci.`CityName` AS `City`, COUNT(a.`AttractionId`) AS `NrAttractions`
	FROM `Attraction` a
	INNER JOIN `Address` ad ON a.`DbAddressAddressId` = ad.`AddressId`
	INNER JOIN `City` ci ON ad.`DbCityCityId` = ci.`CityId`
	INNER JOIN `Country` co ON ci.`CountryId` = co.`CountryId`
	GROUP BY co.`CountryName`, ci.`CityName` WITH ROLLUP;

CREATE OR REPLACE VIEW `vwInfoReviews` AS
	SELECT
		(SELECT COUNT(*) FROM `Review`) AS `NrReviews`,
		(SELECT COUNT(*) FROM `Review` WHERE `CommentId` IS NOT NULL) AS `NrComments`;

CREATE OR REPLACE VIEW `vwInfoCategories` AS
	SELECT c.`Name` AS `Category`, COUNT(jc.`DbAttractionsAttractionId`) AS `NrAttractions`
	FROM `Category` c
	LEFT JOIN `DbAttractionDbCategory` jc ON jc.`DbCategoriesCategoryId` = c.`CategoryId`
	GROUP BY c.`Name`;

DROP PROCEDURE IF EXISTS `spDeleteAll`;
DELIMITER //
CREATE PROCEDURE `spDeleteAll`(
	IN seededParam BOOLEAN,
	OUT nrUsersAffected INT,
	OUT nrAttractionsAffected INT,
	OUT nrAddressesAffected INT,
	OUT nrCitiesAffected INT,
	OUT nrCountriesAffected INT,
	OUT nrCommentsAffected INT,
	OUT nrReviewsAffected INT,
	OUT nrCategoriesAffected INT
)
BEGIN
	DECLARE seededValue BOOLEAN DEFAULT TRUE;
	SET seededValue = COALESCE(seededParam, TRUE);

	SELECT COUNT(*) INTO nrUsersAffected FROM `User` WHERE `Seeded` = seededValue;
	SELECT COUNT(*) INTO nrAttractionsAffected FROM `Attraction` WHERE `Seeded` = seededValue;
	SELECT COUNT(*) INTO nrAddressesAffected FROM `Address` WHERE `Seeded` = seededValue;
	SELECT COUNT(*) INTO nrCitiesAffected FROM `City` WHERE `Seeded` = seededValue;
	SELECT COUNT(*) INTO nrCountriesAffected FROM `Country` WHERE `Seeded` = seededValue;
	SELECT COUNT(*) INTO nrCommentsAffected FROM `Comment` WHERE `Seeded` = seededValue;
	SELECT COUNT(*) INTO nrReviewsAffected FROM `Review` WHERE `Seeded` = seededValue;
	SELECT COUNT(*) INTO nrCategoriesAffected FROM `Category` WHERE `Seeded` = seededValue;

	DELETE jc FROM `DbAttractionDbCategory` jc
	INNER JOIN `Attraction` a ON jc.`DbAttractionsAttractionId` = a.`AttractionId`
	WHERE a.`Seeded` = seededValue;

	DELETE jc FROM `DbAttractionDbCategory` jc
	INNER JOIN `Category` c ON jc.`DbCategoriesCategoryId` = c.`CategoryId`
	WHERE c.`Seeded` = seededValue;

	DELETE FROM `Review` WHERE `Seeded` = seededValue;
	DELETE FROM `Comment` WHERE `Seeded` = seededValue;
	DELETE FROM `Attraction` WHERE `Seeded` = seededValue;
	DELETE FROM `Address` WHERE `Seeded` = seededValue;
	DELETE FROM `City` WHERE `Seeded` = seededValue;
	DELETE FROM `Country` WHERE `Seeded` = seededValue;
	DELETE FROM `User` WHERE `Seeded` = seededValue;
	DELETE FROM `Category` WHERE `Seeded` = seededValue;

	SELECT * FROM `vwInfoDb`;
END//
DELIMITER ;
