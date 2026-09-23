using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

using Seido.Utilities.SeedGenerator;

using Models;
using Models.DTO;

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

    public DbUser() { }

    public DbUser UpdateFromDTO(UserCuDto org)
    {
        UserName = org.UserName;
        Email = org.Email;

        return this;
    }

    public DbUser(UserCuDto org)
    {
        UserId = Guid.NewGuid();
        UpdateFromDTO(org);
    }

}