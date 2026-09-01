using Models;

namespace DbModels;

public sealed class DbAttractions : Attractions, IEquatable<DbAttractions>
{
    public bool Equals(DbAttractions other)
    {
        if (other is null)
            return false;

        return AttractionsId == other.AttractionsId && Name == other.Name;
    }

    public override bool Equals(object obj) => obj is DbAttractions other && Equals(other);
    public override int GetHashCode() => HashCode.Combine(AttractionsId, Name);
}


