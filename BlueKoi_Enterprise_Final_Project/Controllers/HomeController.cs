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

namespace BlueKoi_Enterprise_Final_Project.Controllers
{
    public class HomeController : Controller
    {
        private readonly VirtualStoreDBContext _context;
        private readonly IAccountRepository accountRepository;

        
        public HomeController(IAccountRepository accountRepository)
        {
      
            this.accountRepository = accountRepository;
        }

        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult LoginView()
        {
            return View("LoginView");
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
                    return View("LoginView");
                }
               
            }
            return View("LoginView");
        }

        [HttpGet]
        public ActionResult SignUpView()
        {
            return View("SignUpView");
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SignUpView(Account newAccount)
        {
            if (ModelState.IsValid)
            {
                accountRepository.Add(newAccount);
                return RedirectToAction(nameof(StorePageView));
            }
            return View("SignUpView");
        }


        [HttpGet]
        public ActionResult StorePageView()
        {
            int id = int.Parse(TempData["ID"].ToString());
            return View(accountRepository.GetAnAccount(id));
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
            return View("SignUpView");
        }

        [HttpGet]
        public ActionResult AccountView(int id)
        {
            return View(accountRepository.GetAnAccount(id));
        }
    }
}
