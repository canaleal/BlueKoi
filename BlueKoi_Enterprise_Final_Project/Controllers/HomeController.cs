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
using BlueKoi_Enterprise_Final_Project.Models.ShopCart;
using BlueKoi_Enterprise_Final_Project.Models.ViewModels;

namespace BlueKoi_Enterprise_Final_Project.Controllers
{

    public class HomeController : Controller
    {
        APIServiceClient client;
        private readonly VirtualStoreDBContext _context;
        private readonly IAccountRepository accountRepository;
        private readonly IItemRepository itemRepository;
        private readonly IOrdersCartRepository ordersCartRepository;
        private readonly IShoppingCartRepository shoppingCartRepository;

        public HomeController(IAccountRepository accountRepository, IItemRepository itemRepository, IOrdersCartRepository ordersCartRepository, IShoppingCartRepository shoppingCartRepository)
        {

            this.accountRepository = accountRepository;
            this.itemRepository = itemRepository;
            this.ordersCartRepository = ordersCartRepository;
            this.shoppingCartRepository = shoppingCartRepository;

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

        /// <summary>
        /// Get the storepage view and get all the image data that was searched or selected
        /// </summary>
        /// <param name="id">The user account id</param>
        /// <param name="searchData">The search data used to find images on pinterest</param>
        /// <param name="specialItems">Search data for the saved artists</param>
        /// <returns>Action result with view</returns>
        [HttpGet]
        public ActionResult StorePageView(int id, string searchData, string specialItems, bool hadError)
        {
           


            ViewModelData viewModelData = new ViewModelData();
            viewModelData.Account = accountRepository.GetAnAccount(id);

            IEnumerable<Item> items = specialItems == null ? itemRepository.GetItems() : itemRepository.GetSpecialItems();
            viewModelData.Items = items;

            //Check if search exists and try getting the data
            if (searchData != null)
            {
                try
                {
                   
                    JToken data;
                    client = new APIServiceClient();
                    if (!searchData.Contains("by"))
                    {                      
                        JObject json = JObject.Parse(GetDataAsyncAlpha(searchData).Result);
                        data = json["results"];
                        viewModelData.ItemsSimple1 = data;
                    }
                    else
                    {
                        //Get the data from the service using API Beta
                        JObject jsonDataVal = JObject.Parse(GetDataAsyncBeta(searchData).Result);
                        data = jsonDataVal["rss"]["channel"]["item"];
                        viewModelData.ItemsSimple2 = data;
                    }
                    client.CloseAsync();
                }
                catch
                {
                    ViewBag.msg = "Sorry, there was an Error. The Webservice could be down.";
                }
            }

            if(hadError)
            {
                ViewBag.Message = "An error occured. Try again.";
            }

       

            return View(viewModelData);
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
        public ActionResult StorePageView(int id, string search)
        {
            return RedirectToAction("StorePageView", "Home", new { id, searchData = search });
        }


        [HttpGet]
        public ActionResult AccountView(int id)
        {
            try
            {
                Account account = accountRepository.GetAnAccount(id);
                return View();
            }
            catch{

                return RedirectToAction("StorePageView", "Home", new { id, errorMessage = true });
            }
           
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

        [HttpGet]
        public ActionResult ItemView(int id, int itemId, string url)
        {

            try
            {           
                ViewModelData viewModelData = new ViewModelData();
                viewModelData.Account = accountRepository.GetAnAccount(id);

                if (itemId > 0)
                {
                    viewModelData.Item = itemRepository.GetAnItem(itemId);

                }
                else
                {
                    viewModelData.ItemSimple = url;
                }

                return View(viewModelData);
            }
            catch
            {
                return RedirectToAction("StorePageView", "Home", new { id, errorMessage = true });
            }         
        }


        [HttpGet]
        public ActionResult CreditCardView(int id, int itemId)
        {

            ViewModelData viewModelData = new ViewModelData();
            viewModelData.AccountId = id;
            viewModelData.ItemId = itemId;

            return View(viewModelData);
        }

        /// <summary>
        /// Post the credit card information and specify the type of card to use
        /// </summary>
        /// <param name="card"></param>
        /// <param name="IsSpecial"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult CreditCardView(ViewModelData viewModelData, string IsSpecial)
        {

 

            if (ModelState.IsValid)
            {
                try
                {
                    Card card = viewModelData.Card;

                    //If the card is special or not
                    if (IsSpecial == "1")
                    {
                        viewModelData.Card = new SpecialCard() { AccountId = card.AccountId, CardHolder = card.CardHolder, CardNumber = card.CardNumber, ExpireDate = card.ExpireDate, CVNumber = card.CVNumber, ItemId = card.ItemId, MerchantName = card.MerchantName };
                        viewModelData.CardType = 1;
                    }
                    else
                    {
                        viewModelData.Card = new RegularCard() { AccountId = card.AccountId, CardHolder = card.CardHolder, CardNumber = card.CardNumber, ExpireDate = card.ExpireDate, CVNumber = card.CVNumber, ItemId = card.ItemId, MerchantName = card.MerchantName };
                        viewModelData.CardType = 0;
                    }

                    TempData["ViewModelData"] = JsonConvert.SerializeObject(viewModelData);
                    return RedirectToAction("ConfirmView", "Home");
                }
                catch{
                    //Add error
                    ViewBag.Message = "An error occured. Try again.";
                }
              
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
            if (TempData["ViewModelData"] != null)
            {
                ViewModelData viewModelData = JsonConvert.DeserializeObject<ViewModelData>((string)TempData["ViewModelData"]);
                viewModelData.OrderCartId = ordersCartRepository.GetAnOrdersCart(viewModelData.AccountId).Id;
                viewModelData.Item = itemRepository.GetAnItem(viewModelData.ItemId);
                return View(viewModelData);
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
        public ActionResult ConfirmViewPost(ViewModelData viewModelData)
        {
            if (ModelState.IsValid)
            {
                Order order = viewModelData.Order;
                ordersCartRepository.AddOrder(order);

                //If card is special, send an email
                if (viewModelData.CardType == 1)
                {
                    EmailData emailData = new EmailData();
                    emailData.SendMail(accountRepository.GetAnAccount(viewModelData.AccountId).UserEmail);
                }

                return RedirectToAction("StorePageView", "Home", new { id = viewModelData.AccountId });
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

            try
            {
                ViewModelData viewModelData = new ViewModelData();
                viewModelData.AccountId = id;
                OrdersCart ordersCart = ordersCartRepository.GetAnOrdersCart(id);
                IEnumerable<Order> data = ordersCartRepository.GetOrders(ordersCart.Id);
                viewModelData.Orders = data;

                return View(viewModelData);
            }
            catch
            {
                return RedirectToAction("StorePageView", "Home", new { id, errorMessage = true });
            }
           
        }

        /// <summary>
        /// Do a post with the orders cart to delete a specific image
        /// </summary>
        /// <param name="accountId">The account id for the user</param>
        /// <param name="itemId">The item id that will be used to find and delete the image</param>
        /// <returns>Action result with redirect</returns>
        [HttpPost]
        public ActionResult OrderCartView(int id, int orderId)
        {
            try
            {
                ordersCartRepository.DeleteOrder(orderId);
                return RedirectToAction("OrderCartView", "Home", new { id = id });
            }
            catch
            {
                return RedirectToAction("StorePageView", "Home", new { id, errorMessage = true });
            }
          
        }


        [HttpGet]
        public ActionResult ShoppingCartView(int id)
        {
            try
            {
                ViewModelData viewModelData = new ViewModelData();
                viewModelData.AccountId = id;
                ShoppingCart shoppingCart = shoppingCartRepository.GetAShoppingCart(id);
                IEnumerable<ShoppingCartItem> data = shoppingCartRepository.GetSavedItems(shoppingCart.Id);
                viewModelData.ShoppingCartItems = data ?? null;

                return View(viewModelData);
            }
            catch
            {
                return RedirectToAction("StorePageView", "Home", new { id, errorMessage = true });
            }
        }

        [HttpPost]
        public ActionResult SaveItemView(ViewModelData viewModelData)
        {

            if (ModelState.IsValid)
            {

                ShoppingCart shoppingCart = shoppingCartRepository.GetAShoppingCart(viewModelData.AccountId);
                Item item = itemRepository.GetAnItem(viewModelData.ItemId);

                ShoppingCartItem shoppingCartItem = new ShoppingCartItem();
                shoppingCartItem.ItemURL = item.ItemURL;
                shoppingCartItem.Price = item.ItemPrice;
                shoppingCartItem.ShoppingCartId = shoppingCart.Id;
                shoppingCartRepository.AddShoppingCartItem(shoppingCartItem);
                return RedirectToAction("StorePageView", "Home", new { id = viewModelData.AccountId });

            }
            return View();
        }


    }
}
