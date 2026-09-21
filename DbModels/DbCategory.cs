using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

using Seido.Utilities.SeedGenerator;

using Models;

namespace DbModels;

public sealed class DbCategory : Category, ISeed<DbCategory>
{
    [Key]
    public override Guid CategoryId { get; set; }
    public override string Name { get; set; }

    [NotMapped]
    public override List<IAttraction> Attractions { get => DbAttractions.ToList<IAttraction>(); set => throw new NotImplementedException(); }
    [JsonIgnore]
    public List<DbAttraction> DbAttractions { get; set; } = new();

    public override DbCategory Seed(SeedGenerator seedGenerator)
    {
        base.Seed(seedGenerator);
        return this;
    }
}