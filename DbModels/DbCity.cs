using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

using Seido.Utilities.SeedGenerator;

using Models;

namespace DbModels;

public sealed class DbCity : City
{
    [Key]
    public override Guid CityId { get; set; }
    public override string CityName { get; set; }
    public override List<IAttraction> Attractions { get => DbAttractions.ToList<IAttraction>(); set => throw new NotImplementedException(); }
    public List<DbAttraction> DbAttractions { get; set; } = new();
}