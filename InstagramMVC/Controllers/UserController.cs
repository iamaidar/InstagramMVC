using System.Security.Claims;
using InstagramMVC.Models;
using InstagramMVC.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InstagramMVC.Controllers;

[Authorize]
public class UserController : Controller
{
    private readonly InstagramContext _context;
    private readonly UserManager<MyUser> _userManager;

    public UserController(InstagramContext context, UserManager<MyUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }
    
    public async Task<IActionResult> Profile(int? id)
    {
        MyUser user = await _userManager.GetUserAsync(User);
        MyUser currentUser = await _userManager.GetUserAsync(User);
        
        if (id != null)
        {
            user = await _userManager.FindByIdAsync(id.ToString());
        }
        
        if (user == null)
        {
            return NotFound();
        }
        
        List<Publication> publications = await _context.Publications.Include(p => p.User).Include(p => p.Likes).Include(p => p.Comments).ThenInclude(c => c.User).Where(p => p.UserId == user.Id).ToListAsync();

        publications.Reverse();
        
        return View(new ProfileViewModel()
        {
            User = user,
            CurrentUser = await _userManager.GetUserAsync(User),
            Subscriptions = await _context.Subscriptions.Where(s => s.SubscriberId == currentUser.Id).ToListAsync(),
            Publications = publications
        });
    }

    [HttpPost]
    public async Task<IActionResult> Search(string search)
    {
        if (string.IsNullOrEmpty(search))
        {
            return View(new List<MyUser>());
        }

        List<MyUser> user = await _context.Users.Where(u => u.UserName.ToLower().Contains(search.ToLower()) || u.Email.ToLower().Contains(search.ToLower()) || u.Name.ToLower().Contains(search.ToLower()) || u.Bio.ToLower().Contains(search.ToLower())).ToListAsync();
        
        return View("Search", user);
    }

    [HttpPost]
    public async Task<IActionResult> FollowUnFollow(string? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        MyUser? user = await _userManager.FindByIdAsync(id.ToString());

        if (user == null)
        {
            return NotFound();
        }
        
        MyUser currentUser = await _userManager.GetUserAsync(User);
        if (user.Id == currentUser.Id)
        {
            return BadRequest("Пользователь не может подписаться сам не себя");
        }

        Subscription? subscription = await _context.Subscriptions.FirstOrDefaultAsync(s => s.SubscriberId == currentUser.Id && s.SubscribedToId == user.Id);
        
        if (subscription != null)
        {
            _context.Subscriptions.Remove(subscription);
            currentUser.CountOfSubscriptions--;
            user.CountOfSubscribers--;
        }
        else
        {
            
            subscription = new Subscription
            {
                SubscriberId = currentUser.Id,
                SubscribedToId = user.Id
            };
            
            currentUser.CountOfSubscriptions++;
            user.CountOfSubscribers++;
            await _context.Subscriptions.AddAsync(subscription);
        }

        await _context.SaveChangesAsync();
        return PartialView("_SubscribePartialView", new SubscribeViewModel
        {
            User = user,
            CurrentUser = currentUser,
            Subscriptions = await _context.Subscriptions.Include(s => s.Subscriber).Include(s => s.SubscribedTo).Where(s => s.SubscriberId == currentUser.Id).ToListAsync()
        });
    }
}