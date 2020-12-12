using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Xml;


namespace BlueKoi_Service
{
    
    public class APIService : IAPIService
    {
        public string GetApiDataAlpha(string search)
        {

            string url = "https://api.unsplash.com/search/photos/?client_id=" + "byVpt0dHXyzvmAM-HixXGw_1TGQOxS4ViH1hIhNEanY" + "&per_page=60&query=" + search;
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

        public string GetApiDataBeta(string search)
        {
            string url = "https://backend.deviantart.com/rss.xml?type=deviation&q=" + search;
            HttpWebRequest myReq = (HttpWebRequest)WebRequest.Create(url);
            myReq.ContentType = "application/xml";
            HttpWebResponse response = (HttpWebResponse)myReq.GetResponse();

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(response.GetResponseStream());
            string jsonData = JsonConvert.SerializeXmlNode(xmlDoc);
            return jsonData;
        }
    }
}
