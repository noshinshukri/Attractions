namespace Models;

public interface IReview
{
    public Guid ReviewId { get; set; }
    public IAttraction Attraction { get; set; }
    public IUser User { get; set; }
    public IComment? Comment { get; set; }
    public ReviewRating Rating { get; set; }
    public DateTime CreatedAt { get; set; }

}