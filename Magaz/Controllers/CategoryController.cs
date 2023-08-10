using Magaz.DAL.Data;
using Magaz.DAL.Repository.IRepository;
using Magaz.Models;
using Magaz.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace Magaz.Controllers
{
    [Authorize(Roles = WC.AdminRole)]
    public class CategoryController : Controller
    {
        private ICategoryRepository _catRepo { get; set; }
        public CategoryController( ICategoryRepository catRepo)
        {
            
            this._catRepo = catRepo;
        }
        public IActionResult Index()
        {
            IEnumerable<Category> categories = _catRepo.GetAll();
            return View(categories);
        }
        public IActionResult Create()
        {

            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Category category)
        {
            if (ModelState.IsValid)
            {
                _catRepo.Add(category);
                _catRepo.Save();
                TempData[WC.Success] = "КАТЕГОРИЯ добавленна успешно";
                return RedirectToAction("Index");
            }
            TempData[WC.Error] = "КАТЕГОРИЯ не добавилась, ошибка";
            return View(category);
        }

        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            var obj = _catRepo.FirstOrDefault(c => c.Id == id);
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
                _catRepo.Update(category);
                _catRepo.Save();
                return RedirectToAction("Index");
            }
            return View(category);
        }
        //[HttpGet]
        //[ValidateAntiForgeryToken]      
        //public IActionResult Delete([FromRoute] int? id)
        //{
        //    var obj = _Db.Categories.FirstOrDefault(c => c.Id == id);
        //    if (obj == null)
        //    {
        //        return NotFound();
        //    }
        //    _Db.Categories.Remove(obj);
        //        _Db.SaveChanges();
        //        return RedirectToAction("Index");

        //}
    
      
        public IActionResult Delete( int? id)
        {
            var obj = _catRepo.FirstOrDefault(c => c.Id == id);
            if (obj == null)
            {
                return NotFound();
            }
            _catRepo.Remove(obj);
            _catRepo.Save();
            TempData[WC.Success] = "КАТЕГОРИЯ удалена успешно";
            return RedirectToAction("Index");

        }
      
    }
}
