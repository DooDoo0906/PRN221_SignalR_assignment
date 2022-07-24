using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;
using BusinessObject.Models;
using DataAccess.Repositories;

namespace SignalRAssignment_SE151098.Controllers
{
    public class PostsController : Controller
    {
        private readonly ApplicationDBContext _context;
        private readonly IHubContext<SignalrServer> _signalRHub;
        private readonly IConfiguration Configuration;
        private readonly IPostRepository postRepository;

        public PostsController(ApplicationDBContext context, IPostRepository postRepository, IHubContext<SignalrServer> signalRHub, IConfiguration configuration)
        {
            _context = context;
            _signalRHub = signalRHub;
            Configuration = configuration;
            this.postRepository = postRepository;
        }

        // GET: Posts
        public async Task<IActionResult> Index()
        {
            if (HttpContext.Session.GetInt32("id") == null) { HttpContext.Session.SetString("error", "Please login first to access!"); return Redirect("/"); }

            var context = _context.Posts.Include(p => p.Author).Include(p => p.Category);
            return View(await context.ToListAsync());
        }

        [HttpGet]
        public IActionResult GetPosts()
        {
            if (HttpContext.Session.GetInt32("id") == null) { HttpContext.Session.SetString("error", "Please login first to access!"); return Redirect("/"); }

            var res = _context.Posts.ToList();
            return Ok(res);
        }
        // GET: Posts/Details/5
        public async Task<IActionResult> Details(int id)
        {
            if (HttpContext.Session.GetInt32("id") == null) { HttpContext.Session.SetString("error", "Please login first to access!"); return Redirect("/"); }

            if (id == null)
            {
                return NotFound();
            }
            var post = await postRepository.GetPostById(id);
            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }

        // GET: Posts/Create
        public IActionResult Create()
        {
            if (HttpContext.Session.GetInt32("id") == null) { HttpContext.Session.SetString("error", "Please login first to access!"); return Redirect("/"); }
            ViewData["isAdmin"] = HttpContext.Session.GetString("role").Equals("admin") ? true : false;
            ViewData["AuthorId"] = HttpContext.Session.GetString("role").Equals("admin") ? new SelectList(_context.AppUsers, "UserId", "UserId") : new SelectList(_context.AppUsers, "UserId", "UserId", HttpContext.Session.GetInt32("id"));
            ViewData["CategoryId"] = new SelectList(_context.PostCategories, "CategoryId", "CategoryName");
            return View();
        }

        // POST: Posts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PostId,CreatedDate,UpdatedDate,Title,Content,PublishStatus,AuthorId,CategoryId")] Post post)
        {
            if (HttpContext.Session.GetInt32("id") == null) { HttpContext.Session.SetString("error", "Please login first to access!"); return Redirect("/"); }

            if (ModelState.IsValid)
            {
                var exist = await postRepository.GetPostById(post.PostId);
                if (exist != null)
                {
                    ViewBag.Error = ("Post has been used");
                }
                else
                {
                    await postRepository.InsertPostAsync(post);
                    //_context.Add(appUser);
                    //await _context.SaveChangesAsync();
                    await _signalRHub.Clients.All.SendAsync("LoadAppUsers");
                    ViewData["AuthorId"] = HttpContext.Session.GetString("role").Equals("admin") ? new SelectList(_context.AppUsers, "UserId", "UserId", post.AuthorId) : new SelectList(_context.AppUsers, "UserId", "UserId", HttpContext.Session.GetInt32("id"));
                    ViewData["CategoryId"] = new SelectList(_context.PostCategories, "CategoryId", "CategoryName", post.CategoryId);
                }
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Posts/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            if (HttpContext.Session.GetInt32("id") == null) { HttpContext.Session.SetString("error", "Please login first to access!"); return Redirect("/"); }

            if (id == null)
            {
                return NotFound();
            }
            var post = await postRepository.GetPostById(id);
            if (!HttpContext.Session.GetString("role").Equals("admin") && post.AuthorId != HttpContext.Session.GetInt32("id"))
            {
                HttpContext.Session.SetString("error", "You are not allow to edit someone's post!"); return Redirect("/");
            }
            //var appUser = await _context.AppUsers.FindAsync(id);
            if (post == null)
            {
                return NotFound();
            }
            ViewData["isAdmin"] = HttpContext.Session.GetString("role").Equals("admin") ? true : false;
            ViewData["AuthorId"] = HttpContext.Session.GetString("role").Equals("admin") ? new SelectList(_context.AppUsers, "UserId", "UserId") : new SelectList(_context.AppUsers, "UserId", "UserId", HttpContext.Session.GetInt32("id"));
            ViewData["CategoryId"] = new SelectList(_context.PostCategories, "CategoryId", "CategoryName", post.CategoryId);
            return View(post);
        }

        // POST: Posts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PostId,CreatedDate,UpdatedDate,Title,Content,PublishStatus,AuthorId,CategoryId")] Post post)
        {
            if (HttpContext.Session.GetInt32("id") == null || (HttpContext.Session.GetString("role") != null && !HttpContext.Session.GetString("role").Equals("admin"))) { HttpContext.Session.SetString("error", "Please login first to access!"); return Redirect("/"); }

            if (id != post.PostId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var exist = await postRepository.GetPostById(post.PostId);
                if (exist == null)
                {
                    ViewBag.Error = ("Post  has been deleted");
                }
                else
                {
                    await postRepository.UpdatePostAsync(post);
                    //_context.Update(appUser);
                    //await _context.SaveChangesAsync();
                    await _signalRHub.Clients.All.SendAsync("LoadAppUsers");
                    ViewData["AuthorId"] = new SelectList(_context.AppUsers, "UserId", "UserId", post.AuthorId);
                    ViewData["CategoryId"] = new SelectList(_context.PostCategories, "CategoryId", "CategoryName", post.CategoryId);
                }
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Posts/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            if (HttpContext.Session.GetInt32("id") == null || (HttpContext.Session.GetString("role") != null && !HttpContext.Session.GetString("role").Equals("admin"))) { HttpContext.Session.SetString("error", "Please login first to access!"); return Redirect("/"); }

            if (id == null)
            {
                return NotFound();
            }

            var post = await postRepository.GetPostById(id);
            //var appUser = await _context.AppUsers
            //    .FirstOrDefaultAsync(m => m.UserId == id);
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
            if (HttpContext.Session.GetInt32("id") == null || (HttpContext.Session.GetString("role") != null && !HttpContext.Session.GetString("role").Equals("admin"))) { HttpContext.Session.SetString("error", "Please login first to access!"); return Redirect("/"); }

            var exist = await postRepository.GetPostById(id);
            if (exist == null)
            {
                ViewBag.Error = ("There is no post");
            }
            else
            {
                await postRepository.DeletePostAsync(id);
                //var appUser = await _context.AppUsers.FindAsync(id);
                //_context.AppUsers.Remove(appUser);
                //await _context.SaveChangesAsync();
                await _signalRHub.Clients.All.SendAsync("LoadAppUsers");
            }
            return RedirectToAction(nameof(Index));
        }

        private bool PostExists(int id)
        {
            return _context.Posts.Any(e => e.PostId == id);
        }

    }
}

