using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Cibrary.Models;
using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.Rendering;
using Microsoft.Data.Entity;

namespace Cibrary.Controllers
{

    [Authorize]
    public class BookController : Controller
    {
        private ApplicationDbContext _db;

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
            if(newBook.ISBN.Length == 10 || newBook.ISBN.Length == 13)
            {
                var existingBook = _db.Books.FirstOrDefault(x => x.ISBN == newBook.ISBN);
                if (existingBook == null)
                {

                    newBook.CountAvailable = newBook.TotalCount;
                    _db.Books.Add(newBook);
                }
                else
                {
                    existingBook.TotalCount += newBook.TotalCount;
                    existingBook.CountAvailable += newBook.TotalCount;
                }
                
                _db.SaveChanges();
                return RedirectToAction("Index");
            }
            
            ViewBag.Message = "Feil i ISBN-nummer";
            return View();
        }

        public IActionResult Borrow(String query = null)
        {

            IEnumerable<Book> books = _db.Books.ToList();
            return View(books);
        }
        
        public IActionResult Borrowed(int id)
        {
            var book = _db.Books.FirstOrDefault(x => x.Id == id);
            if (book != null && book.CountAvailable > 0)
            {
                book.CountAvailable -= 1;
                Borrow loan = new Borrow();
                loan.StartTime = DateTime.Now;
                loan.UserId = User.GetUserId();
                loan.BookId = id;
                _db.Borrows.Add(loan);
                _db.SaveChanges();

                return View();
            }
            else
            {
                throw new Exception();
            }
        }

        public IActionResult ReturnBook()
        {

            var loans = _db.Borrows.Include(x => x.Book).Where(x => x.UserId == User.GetUserId() && x.EndTime == null).ToList();
            return View(loans);
        }

        public IActionResult Returned(int id)
        {
            var loan = _db.Borrows.Include( x=> x.Book).FirstOrDefault(x => x.Id == id);
            loan.Book.CountAvailable += 1;
            loan.EndTime = DateTime.Now;
            _db.SaveChanges();
            return View(loan.Book);

        }

    }
}
