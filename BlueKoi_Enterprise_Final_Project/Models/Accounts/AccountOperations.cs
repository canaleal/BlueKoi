using BlueKoi_Enterprise_Final_Project.Models.Data;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace BlueKoi_Enterprise_Final_Project.Models.Accounts
{
    public class AccountOperations : IAccountRepository
    {
        private readonly VirtualStoreDBContext context;
    

        public AccountOperations(VirtualStoreDBContext context)
        {
            this.context = context;
        }

        public void Add(Account account)
        {
            context.Accounts.Add(account);
            context.SaveChanges();
        }

        public void Delete(Account account)
        {
            context.Accounts.Remove(account);
            context.SaveChanges();
        }

        public Account GetAnAccount(int id)
        {
            return context.Accounts.Find(id);
        }

        public Account GetAnAccountEmailPass(string email, string password)
        {
           
            Account account = context.Accounts.Where(a => a.UserEmail.Equals(email) && a.UserPassword.Equals(password)).FirstOrDefault();
            return account;
        }

        public IEnumerable<Account> GetAccounts()
        {
            return context.Accounts.ToList();
        }

        public void Update(Account accountChange)
        {
            var account = context.Accounts.Attach(accountChange);
            account.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            context.SaveChanges();
        }

        public bool CheckAccount(Account accountCheck)
        {

            Account account = context.Accounts.Where(a => a.UserEmail.Equals(accountCheck.UserEmail) && a.UserPassword.Equals(accountCheck.UserPassword)).FirstOrDefault();
            if(account != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }


    }
}
