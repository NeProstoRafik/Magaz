using Magaz.DAL.Repository.IRepository;
using Magaz.Models;
using Magaz.Models.ViewModels;
using Magaz.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Magaz.Controllers
{
    [Authorize(Roles = WC.AdminRole)]
    public class ProductController : Controller
    {
        private readonly IProductRepository _Db;
        private readonly IWebHostEnvironment webHostEnvironment;
        public ProductController(IProductRepository db, IWebHostEnvironment webHostEnvironment)
        {
            _Db = db;
            this.webHostEnvironment = webHostEnvironment;
        }
        public IActionResult Index()
        {
            IEnumerable<Product> products = _Db.GetAll(includeProperties: "Category,ApplicationType");
            return View(products);
        }
        public IActionResult Upsert(int? id)
        {
            //IEnumerable<SelectListItem> CategoryDrop = _Db.Categories.Select(i => new SelectListItem
            //{
            //    Text = i.Name,
            //    Value = i.Id.ToString(),
            //});
            //ViewBag.CategoryDrop = CategoryDrop;

            //Product product = new Product();

            ProductVM productVM = new ProductVM()
            {
                Product = new Product(),
                SelectListItems = _Db.GetAllDropDownList(WC.CategoryName),
                ApplicationSelectList = _Db.GetAllDropDownList(WC.ApplicationTypeName),
            };

            if (id == null)
            {
                // это для создания
                return View(productVM);
            }
            else
            {
                productVM.Product = _Db.Find(id.GetValueOrDefault());
                if (productVM.Product == null)
                {
                    return NotFound();
                }
                return View(productVM);
            }

        }
        [HttpPost]

        public IActionResult Upsert(ProductVM productVM)
        {
            //var errors = ModelState.Select(x => x.Value.Errors)
            //               .Where(y => y.Count > 0)
            //               .ToList();
            if (ModelState.IsValid)
            {
                var files = HttpContext.Request.Form.Files;
                string webRootPath = webHostEnvironment.WebRootPath;

                if (productVM.Product.Id == 0)
                {
                    //creating
                    //получаем путь до папки ввврут + путь до папки с картинками
                    string upload = webRootPath + WC.ImagePath;
                    string fileName = Guid.NewGuid().ToString();
                    string extention = Path.GetExtension(files[0].FileName);

                    //установим новый путь для файла
                    using (var filestrim = new FileStream(Path.Combine(upload, fileName + extention), FileMode.Create))
                    {
                        files[0].CopyTo(filestrim);
                    }

                    //добавим новый путь к изображению
                    productVM.Product.Image = fileName + extention;

                    _Db.Add(productVM.Product);

                }
                else
                {
                    //upload
                    // отключаем слежение для ЕФ
                    var objFromDb = _Db.FirstOrDefault(u => u.Id == productVM.Product.Id, isTracking: false);;
                    if (files.Count > 0)
                    {
                        string upload = webRootPath + WC.ImagePath;
                        string fileName = Guid.NewGuid().ToString();
                        string extention = Path.GetExtension(files[0].FileName);

                        //обеденяет пути
                        var oldFile = Path.Combine(upload, objFromDb.Image);

                        if (System.IO.File.Exists(oldFile))
                        {
                            System.IO.File.Delete(oldFile);
                        }
                        using (var filestrim = new FileStream(Path.Combine(upload, fileName + extention), FileMode.Create))
                        {
                            files[0].CopyTo(filestrim);
                        }
                        productVM.Product.Image = fileName + extention;
                    }
                    else
                    {
                        productVM.Product.Image = objFromDb.Image;

                    }
                    _Db.Update(productVM.Product);
                }
                _Db.Save();
                return RedirectToAction("Index");
            }
            productVM.SelectListItems = _Db.GetAllDropDownList(WC.CategoryName);
             
            productVM.ApplicationSelectList = _Db.GetAllDropDownList(WC.ApplicationTypeName);


            return View(productVM);
        }



        // удалить


        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public IActionResult Create(Category category)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _Db.Add(category);
        //        _Db.Save();
        //        return RedirectToAction("Index");
        //    }
        //    return View(category);
        //}

        //public IActionResult Edit(int? id)
        //{
        //    if (id == null || id == 0)
        //    {
        //        return NotFound();
        //    }
        //    var obj = _Db.Categories.FirstOrDefault(c => c.Id == id);
        //    if (obj == null)
        //    {
        //        return NotFound();
        //    }
        //    return View(obj);
        //}
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public IActionResult Edit(Category category)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _Db.Update(category);
        //        _Db.Save();
        //        return RedirectToAction("Index");
        //    }
        //    return View(category);
        //}


        public IActionResult Delete([FromRoute] int? id)
        {

            var obj=_Db.Find(id.GetValueOrDefault());
          //  var obj = _Db.FirstOrDefault(u => u.Id == id, includeProperties:"Category,ApplicationType") ;

            //жадная загрузкаype
            //  Product product = _Db.Products.Include(u=>u.Id==id).FirstOrDefault(u=>u.Id==id);

            if (obj == null)
            {
                return NotFound();
            }

            string upload = webHostEnvironment.WebRootPath + WC.ImagePath;

            //обеденяет пути
            var oldFile = Path.Combine(upload, obj.Image);

            if (System.IO.File.Exists(oldFile))
            {
                System.IO.File.Delete(oldFile);
            }

            _Db.Remove(obj);
            _Db.Save();
            return RedirectToAction("Index");

        }
    }
}
