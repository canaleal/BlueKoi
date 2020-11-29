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

namespace BlueKoi_Enterprise_Final_Project.Controllers
{
    public class HomeController : Controller
    {
        private readonly VirtualStoreDBContext _context;
        private readonly IAccountRepository accountRepository;
        private readonly IItemRepository itemRepository;


        public HomeController(IAccountRepository accountRepository, IItemRepository itemRepository)
        {
      
            this.accountRepository = accountRepository;
            this.itemRepository = itemRepository;
        }

        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult LoginView()
        {
            return View();
        }

        [HttpPost]
        public ActionResult LoginView(Account loginAccount)
        {

            if (ModelState.IsValid)
            {
                Account account = accountRepository.GetAnAccountEmailPass(loginAccount.UserEmail, loginAccount.UserPassword);
                
                if (account != null)
                {
                    TempData["ID"] = account.Id;
                   
                    return RedirectToAction(nameof(StorePageView));
                }
  
            }
            return View();
        }

        [HttpGet]
        public ActionResult SignUpView()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SignUpView(Account newAccount)
        {
            if (ModelState.IsValid)
            {
                accountRepository.Add(newAccount);
                TempData["ID"] = newAccount.Id;
                return RedirectToAction(nameof(StorePageView));
            }
            return View();
        }


        [HttpGet]
        public ActionResult StorePageView(int accountId)
        {
            int id;

            if (TempData["ID"] != null)
            {
                id = int.Parse(TempData["ID"].ToString());
            }
            else
            {
                id = accountId;
            }
                      
            ViewBag.images = itemRepository.GetItems();
            return View(accountRepository.GetAnAccount(id));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult StorePageView(string accountId, string search)
        {
           
            ViewBag.images = itemRepository.GetItems();

            string url = "https://api.unsplash.com/search/photos/?client_id=" + "byVpt0dHXyzvmAM-HixXGw_1TGQOxS4ViH1hIhNEanY" + "&per_page=20&query=" + search;
            HttpWebRequest myReq = (HttpWebRequest)WebRequest.Create(url);
            myReq.ContentType = "application/json";
            HttpWebResponse response = (HttpWebResponse)myReq.GetResponse();
            
            string text;
            using (var sr = new StreamReader(response.GetResponseStream()))
            {
                text = sr.ReadToEnd();
            }

            JObject json = JObject.Parse(text);
            ViewBag.pinterest = search;
            ViewBag.pinterestData = json["results"];
            

            return View(accountRepository.GetAnAccount(int.Parse(accountId)));
        }

        [HttpGet]
        public ActionResult AccountView(int id)
        {
            TempData["ID"] = id;
            return View(accountRepository.GetAnAccount(id));
        }


        [HttpGet]
        public ActionResult ItemView(int id, string url)
        {
            TempData["ID"] = id;
            ViewBag.url = url;
            return View(accountRepository.GetAnAccount(id));
        }


        [HttpGet]
        public ActionResult CreditCardView(int id, string url)
        {
            TempData["ID"] = id;
            ViewBag.id = id;
            ViewBag.url = url;
            return View();
        }


    }
}
