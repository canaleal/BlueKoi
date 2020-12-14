using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Xml;

//Alex
namespace BlueKoi_Service
{
    
    [ServiceContract]
    public interface IAPIService
    {
        [OperationContract]
        string GetApiDataAlpha(string search);

        [OperationContract]
        string GetApiDataBeta(string search);
    }
}
