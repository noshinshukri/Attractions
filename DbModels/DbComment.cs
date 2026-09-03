using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

using Seido.Utilities.SeedGenerator;

using Models;

namespace DbModels;

public sealed class DbComment : Comment
{
    [Key]
    public Guid CommentsId { get; set; }
    public string CommentText { get; set; }
    [NotMapped]
    public override IAttraction Attraction { get => DbAttraction; set => throw new NotImplementedException(); }
    public DbAttraction DbAttraction { get; set; } = null;

}