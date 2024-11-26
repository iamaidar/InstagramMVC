namespace InstagramMVC.Models;

public class Publication
{
    public int Id { get; set; }
    public string ImagePath { get; set; }
    public string Description { get; set; }
    public DateOnly DateOfCreation { get; set; }
    public int UserId { get; set; }
    public MyUser User { get; set; }

    public List<Like> Likes { get; set; }
    public List<Comment> Comments { get; set; }

    public Publication()
    {
        Likes = new List<Like>();
        Comments = new List<Comment>();
    }
}