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

namespace BlueKoi_Enterprise_Final_Project.Controllers
{
   


    public class HomeController : Controller
    {
        private readonly VirtualStoreDBContext _context;
        private readonly IAccountRepository accountRepository;
        private readonly IItemRepository itemRepository;
        private readonly IOrdersCartRepository ordersCartRepository;

        public HomeController(IAccountRepository accountRepository, IItemRepository itemRepository, IOrdersCartRepository ordersCartRepository)
        {
      
            this.accountRepository = accountRepository;
            this.itemRepository = itemRepository;
            this.ordersCartRepository = ordersCartRepository;
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
                bool accountExist = accountRepository.CheckAccount(newAccount);
                if (!accountExist)
                {
                    accountRepository.Add(newAccount);
                    TempData["ID"] = newAccount.Id;

                    OrdersCart ordersCart = new OrdersCart();
                    ordersCart.AccountId = newAccount.Id;
                    ordersCartRepository.Add(ordersCart);

                    return RedirectToAction(nameof(StorePageView));
                }
               
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
            TempData["ID"] = id;

            if (TempData["Search"] != null)
            {
                string text = 

                JObject json = JObject.Parse(text);
                ViewBag.pinterest = search;
                ViewBag.pinterestData = json["results"];
               
            }
            else
            {
                ViewBag.images = itemRepository.GetItems();
            }

            return View(accountRepository.GetAnAccount(id));
        }

       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult StorePageView(string id, string search)
        {
            TempData["ID"] = id;
            TempData["Search"] = search;
            return RedirectToAction(nameof(StorePageView));
        }


        [HttpGet]
        public ActionResult AccountView(int id)
        {
            TempData["ID"] = id;
            return View(accountRepository.GetAnAccount(id));
        }

        [HttpGet]
        public ActionResult DeleteView(int id)
        {
            TempData["ID"] = id;
            return View(accountRepository.GetAnAccount(id));
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteView(Account deleteAccount)
        {
            ordersCartRepository.Delete(deleteAccount.Id);
            accountRepository.Delete(deleteAccount);
            return RedirectToAction(nameof(Index));

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


        [HttpPost]
        public ActionResult CreditCardView(Card card)
        {

            if (ModelState.IsValid)
            {
                TempData["Card"] = JsonConvert.SerializeObject(card, Formatting.Indented);              

                return RedirectToAction(nameof(ConfirmView));
            }
            return View();
        }

        [HttpGet]
        public ActionResult ConfirmView()
        {
            if(TempData["Card"] != null)
            {
                Card card = JsonConvert.DeserializeObject<Card>(TempData["Card"].ToString());

                //Get the cart using the ID
                //Do service worker check here


                ViewBag.url = card.ItemURL;
                ViewBag.id = card.AccountId;
                return View();
            }
            else
            {
                return RedirectToAction(nameof(StorePageView));
            }
          
        }

        [HttpPost]
        public ActionResult ConfirmView(Order order, string accountId)
        {
            if (ModelState.IsValid)
            {
                ordersCartRepository.AddOrder(order);
                TempData["ID"] = int.Parse(accountId);
                return RedirectToAction(nameof(StorePageView));
            }          
            return View();
        }


        [HttpGet]
        public ActionResult OrderCartView(int id)
        {
            TempData["ID"] = id;

    
            OrdersCart ordersCart = ordersCartRepository.GetAnOrdersCart(id);
            ViewBag.id = id;
            IEnumerable<Order> data = ordersCartRepository.GetOrders(ordersCart.Id);
            return View(data);
        }



    }
}
