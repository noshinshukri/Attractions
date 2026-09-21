using System.ComponentModel.DataAnnotations;
using Seido.Utilities.SeedGenerator;

namespace Models;

public enum ReviewRating
{
    One = 1,
    Two = 2,
    Three = 3,
    Four = 4,
    Five = 5 
}


public class Review : IReview, ISeed<Review>
{
    public virtual Guid ReviewId { get; set; }
    public virtual IAttraction Attraction { get; set; }
    public virtual IUser User { get; set; }
    public virtual IComment? Comment { get; set; }
    public virtual ReviewRating Rating { get; set; }
    public virtual DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public bool Seeded { get; set; } = false;
    public virtual Review Seed(SeedGenerator seedGenerator)
    {
        ReviewId = Guid.NewGuid();
        Rating = (ReviewRating)seedGenerator.Next(1, 6);
        CreatedAt = DateTime.UtcNow;
        return this;
    }
    
}