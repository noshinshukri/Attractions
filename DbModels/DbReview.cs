using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

using Seido.Utilities.SeedGenerator;

using Models;

namespace DbModels;

public sealed class DbReview : Review
{
    [Key]
    public override Guid ReviewId { get; set; }



    [NotMapped]
    public override IAttraction Attraction { get => DbAttraction; set => throw new NotImplementedException(); }
    [ForeignKey("AttractionId")]
    public DbAttraction DbAttraction { get; set; }


    [NotMapped]
    public override IUser User { get => DbUser; set => throw new NotImplementedException(); }
    public DbUser DbUser { get; set; }


    [NotMapped]
    public override IComment Comment { get => DbComment; set => throw new NotImplementedException(); }
    [ForeignKey("CommentId")]
    public DbComment DbComment { get; set; }


    public override ReviewRating Rating { get; set; }
    public override DateTime CreatedAt { get; set; }
}
