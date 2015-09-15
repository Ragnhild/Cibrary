using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Cibrary.Models;
using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Mvc;

namespace Cibrary.Controllers
{
    public class BookController : Controller
    {

        private readonly ApplicationDbContext db;

        public BookController(ApplicationDbContext db)
        {
            this.db = db;
        }

        public ActionResult Index()
        {
            return View();
        }

        public IActionResult Borrow(String query = null)
        {

            IEnumerable<Book> books = db.Books.ToList();
            return View(books);
        }
        
        public IActionResult Borrowed(int id)
        {
            var book = db.Books.FirstOrDefault(x => x.Id == id);
            if (book != null && book.CountAvailable > 0)
            {
                book.CountAvailable -= 1;
                Borrow loan = new Borrow();
                loan.StartTime = DateTime.Now;
                loan.UserId = User.GetUserId();
                loan.BookId = id;
                db.Borrows.Add(loan);
                db.SaveChanges();

                return View();
            }
            else
            {
                throw new Exception();
            }
        }

    }
}
