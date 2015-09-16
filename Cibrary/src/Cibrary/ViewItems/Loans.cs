using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cibrary.Models;

namespace Cibrary.ViewItems
{
    public class Loans
    {
        public IEnumerable<Borrow> Current { get; set; }
        public IEnumerable<Borrow> Previous { get; set; }
        public DateTime from { get; set; }
        public DateTime to { get; set; }
    }
}
