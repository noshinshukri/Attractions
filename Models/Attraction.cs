using System.ComponentModel.DataAnnotations;
using Seido.Utilities.SeedGenerator;

namespace Models;

public class Attraction : IAttraction, ISeed<Attraction>
{
    public virtual Guid AttractionId { get; set; }
    public virtual string Name { get; set; }

    public virtual List<IReview> Reviews { get; set; } = null;
    public virtual List<ICategory> Categories { get; set; } = null;
    public virtual IAddress Address { get; set; } 

    public bool Seeded { get; set; } = false;
    public virtual Attraction Seed(SeedGenerator seedGenerator)
    {
        Seeded = true;
        AttractionId = Guid.NewGuid();
        Name = seedGenerator.MusicAlbumName;

        return this;
    }

}