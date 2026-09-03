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
    public Guid AttractionId { get; set; }
    public string Name { get; set; }
    [NotMapped]
    public override List<IComment> Comments { get => DbComments.ToList<IComment>(); set => throw new NotImplementedException(); }
    public List<DbComment> DbComments { get; set; } = null;

}


