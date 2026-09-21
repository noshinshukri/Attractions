namespace Models;
public interface IAttraction
{
    
    public Guid AttractionId { get; set; }
    public string Name { get; set; }
    public List<IReview> Reviews { get; set; }
    public List<ICategory> Categories { get; set; }
    public IAddress Address { get; set; }

}