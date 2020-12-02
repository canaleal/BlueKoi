using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace BlueKoi_Service
{
    
    public class APIService : IAPIService
    {
        public string GetApiData(string search)
        {

            string url = "https://api.unsplash.com/search/photos/?client_id=" + "byVpt0dHXyzvmAM-HixXGw_1TGQOxS4ViH1hIhNEanY" + "&per_page=20&query=" + search;
            HttpWebRequest myReq = (HttpWebRequest)WebRequest.Create(url);
            myReq.ContentType = "application/json";
            HttpWebResponse response = (HttpWebResponse)myReq.GetResponse();

            string text;
            using (var sr = new StreamReader(response.GetResponseStream()))
            {
                text = sr.ReadToEnd();
            }
            return text;

        }
    }
}
