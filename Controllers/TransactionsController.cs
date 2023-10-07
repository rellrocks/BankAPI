using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BankAPI.Models;
using BankAPI.Services;

namespace BankAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionsController : ControllerBase
    {
        private readonly BankDBContext _context;
        private readonly AccountService _accountService;

        public TransactionsController(BankDBContext context,
               AccountService accountService)
        {
            _context = context;
            _accountService = accountService;
        }

        /// <summary>
        /// Deposit
        /// </summary>
        /// <param name="transaction"></param>
        /// <returns></returns>
        [HttpPost("deposit")]
        public async Task<ActionResult<Transaction>> Deposit(Transaction transaction)
        {
            try
            {
                if (_context.Transactions == null)
                {
                    throw new Exception("Transactions does not exist.");
                }

                var account = _accountService.IsExist(transaction.AccountId);

                // If User Account does not exist, return an error
                if (!account)
                {
                    throw new Exception("Account Does Not Exist.");
                }

                // If Updating Balance failed, return an error
                if (!_accountService.UpdateBalance(transaction.AccountId, transaction.Amount))
                {
                    throw new Exception("Invalid Transaction. Please check account validity.");
                }

                // Add transaction and save changes
                _context.Transactions.Add(transaction);
                await _context.SaveChangesAsync();

                return Ok(transaction);

            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Withdraw
        /// </summary>
        /// <param name="transaction"></param>
        /// <returns></returns>
        [HttpPost("withdraw")]
        public async Task<ActionResult<Transaction>> Withdraw(Transaction transaction)
        {
            try
            {
                if (_context.Transactions == null)
                {
                    throw new Exception("Transactions Does Not Exist.");
                }

                var account = _accountService.IsExist(transaction.AccountId);

                // If User Account does not exist, return an error
                if (!account)
                {
                    throw new Exception("User Account Does Not Exist.");
                }

                decimal currentBalance = _accountService.GetCurrentBalance(transaction.AccountId);

                // If Transaction Amount is greater than Current Balance, return an error
                if(transaction.Amount > currentBalance)
                {
                    throw new Exception("Insufficient Balance.");
                }

                // Change amount to negative value to deduct balance
                transaction.Amount = transaction.Amount * -1;

                // If Updating Balance failed, return an error
                if (!_accountService.UpdateBalance(transaction.AccountId, transaction.Amount))
                {
                    throw new Exception("Invalid Transaction. Please Check User Account Validity.");
                }

                // Add transaction and save changes
                _context.Transactions.Add(transaction);
                await _context.SaveChangesAsync();

                return Ok(transaction);

            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Transfer Balance to an Account
        /// </summary>
        /// <param name="accountId">Account Id to Transfer to</param>
        /// <param name="transaction">Transfer Details</param>
        /// <returns></returns>
        [HttpPost("transfer/{accountId}")]
        public async Task<ActionResult<Transaction>> Transfer(int accountId, Transaction transaction)
        {
            try
            {
                if (_context.Transactions == null)
                {
                    throw new Exception("Transactions Does Not Exist.");
                }

                // If User Account Id is the same with transfer Account Id, return an error
                if (transaction.AccountId == accountId)
                {
                    throw new Exception("Cannot Transfer to Same Account Id.");
                }

                var transferAccount = _accountService.GetUserAccount(accountId);

                // If User Account to transfer to does not exist, return an error
                if (transferAccount == null)
                {
                    throw new Exception("User Account to Transfer to Does Not Exist.");
                }

                var account = _accountService.IsExist(transaction.AccountId);

                // If User Account does not exist, return an error
                if (!account)
                {
                    throw new Exception("User Account Does Not Exist.");
                }

                decimal currentBalance = _accountService.GetCurrentBalance(transaction.AccountId);

                // If Transaction Amount is greater than Current Balance, return an error
                if (transaction.Amount > currentBalance)
                {
                    throw new Exception("Insufficient Balance.");
                }

                // Change amount to negative value to deduct balance
                var inverseAmount = transaction.Amount * -1;

                // If Deducting Balance failed, return an error
                if (!_accountService.UpdateBalance(transaction.AccountId, inverseAmount))
                {
                    throw new Exception("Invalid Transaction. Please Check User Account Validity.");
                }

                // If Adding to Balance failed, return an error
                if (!_accountService.UpdateBalance(accountId, transaction.Amount))
                {
                    throw new Exception("Invalid Transaction. Please Check User Account Validity.");
                }

                // Add transfer transaction for account to transfer to
                Transaction transfer = new Transaction();
                transfer.Amount = transaction.Amount;
                transfer.AccountId = accountId;
                _context.Transactions.Add(transfer);

                // Add transaction and save changes
                transaction.Amount = inverseAmount;
                _context.Transactions.Add(transaction);
                await _context.SaveChangesAsync();

                return Ok(transaction);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
