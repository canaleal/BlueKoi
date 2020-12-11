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
        APIServiceClient client;
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
                    return RedirectToAction("StorePageView", "Home", new { id = account.Id });
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
        public ActionResult StorePageView(int id, string searchData, string specialItems)
        {

            if (TempData["Search"] != null || searchData != null)
            {
                try
                {
                    string search = TempData["Search"] != null ? TempData["Search"].ToString() : searchData;

                    Task<string> data;
                    client = new APIServiceClient();
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
                    client.CloseAsync();

                }
                catch
                {
                    ViewBag.msg = "Sorry, there was an Error. The Webservice could be down.";
                }
            }

            if(specialItems != null)
            {
                ViewBag.specialImages = itemRepository.GetSpecialItems();
            }

            ViewBag.images = itemRepository.GetItems();

            return View(accountRepository.GetAnAccount(id));
        }

        public async Task<string> GetDataAsyncAlpha(string search)
        {
            return await client.GetApiDataAlphaAsync(search);
        }

        public async Task<string> GetDataAsyncBeta(string search)
        {
            return await client.GetApiDataBetaAsync(search);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult StorePageView(string id, string search)
        {
            return RedirectToAction("StorePageView", "Home", new { id = int.Parse(id), searchData = search });
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
            if (ModelState.IsValid)
            {
                int accountId = deleteAccount.Id;
                ordersCartRepository.DeleteOrders(ordersCartRepository.GetAnOrdersCart(accountId).Id);
                ordersCartRepository.Delete(accountId);
                accountRepository.Delete(deleteAccount);
                return RedirectToAction(nameof(Index));
            }
            return View();

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
        public ActionResult CreditCardView(Card card, string IsSpecial)
        {

            if (ModelState.IsValid)
            {
                
                if(IsSpecial == "1")
                {
                    SpecialCard cardData = new SpecialCard() { AccountId = card.AccountId , CardHolder = card.CardHolder, CardNumber = card.CardNumber , ExpireDate  = card.ExpireDate , CVNumber = card.CVNumber , ItemURL = card.ItemURL , MerchantName = card.MerchantName };
                    TempData["Type"] = 1;
                    TempData["Card"] = JsonConvert.SerializeObject(cardData, Newtonsoft.Json.Formatting.Indented);
                }
                else
                {
                    RegularCard cardData = new RegularCard() { AccountId = card.AccountId , CardHolder = card.CardHolder, CardNumber = card.CardNumber, ExpireDate = card.ExpireDate, CVNumber = card.CVNumber, ItemURL = card.ItemURL, MerchantName = card.MerchantName };
                    TempData["Type"] = 0;
                    TempData["Card"] = JsonConvert.SerializeObject(cardData, Newtonsoft.Json.Formatting.Indented);
                }

                return RedirectToAction("ConfirmView", "Home");
            }
            return View();
        }

        [HttpGet]
        public ActionResult ConfirmView()
        {
            if (TempData["Card"] != null)
            {

                List<string> cardData;
                if(TempData["Type"].ToString() == "0")
                {
                    var savedCard = JsonConvert.DeserializeObject<RegularCard>(TempData["Card"].ToString());
                    cardData = new List<string>() { savedCard.CardSection.ToString(), savedCard.ItemURL, savedCard.AccountId.ToString()};
                }
                else
                {
                    var savedCard = JsonConvert.DeserializeObject<SpecialCard>(TempData["Card"].ToString());
                    cardData = new List<string>() { savedCard.CardSection.ToString(), savedCard.ItemURL, savedCard.AccountId.ToString(), savedCard.CardType };
                }

                ViewBag.cardType = cardData[0];
                ViewBag.url = cardData[1];
                ViewBag.id = cardData[2];
                ViewBag.cartId = ordersCartRepository.GetAnOrdersCart(int.Parse(cardData[2])).Id;
                ViewBag.cardSection = cardData[3];
                return View();
            }
            else
            {
                return RedirectToAction(nameof(StorePageView));
            }
        }


        [HttpPost]
        public ActionResult ConfirmView(Order order, string accountId, string cardType)
        {
            if (ModelState.IsValid)
            {
               
                ordersCartRepository.AddOrder(order);

                //Special Order
                if (cardType == "1")
                {
                    EmailData emailData = new EmailData();             
                    emailData.SendMail(accountRepository.GetAnAccount(int.Parse(accountId)).UserEmail);
                }

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
            return View(data.Reverse());
        }



    }
}
