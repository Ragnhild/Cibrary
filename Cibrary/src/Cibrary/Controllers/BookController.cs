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
using Microsoft.Framework.Logging;

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

        private IEnumerable<SelectListItem> GetCategoryListItems(int id = -1)
        {
            var tmp = _db.Categories.ToList();
            return tmp.Select(category => new SelectListItem
            {
                Text = category.CategoryName,
                Value = category.Id.ToString(),
                Selected = category.Id == id
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

        public ActionResult Edit()
        {

            IEnumerable<Book> books = _db.Books.Include(x => x.Category).ToList();
         
            return View(books);
        }
        
        public ActionResult Editor(int id)
        {
            Book book = _db.Books.SingleOrDefault(x => x.Id == id);
            ViewBag.Items = GetCategoryListItems(id);
            return View(book);
        }

        [HttpPost]
        public ActionResult Editor(Book book, int id)
        {
            book.Id = id;
            _db.Books.Attach(book);
            _db.Entry(book).State = EntityState.Modified;
            _db.SaveChanges();
            return RedirectToAction("Index");

        }

        public IActionResult Borrow(String query = null, bool available = false)
        {

            IEnumerable<Book> books = _db.Books.Include(x=> x.Category).Where(
                x =>
                    x.Author.Contains(query) || x.Title.Contains(query) || x.Category.CategoryName == query ||
                    x.Year.ToString() == query);

            if (String.IsNullOrEmpty(query))
            {
                books = _db.Books.Include(x=> x.Category);
            }

            if (available)
            {
                books = books.Where(x => x.CountAvailable > 0);
            }

            return View(books.ToList());

        }

        public IActionResult BorrowBook(int id)
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

                return RedirectToAction("Borrowed", new {id = id});
            }
            else
            {
                throw new Exception();
            }
       


        }
        
        public IActionResult Borrowed(int id)
        {
            var book = _db.Books.FirstOrDefault(x => x.Id == id);
            return View(book);
          
        }

        public IActionResult Return()
        {

            var loans = _db.Borrows.Include(x => x.Book).Where(x => x.UserId == User.GetUserId() && x.EndTime == null).ToList();
            return View(loans);
        }

        public IActionResult ReturnBook(int id)
        {
            var loan = _db.Borrows.Include(x => x.Book).FirstOrDefault(x => x.Id == id);
            loan.Book.CountAvailable += 1;
            loan.EndTime = DateTime.Now;
            _db.SaveChanges();
            return RedirectToAction("Returned", new {id = id});

        }

        public IActionResult Returned(int id)
        {
            var loan = _db.Borrows.Include( x=> x.Book).FirstOrDefault(x => x.Id == id);
            return View(loan.Book);

        }

    }
}
