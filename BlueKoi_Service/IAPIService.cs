using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Xml;

namespace BlueKoi_Service
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IAPIService" in both code and config file together.
    [ServiceContract]
    public interface IAPIService
    {
        [OperationContract]
        string GetApiDataAlpha(string search);

        [OperationContract]
        string GetApiDataBeta(string search);
    }
}
