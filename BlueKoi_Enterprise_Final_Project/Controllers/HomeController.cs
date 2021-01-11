using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using BlueKoi_Enterprise_Final_Project.Models;
using BlueKoi_Enterprise_Final_Project.Models.Accounts;
using BlueKoi_Enterprise_Final_Project.Models.Data;
using BlueKoi_Enterprise_Final_Project.Models.Items;
using System.Net;
using System.IO;
using Newtonsoft.Json.Linq;
using BlueKoi_Enterprise_Final_Project.Models.Payment;
using BlueKoi_Enterprise_Final_Project.Models.Orders;
using Newtonsoft.Json;
using ServiceReference1;
using System.Xml;
using System.Text.RegularExpressions;
using BlueKoi_Enterprise_Final_Project.Models.ShopCart;
using BlueKoi_Enterprise_Final_Project.Models.ViewModels;

namespace BlueKoi_Enterprise_Final_Project.Controllers
{

    public class HomeController : Controller
    {



        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

    }
}
