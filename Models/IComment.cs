namespace Models;

public interface IComment
{
    public string CommentText { get; set; }
    public IAttraction Attraction { get; set; }
}