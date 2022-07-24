using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Http;
using BusinessObject.Models;
using DataAccess.Repositories;

namespace SignalRAssignment_SE151098.Controllers
{
    public class PostCategoriesController : Controller
    {
        private readonly IPostCategoryRepository postCategoryRepository;
        private readonly ApplicationDBContext _context;
        private readonly IHubContext<SignalrServer> _signalRHub;
        public PostCategoriesController(ApplicationDBContext context, IHubContext<SignalrServer> signalRHub, IPostCategoryRepository postCategoryRepository)
        {
            _context = context;
            _signalRHub = signalRHub;
            this.postCategoryRepository = postCategoryRepository;
        }

        // GET: PostCategories
        public async Task<IActionResult> Index()
        {
            if (HttpContext.Session.GetInt32("id") == null || (HttpContext.Session.GetString("role") != null && !HttpContext.Session.GetString("role").Equals("admin"))) { HttpContext.Session.SetString("error", "Please login first to access!"); return Redirect("/"); }
            return View(await _context.PostCategories.ToListAsync());
        }

        [HttpGet]
        public IActionResult GetPostCates()
        {
            if (HttpContext.Session.GetInt32("id") == null || (HttpContext.Session.GetString("role") != null && !HttpContext.Session.GetString("role").Equals("admin"))) { HttpContext.Session.SetString("error", "Please login first to access!"); return Redirect("/"); }
            var res = _context.PostCategories.ToList();
            return Ok(res);
        }
        // GET: PostCategories/Details/5
        public async Task<IActionResult> Details(int id)
        {
            if (HttpContext.Session.GetInt32("id") == null || (HttpContext.Session.GetString("role") != null && !HttpContext.Session.GetString("role").Equals("admin"))) { HttpContext.Session.SetString("error", "Please login first to access!"); return Redirect("/"); }

            if (id == null)
            {
                return NotFound();
            }

            var postCategory = await postCategoryRepository.GetPostCategoryById(id);
            if (postCategory == null)
            {
                return NotFound();
            }

            return View(postCategory);
        }

        // GET: PostCategories/Create
        public IActionResult Create()
        {
            if (HttpContext.Session.GetInt32("id") == null || (HttpContext.Session.GetString("role") != null && !HttpContext.Session.GetString("role").Equals("admin"))) { HttpContext.Session.SetString("error", "Please login first to access!"); return Redirect("/"); }

            return View();
        }

        // POST: PostCategories/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CategoryId,CategoryName,Description")] PostCategory postCategory)
        {
            if (HttpContext.Session.GetInt32("id") == null || (HttpContext.Session.GetString("role") != null && !HttpContext.Session.GetString("role").Equals("admin"))) { HttpContext.Session.SetString("error", "Please login first to access!"); return Redirect("/"); }

            if (ModelState.IsValid)
            {
                await postCategoryRepository.InsertPostCategoryAsync(postCategory);
                await _signalRHub.Clients.All.SendAsync("LoadPostCategories");
                return RedirectToAction(nameof(Index));
            }
            return View(postCategory);
        }

        // GET: PostCategories/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            if (HttpContext.Session.GetInt32("id") == null || (HttpContext.Session.GetString("role") != null && !HttpContext.Session.GetString("role").Equals("admin"))) { HttpContext.Session.SetString("error", "Please login first to access!"); return Redirect("/"); }

            if (id == null)
            {
                return NotFound();
            }

            var postCategory = await postCategoryRepository.GetPostCategoryById(id);
            if (postCategory == null)
            {
                return NotFound();
            }
            return View(postCategory);
        }

        // POST: PostCategories/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CategoryId,CategoryName,Description")] PostCategory postCategory)
        {
            if (HttpContext.Session.GetInt32("id") == null || (HttpContext.Session.GetString("role") != null && !HttpContext.Session.GetString("role").Equals("admin"))) { HttpContext.Session.SetString("error", "Please login first to access!"); return Redirect("/"); }

            if (id != postCategory.CategoryId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    
                    await postCategoryRepository.UpdatePostCategoryAsync(postCategory);
                    await _signalRHub.Clients.All.SendAsync("LoadPostCategories");
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PostCategoryExists(postCategory.CategoryId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(postCategory);
        }

        // GET: PostCategories/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            if (HttpContext.Session.GetInt32("id") == null || (HttpContext.Session.GetString("role") != null && !HttpContext.Session.GetString("role").Equals("admin"))) { HttpContext.Session.SetString("error", "Please login first to access!"); return Redirect("/"); }

            if (id == null)
            {
                return NotFound();
            }

            var postCategory = await postCategoryRepository.GetPostCategoryById(id);
            if (postCategory == null)
            {
                return NotFound();
            }

            return View(postCategory);
        }

        // POST: PostCategories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (HttpContext.Session.GetInt32("id") == null || (HttpContext.Session.GetString("role") != null && !HttpContext.Session.GetString("role").Equals("admin"))) { HttpContext.Session.SetString("error", "Please login first to access!"); return Redirect("/"); }

           
            await postCategoryRepository.DeletePostCategoryAsync(id);
            
            await _signalRHub.Clients.All.SendAsync("LoadPostCategories");
            return RedirectToAction(nameof(Index));
        }

        private bool PostCategoryExists(int id)
        {
            return _context.PostCategories.Any(e => e.CategoryId == id);
        }
    }
}
