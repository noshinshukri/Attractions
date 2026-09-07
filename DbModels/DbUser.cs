using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

using Seido.Utilities.SeedGenerator;

using Models;

namespace DbModels;

public sealed class DbUser : User
{
    [Key]
    public override Guid UserId { get; set; }
    public override string UserName { get; set; }
    public override string Email { get; set; }



    [NotMapped]
    public override List<IReview> Reviews { get => new List<IReview> { DbReview }; set => throw new NotImplementedException(); }
    public DbReview DbReview { get; set; } = null;

}