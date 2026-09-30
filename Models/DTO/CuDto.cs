using System.IO.Compression;
using System.Text.RegularExpressions;

namespace Models.DTO;

public class UserCuDto
{
    public Guid? UserId { get; set; }
    public string UserName { get; set; }
    public string Email { get; set; }

    public UserCuDto() { }

    public UserCuDto(IUser org)
    {
        UserId = org.UserId;
        UserName = org.UserName;
        Email = org.Email;
    }

}

public class AddressCuDto
{
    public Guid? AddressId { get; set; }

    public Guid CityId { get; set; }
    public Guid CountryId { get; set; }
    public List<Guid> AttractionsId { get; set; } = null;

    public AddressCuDto() { }
    public AddressCuDto(IAddress org)
    {
        AddressId = org.AddressId;
        CityId = org.City.CityId;
        CountryId = org.Country.CountryId;
        AttractionsId = org.Attractions?.Select(a => a.AttractionId).ToList();

    }
}

public class CityCuDto
{
    public Guid? CityId { get; set; }
    public string CityName { get; set; }
    public Guid? CountryId { get; set; } = null;

    public CityCuDto() { }

    public CityCuDto(ICity org)
    {
        CityId = org.CityId;
        CityName = org.CityName;
        CountryId = org.Country?.CountryId;
    }
}

public class CountryCuDto
{
    public Guid? CountryId { get; set; }
    public string CountryName { get; set; }

    public List<Guid> CitiesId { get; set; } = null;

    public CountryCuDto() { }
    public CountryCuDto(ICountry org)
    {
        CountryId = org.CountryId;
        CountryName = org.CountryName;
        CitiesId = org.Cities?.Select(c => c.CityId).ToList();
    }
}

public class AttractionCuDto
{
    public Guid? AttractionId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }

    public Guid? AddressId { get; set; } = null;
    public Guid? CountryId { get; set; }
    public Guid? CityId { get; set; }
    public List<Guid> ReviewsId { get; set; } = null;
    public List<Guid> CategoriesId { get; set; } = null;

    public AttractionCuDto() { }
    public AttractionCuDto(IAttraction org)
    {
        AttractionId = org.AttractionId;
        Name = org.Name;
        Description = org.Description;
        AddressId = org.Address?.AddressId;
        CategoriesId = org.Categories?.Select(c => c.CategoryId).ToList();
    }
}

public class ReviewCuDto
{
    public Guid? ReviewId { get; set; }
    public Guid AttractionId { get; set; }
    public Guid UserId { get; set; }
    public string CommentText { get; set; }
    public ReviewRating Rating { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public ReviewCuDto() { }
    public ReviewCuDto(IReview org)
    {
        ReviewId = org.ReviewId;
        AttractionId = org.Attraction.AttractionId;
        UserId = org.User.UserId;
        CommentText = org.Comment?.CommentText;
        Rating = org.Rating;
        CreatedAt = org.CreatedAt;
    }
}

public class CommentCuDto
{
    public Guid? CommentId { get; set; }
    public string CommentText { get; set; }
    public Guid? ReviewId { get; set; }

    public CommentCuDto() { }

    public CommentCuDto(IComment org)
    {
        CommentId = org.CommentId;
        CommentText = org.CommentText;
        ReviewId = org.Review?.ReviewId;
    }
}

public class Category
{

}