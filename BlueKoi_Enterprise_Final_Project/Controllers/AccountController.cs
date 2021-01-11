using BlueKoi_Enterprise_Final_Project.Models;
using BlueKoi_Enterprise_Final_Project.Models.Accounts;
using BlueKoi_Enterprise_Final_Project.Models.Orders;
using BlueKoi_Enterprise_Final_Project.Models.ShopCart;
using BlueKoi_Enterprise_Final_Project.Models.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BlueKoi_Enterprise_Final_Project.Controllers
{
    public class AccountController : Controller
    {

  
        private readonly IAccountRepository accountRepository;
        private readonly IOrdersCartRepository ordersCartRepository;
        private readonly IShoppingCartRepository shoppingCartRepository;


        public AccountController( IAccountRepository accountRepository, IOrdersCartRepository ordersCartRepository, IShoppingCartRepository shoppingCartRepository)
        {

            this.accountRepository = accountRepository;
            this.ordersCartRepository = ordersCartRepository;
            this.shoppingCartRepository = shoppingCartRepository;
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

                    var artClaim = new List<Claim>()
                    {
                        new Claim(ClaimTypes.Name, account.UserName),
                        new Claim(ClaimTypes.Email, account.UserEmail),
                    };

                    var artIdentity = new ClaimsIdentity(artClaim, "Art Identity");
                    var userPrinciple = new ClaimsPrincipal(new[] { artIdentity });

                    HttpContext.SignInAsync(userPrinciple);
            

                    return RedirectToAction("StorePageView", "Item", new { id = account.Id });
                }

                //Add error
                ViewBag.Message = "An error occured. Try again.";
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

                if (!accountRepository.CheckAccount(newAccount))
                {
                    accountRepository.Add(newAccount);

                    OrdersCart ordersCart = new OrdersCart(newAccount.Id);
                    ordersCartRepository.Add(ordersCart);

                    ShoppingCart shoppingCart = new ShoppingCart(newAccount.Id);
                    shoppingCartRepository.Add(shoppingCart);

                    return RedirectToAction("StorePageView", "Item", new { id = newAccount.Id });
                }


                //Add error
                ViewBag.Message = "An error occured. Try again.";
            }

            return View();
        }

        [Authorize]
        [HttpGet]
        public ActionResult AccountView(int id)
        {
            try
            {
                Account account = accountRepository.GetAnAccount(id);
                return View(account);
            }
            catch
            {

                return RedirectToAction("StorePageView", "Item", new { id, errorMessage = true });
            }

        }

        [Authorize]
        [HttpGet]
        public ActionResult DeleteView(int id)
        {
            return View(accountRepository.GetAnAccount(id));
        }


        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteView(Account deleteAccount)
        {
            if (ModelState.IsValid)
            {

                try
                {
                    //Get the account ID
                    int accountId = deleteAccount.Id;

                    //Delete the content
                    ordersCartRepository.DeleteOrders(ordersCartRepository.GetAnOrdersCart(accountId).Id);
                    ordersCartRepository.Delete(accountId);
                    shoppingCartRepository.Delete(accountId);
                    accountRepository.Delete(deleteAccount);

                    return RedirectToAction(nameof(Index));
                }
                catch
                {
                    //Add error
                    ViewBag.Message = "An error occured. Try again.";
                }

            }
            return View();

        }
    }
}
