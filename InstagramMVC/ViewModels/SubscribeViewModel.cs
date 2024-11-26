using InstagramMVC.Models;

namespace InstagramMVC.ViewModels;

public class SubscribeViewModel
{
    public MyUser User { get; set; }
    public MyUser CurrentUser { get; set; }
    public List<Subscription> Subscriptions { get; set; }
}