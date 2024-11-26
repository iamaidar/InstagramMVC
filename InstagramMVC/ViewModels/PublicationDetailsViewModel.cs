using InstagramMVC.Models;

namespace InstagramMVC.ViewModels;

public class PublicationDetailsViewModel
{
    public MyUser CurrentUser { get; set; }

    public Publication Publication { get; set; }
}