using System.ComponentModel.DataAnnotations;

namespace InstagramMVC.ViewModels;

public class LoginViewModel
{
    [Required(ErrorMessage = "Логин или почта не могут быть пустыми")]
    public string UserNameOrEmail { get; set; }
    
    [Required(ErrorMessage = "Пароль не может быть пустым")]
    [DataType(DataType.Password)]
    public string Password { get; set; }

    public bool RememberMe { get; set; }
    
    public string? ReturnUrl { get; set; }
}