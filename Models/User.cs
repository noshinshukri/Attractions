namespace Models;

public class User : IUser
{
    public Guid UserId { get; set; }
    public string UserName { get; set; }
    public string Email { get; set; }
}