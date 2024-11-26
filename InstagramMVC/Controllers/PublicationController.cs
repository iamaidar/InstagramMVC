using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using InstagramMVC.Models;
using InstagramMVC.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace InstagramMVC.Controllers
{
    [Authorize]
    public class PublicationController : Controller
    {
        private readonly InstagramContext _context;
        private readonly UserManager<MyUser> _userManager;
        private readonly IWebHostEnvironment _environment;

        public PublicationController(InstagramContext context, UserManager<MyUser> userManager, IWebHostEnvironment environment)
        {
            _context = context;
            _userManager = userManager;
            _environment = environment;
        }
        
        public async Task<IActionResult> Index()
        {
            MyUser currentUser = await _userManager.GetUserAsync(User);

            IQueryable<int> currentUserSubscribedUsers = _context.Subscriptions.Include(s => s.SubscribedTo).Where(s => s.SubscriberId == currentUser.Id).Select(s => s.SubscribedToId);
            IQueryable<Publication> subscribedsPublications = _context.Publications.Where(p => currentUserSubscribedUsers.Contains(p.UserId) || p.UserId == currentUser.Id);
            
            List<Publication> publications = await subscribedsPublications
                .Include(p => p.Comments)
                .ThenInclude(c => c.User)
                .Include(p => p.User)
                .Include(p => p.Likes)
                .OrderBy(p => p.DateOfCreation)
                .ToListAsync();

            publications.Reverse();
            
            return View(new IndexViewModel
            {
                CurrentUser = currentUser,
                Publications = publications
            });
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PublicationCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                MyUser user = await _userManager.GetUserAsync(User);
                if (model.Image != null && model.Image.Length > 0 && model.Image.ContentType.StartsWith("image/"))
                {
                    string fileName = $"publication_{user.Email}_{Guid.NewGuid().ToString()}{Path.GetExtension(model.Image.FileName)}";
                    string filePath = Path.Combine(_environment.WebRootPath, "publications", fileName);
                    Directory.CreateDirectory(Path.GetDirectoryName(filePath));
                    using (FileStream stream = new FileStream(filePath, FileMode.Create))
                    {
                        await model.Image.CopyToAsync(stream);
                    }
                    Publication publication = new Publication()
                    {
                        ImagePath = $"/publications/{fileName}",
                        Description = model.Description,
                        UserId = user.Id,
                        DateOfCreation = DateOnly.FromDateTime(DateTime.Now)
                    };
                    _context.Publications.Add(publication);
                    user.CountOfPublications++;
                    _context.Users.Update(user);
                    await _context.SaveChangesAsync();
                    
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ModelState.AddModelError("Image", "Можно загружать только изображения.");
                }
                
            }
            
            string? returnUrl = Request.Headers["Referer"].ToString();

            if (!string.IsNullOrEmpty(returnUrl))
            {
                return Redirect(returnUrl);
            }
            
            return RedirectToAction("Index");
        }
        
        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            Publication? publication = await _context.Publications.FirstOrDefaultAsync(p => p.Id.ToString() == id);
            MyUser currentUser = await _userManager.GetUserAsync(User);
            
            if (publication != null)
            {
                if (currentUser.Id != publication.UserId)
                {
                    return BadRequest();
                }
                
                _context.Publications.Remove(publication);
                currentUser.CountOfPublications--;
            }
            else
            {
                return NotFound();
            }

            await _context.SaveChangesAsync();
            
            
            
            return PartialView("/Views/User/_ProfilePublicationsPartialView.cshtml", new ProfileViewModel
            {
                User = currentUser,
                CurrentUser = currentUser,
                Publications = await _context.Publications.Include(p => p.User).Include(p => p.Likes).Include(p => p.Comments).Where(p => p.UserId == currentUser.Id).ToListAsync(),
                Subscriptions = await _context.Subscriptions.Where(s => s.SubscriberId == currentUser.Id).ToListAsync()
            });
        }
        
        [HttpPost]
        public async Task<IActionResult> Edit(string id, string newDescription)
        {
            Publication? publication = await _context.Publications.FirstOrDefaultAsync(p => p.Id.ToString() == id);
            MyUser currentUser = await _userManager.GetUserAsync(User);
            
            if (publication != null)
            {
                if (currentUser.Id != publication.UserId)
                {
                    return BadRequest();
                }

                publication.Description = newDescription;
                _context.Publications.Update(publication);
            }
            else
            {
                return NotFound();
            }

            await _context.SaveChangesAsync();
            
            return PartialView("_DetailsPartialView", new PublicationDetailsViewModel
            {
                CurrentUser = currentUser,
                Publication = publication
            });
        }

        [HttpPost]
        public async Task<IActionResult> LikeUnlike(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            
            Publication? publication = await _context.Publications.Include(p => p.User).Include(p => p.Likes).FirstOrDefaultAsync(p => p.Id.ToString() == id);

            if (publication == null)
            {
                return NotFound();
            }

            MyUser user = await _userManager.GetUserAsync(User);
            
            Like? like = await _context.Likes.FirstOrDefaultAsync(l => l.UserId == user.Id && l.PublicationId == publication.Id);

            if (like == null)
            {
                like = new Like
                {
                    PublicationId = publication.Id,
                    UserId = user.Id
                };

                await _context.Likes.AddAsync(like);
            }
            else
            {
                _context.Likes.Remove(like);
            }
            
            await _context.SaveChangesAsync();
            
            return PartialView("_LikeFormPartialView", new PublicationDetailsViewModel
            {
                CurrentUser = user,
                Publication = publication
            });
        }

        [HttpPost]
        public async Task<IActionResult> Comment(int publicationId, string commentText)
        {
            string? returnUrl = Request.Headers["Referer"].ToString();
            
            if (string.IsNullOrEmpty(commentText))
            {
                ModelState.AddModelError("CommentText", "Комментарий не может быть пустым");

                if (!string.IsNullOrEmpty(returnUrl))
                {
                    return Redirect(returnUrl);
                }
                
                return RedirectToAction("Index");
            }
            
            Publication? publication = await _context.Publications.Include(p => p.User).Include(p => p.Comments).FirstOrDefaultAsync(p => p.Id == publicationId);
            
            if (publication == null)
            {
                return NotFound();
            }
            
            MyUser user = await _userManager.GetUserAsync(User);
            Comment comment = new Comment()
            {
                Text = commentText,
                PublicationId = publicationId,
                UserId = user.Id,
                DateOfCreation = DateOnly.FromDateTime(DateTime.Now)
            };

            await _context.Comments.AddAsync(comment);
            await _context.SaveChangesAsync();
            
            returnUrl = Request.Headers["Referer"].ToString();

            if (!string.IsNullOrEmpty(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction("Index");
        }

        private bool PublicationExists(int id)
        {
            return _context.Publications.Any(e => e.Id == id);
        }
    }
}
