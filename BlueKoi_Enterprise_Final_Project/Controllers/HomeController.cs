using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using BlueKoi_Enterprise_Final_Project.Models;

namespace BlueKoi_Enterprise_Final_Project.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public ViewResult Index()
        {
            return View();
        }

        [HttpGet]
        public ViewResult LoginView()
        {
            return View("LoginView");
        }

        [HttpPost]
        public ViewResult LoginRP()
        {
            return View("LoginView");
        }

        [HttpGet]
        public ViewResult SignUp()
        {
            return View("SignUpView");
        }




    }
}
