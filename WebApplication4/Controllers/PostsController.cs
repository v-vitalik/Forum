using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApplication4.Data;
using WebApplication4.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System.Text.RegularExpressions;

namespace WebApplication4.Controllers
{
    public class PostsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public PostsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }


        // GET: Posts
        public async Task<IActionResult> Index(string searchString)
        {
            IEnumerable<Post> posts = await _context.Posts.ToListAsync();
            if (!String.IsNullOrEmpty(searchString))
            {
                if (Regex.IsMatch(searchString, @"\p{IsCyrillic}"))
                {
                    posts = posts.Where(s => s.Title.ConteinsCyrillic(searchString));
                }
                else
                {
                    posts = posts.Where(s => s.Title.Contains(searchString));
                }
            }
            return View(posts);
        }

        // GET: Posts/5
        public async Task<IActionResult> Post(int? id)
        {
            BigModel bm = new BigModel();
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Posts
                .SingleOrDefaultAsync(m => m.PostID == id);
            if (post == null)
            {
                return NotFound();
            }
            post.Comments = await _context.Comments.ToListAsync();
            post.Comments = post.Comments.Where(c => c.PostID == post.PostID).ToList();
            bm.post = post;
            bm.comment = new Comment();

            return View(bm);
        }

        
        
        [HttpPost]
        public async Task<IActionResult> Post(int? id, [Bind("CommentID,Text,PostID,ApplicationUserId,Created")] Comment comment)
        {
            var post = await _context.Posts
               .SingleOrDefaultAsync(m => m.PostID == id);
            post.Comments = await _context.Comments.ToListAsync();
            post.Comments = post.Comments.Where(c => c.PostID == post.PostID).ToList();
            if (!String.IsNullOrEmpty(comment.Text))
            {
                var user = await _userManager.GetUserAsync(HttpContext.User);
                comment.ApplicationUserId = user.Id;
                comment.ApplicationUserName = user.UserName;
                comment.Created = DateTime.Now;
                comment.PostID = post.PostID;
                post.Comments.Add(comment);
                _context.Add(comment);
                await _context.SaveChangesAsync();
                comment.Text = "";
            }
            return RedirectToAction("Post");
        }

        public IActionResult CommentPartial()
        {
            return View();
        }

        // GET: Posts/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Posts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PostID,Title,Text,ApplicationUserId,Created")] Post post)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            post.ApplicationUserId = user.Id;
            if (ModelState.IsValid)
            {
                post.Author = user.UserName;
                post.Created = DateTime.Now;
                _context.Add(post);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(post);
        }

        // GET: Posts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Posts.SingleOrDefaultAsync(m => m.PostID == id);
            if (post == null)
            {
                return NotFound();
            }
            return View(post);
        }

        // POST: Posts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PostID,Title,Text,ApplicationUserID,Created")] Post post)
        {
            if (id != post.PostID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(post);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PostExists(post.PostID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Index");
            }
            return View(post);
        }

        // GET: Posts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Posts
                .SingleOrDefaultAsync(m => m.PostID == id);
            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }

        // POST: Posts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var post = await _context.Posts.SingleOrDefaultAsync(m => m.PostID == id);
            _context.Posts.Remove(post);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        private bool PostExists(int id)
        {
            return _context.Posts.Any(e => e.PostID == id);
        }
    }
}
