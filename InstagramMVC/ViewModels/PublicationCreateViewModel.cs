using System.ComponentModel.DataAnnotations;

namespace InstagramMVC.ViewModels;

public class PublicationCreateViewModel
{
    public IFormFile Image { get; set; }
    [StringLength(300, ErrorMessage = "Описание не может быть больше 300 символов")]
    public string Description { get; set; }
}