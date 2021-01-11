using BlueKoi_Enterprise_Final_Project.Models;
using BlueKoi_Enterprise_Final_Project.Models.Accounts;
using BlueKoi_Enterprise_Final_Project.Models.Items;
using BlueKoi_Enterprise_Final_Project.Models.Orders;
using BlueKoi_Enterprise_Final_Project.Models.Payment;
using BlueKoi_Enterprise_Final_Project.Models.ShopCart;
using BlueKoi_Enterprise_Final_Project.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
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

        [Authorize]
        [HttpGet]
        public ActionResult CreditCardView(int id, int itemId)
        {

            ViewModelData viewModelData = new ViewModelData(id);
            viewModelData.ItemId = itemId;

            return View(viewModelData);
        }

        [Authorize]
        [HttpPost]
        public ActionResult CreditCardView(ViewModelData viewModelData, int IsSpecial)
        {



            if (ModelState.IsValid)
            {
                try
                {
                    Card card = viewModelData.Card;

                    //If the card is special or not
                    if (IsSpecial == 1)
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
                    return RedirectToAction("ConfirmView", "Order");
                }
                catch
                {
                    //Add error
                    ViewBag.Message = "An error occured. Try again.";
                }

            }
            return View();

        }



        [Authorize]
        [HttpGet]
        public ActionResult ConfirmView()
        {
            //If card exists, perform action
            if (TempData["ViewModelData"] != null)
            {
                ViewModelData viewModelData = JsonConvert.DeserializeObject<ViewModelData>((string)TempData["ViewModelData"]);
                viewModelData.OrderCartId = ordersCartRepository.GetAnOrdersCart(viewModelData.AccountId).Id;
                viewModelData.Item = itemRepository.GetAnItem(viewModelData.Card.ItemId);
                return View(viewModelData);
            }
            else
            {
                return RedirectToAction("StorePageView", "Item");
            }
        }

        [Authorize]
        [HttpPost]
        public ActionResult ConfirmViewPost(ViewModelData viewModelData)
        {
            if (ModelState.IsValid)
            {
                ordersCartRepository.AddOrder(viewModelData.Order);

                //If card is special, send an email
                if (viewModelData.CardType == 1)
                {
                    EmailData emailData = new EmailData();
                    emailData.SendMail(accountRepository.GetAnAccount(viewModelData.AccountId).UserEmail);
                }

                return RedirectToAction("StorePageView", "Item", new { id = viewModelData.AccountId });
            }
            return View();
        }



        [Authorize]
        [HttpGet]
        public ActionResult OrderCartView(int id)
        {

            try
            {
                ViewModelData viewModelData = new ViewModelData(id);
         
                OrdersCart ordersCart = ordersCartRepository.GetAnOrdersCart(id);
                viewModelData.Orders = ordersCartRepository.GetOrders(ordersCart.Id);

                return View(viewModelData);
            }
            catch
            {
                return RedirectToAction("StorePageView", "Item", new { id, errorMessage = true });
            }

        }

        [HttpPost]
        public ActionResult OrderCartView(int id, int orderId)
        {
            ordersCartRepository.DeleteOrder(orderId);
            return RedirectToAction("OrderCartView", "Order", new { id });
        }

        [Authorize]
        [HttpGet]
        public ActionResult ShoppingCartView(int id)
        {
            try
            {
                ViewModelData viewModelData = new ViewModelData(id);
               
                ShoppingCart shoppingCart = shoppingCartRepository.GetAShoppingCart(id);
                viewModelData.ShoppingCartItems = shoppingCartRepository.GetSavedItems(shoppingCart.Id);

                return View(viewModelData);
            }
            catch
            {
                return RedirectToAction("StorePageView", "Item", new { id, errorMessage = true });
            }
        }

        [HttpPost]
        public ActionResult ShoppingCartView(int id, int itemId)
        {
            shoppingCartRepository.DeleteItem(itemId);
            return RedirectToAction("ShoppingCartView", "Order", new { id });
        }

        [Authorize]
        [HttpPost]
        public ActionResult SaveItemView(ViewModelData viewModelData)
        {

            if (ModelState.IsValid)
            {

                ShoppingCart shoppingCart = shoppingCartRepository.GetAShoppingCart(viewModelData.AccountId);
                Item item = itemRepository.GetAnItem(viewModelData.ItemId);

                ShoppingCartItem shoppingCartItem = new ShoppingCartItem
                {
                    ItemId = item.Id,
                    ItemURL = item.ItemURL,
                    Price = item.ItemPrice,
                    ShoppingCartId = shoppingCart.Id
                };
                shoppingCartRepository.AddShoppingCartItem(shoppingCartItem);
                return RedirectToAction("StorePageView", "Item", new { id = viewModelData.AccountId });

            }
            return View();
        }

    }
}
