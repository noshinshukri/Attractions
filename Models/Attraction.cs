using System.ComponentModel.DataAnnotations;

namespace Models;

public class Attraction : IAttraction
{
    public Guid AttractionId { get; set; }
    public string Name { get; set; }

    public virtual List<IComment> Comments { get; set; } = null;

}