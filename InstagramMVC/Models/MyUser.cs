using Microsoft.AspNetCore.Identity;

namespace InstagramMVC.Models;

public class MyUser : IdentityUser<int>
{
    public string PathToAvatarPhoto { get; set; }
    
    public string? Name { get; set; }
    
    public string? Bio { get; set; }
    
    public string? Gender { get; set; }

    public int CountOfPublications { get; set; }

    public int CountOfSubscriptions { get; set; }

    public int CountOfSubscribers { get; set; }

    public List<Like> Likes { get; set; }
    public List<Comment> Comments { get; set; }

    public MyUser()
    {
        Likes = new List<Like>();
        Comments = new List<Comment>();
    }
}