using System;
using System.Threading.Tasks;
using Cibrary.Models;

namespace Cibrary.Services
{
    public interface IScrapingService
    {
        Task<Book> Scrape(String isbn);
    }
}