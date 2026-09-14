using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

using Seido.Utilities.SeedGenerator;

using Models;

namespace DbModels;

public sealed class DbCity : City, ISeed<DbCity>
{
    [Key]
    public override Guid CityId { get; set; }
    public override string CityName { get; set; }
    [NotMapped]
    public override List<IAttraction> Attractions { get => DbAttractions.ToList<IAttraction>(); set => throw new NotImplementedException(); }
    public List<DbAttraction> DbAttractions { get; set; } = new();
    [NotMapped]
    public override ICountry Country { get => DbCountry; set => throw new NotImplementedException(); }
    [JsonIgnore]
    public DbCountry DbCountry { get; set; }

        public override DbCity Seed(SeedGenerator seedGenerator)
    {
        base.Seed(seedGenerator);
        return this;
    }
}