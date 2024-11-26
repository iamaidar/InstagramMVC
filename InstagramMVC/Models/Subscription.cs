namespace InstagramMVC.Models;

public class Subscription
{
    public int Id { get; set; }
    
    public int SubscriberId { get; set; }
    public MyUser? Subscriber { get; set; }
    
    public int SubscribedToId { get; set; }
    public MyUser? SubscribedTo { get; set; }
}