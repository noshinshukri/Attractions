using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

using Seido.Utilities.SeedGenerator;

using Models;

namespace DbModels;

public sealed class DbAttraction : Attraction
{
    [Key]
    public override Guid AttractionId { get; set; }
    public override string Name { get; set; }

    [NotMapped]
    public override List<IReview> Reviews { get => DbReviews.ToList<IReview>(); set => throw new NotImplementedException(); }
    public List<DbReview> DbReviews { get; set; } = new();

    [NotMapped]
    public override List<ICategory> Categories { get => DbCategories.ToList<ICategory>(); set => throw new NotImplementedException(); }
    public List<DbCategory> DbCategories { get; set; } = new();

}


