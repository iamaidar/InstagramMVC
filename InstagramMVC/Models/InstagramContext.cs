using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace InstagramMVC.Models;

public class InstagramContext : IdentityDbContext<MyUser, IdentityRole<int>, int>
{
    public DbSet<MyUser> Users { get; set; }
    public DbSet<Publication> Publications { get; set; }
    public DbSet<Like> Likes { get; set; }
    public DbSet<Subscription> Subscriptions { get; set; }
    public DbSet<Comment> Comments { get; set; }
    
    public InstagramContext(DbContextOptions<InstagramContext> options) : base(options) {}
}