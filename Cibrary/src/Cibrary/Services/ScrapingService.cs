using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
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
            var address = $"http://www.barnesandnoble.com/w/clean-code-robert-c-martin/?ean={isbn}#productInfoTabs";
            //var result = await client.GetByteArrayAsync($"http://www.barnesandnoble.com/w/clean-code-robert-c-martin/?ean={isbn}#productInfoTabs");
            var result = await client.GetByteArrayAsync(address);

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

            
            string datePattern = "\\d{1,2}\\/\\d{1,2}\\/\\d{4,4}";
            Regex regEx = new Regex(datePattern);
            var additionalProductInfo =
                doc.DocumentNode.Descendants()
                    .FirstOrDefault(x => x.Attributes.Any(y => y.Name == "id" && y.Value == "additionalProductInfo"));
            var dl = additionalProductInfo?.ChildNodes.FirstOrDefault(x => x.Name == "dl");
            var date =
                dl?.ChildNodes.Where(x => x.Name == "dd")
                    .FirstOrDefault(z => regEx.Match(z.InnerText).Success).InnerText;
            string yearPattern = "\\d{4,4}";
            regEx = new Regex(yearPattern);
            var year = int.Parse(regEx.Match(date).Groups[0].ToString());
            
            var book = new Book
            {
                Author = authorsString,
                Title = title,
                Year = year,
                InformationLink = address 
            };
            return book;
        }
    }
}
