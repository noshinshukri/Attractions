using System.ComponentModel.DataAnnotations;

namespace Models;

public class Attraction : IAttraction
{
    public virtual Guid AttractionId { get; set; }
    public virtual string Name { get; set; }

    public virtual List<IReview> Reviews { get; set; } = null;

}