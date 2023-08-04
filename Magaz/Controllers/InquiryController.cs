using Magaz.DAL.Repository.IRepository;
using Magaz.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Magaz.Controllers
{
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
    }
}
