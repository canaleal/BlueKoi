using BlueKoi_Enterprise_Final_Project.Models.Accounts;
using BlueKoi_Enterprise_Final_Project.Models.Items;
using BlueKoi_Enterprise_Final_Project.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using ServiceReference1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlueKoi_Enterprise_Final_Project.Controllers
{
    public class ItemController : Controller
    {
        APIServiceClient client;

        private readonly IAccountRepository accountRepository;
        private readonly IItemRepository itemRepository;
       

        public ItemController(IAccountRepository accountRepository, IItemRepository itemRepository)
        {

            this.accountRepository = accountRepository;
            this.itemRepository = itemRepository;
  

        }

        [Authorize]
        [HttpGet]
        public ActionResult StorePageView(int id, string searchData, string specialItems, bool hadError)
        {

            ViewModelData viewModelData = new ViewModelData(id);

            List<string> names = new List<string>
            {
                "leifheanzo",
                "snatti89",
                "unpreti",
                "nixeu",
                "wlop",
                "talros",
                "razaras"
            };
            viewModelData.SimpleList = names;

           
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
                        JObject json = JObject.Parse(GetDataAsync(searchData, 0).Result);
                        data = json["results"];
                        viewModelData.ItemsSimple1 = data;
                    }
                    else
                    {
                        //Get the data from the service using API Beta
                        JObject jsonDataVal = JObject.Parse(GetDataAsync(searchData, 1).Result);
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

            if (hadError)
            {
                ViewBag.Message = "An error occured. Try again.";
            }

           

            return View(viewModelData);
        }


        public async Task<string> GetDataAsync(string search, int option)
        {
            return option == 0 ? await client.GetApiDataAlphaAsync(search) : await client.GetApiDataBetaAsync(search);
        }


        [Authorize]
        [HttpPost]
        public ActionResult StorePageView(int id, string search)
        {
            return RedirectToAction("StorePageView", "Item", new { id, searchData = search });
        }

        [Authorize]
        [HttpGet]
        public ActionResult ItemView(int id, int itemId, string url)
        {

            try
            {
                ViewModelData viewModelData = new ViewModelData(id);
               
                if (itemId > 0)
                {
                    Item item = itemRepository.GetAnItem(itemId);
                    viewModelData.Item = item;
                    viewModelData.ItemSimple = item.ItemURL;
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

    }
}
