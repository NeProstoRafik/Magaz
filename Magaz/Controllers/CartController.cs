﻿using Braintree;
using Magaz.DAL.Data;
using Magaz.DAL.Repository.IRepository;
using Magaz.Models;
using Magaz.Models.ViewModels;
using Magaz.Utility;
using Magaz.Utility.BrainTreeSettings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text;


namespace Magaz.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        private readonly IApplicationUserRepository _appUser;
        private readonly IProductRepository _prodRep;
        private readonly IInquiryDetailRepository _inquiryDetail;
        private readonly IInquiryHeaderRepository _inquiryHeader;

        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IEmailSender _emailSender;

        private readonly IOrderHeaderRepository _orderHRepo;
        private readonly IOrderDetailRepository _orderDRepo;

        private readonly IBrainTreeGate _brain;

        [BindProperty] // чтобы не узакзывать в методах
        public ProductUserVM ProductUserVM { get; set; }
        public CartController(IWebHostEnvironment webHostEnvironment, IEmailSender emailSender, IApplicationUserRepository appUser, IProductRepository prodRep, IInquiryDetailRepository inquiryDetail, IInquiryHeaderRepository inquiryHeader, IOrderDetailRepository orderDRepo, IOrderHeaderRepository orderHRepo, IBrainTreeGate brain)
        {

            _webHostEnvironment = webHostEnvironment;
            _emailSender = emailSender;
            _appUser = appUser;
            _prodRep = prodRep;
            _inquiryDetail = inquiryDetail;
            _inquiryHeader = inquiryHeader;
            _orderDRepo = orderDRepo;
            _orderHRepo = orderHRepo;
            _brain = brain;
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
            IEnumerable<Product> prodListTemp = _prodRep.GetAll(i => prodIdCart.Contains(i.Id));
            IList<Product> prodList=new List<Product>();

            foreach (var item in shops)
            {
                Product prodTemp = prodListTemp.FirstOrDefault(u => u.Id == item.ProductId);
                prodTemp.SqrtM = item.SqrtM;
                prodList.Add(prodTemp);
            }
            return View(prodList);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Index")]
        public IActionResult IndexPost(IEnumerable<Product> prodList)
        {
            List<ShopingCart> shoppingCartList = new List<ShopingCart>();
            foreach (Product prod in prodList)
            {
                shoppingCartList.Add(new ShopingCart { ProductId = prod.Id, SqrtM = prod.SqrtM });
            }
            HttpContext.Session.Set(WC.SessionCart, shoppingCartList);
            return RedirectToAction(nameof(Summary));
        }
        public IActionResult Summary()
        {
           // ApplicationUser applicationUser;
            IdentityUser applicationUser;
            if (User.IsInRole(WC.AdminRole))
            {
                if (HttpContext.Session.Get<int>(WC.SessionInquiryId)!=0)
                {                   
                    // если корзина загрузила данные
                    InquiryHeader inquiryHeader = _inquiryHeader.FirstOrDefault(u => u.id == HttpContext.Session.Get<int>(WC.SessionInquiryId));
                    applicationUser = new ApplicationUser()
                    {
                        Email = inquiryHeader.Email,
                        UserName = inquiryHeader.FullName,
                        PhoneNumber = inquiryHeader.PhoneNumber,
                    };
                }
                else
                {
                   // applicationUser = new ApplicationUser() { };
                    applicationUser = new IdentityUser() { };
                }
                var getway = _brain.GetGateway();
                var clientToken = getway.ClientToken.Generate();
                ViewBag.ClientToken = clientToken;
            }
            else
            {
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
                applicationUser = _appUser.FirstOrDefault(u => u.Id == claim.Value);
            }
         
          // var userId = User.FindFirstValue(ClaimTypes.Name);  //2й способ как достать юзера который зареган
            
            List<ShopingCart> shops = new List<ShopingCart>();
            if (HttpContext.Session.Get<IEnumerable<ShopingCart>>(WC.SessionCart) != null
                    && HttpContext.Session.Get<IEnumerable<ShopingCart>>(WC.SessionCart).Count() > 0)
            {
                shops = HttpContext.Session.Get<List<ShopingCart>>(WC.SessionCart); //товары в корзине
            }
            List<int> prodIdCart = shops.Select(i => i.ProductId).ToList();
            IEnumerable<Product> prodList = _prodRep.GetAll(i => prodIdCart.Contains(i.Id));

            ProductUserVM = new ProductUserVM()
            {
              // ApplicationUser = _db.ApplicationUsers.FirstOrDefault(u => u.UserName == userId),
               ApplicationUser =applicationUser,
                  
                    
            };
            foreach (var item in shops)
            {
                Product prodTemp = _prodRep.FirstOrDefault(u => u.Id == item.ProductId);
                prodTemp.SqrtM = item.SqrtM;
                ProductUserVM.ProductList.Add(prodTemp);
            }
           
            return View(ProductUserVM);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Summary")]
        public async Task<IActionResult> SummaryPost(IFormCollection collection, ProductUserVM productUserVM)
        {
            var clamsIdentity = (ClaimsIdentity)User.Identity;
            var claim = clamsIdentity.FindFirst(ClaimTypes.NameIdentifier); // получаем пользователя

            if (User.IsInRole(WC.AdminRole))
            {
                //создаем заказ
                OrderHeader orderHeader = new OrderHeader()
                {
                    CreatedByUserId = claim.Value,
                    FinalOrderTotal = ProductUserVM.ProductList.Sum(x => x.SqrtM * x.Price),
                    
                    StreetAddress = ProductUserVM.ApplicationUser.PhoneNumber,
                    
                    Email = ProductUserVM.ApplicationUser.Email,
                    PhoneNumber = ProductUserVM.ApplicationUser.PhoneNumber,
                    OrderDate = DateTime.Now,
                    OrderStatus = WC.StatusPending
                };
                _orderHRepo.Add(orderHeader);
                _orderHRepo.Save();

                foreach (var prod in ProductUserVM.ProductList)
                {
                    OrderDetail orderDetail = new OrderDetail()
                    {
                        OrderHeaderId = orderHeader.Id,
                        PricePerSqFt = prod.Price,
                        Sqft = prod.SqrtM,
                        ProductId = prod.Id
                    };
                    _orderDRepo.Add(orderDetail);

                }
                _orderDRepo.Save();

                //braintree настройки с сайта
                string nonceFromTheClient = collection["payment_method_nonce"];

                var request = new TransactionRequest
                {
                    Amount = Convert.ToDecimal(orderHeader.FinalOrderTotal),
                    PaymentMethodNonce = nonceFromTheClient,
                    OrderId = orderHeader.Id.ToString(),
                    Options = new TransactionOptionsRequest
                    {
                        SubmitForSettlement = true
                    }
                };

                var gateway = _brain.GetGateway();
                Result<Transaction> result = gateway.Transaction.Sale(request);

                if (result.Target.ProcessorResponseText == "Approved")
                {
                    orderHeader.TransactionId = result.Target.Id;
                    orderHeader.OrderStatus = WC.StatusApproved;
                }
                else
                {
                    orderHeader.OrderStatus = WC.StatusCancelled;
                }
                _orderHRepo.Save();
                return RedirectToAction(nameof(InquiryConfirmation), new { id = orderHeader.Id });

            }
            else
            {

            
            var pathToTeamplate= _webHostEnvironment.WebRootPath + Path.DirectorySeparatorChar.ToString()+
              "Templates" + Path.DirectorySeparatorChar.ToString() + "Inquiry.html";
            var subject = "New Inquiry";
            var htmlBody = "";
            using (StreamReader sr=System.IO.File.OpenText(pathToTeamplate))
            {
                htmlBody=sr.ReadToEnd();
            }

            StringBuilder productListSB= new StringBuilder();
            foreach (var item in productUserVM.ProductList)
            {
                productListSB.Append($" - Name: {item.Name} <span style='font-size:14px;'> (ID: {item.Id})</span><br />");
            }
            string messageBody = string.Format(htmlBody,
                 ProductUserVM.ApplicationUser.UserName,
                ProductUserVM.ApplicationUser.Email,
                ProductUserVM.ApplicationUser.PhoneNumber,
                productListSB.ToString());

            await _emailSender.SendEmailAsync( WC.AdminEmail, subject, messageBody); // отправляем сообщение

            //отправляем в БД заголовки из письма емейла
            InquiryHeader inquiryHeader = new InquiryHeader()
            {
                ApllicationUserId = claim.Value,
                Email=ProductUserVM.ApplicationUser.Email,
                PhoneNumber=productUserVM.ApplicationUser.PhoneNumber,
                InquiryDate=DateTime.Now,
            };
            _inquiryHeader.Add(inquiryHeader);
            _inquiryHeader.Save();

            foreach (var item in productUserVM.ProductList)
            {
                InquiryDetail inquiryDetail = new InquiryDetail()
                {
                    InquiryHederId = inquiryHeader.id,
                    ProductId = item.Id
                };
                _inquiryDetail.Add(inquiryDetail);
              
            }
                _inquiryDetail.Save();
            }
           
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
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateCart(IEnumerable<Product> prodList)
        {
            List<ShopingCart> shoppingCartList= new List<ShopingCart>();
            foreach (Product prod in prodList)
            {
                shoppingCartList.Add(new ShopingCart { ProductId = prod.Id, SqrtM = prod.SqrtM });
            }
            HttpContext.Session.Set(WC.SessionCart, shoppingCartList);
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Clear()
        {

            HttpContext.Session.Clear();


            return RedirectToAction("Index", "Home");
        }
    }
}
