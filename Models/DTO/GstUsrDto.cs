namespace Models.DTO;

public class GstUsrInfoDbDto
{
    public int NrSeededAttraction { get; set; } = 0;
    public int NrUnseededAttractions { get; set; } = 0;
    public int NrAttractionsWithAddress { get; set; } = 0;

    public int NrSeededCities { get; set; } = 0;
    public int NrUnseededCities { get; set; } = 0;

    public int NrSeededCountries { get; set; } = 0;
    public int NrUnseededCountries { get; set; } = 0;

    public int NrSeededUsers { get; set; } = 0;
    public int NrUnseededUsers { get; set; } = 0;

    public int NrSeededComments { get; set; } = 0;
    public int NrUnseededComments { get; set; } = 0;

    public int NrSeededReviews { get; set; } = 0;
    public int NrUnseededReviews { get; set; } = 0;
}

public class GstUsrInfoAttractionsDto
{
    public string Country { get; set; } = null;
    public string City { get; set; } = null;
    public int NrAttractions { get; set; } = 0;
}

public class GstUsrInfoReviewsDto
{
    public int NrComments { get; set; } = 0;
    public int NrReviews { get; set; } = 0;
}


public class GstUsrInfoAllDto
{
    public GstUsrInfoDbDto Db { get; set; } = null;
    public List<GstUsrInfoReviewsDto> Reviews { get; set; } = null;
    public List<GstUsrInfoAttractionsDto> Attractions { get; set; } = null;
}


