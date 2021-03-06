﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Cibrary.Models;
using Cibrary.Services;
using HtmlAgilityPack;
using Cibrary.ViewItems;
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
        private readonly IScrapingService _scraping;

        public BookController(ApplicationDbContext db, IScrapingService scraping)
        {
            _db = db;
            _scraping = scraping;
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

        public async Task<Book> SearchByISBN(String isbn)
        {
            return await _scraping.Scrape(isbn);
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
            ViewBag.Categories = GetCategoryListItems(id);
            return View(book);
        }

        [HttpPost]
        public ActionResult Editor(Book book, int id)
        {
            book.Id = id;
            var eBook = _db.Books.FirstOrDefault(x => x.Id == id);
            eBook.ImageLink = book.ImageLink;
            eBook.Author = book.Author;
            eBook.Category = book.Category;
            eBook.CountAvailable = book.CountAvailable;
            eBook.TotalCount = book.TotalCount;
            eBook.Title = book.Title;
            eBook.InformationLink = book.InformationLink;
            eBook.Year = book.Year;
            _db.Entry(eBook).State = EntityState.Modified;
            _db.SaveChanges();
            return RedirectToAction("Index");

        }

        public IActionResult Borrow(String query = null, bool available = false)
        {

            ViewBag.isChecked = available;
            ViewBag.query = query;

            if (query != null)
            {
                query = query.ToLower();

            }

            IEnumerable<Book> books = _db.Books.Include(x=> x.Category).Where(
                x =>
                    x.Author.ToLower().Contains(query) || x.Title.ToLower().Contains(query) || x.Category.CategoryName.ToLower() == query ||
                    x.Year.ToString() == query || x.ISBN == query);

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

        public IActionResult Return(DateTime? from = null, DateTime? to = null)
        {
            var searched = true;

            if ((to == null && from == null))
            {
                searched = false;
            }
            if (to == null)
            {
                to = DateTime.Today;
            }
            if (from == null)
            {
                from = DateTime.Today;
            }
            if (searched)
            {
                from = from.Value.Date;
                to = new DateTime(to.Value.Year, to.Value.Month, to.Value.Day, 23, 59, 59);
            }

            var currentLoans = _db.Borrows.Include(x => x.Book).Where(x => x.UserId == User.GetUserId() && x.EndTime == null).ToList();

            var previousLoans =
                _db.Borrows.Include(x => x.Book).Where(x => x.StartTime >= from && x.StartTime <= to).ToList(); 

            var loans = new Loans();
            loans.Current = currentLoans;
            loans.Previous = previousLoans;
            loans.from = from.Value;
            loans.to = to.Value;

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
