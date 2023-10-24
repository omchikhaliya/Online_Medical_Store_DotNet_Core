using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MM_demo_core.Data;
using MM_demo_core.Models;
using System.Diagnostics;
using System.Dynamic;

namespace MM_demo_core.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext db;

       
        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            db = context;
        }
       

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult medico()
        {
            return View();
        }
        public IActionResult drug()
        {
            return View();
        }
        public IActionResult search(string search)
        {
            // return View(db.drugs.Where(x => x.Ename.Contains(search)).ToList());
            var dg = db.drugs.Where(x => x.Ename.Contains(search)).ToList();
            dynamic y1 = new ExpandoObject();
            y1.check = dg;
            return View(y1);

        }

        public IActionResult details(string name)
        {
            var dg = db.drugs.Where(x => x.Ename.Contains(name)).ToList();
            dynamic y1 = new ExpandoObject();
            y1.check = dg;
            return View(y1);
        }

        [Authorize]
        public IActionResult cart()
        {
            string email = User.Identity?.Name;
            //string email = Session["Email"].ToString();

            var na = db.carts.Where(x => x.Cemail == email).ToList();
            return View(na);
            //return View();
        }

        public IActionResult addtocart(int id)         
        {
            string email = User.Identity?.Name;
            drug dg = db.drugs.Where(s => s.EId == id).FirstOrDefault();
            cart alreadyInCart = db.carts.Where(c => c.Cemail == email && c.name == dg.Ename).FirstOrDefault();
            if (alreadyInCart == null)
            {
                
                cart c = new cart();
                c.Cemail = email;
                //c.Cid = id;
                c.name = dg.Ename;
                c.price = Convert.ToInt32(dg.Eprize);
                c.image = dg.ImageUrl3;
                //  c.qty = Convert.ToInt32(qty);
                c.qty = 1;
                c.bil = c.price;

                db.carts.Add(c);
                db.SaveChanges();
            }
            else
            {
                TempData["msg1"] = "You Are Not Logged In The System";

            }


            return RedirectToAction("cart");
        }

        [HttpPost]
        public IActionResult UpdateQuantity(int cid, int quantity)
        {
            var cartItem = db.carts.Find(cid);

            if (cartItem == null)
            {
                return NotFound(); // Handle item not found error
            }

            cartItem.qty = quantity;
            cartItem.bil = cartItem.qty * cartItem.price; // Update subtotal

            db.SaveChanges(); // Save changes to the database

            return Ok(); // Return success status
        }

        public ActionResult Delete(int id)
        {

            var del = db.carts.Find(id);
            db.carts.Remove(del);
            db.SaveChanges();
            return RedirectToAction("cart");
        }

        [HttpPost]
        public IActionResult EmptyCart(string userEmail)
        {
            // Get cart items for the specified user email and remove them from the database
            var cartItems = db.carts.Where(c => c.Cemail == userEmail).ToList();
            db.carts.RemoveRange(cartItems);
            db.SaveChanges();

            return Ok(); // Return success status
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}