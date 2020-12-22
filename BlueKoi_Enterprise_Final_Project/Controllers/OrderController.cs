using BlueKoi_Enterprise_Final_Project.Models;
using BlueKoi_Enterprise_Final_Project.Models.Accounts;
using BlueKoi_Enterprise_Final_Project.Models.Items;
using BlueKoi_Enterprise_Final_Project.Models.Orders;
using BlueKoi_Enterprise_Final_Project.Models.Payment;
using BlueKoi_Enterprise_Final_Project.Models.ShopCart;
using BlueKoi_Enterprise_Final_Project.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlueKoi_Enterprise_Final_Project.Controllers
{
    public class OrderController : Controller
    {

       
        private readonly IAccountRepository accountRepository;
        private readonly IItemRepository itemRepository;
        private readonly IOrdersCartRepository ordersCartRepository;
        private readonly IShoppingCartRepository shoppingCartRepository;

        public OrderController(IAccountRepository accountRepository, IItemRepository itemRepository, IOrdersCartRepository ordersCartRepository, IShoppingCartRepository shoppingCartRepository)
        {

            this.accountRepository = accountRepository;
            this.itemRepository = itemRepository;
            this.ordersCartRepository = ordersCartRepository;
            this.shoppingCartRepository = shoppingCartRepository;

        }

        [HttpGet]
        public ActionResult CreditCardView(int id, int itemId)
        {

            ViewModelData viewModelData = new ViewModelData();
            viewModelData.AccountId = id;
            viewModelData.ItemId = itemId;

            return View(viewModelData);
        }


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
                catch
                {
                    //Add error
                    ViewBag.Message = "An error occured. Try again.";
                }

            }
            return View();

        }




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
                return RedirectToAction("StorePageView", "Item");
            }
        }


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
