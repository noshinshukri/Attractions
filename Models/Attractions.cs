using System.ComponentModel.DataAnnotations;

namespace Models;

public class Attractions : IAttractions, IEquatable<Attractions>
{
    [Key]
    public Guid AttractionsId { get; set; }
    public string Name { get; set; }

    public bool Equals(Attractions other)
    {
        if (other is null)
            return false;

        return AttractionsId == other.AttractionsId && Name == other.Name;
    }

    public override bool Equals(object obj) => obj is Attractions other && Equals(other);
    public override int GetHashCode() => HashCode.Combine(AttractionsId, Name);
}