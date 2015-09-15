using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Cibrary.Models
{
    public class Book
    {
        public int Id { get; set; }
        public string ISBN { get; set; }
        public string Author { get; set; }
        public string Title { get; set; }
        public int Year { get; set; }
        public int CategoryId { get; set; }   
        public Category Category { get; set; }
        public int TotalCount { get; set; }
        public int CountAvailable { get; set; }
        public string InformationLink { get; set; }
    }
}
