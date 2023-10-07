using BankAPI.Models;

namespace BankAPI.Services
{
    public class UserService
    {
        private readonly BankDBContext _context;

        public UserService(BankDBContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Existing Check of User Data
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public bool IsExist(int userId)
        {
            return (_context.Users?.Any(e => e.UserId == userId)).GetValueOrDefault();
        }

        /// <summary>
        /// Encrypt PIN by Hashing
        /// </summary>
        /// <param name="PIN"></param>
        /// <returns>Hashed PIN</returns>
        public string HashPIN(string PIN)
        {
            // Generate a unique salt for the user.
            string salt = BCrypt.Net.BCrypt.GenerateSalt(8);

            string hashedPIN = BCrypt.Net.BCrypt.HashPassword(PIN, salt);

            // Return Hash the user's PIN with the generated salt.
            return hashedPIN;
        }
    }
}
