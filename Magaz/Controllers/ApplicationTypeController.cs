using Magaz.DAL.Data;
using Magaz.Models;
using Magaz.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace Magaz.Controllers
{
    [Authorize(Roles = WC.AdminRole)]
    public class ApplicationTypeController : Controller
    {

        private Context _Db { get; set; }
        public ApplicationTypeController(Context context)
        {
            _Db = context;
        }
        public IActionResult Index()
        {
            IEnumerable<ApplicationType> app = _Db.Applications;
            return View(app);
        }
        
        public IActionResult Create()
        {

            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(ApplicationType obj)
        {
            _Db.Applications.Add(obj);
            _Db.SaveChanges();
            return RedirectToAction("Index");
        }
        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            var obj = _Db.Applications.FirstOrDefault(c => c.Id == id);
            if (obj == null)
            {
                return NotFound();
            }
            return View(obj);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(ApplicationType application)
        {
            if (ModelState.IsValid)
            {
                _Db.Applications.Update(application);
                _Db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(application);
        }

        
        public IActionResult Delete(int? id)
        {
            var obj = _Db.Applications.FirstOrDefault(c => c.Id == id);
            if (obj == null)
            {
                return NotFound();
            }
            _Db.Applications.Remove(obj);
            _Db.SaveChanges();
            return RedirectToAction("Index");

        }
    }
}
