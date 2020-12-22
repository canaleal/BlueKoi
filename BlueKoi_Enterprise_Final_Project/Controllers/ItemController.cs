using BlueKoi_Enterprise_Final_Project.Models.Accounts;
using BlueKoi_Enterprise_Final_Project.Models.Items;
using BlueKoi_Enterprise_Final_Project.Models.ViewModels;
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

            if (hadError)
            {
                ViewBag.Message = "An error occured. Try again.";
            }



            return View(viewModelData);
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
        public ActionResult StorePageView(int id, string search)
        {
            return RedirectToAction("StorePageView", "Home", new { id, searchData = search });


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

    }
}
