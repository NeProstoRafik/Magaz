using Magaz.DAL.Data;
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
        private Context _Db { get; set; }
        public CategoryController(Context context)
        {
            _Db = context;
        }
        public IActionResult Index()
        {
            IEnumerable<Category> categories = _Db.Categories;
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
            var obj = _Db.Categories.FirstOrDefault(c => c.Id == id);
            if (obj == null)
            {
                return NotFound();
            }
            _Db.Categories.Remove(obj);
            _Db.SaveChanges();
            return RedirectToAction("Index");

        }
      
    }
}
