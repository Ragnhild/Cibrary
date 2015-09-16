using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cibrary.Models;
using Microsoft.AspNet.Mvc;
using Microsoft.Data.Entity;

namespace Cibrary.Controllers
{
    public class CategoryController : Controller
    {
        private ApplicationDbContext _db;

        public CategoryController(ApplicationDbContext db)
        {
            _db = db;
        }

        public ActionResult Index()
        {
            IEnumerable<Category> category = _db.Categories.ToList();
            return View(category);
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(Category category)
        {
            _db.Categories.Add(category);
            _db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult Edit(int id)
        {
            Category category = _db.Categories.SingleOrDefault(x => x.Id == id);
            return View(category);
        }

        [HttpPost]
        public ActionResult Edit(Category category)
        {
            _db.Categories.Attach(category);
            _db.Entry(category).State = EntityState.Modified;
            _db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult Delete(int id)
        {
            Category category = _db.Categories.SingleOrDefault(x => x.Id == id);
            _db.Categories.Remove(category);
            _db.SaveChanges();
            return RedirectToAction("Index");
        }


    }
}
