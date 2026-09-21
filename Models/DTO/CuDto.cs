using System.IO.Compression;
using System.Text.RegularExpressions;

namespace Models.DTO;

public class UserCuDto
{
    public Guid? UserId { get; set; }
    public string UserName { get; set; }
    public string Email { get; set; }

    public List<Guid> ReviewsId { get; set; } = null;

    public UserCuDto() {}

    public UserCuDto(IUser org)
    {
        UserId = org.UserId;
        UserName = org.UserName;
        Email = org.Email;
        
        ReviewsId = org.Reviews?.Select(i => i.ReviewId).ToList();
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

    public List<Guid> ReviewsId { get; set; } = null;
    public List<Guid> CategoriesId { get; set; } = null;

    public AttractionCuDto() { }
    public AttractionCuDto(IAttraction org)
    {
        AttractionId = org.AttractionId;
        Name = org.Name;
        ReviewsId = org.Reviews?.Select(r => r.ReviewId).ToList();
        CategoriesId = org.Categories?.Select(c => c.CategoryId).ToList();
    }
}

public class ReviewCuDto
{

    
}

public class CommentCuDto
{
    
}

public class Category
{
    
}