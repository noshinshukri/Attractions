using Models;

public class Comment : IComment
{
    public Guid CommentId { get; set; }
    public string CommentText { get; set; }
    public virtual IAttraction Attraction { get; set; }
}