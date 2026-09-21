namespace Models;

public interface IComment
{
    public Guid CommentId { get; set; }
    public string CommentText { get; set; }
    public IReview Review { get; set; }
}