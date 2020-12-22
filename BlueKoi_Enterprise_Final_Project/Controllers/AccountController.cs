using BlueKoi_Enterprise_Final_Project.Models;
using BlueKoi_Enterprise_Final_Project.Models.Accounts;
using BlueKoi_Enterprise_Final_Project.Models.Orders;
using BlueKoi_Enterprise_Final_Project.Models.ShopCart;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlueKoi_Enterprise_Final_Project.Controllers
{
    public class AccountController : Controller
    {
       
       
        private readonly IAccountRepository accountRepository;
        private readonly IOrdersCartRepository ordersCartRepository;
        private readonly IShoppingCartRepository shoppingCartRepository;


        public AccountController(IAccountRepository accountRepository, IOrdersCartRepository ordersCartRepository, IShoppingCartRepository shoppingCartRepository)
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
                    return RedirectToAction("StorePageView", "Home", new { id = account.Id });
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

                    return RedirectToAction("StorePageView", "Home", new { id = newAccount.Id });
                }


                //Add error
                ViewBag.Message = "An error occured. Try again.";
            }

            return View();
        }


        [HttpGet]
        public ActionResult AccountView(int id)
        {
            try
            {
                Account account = accountRepository.GetAnAccount(id);
                return View();
            }
            catch
            {

                return RedirectToAction("StorePageView", "Home", new { id, errorMessage = true });
            }

        }

        [HttpGet]
        public ActionResult DeleteView(int id)
        {
            return View(accountRepository.GetAnAccount(id));
        }


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
