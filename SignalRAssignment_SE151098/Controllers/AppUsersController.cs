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
    public class AppUsersController : Controller
    {
        private readonly IAppUserRepository appUserRepository;
        private readonly ApplicationDBContext _context;
        private readonly IHubContext<SignalrServer> _signalRHub;
        public AppUsersController(ApplicationDBContext context, IHubContext<SignalrServer> signalRHub, IAppUserRepository appUserRepository)
        {
            _context = context;
            _signalRHub = signalRHub;
            this.appUserRepository = appUserRepository;
        }

        // GET: AppUsers
        public async Task<IActionResult> Index()
        {
            if (HttpContext.Session.GetInt32("id") == null || (HttpContext.Session.GetString("role") != null && !HttpContext.Session.GetString("role").Equals("admin"))) { HttpContext.Session.SetString("error", "Please login first to access!"); return Redirect("/"); }
            return View(await _context.AppUsers.ToListAsync());
        }

        [HttpGet]
        public IActionResult GetAppUsers()
        {
            if (HttpContext.Session.GetInt32("id") == null || (HttpContext.Session.GetString("role") != null && !HttpContext.Session.GetString("role").Equals("admin"))) { HttpContext.Session.SetString("error", "Please login first to access!"); return Redirect("/"); }
            var res = _context.PostCategories.ToListAsync();
            return Ok(res);
        }

        // GET: AppUsers/Details/5
        public async Task<IActionResult> Details(int id)
        {
            if (HttpContext.Session.GetInt32("id") == null || (HttpContext.Session.GetString("role") != null && !HttpContext.Session.GetString("role").Equals("admin"))) { HttpContext.Session.SetString("error", "Please login first to access!"); return Redirect("/"); }

            if (id == null)
            {
                return NotFound();
            }
            var appUser = await appUserRepository.GetUserById(id);
            if (appUser == null)
            {
                return NotFound();
            }

            return View(appUser);
        }

        // GET: AppUsers/Create
        public IActionResult Create()
        {
            if (HttpContext.Session.GetInt32("id") == null || (HttpContext.Session.GetString("role") != null && !HttpContext.Session.GetString("role").Equals("admin"))) { HttpContext.Session.SetString("error", "Please login first to access!"); return Redirect("/"); }

            return View();
        }

        // POST: AppUsers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("UserId,FullName,Address,Password,Email")] AppUser appUser)
        {
            if (HttpContext.Session.GetInt32("id") == null || (HttpContext.Session.GetString("role") != null && !HttpContext.Session.GetString("role").Equals("admin"))) { HttpContext.Session.SetString("error", "Please login first to access!"); return Redirect("/"); }

            if (ModelState.IsValid)
            {
                var exist = await appUserRepository.GetUserById(appUser.UserId);
                if (exist != null)
                {
                    ViewBag.Error = ("Email has been used");
                }
                else
                {
                    await appUserRepository.InsertUserAsync(appUser);
                    //_context.Add(appUser);
                    //await _context.SaveChangesAsync();
                    await _signalRHub.Clients.All.SendAsync("LoadAppUsers");
                }
                
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: AppUsers/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            if (HttpContext.Session.GetInt32("id") == null || (HttpContext.Session.GetString("role") != null && !HttpContext.Session.GetString("role").Equals("admin"))) { HttpContext.Session.SetString("error", "Please login first to access!"); return Redirect("/"); }

            if (id == null)
            {
                return NotFound();
            }
            var appUser = await appUserRepository.GetUserById(id);
            //var appUser = await _context.AppUsers.FindAsync(id);
            if (appUser == null)
            {
                return NotFound();
            }
            return View(appUser);
        }

        // POST: AppUsers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("UserId,FullName,Address,Password,Email")] AppUser appUser)
        {
            if (HttpContext.Session.GetInt32("id") == null || (HttpContext.Session.GetString("role") != null && !HttpContext.Session.GetString("role").Equals("admin"))) { HttpContext.Session.SetString("error", "Please login first to access!"); return Redirect("/"); }

            if (id != appUser.UserId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var exist = await appUserRepository.GetUserById(appUser.UserId);
                if (exist == null)
                {
                    ViewBag.Error = ("User  has been deleted");
                }
                else
                {
                    await appUserRepository.UpdateUserAsync(appUser);
                    //_context.Update(appUser);
                    //await _context.SaveChangesAsync();
                    await _signalRHub.Clients.All.SendAsync("LoadAppUsers");

                }
            }
            return RedirectToAction(nameof(Index));
        }


        // GET: AppUsers/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            if (HttpContext.Session.GetInt32("id") == null || (HttpContext.Session.GetString("role") != null && !HttpContext.Session.GetString("role").Equals("admin"))) { HttpContext.Session.SetString("error", "Please login first to access!"); return Redirect("/"); }

            if (id == null)
            {
                return NotFound();
            }

            var appUser = await appUserRepository.GetUserById(id);
            //var appUser = await _context.AppUsers
            //    .FirstOrDefaultAsync(m => m.UserId == id);
            if (appUser == null)
            {
                return NotFound();
            }

            return View(appUser);
        }

        // POST: AppUsers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (HttpContext.Session.GetInt32("id") == null || (HttpContext.Session.GetString("role") != null && !HttpContext.Session.GetString("role").Equals("admin"))) { HttpContext.Session.SetString("error", "Please login first to access!"); return Redirect("/"); }

            var exist = await appUserRepository.GetUserById(id);
            if (exist == null)
            {
                ViewBag.Error = ("There is no email");
            }
            else
            {
                await appUserRepository.DeleteUserAsync(id);
                //var appUser = await _context.AppUsers.FindAsync(id);
                //_context.AppUsers.Remove(appUser);
                //await _context.SaveChangesAsync();
                await _signalRHub.Clients.All.SendAsync("LoadAppUsers");
            }
            return RedirectToAction(nameof(Index));
        }


    }
}
