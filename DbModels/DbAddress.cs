using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Seido.Utilities.SeedGenerator;
using Models;

namespace DbModels;


public sealed class DbAddress : Address
{
    [Key]
    public override Guid AddressId { get; set; }

    [NotMapped]
    public override ICity City { get => DbCity; set => throw new NotImplementedException(); }
    public DbCity DbCity { get; set; }

    [NotMapped]
    public override ICountry Country { get => DbCountry; set => throw new NotImplementedException(); }
    public DbCountry DbCountry { get; set; }

}