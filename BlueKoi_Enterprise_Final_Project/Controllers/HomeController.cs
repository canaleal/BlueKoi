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
                else
                {
                    return View();
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
        public ActionResult StorePageView()
        {
            MyViewModel model = new MyViewModel();
            int id = int.Parse(TempData["ID"].ToString());

            model.account = accountRepository.GetAnAccount(id);
            model.items = itemRepository.GetItems();

            //return View(accountRepository.GetAnAccount(id));

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult StorePageView(Account newAccount)
        {
            if (ModelState.IsValid)
            {
                accountRepository.Add(newAccount);
                return RedirectToAction(nameof(Index));
            }
            return View();
        }

        [HttpGet]
        public ActionResult AccountView(int id)
        {
            return View(accountRepository.GetAnAccount(id));
        }
    }
}
