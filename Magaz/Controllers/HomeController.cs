using Magaz.DAL.Data;
using Magaz.DAL.Repository.IRepository;
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
        private readonly IProductRepository _prodRep;
        private readonly ICategoryRepository _catRep;
        public HomeController(ILogger<HomeController> logger, IProductRepository prodRep, ICategoryRepository catRep)
        {
            _logger = logger;
         
            _prodRep = prodRep;
            _catRep = catRep;
        }

        public IActionResult Index()
        {
            HomeVM homeVM = new HomeVM()
            {
                Products = _prodRep.GetAll(includeProperties: "Category,ApplicationType"),
                Categories = _catRep.GetAll()
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
                //  Product = _db.Products.Include(u => u.Category).Include(u => u.ApplicationType).FirstOrDefault(u => u.Id == id),
                Product = _prodRep.FirstOrDefault(u=>u.Id==id, includeProperties: "Category,ApplicationType"),
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