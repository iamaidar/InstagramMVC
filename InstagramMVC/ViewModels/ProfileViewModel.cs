using InstagramMVC.Models;

namespace InstagramMVC.ViewModels;

public class ProfileViewModel
{
    public MyUser User { get; set; }
    
    public MyUser CurrentUser { get; set; }
    public List<Subscription> Subscriptions { get; set; }
    public List<Publication> Publications { get; set; }
}