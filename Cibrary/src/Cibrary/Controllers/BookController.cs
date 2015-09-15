using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Cibrary.Models;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.Rendering;

namespace Cibrary.Controllers
{
    public class BookController : Controller
    {
        private readonly ApplicationDbContext _db;

        public BookController(ApplicationDbContext db)
        {
            _db = db;
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Add()
        {
            ViewBag.Categories = GetCategoryListItems();
            return View();

        }

        private IEnumerable<SelectListItem> GetCategoryListItems()
        {
            var tmp = _db.Categories.ToList();
            return tmp.Select(category => new SelectListItem
            {
                Text = category.CategoryName,
                Value = category.Id.ToString()
            });
        }

        [HttpPost]
        public ActionResult Add(Book newBook)
        {
            if(newBook.ISBN.Length == 10 || newBook.ISBN.Length == 13) { 
                _db.Books.Add(newBook);
                _db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Message = "Feil i ISBN-nummer";
            return View();
        }



    }
}
