using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

using Seido.Utilities.SeedGenerator;

using Models;

namespace DbModels;

public sealed class DbAttraction : Attraction, ISeed<DbAttraction>
{
    [Key]
    public override Guid AttractionId { get; set; }
    public override string Name { get; set; }

    [NotMapped]
    public override List<IReview> Reviews { get => DbReviews.ToList<IReview>(); set => throw new NotImplementedException(); }
    [JsonIgnore]
    public List<DbReview> DbReviews { get; set; } = new();

    [NotMapped]
    public override List<ICategory> Categories { get => DbCategories.ToList<ICategory>(); set => throw new NotImplementedException(); }
    [JsonIgnore]
    public List<DbCategory> DbCategories { get; set; } = new();
    
    [NotMapped]
    public override IAddress Address { get => DbAddress; set => throw new NotImplementedException(); }
    [ForeignKey("AdressId")]
    public DbAddress DbAddress { get; set; }

    public override DbAttraction Seed(SeedGenerator seedGenerator)
    {
        base.Seed(seedGenerator);
        return this;
    }

}


