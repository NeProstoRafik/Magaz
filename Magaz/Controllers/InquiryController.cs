using Magaz.DAL.Repository.IRepository;
using Magaz.Models;
using Magaz.Models.ViewModels;
using Magaz.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Magaz.Controllers
{
    [Authorize(Roles=WC.AdminRole)]
    public class InquiryController : Controller
    {
        private readonly IInquiryDetailRepository _detailRep;
        private readonly IInquiryHeaderRepository _headerRep;

        [BindProperty]
        public InquiryVM InquiryVM { get; set; }
        public InquiryController(IInquiryDetailRepository detailRep, IInquiryHeaderRepository headerRep)
        {
            _detailRep = detailRep;
            _headerRep = headerRep;
        }

       
        public IActionResult Index()
        {
            return View();
        }

        //АПИ ВЫЗОВ
        [HttpGet]
        public IActionResult GetInquryList()
        {
            return Json(new { data = _headerRep.GetAll() });

        }
        public IActionResult Details(int id)
        {
            InquiryVM = new InquiryVM()
            {
                InquiryHeader = _headerRep.FirstOrDefault(t => t.id == id),
                InquiryDetail = _detailRep.GetAll(u => u.Id == id, includeProperties: "Product")
            };

            return View(InquiryVM);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Details()
        {
          List<ShopingCart> shopingCartsList=new List<ShopingCart>();
            InquiryVM.InquiryDetail=_detailRep.GetAll(u=>u.InquiryHederId==InquiryVM.InquiryHeader.id);
            foreach (var item in InquiryVM.InquiryDetail)
            {
                ShopingCart shopingCart = new ShopingCart()
                {
                    ProductId = item.ProductId,
                };
                shopingCartsList.Add(shopingCart);
            }
            HttpContext.Session.Clear();
            HttpContext.Session.Set(WC.SessionCart, shopingCartsList);
            HttpContext.Session.Set(WC.SessionInquiryId, InquiryVM.InquiryHeader.id);
            return RedirectToAction("Index", "Cart");
        }
        [HttpPost]
        public IActionResult Delete()
        {
            InquiryHeader inquiryHeader = _headerRep.FirstOrDefault(u=>u.id==InquiryVM.InquiryHeader.id);
            IEnumerable<InquiryDetail> inquiryDetails=_detailRep.GetAll(u=>u.InquiryHederId==InquiryVM.InquiryHeader.id);
            _detailRep.RemoveRange(inquiryDetails);
            _headerRep.Remove(inquiryHeader);
            _headerRep.Save();
            return RedirectToAction(nameof(Index));

        }
    }
}
