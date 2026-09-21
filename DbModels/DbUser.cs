using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

using Seido.Utilities.SeedGenerator;

using Models;

namespace DbModels;

public sealed class DbUser : User, ISeed<DbUser>
{
    [Key]
    public override Guid UserId { get; set; }
    public override string UserName { get; set; }
    public override string Email { get; set; }



    [NotMapped]
    public override List<IReview> Reviews { get => DbReviews.ToList<IReview>(); set => throw new NotImplementedException(); }
    [JsonIgnore]
    public List<DbReview> DbReviews { get; set; } = new();

        public override DbUser Seed(SeedGenerator seedGenerator)
    {
        base.Seed(seedGenerator);
        return this;
    }

}