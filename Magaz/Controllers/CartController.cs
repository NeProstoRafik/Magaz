using Magaz.Data;
using Magaz.Models;
using Magaz.Models.ViewModels;
using Magaz.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Magaz.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        private readonly Context _db;
        [BindProperty] // чтобы не узакзывать в методах
        public ProductUserVM ProductUserVM { get; set; }
        public CartController(Context db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            List<ShopingCart> shops = new List<ShopingCart>();
            if (HttpContext.Session.Get<IEnumerable<ShopingCart>>(WC.SessionCart)!=null 
                    && HttpContext.Session.Get<IEnumerable<ShopingCart>>(WC.SessionCart).Count()>0)
            {
                shops= HttpContext.Session.Get<List<ShopingCart>>(WC.SessionCart);
            }
            List<int> prodIdCart=shops.Select(i=>i.ProductId).ToList();
            IEnumerable<Product> prodList = _db.Products.Where(i => prodIdCart.Contains(i.Id));
            return View(prodList);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Index")]
        public IActionResult IndexPost()
        {
            return RedirectToAction(nameof(Summary));
        }
        public IActionResult Summary()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
          // var userId = User.FindFirstValue(ClaimTypes.Name);  //2й способ как достать юзера который зареган
            
            List<ShopingCart> shops = new List<ShopingCart>();
            if (HttpContext.Session.Get<IEnumerable<ShopingCart>>(WC.SessionCart) != null
                    && HttpContext.Session.Get<IEnumerable<ShopingCart>>(WC.SessionCart).Count() > 0)
            {
                shops = HttpContext.Session.Get<List<ShopingCart>>(WC.SessionCart);
            }
            List<int> prodIdCart = shops.Select(i => i.ProductId).ToList();
            IEnumerable<Product> prodList = _db.Products.Where(i => prodIdCart.Contains(i.Id));

            ProductUserVM = new ProductUserVM()
            {
              // ApplicationUser = _db.ApplicationUsers.FirstOrDefault(u => u.UserName == userId),
               ApplicationUser =_db.ApplicationUsers.FirstOrDefault(u=>u.Id==claim.Value),
                    ProductList= prodList,
                    
            };
            return View(ProductUserVM);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Summary")]
        public IActionResult SummaryPost(ProductUserVM productUserVM)
        {

            return RedirectToAction(nameof(InquiryConfirmation));
        }
        public IActionResult InquiryConfirmation()
        {
            HttpContext.Session.Clear();
            return View();
        }
        public IActionResult Remove(int id)
        {
            List<ShopingCart> shops = new List<ShopingCart>();
            if (HttpContext.Session.Get<IEnumerable<ShopingCart>>(WC.SessionCart) != null
                    && HttpContext.Session.Get<IEnumerable<ShopingCart>>(WC.SessionCart).Count() > 0)
            {
                shops = HttpContext.Session.Get<List<ShopingCart>>(WC.SessionCart);
            }
            shops.Remove(shops.FirstOrDefault(u => u.ProductId == id));
            HttpContext.Session.Set(WC.SessionCart, shops);

            
            return RedirectToAction(nameof(Index));
        }
    }
}
