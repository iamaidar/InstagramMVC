using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace InstagramMVC.ViewModels;

public class RegisterViewModel
{
    [Required(ErrorMessage = "Логин не может быть пустым")]
    [Remote(action: "CheckUserName", controller: "Validation", ErrorMessage = "Пользователь с таким именем пользователя уже существует")]
    public string UserName { get; set; }

    [Required(ErrorMessage = "Почта не может быть пустой")]
    [EmailAddress(ErrorMessage = "Почта в некорректном формате")]
    [Remote(action: "CheckUserEmail", controller: "Validation", ErrorMessage = "Пользователь с такой почтой уже существует")]

    public string Email { get; set; }
    
    [Required(ErrorMessage = "Аватар пользователя не может быть пустым")]
    public IFormFile Avatar { get; set; }

    [Required(ErrorMessage = "Пароль не может быть пустым")]
    [DataType(DataType.Password)]
    public string Password { get; set; }
    
    [Required(ErrorMessage = "Повтор пароля не может быть пустым")]
    [Compare("Password", ErrorMessage = "Пароли не совпадают")]
    [DataType(DataType.Password)]
    public string ConfirmPassword { get; set; }
    
    [StringLength(20, ErrorMessage = "Максимальная длина имени 20 символов")]
    public string? Name { get; set; }
    
    [MaxLength(150, ErrorMessage = "Длина информации о пользователе не может превышать 150 символов")]
    public string? Bio { get; set; }
    
    public string? PhoneNumber { get; set; }
    
    public string? Gender { get; set; }
}