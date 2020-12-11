using BlueKoi_Enterprise_Final_Project.Models.Data;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

//Angela
namespace BlueKoi_Enterprise_Final_Project.Models.Accounts
{
    /// <summary>
    /// Account operations class to get, update, add, and delete a user from the database
    /// </summary>
    public class AccountOperations : IAccountRepository
    {
        private readonly VirtualStoreDBContext context;
        private readonly Encryption encryption;

        public AccountOperations(VirtualStoreDBContext context)
        {
            this.context = context;
            encryption = new Encryption();
        }

        /// <summary>
        /// Add a user to the database, and encrypt the password
        /// </summary>
        /// <param name="account">The account that will be saved into the database</param>
        public void Add(Account account)
        {
            account.UserPassword = encryption.HashPassword(account.UserPassword);
            context.Accounts.Add(account);
            context.SaveChanges();
        }

        /// <summary>
        /// Delete a user from the database given the account
        /// </summary>
        /// <param name="account">Account used to check which user will be deleted</param>
        public void Delete(Account account)
        {
            context.Accounts.Remove(account);
            context.SaveChanges();
        }

        /// <summary>
        /// Get an account from databse using the id
        /// </summary>
        /// <param name="id">The id used to check the primary key for an account in databse</param>
        /// <returns>An account from the database</returns>
        public Account GetAnAccount(int id)
        {
            return context.Accounts.Find(id);
        }

        /// <summary>
        /// Get an account from the database given email and password
        /// </summary>
        /// <param name="email">The inputed email that will be used to check</param>
        /// <param name="password">The password that will be used to check if account exists</param>
        /// <returns></returns>
        public Account GetAnAccountEmailPass(string email, string password)
        {
           
            Account account = context.Accounts.Where(a => a.UserEmail.Equals(email)).FirstOrDefault();

            if (account == null || !encryption.Authenticate(password, account.UserPassword))
            {
                // Account does not exist
                return null;
            }
            else
            {
                // Account Exists
                return account;
            }
          
        }

        /// <summary>
        /// Get all account from the databse. (Not used)
        /// </summary>
        /// <returns>A list of all the accounts in the database</returns>
        public IEnumerable<Account> GetAccounts()
        {
            return context.Accounts.ToList();
        }

        /// <summary>
        /// Update an already existing account in the database
        /// </summary>
        /// <param name="accountChange">The account with the updated information</param>
        public void Update(Account accountChange)
        {
            var account = context.Accounts.Attach(accountChange);
            account.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            context.SaveChanges();
        }

        /// <summary>
        /// Check if an account exists in the database or not
        /// </summary>
        /// <param name="accountCheck">The account that will be used to check if an account already exists</param>
        /// <returns>True if an account exists, false if no account exists</returns>
        public bool CheckAccount(Account accountCheck)
        {

            Account account = context.Accounts.Where(a => a.UserEmail.Equals(accountCheck.UserEmail)).FirstOrDefault();

            if (account == null || !encryption.Authenticate(accountCheck.UserPassword, account.UserPassword))
            {
                // Account does not exist
                return false;
            }
            else
            {
                // Account Exists
                return true;
            }
        }
    }
}
