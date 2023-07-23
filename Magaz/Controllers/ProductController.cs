using Magaz.Data;
using Magaz.Models;
using Magaz.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Magaz.Controllers
{
    public class ProductController : Controller
    {
        private readonly Context _Db;
        private readonly IWebHostEnvironment webHostEnvironment;
        public ProductController(Context context, IWebHostEnvironment webHostEnvironment)
        {
            _Db = context;
            this.webHostEnvironment = webHostEnvironment;
        }
        public IActionResult Index()
        {
            IEnumerable<Product> products = _Db.Products.Include(u=>u.Category).Include(u=>u.ApplicationType);

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
            {Product = new Product(),
            SelectListItems = _Db.Categories.Select(i => new SelectListItem
            {
                Text = i.Name,
                Value = i.Id.ToString(),
            }),
               ApplicationSelectList = _Db.Applications.Select(i => new SelectListItem
               {
                   Text = i.Name,
                   Value = i.Id.ToString(),
               })
            };
            if (id == null)
            {
                // это для создания
                return View(productVM);
            }
            else
            {
                productVM.Product = _Db.Products.Find(id);
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

                if (productVM.Product.Id==0)
                {
                    //creating
                    //получаем путь до папки ввврут + путь до папки с картинками
                    string upload = webRootPath + WC.ImagePath;
                    string fileName=Guid.NewGuid().ToString();
                    string extention = Path.GetExtension(files[0].FileName);

                    //установим новый путь для файла
                    using (var filestrim= new FileStream(Path.Combine(upload, fileName+extention), FileMode.Create))
                    {
                        files[0].CopyTo(filestrim);
                    }

                    //добавим новый путь к изображению
                    productVM.Product.Image = fileName + extention;

                    _Db.Products.Add(productVM.Product);
                  
                }
                else
                {
                    //upload
                    // отключаем слежение для ЕФ
                    var objFromDb = _Db.Products.AsNoTracking().FirstOrDefault(u => u.Id == productVM.Product.Id);
                    if (files.Count>0)
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
                        productVM.Product.Image= fileName + extention;
                    }
                    else
                    {
                        productVM.Product.Image = objFromDb.Image;

                    }
                    _Db.Products.Update(productVM.Product);
                }
                _Db.SaveChanges();
                return RedirectToAction("Index");
            }
            productVM.SelectListItems = _Db.Categories.Select(i => new SelectListItem
            {
                Text = i.Name,
                Value = i.Id.ToString(),
            });
            productVM.ApplicationSelectList = _Db.Applications.Select(i => new SelectListItem
            {
                Text = i.Name,
                Value = i.Id.ToString(),
            });


            return View(productVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Category category)
        {
            if (ModelState.IsValid)
            {
                _Db.Categories.Add(category);
                _Db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(category);
        }

        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            var obj = _Db.Categories.FirstOrDefault(c => c.Id == id);
            if (obj == null)
            {
                return NotFound();
            }
            return View(obj);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Category category)
        {
            if (ModelState.IsValid)
            {
                _Db.Categories.Update(category);
                _Db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(category);
        }
        
       
        public IActionResult Delete([FromRoute] int? id)
        {
            var obj = _Db.Products.Find(id);

            //жадная загрузка
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

            _Db.Products.Remove(obj);
            _Db.SaveChanges();
          return RedirectToAction("Index");

        }
    }
}
