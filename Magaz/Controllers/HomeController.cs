using Magaz.Data;
using Magaz.Models;
using Magaz.Models.ViewModels;
using Magaz.Utility;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace Magaz.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly Context _db;
        public HomeController(ILogger<HomeController> logger, Context db = null)
        {
            _logger = logger;
            _db = db;
        }

        public IActionResult Index()
        {
            HomeVM homeVM = new HomeVM()
            {
                Products = _db.Products.Include(u => u.Category).Include(u => u.ApplicationType),
                Categories = _db.Categories
            };
            return View(homeVM);
        }
        public IActionResult Details(int id)
        {

            List<ShopingCart> shopingCartsList = new List<ShopingCart>();
            if (HttpContext.Session.Get<IEnumerable<ShopingCart>>(WC.SessionCart) != null
                && HttpContext.Session.Get<IEnumerable<ShopingCart>>(WC.SessionCart).Count() > 0)
            {
                shopingCartsList = HttpContext.Session.Get<List<ShopingCart>>(WC.SessionCart);
            }
            DetailsVM detailVM = new DetailsVM()
            {
                Product = _db.Products.Include(u => u.Category).Include(u => u.ApplicationType).FirstOrDefault(u => u.Id == id),
                IsExist = false,
            };
            foreach(var item in shopingCartsList)
            {
                if(item.ProductId==id)
                {
                    detailVM.IsExist = true;
                }
            }
            return View(detailVM);
        }
        [HttpPost, ActionName("Details")]
        public IActionResult DetailsPost (int id)
        {
            List<ShopingCart> shopingCartsList= new List<ShopingCart>();
            if (HttpContext.Session.Get<IEnumerable<ShopingCart>>(WC.SessionCart)!=null 
                && HttpContext.Session.Get<IEnumerable<ShopingCart>>(WC.SessionCart).Count()>0)
            {
                shopingCartsList = HttpContext.Session.Get<List<ShopingCart>>(WC.SessionCart);
            }
            shopingCartsList.Add(new ShopingCart { ProductId = id });
            HttpContext.Session.Set(WC.SessionCart, shopingCartsList);

            return RedirectToAction(nameof(Index));
        }
        public IActionResult RemoveFromCart(int id)
        {
            List<ShopingCart> shopingCartsList = new List<ShopingCart>();
            if (HttpContext.Session.Get<IEnumerable<ShopingCart>>(WC.SessionCart) != null
                && HttpContext.Session.Get<IEnumerable<ShopingCart>>(WC.SessionCart).Count() > 0)
            {
                shopingCartsList = HttpContext.Session.Get<List<ShopingCart>>(WC.SessionCart);
            }
            var itemToRemove = shopingCartsList.SingleOrDefault(u => u.ProductId == id);
            if (itemToRemove != null)
            {
                shopingCartsList.Remove(itemToRemove);
            }
            HttpContext.Session.Set(WC.SessionCart, shopingCartsList);

            return RedirectToAction(nameof(Index));
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