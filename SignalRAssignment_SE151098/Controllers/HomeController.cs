using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace SignalRAssignment_SE151098.Controllers

{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private readonly ApplicationDBContext _context;
        private readonly IHubContext<SignalrServer> _signalRHub;

        public HomeController(ILogger<HomeController> logger, ApplicationDBContext context, IHubContext<SignalrServer> signalRHub)
        {
            _logger = logger;
            _context = context;
            _signalRHub = signalRHub;
        }

        public IActionResult Index()
        {
            if (HttpContext.Session.GetInt32("id") != null) return Redirect("/posts");
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Remove("role");
            HttpContext.Session.Remove("id");
            HttpContext.Session.Remove("username");
            HttpContext.Session.Remove("error");
            return RedirectToAction(nameof(Index));
        }
        [BindProperty]
        public AppUser User { get; set; }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult Create()
        {
            return View();
        }

        // POST: AppUsers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("UserId,FullName,Address,Password,Email")] AppUser appUser)
        {
            if (ModelState.IsValid)
            {
                appUser.UserId = _context.AppUsers.Count() + 100;
                _context.Add(appUser);
                await _context.SaveChangesAsync();
                await _signalRHub.Clients.All.SendAsync("LoadAppUsers");
                return RedirectToAction(nameof(Index));
            }
            return View(appUser);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index([Bind("Password,Email")] AppUser appUser)
        {
            var admin = Admin();

            if (admin.Email.Equals(User.Email) && admin.Password.Equals(User.Password))
            {
                HttpContext.Session.SetString("role", "admin");
                HttpContext.Session.SetString("id", User.Email);
                HttpContext.Session.SetString("username", "Admin");
            }
            else
            {
                User = await _context.AppUsers.FirstOrDefaultAsync(m => m.Email == User.Email && m.Password == User.Password);
                if (User == null)
                {
                   
                    return View(appUser);
                }
                HttpContext.Session.SetString("role", value: "user");
                HttpContext.Session.SetInt32("id", User.UserId);
                HttpContext.Session.SetString("username", User.Email);
            }
            return View(appUser);
        }
        public AppUser Admin()
        {
            string fileName = "config.json";
            //Read json file
            string jsonString = System.IO.File.ReadAllText(fileName);
            //Deserialize json file
            AppUser customer = JsonSerializer.Deserialize<AppUser>(jsonString);
            var admin = new AppUser
            {
                Email = customer.Email,
                Password = customer.Password,
            };
            return admin;
        }

    }
}
