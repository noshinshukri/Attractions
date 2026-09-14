using System.ComponentModel.DataAnnotations;
using Seido.Utilities.SeedGenerator;

namespace Models;

public class User : IUser, ISeed<User>
{
    public virtual Guid UserId { get; set; }
    public virtual string UserName { get; set; }
    public virtual string Email { get; set; }

    public virtual List<IReview> Reviews { get; set; } = null;


    public bool Seeded {get; set;} = false;

        public virtual User Seed(SeedGenerator seedGenerator)
    {
        Seeded = true;
        UserId = Guid.NewGuid();

        UserName = seedGenerator.FullName;
        Email = seedGenerator.Email();

        return this;
    }
}