using Seido.Utilities.SeedGenerator;

namespace Models;

public class Category : ICategory, ISeed<Category>
{
    public virtual Guid CategoryId { get; set; }
    public virtual string Name { get; set; }

    public virtual List<IAttraction> Attractions { get; set; } = new();

    public bool Seeded { get; set; } = false;
    public virtual Category Seed(SeedGenerator seedGenerator)
    {
        Seeded = true;
        CategoryId = Guid.NewGuid();
        Name = seedGenerator.LatinWords(1).FirstOrDefault();

        return this;
    }
}