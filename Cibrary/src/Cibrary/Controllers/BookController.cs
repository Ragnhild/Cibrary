﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;

namespace Cibrary.Controllers
{
    public class BookController : Controller
    {

        public ActionResult Index()
        {
            return View();
        }
    }
}
