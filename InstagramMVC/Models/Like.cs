namespace InstagramMVC.Models;

public class Like
{
    public int Id { get; set; }

    public int PublicationId { get; set; }
    
    public Publication? Publication { get; set; }

    public int UserId { get; set; }

    public MyUser? User { get; set; }
}