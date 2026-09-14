using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

using Seido.Utilities.SeedGenerator;

using Models;

namespace DbModels;

public sealed class DbCountry : Country, ISeed<DbCountry>
{
    [Key]
    public override Guid CountryId { get; set; }
    public override string CountryName { get; set; }
    [NotMapped]
    public override List<ICity> Cities { get => DbCities.ToList<ICity>(); set => throw new NotImplementedException(); }
    [JsonIgnore]
    public List<DbCity> DbCities { get; set; } = new();


    public override DbCountry Seed(SeedGenerator seedGenerator)
    {
        base.Seed(seedGenerator);
        return this;
    }
}