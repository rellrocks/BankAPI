using BankAPI.Models;
using Microsoft.Identity.Client;

namespace BankAPI.Services
{
    public class AccountService
    {
        private readonly BankDBContext _context;

        public AccountService(BankDBContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Get User Account Data
        /// </summary>
        /// <param name="accountId"></param>
        /// <returns></returns>
        public Account? GetUserAccount(int accountId)
        {
            return _context.Accounts.Where(e => e.AccountId == accountId).FirstOrDefault();
        }

        /// <summary>
        /// Existence Check for User Account
        /// </summary>
        /// <param name="accountId"></param>
        /// <returns></returns>
        public bool IsExist(int accountId)
        {
            return (_context.Accounts?.Any(e => e.AccountId == accountId)).GetValueOrDefault();
        }

        /// <summary>
        /// Check Balance of User Account
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public bool UpdateBalance(int accountId, decimal amount)
        {
            decimal CurrentBalance = GetCurrentBalance(accountId);

            // If Account does not exist return false
            if (!IsExist(accountId))
            {
                return false;
            }

            var account = _context.Accounts.Where(e => e.AccountId == accountId).FirstOrDefault();

            // If account exist, update balance
            if (account != null)
            {
                account.Balance = CurrentBalance + amount;
                _context.Accounts.Update(account);
            }
            else
            { return false; }

            // return true if successful
            return true;
        }

        /// <summary>
        /// Check Current Balance of User Account
        /// </summary>
        /// <param name="accountId"></param>
        /// <returns></returns>
        public decimal GetCurrentBalance(int accountId)
        {
            decimal balance = 0;

            // If Account does not exist return 0
            if(!IsExist(accountId))
            {
                return 0;
            }

            var account = _context.Accounts.Where(e => e.AccountId == accountId).FirstOrDefault();

            // If account exist, return current balance if exist
            if (account != null)
            {
                balance = account.Balance;
            }

            // return balance
            return balance;
        }
    }
}
