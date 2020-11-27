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
        private Account currentUser;

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
              
                if (account.UserEmail != null)
                {
                    currentUser = account;
                    return View("Index");
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
                return RedirectToAction(nameof(Index));
            }
            return View("SignUpView");
        }



    }
}
