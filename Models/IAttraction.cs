namespace Models;
public interface IAttraction
{
    
    public Guid AttractionId { get; set; }
    public string Name { get; set; }
    public List<IComment> Comments { get; set; }

}