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

namespace BlueKoi_Enterprise_Final_Project.Controllers
{
   
    public class HomeController : Controller
    {
        APIServiceClient client = new APIServiceClient();
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

        /// <summary>
        /// We used this method to get a user and pass the id to the storepage.
        /// </summary>
        /// <param name="loginAccount"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult LoginView(Account loginAccount)
        {

            if (ModelState.IsValid)
            {
                Account account = accountRepository.GetAnAccountEmailPass(loginAccount.UserEmail, loginAccount.UserPassword);
      
                if (account != null)
                {  
                    return RedirectToAction("StorePageView", "Home", new { id = account.Id});
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
                    OrdersCart ordersCart = new OrdersCart(newAccount.Id);
                    ordersCartRepository.Add(ordersCart);

                    return RedirectToAction("StorePageView", "Home", new { id = newAccount.Id });
                }
               
            }
            return View();
        }


        [HttpGet]
        public ActionResult StorePageView(int id, string searchData)
        {

            if (TempData["Search"] != null || searchData != null)
            {
                try
                {
                    string search;
                    if (TempData["Search"] != null)
                    {
                        search = TempData["Search"].ToString();
                    }
                    else
                    {
                        search = searchData;
                    }

                      Task<string> data;
                      if (!search.Contains("by"))
                      {
                            data = GetDataAsyncAlpha(search);
                            JObject json = JObject.Parse(data.Result);
                            ViewBag.pinterest = search;
                            ViewBag.pinterestData = json["results"];
                      }
                      else
                      {
                            data = GetDataAsyncBeta(search);
                            JObject jsonDataVal = JObject.Parse(data.Result);
                            ViewBag.devData = jsonDataVal["rss"]["channel"]["item"];
                            
                      }
                    
                }
                catch
                {
                    ViewBag.msg = "Sorry, there was an Error. The Webservice could be down.";
                }
               
            }

            ViewBag.images = itemRepository.GetItems();

            return View(accountRepository.GetAnAccount(id));
        }

        public async Task<string> GetDataAsyncAlpha(string search)
        {

            string text = await client.GetApiDataAlphaAsync(search);
            return text;
        }

        public async Task<string> GetDataAsyncBeta(string search)
        {
            string text = await client.GetApiDataBetaAsync(search);
            return text;
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult StorePageView(string id, string search)
        {
            TempData["Search"] = search;
            return RedirectToAction("StorePageView", "Home", new { id = int.Parse(id) });
        }


        [HttpGet]
        public ActionResult AccountView(int id)
        {
            return View(accountRepository.GetAnAccount(id));
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
            ordersCartRepository.Delete(deleteAccount.Id);
            accountRepository.Delete(deleteAccount);
            return RedirectToAction(nameof(Index));

        }

        [HttpGet]
        public ActionResult ItemView(int id, string url)
        {
            ViewBag.url = url;
            return View(accountRepository.GetAnAccount(id));
        }


        [HttpGet]
        public ActionResult CreditCardView(int id, string url)
        {        
            ViewBag.id = id;
            ViewBag.url = url;
            return View();
        }


        [HttpPost]
        public ActionResult CreditCardView(Card card)
        {

            if (ModelState.IsValid)
            {
               
               TempData["Card"] = JsonConvert.SerializeObject(card, Newtonsoft.Json.Formatting.Indented);
               return RedirectToAction("ConfirmView", "Home");  
            }
            return View();
        }

        [HttpGet]
        public ActionResult ConfirmView()
        {
            if(TempData["Card"] != null)
            {
                Card savedCard = JsonConvert.DeserializeObject<Card>(TempData["Card"].ToString());
                ViewBag.url = savedCard.ItemURL;
                ViewBag.id = savedCard.AccountId;
                ViewBag.cartId = ordersCartRepository.GetAnOrdersCart(savedCard.AccountId).Id;
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
                return RedirectToAction("StorePageView", "Home", new { id = int.Parse(accountId) });
            }          
            return View();
        }


        [HttpGet]
        public ActionResult OrderCartView(int id)
        {

            OrdersCart ordersCart = ordersCartRepository.GetAnOrdersCart(id);         
            IEnumerable<Order> data = ordersCartRepository.GetOrders(ordersCart.Id);

            ViewBag.id = id;
            return View(data);
        }



    }
}
