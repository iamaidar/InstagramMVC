using InstagramMVC.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace InstagramMVC.Controllers;

public class ValidationController : Controller
{
    private readonly UserManager<MyUser> _userManager;

    public ValidationController(UserManager<MyUser> userManager)
    {
        _userManager = userManager;
    }

    [AcceptVerbs("GET", "POST")]
    public async Task<IActionResult> CheckUserEmail(string email)
    {
        MyUser user = await _userManager.FindByEmailAsync(email);

        if (user == null)
        {
            return Json(true);
        }

        return Json(false);
    }
    
    [AcceptVerbs("GET", "POST")]
    public async Task<IActionResult> CheckUserName(string username)
    {
        MyUser user = await _userManager.FindByNameAsync(username);

        if (user == null)
        {
            return Json(true);
        }

        return Json(false);
    }
}