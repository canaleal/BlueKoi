using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using BC = BCrypt.Net.BCrypt;


//Alex Canales
namespace BlueKoi_Enterprise_Final_Project.Models.Data
{
    /// <summary>
    /// Class contains two methods to Hash a given password and Authenticate/check 2 passwords
    /// </summary>
    public class Encryption
    {
        /// <summary>
        /// Method hashes a password using BCrypt
        /// </summary>
        /// <param name="password">The password that will be hashed</param>
        /// <returns>A hashed password to save in database</returns>
        public string HashPassword(string password)
        {
            if (password == null)
            {
                throw new ArgumentNullException("password");
            }
            string passwordHash = BC.HashPassword(password);
            return passwordHash;
        }

        /// <summary>
        /// Authenticate and check if the give password matched the hashed password
        /// </summary>
        /// <param name="passAuth">The password that will be compared to the stored hash password</param>
        /// <param name="passSaved">The hashed password saved in the database</param>
        /// <returns>True if both passwords match, false if both passwords do not match</returns>
        public bool Authenticate(string passAuth, string passSaved)
        {
            if (!BC.Verify(passAuth, passSaved))
            {
                // authentication failed
                return false;
            }
            else
            {
                // authentication successful
                return true;
            }
        }
       
    }
}
