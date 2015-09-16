using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Cibrary.Models;
using HtmlAgilityPack;

namespace Cibrary.Services
{
    public class ScrapingService : IScrapingService
    {

        public async Task<Book> Scrape(string isbn)
        {
            var doc = new HtmlDocument();

            var client = new HttpClient();
            var result = await client.GetByteArrayAsync($"http://www.barnesandnoble.com/w/clean-code-robert-c-martin/?ean={isbn}#productInfoTabs");

            doc.Load(new MemoryStream(result));
            var title = doc.DocumentNode.Descendants()
                .FirstOrDefault(x => x.Attributes.Any(y => y.Name == "itemprop" && y.Value == "name"))?.InnerText ?? string.Empty;

            var authors = doc.DocumentNode.Descendants()
                .FirstOrDefault(x => x.Attributes.Any(y => y.Name == "class" && y.Value == "contributors"))?.Descendants().Where(x => x.Name == "a")?.Select(y => y.InnerText) ?? Enumerable.Empty<String>();
            var authorsString = string.Join(", ", authors);

            var imageUrl =
                doc.DocumentNode.Descendants()?
                    .FirstOrDefault(x => x.Attributes.Any(y => y.Name == "itemprop" && y.Value == "image"))?
                    .Attributes.FirstOrDefault(z => z.Name == "src")?.Value;
            imageUrl = (imageUrl.StartsWith("//")) ? imageUrl.Remove(0, 2) : imageUrl;

            var book = new Book
            {
                Author = authorsString,
                Title = title
            };
            return book;
        }
    }
}
