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
    public class ApplicationTypeController : Controller
    {

        private IApplicationTypeRepository _appRep { get; set; }
       
        public ApplicationTypeController(IApplicationTypeRepository appRep)
        {
            _appRep = appRep;
        }

        public IActionResult Index()
        {
            IEnumerable<ApplicationType> app = _appRep.GetAll();
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
            _appRep.Add(obj);
            _appRep.Save();
            return RedirectToAction("Index");
        }
        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            var obj = _appRep.FirstOrDefault(c => c.Id == id);
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
                _appRep.Update(application);
                _appRep.Save();
                return RedirectToAction("Index");
            }
            return View(application);
        }

        
        public IActionResult Delete(int? id)
        {
            var obj = _appRep.FirstOrDefault(c => c.Id == id);
            if (obj == null)
            {
                return NotFound();
            }
            _appRep.Remove(obj);
            _appRep.Save();
            return RedirectToAction("Index");

        }
    }
}
