using Albayan.Areas.Admin.Data;
using Albayan.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Albayan.Controllers
{
    public class BlogController : Controller
    {
        private readonly PlatformDbContext _context;

        public BlogController(PlatformDbContext context)
        {
            _context = context;
        }

        // GET: /Blog
        public async Task<IActionResult> Index()
        {
            var blogPosts = await _context.BlogPosts
                .OrderByDescending(p => p.PublishDate)
                .Select(p => new BlogPostViewModel
                {
                    Id = p.Id,
                    Title = p.Title,
                    PublishDate = p.PublishDate,
                    ImageUrl = p.ImageUrl,

                    ShortDescription = p.Content.Length > 100 ? p.Content.Substring(0, 100) + "..." : p.Content,
     
                    Keywords = p.Keywords.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList()
                })
                .ToListAsync();

            return View(blogPosts);
        }

        // GET: /Blog/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var blogPost = await _context.BlogPosts.FindAsync(id);

            if (blogPost == null)
            {
                return NotFound();
            }

            return View(blogPost); 
        }
    }
}