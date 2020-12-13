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
        /// <param name="loginAccount">The account model that will be used to login</param>
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

        /// <summary>
        /// Method to create a new user and register it into the account
        /// </summary>
        /// <param name="newAccount">The account that will be saved into the database</param>
        /// <returns></returns>
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

        /// <summary>
        /// Get the storepage view and get all the image data that was searched or selected
        /// </summary>
        /// <param name="id">The user account id</param>
        /// <param name="searchData">The search data used to find images on pinterest</param>
        /// <param name="specialItems">Search data for the saved artists</param>
        /// <returns>Action result with view</returns>
        [HttpGet]
        public ActionResult StorePageView(int id, string searchData, string specialItems)
        {
            //Check if search exists and try getting the data
            if (TempData["Search"] != null || searchData != null)
            {
                try
                {
                    //Get the search result
                    string search = TempData["Search"] != null ? TempData["Search"].ToString() : searchData;

                    Task<string> data;
                    client = new APIServiceClient();
                    if (!search.Contains("by"))
                    {
                       
                        //Get the data from the service using API Aplha
                        data = GetDataAsyncAlpha(search);
                        JObject json = JObject.Parse(data.Result);
                        ViewBag.pinterestData = json["results"];
                        
                    }
                    else
                    {
                        //Get the data from the service using API Beta
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

            //If the search also includes special artists
            if(specialItems != null)
            {
                ViewBag.specialImages = itemRepository.GetSpecialItems();
            }

            //Get developer imagaes
            ViewBag.images = itemRepository.GetItems();

            return View(accountRepository.GetAnAccount(id));
        }

        /// <summary>
        /// Get data from the API Alpha using the service
        /// </summary>
        /// <param name="search">The search data used</param>
        /// <returns>A list of string image urls</returns>
        public async Task<string> GetDataAsyncAlpha(string search)
        {
            return await client.GetApiDataAlphaAsync(search);
        }

        /// <summary>
        /// Get data from the API Beta using the service
        /// </summary>
        /// <param name="search">The search data used</param>
        /// <returns>A list of string image urls</returns>
        public async Task<string> GetDataAsyncBeta(string search)
        {
            return await client.GetApiDataBetaAsync(search);
        }

        /// <summary>
        /// When user uses the search functionality, pass the search to the get
        /// </summary>
        /// <param name="id">The user account id</param>
        /// <param name="search">The search result that the user wants to see</param>
        /// <returns>Action result with a redirect</returns>
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

        /// <summary>
        /// Delete all user data including orders cart, orders, and account
        /// </summary>
        /// <param name="deleteAccount">The account that will be deleted</param>
        /// <returns>Action result with redirect</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteView(Account deleteAccount)
        {
            if (ModelState.IsValid)
            {
                //Get the account ID
                int accountId = deleteAccount.Id;
                //Delete the orders then orders cart
                try
                {
                    ordersCartRepository.DeleteOrders(ordersCartRepository.GetAnOrdersCart(accountId).Id);
                }
                catch
                {
                    Debug.WriteLine("No orders");
                }

               
                ordersCartRepository.Delete(accountId);
                //Delete the account
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

        /// <summary>
        /// Post the credit card information and specify the type of card to use
        /// </summary>
        /// <param name="card"></param>
        /// <param name="IsSpecial"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult CreditCardView(Card card, string IsSpecial)
        {

            if (ModelState.IsValid)
            {
                //If the card is special or not
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

        /// <summary>
        /// Confirm view with all account data and order data
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult ConfirmView()
        {
            //If card exists, perform action
            if (TempData["Card"] != null)
            {
                //Save the card data in list
                List<string> cardData;

                //Check what type of card is used. 0 for regular, 1 for special
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

                //Save the data
                ViewBag.cardType = cardData[0];
                ViewBag.url = cardData[1];
                ViewBag.id = cardData[2];
                ViewBag.cartId = ordersCartRepository.GetAnOrdersCart(int.Parse(cardData[2])).Id;
      
                return View();
            }
            else
            {
                return RedirectToAction(nameof(StorePageView));
            }
        }

        /// <summary>
        /// Confirm order page used to see if user wants to get the image
        /// </summary>
        /// <param name="order">The order with all the data</param>
        /// <param name="accountId">The account id of the user</param>
        /// <param name="cardType">The type of card used. Special or regular</param>
        /// <returns>Action result with view</returns>
        [HttpPost]
        public ActionResult ConfirmView(Order order, string accountId, string cardType)
        {
            if (ModelState.IsValid)
            {               
                ordersCartRepository.AddOrder(order);

                //If card is special, send an email
                if (cardType == "1")
                {
                    EmailData emailData = new EmailData();
                    emailData.SendMail(accountRepository.GetAnAccount(int.Parse(accountId)).UserEmail);
                }

                return RedirectToAction("StorePageView", "Home", new { id = int.Parse(accountId) });
            }
            return View();
        }

       

        /// <summary>
        /// Get the orders cart and all the orders for a user
        /// </summary>
        /// <param name="id">The account id used to find the orders cart</param>
        /// <returns>Action result with view</returns>
        [HttpGet]
        public ActionResult OrderCartView(int id)
        {
            //Get the orders
            OrdersCart ordersCart = ordersCartRepository.GetAnOrdersCart(id);
            IEnumerable<Order> data = ordersCartRepository.GetOrders(ordersCart.Id);
            ViewBag.id = id;

            //Return the orders, but reverse them to show the latest
            return View(data.Reverse());
        }

        /// <summary>
        /// Do a post with the orders cart to delete a specific image
        /// </summary>
        /// <param name="accountId">The account id for the user</param>
        /// <param name="itemId">The item id that will be used to find and delete the image</param>
        /// <returns>Action result with redirect</returns>
        [HttpPost]
        public ActionResult OrderCartView(int accountId, int itemId)
        {
            ordersCartRepository.DeleteOrder(itemId);
            return RedirectToAction("OrderCartView", "Home", new { id = accountId });
        }



    }
}
