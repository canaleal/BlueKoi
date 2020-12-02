using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace BlueKoi_Service
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "CreditCardService" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select CreditCardService.svc or CreditCardService.svc.cs at the Solution Explorer and start debugging.
    public class CreditCardService : ICreditCardService
    {
        public bool CheckCard()
        {
            throw new NotImplementedException();
        }

        public bool CheckMerchantBank()
        {
            throw new NotImplementedException();
        }

        public void DoWork()
        {
        }

        public int GetBankKey()
        {
            throw new NotImplementedException();
        }
    }
}
