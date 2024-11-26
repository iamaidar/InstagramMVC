namespace InstagramMVC.Models;

public class Comment
{
    public int Id { get; set; }
    public string Text { get; set; }
    
    public int PublicationId { get; set; }
    public Publication? Publication { get; set; }
    
    public int UserId { get; set; }
    public MyUser? User { get; set; }

    public DateOnly DateOfCreation { get; set; } = DateOnly.FromDateTime(DateTime.Now);
}