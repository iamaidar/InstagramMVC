using InstagramMVC.Models;

namespace InstagramMVC.ViewModels;

public class IndexViewModel
{
    public MyUser CurrentUser { get; set; }

    public List<Publication> Publications { get; set; }
}