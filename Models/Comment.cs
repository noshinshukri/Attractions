using Models;

public class Comment : IComment
{
    public virtual Guid CommentId { get; set; }
    public virtual string CommentText { get; set; }
    public virtual IAttraction Attraction { get; set; }
}