using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

using Seido.Utilities.SeedGenerator;

using Models;
using Models.DTO;

namespace DbModels;

public sealed class DbComment : Comment, ISeed<DbComment>
{
    [Key]
    public override Guid CommentId { get; set; }
    public override string CommentText { get; set; }
    [NotMapped]
    public override IReview Review { get => DbReview; set => throw new NotImplementedException(); }
    [JsonIgnore]
    public DbReview DbReview { get; set; }

    public override DbComment Seed(SeedGenerator seedGenerator)
    {
        base.Seed(seedGenerator);
        return this;
    }

    public DbComment() { }

    public DbComment UpdateFromDTO(CommentCuDto org)
    {
        CommentText = org.CommentText;
        return this;
    }

    public DbComment(CommentCuDto org)
    {
        CommentId = Guid.NewGuid();
        UpdateFromDTO(org);
    }

}