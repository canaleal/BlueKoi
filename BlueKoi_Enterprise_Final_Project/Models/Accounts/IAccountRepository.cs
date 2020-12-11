using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

//Angela
namespace BlueKoi_Enterprise_Final_Project.Models.Accounts
{
    public interface IAccountRepository
    {
        Account GetAnAccount(int id);

        Account GetAnAccountEmailPass(string email, string password);

        IEnumerable<Account> GetAccounts();

        void Add(Account account);

        void Update(Account accountChange);

        void Delete(Account account);

        bool CheckAccount(Account accountCheck);
    }
}
