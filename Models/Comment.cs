using Seido.Utilities.SeedGenerator;
using Models;

public class Comment : IComment, ISeed<Comment>
{
    public virtual Guid CommentId { get; set; }
    public virtual string CommentText { get; set; }
    public virtual IReview Review { get; set; }

    public bool Seeded {get; set;} = false;

    public virtual Comment Seed(SeedGenerator seedGenerator)
    {
        Seeded = true;
        CommentId = Guid.NewGuid();

        CommentText = seedGenerator.LatinSentence;

        return this;
    }
}