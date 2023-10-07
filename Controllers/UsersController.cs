using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BankAPI.Models;
using System.Linq.Expressions;
using BCrypt.Net;
using BankAPI.Services;

namespace BankAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly BankDBContext _context;
        private readonly UserService _userService;
        private readonly AccountService _accountService;

        public UsersController(BankDBContext context,
               UserService userService,
               AccountService accountService)
        { 
            _userService = userService;
            _accountService = accountService;
            _context = context;
        }

        /// <summary>
        /// Get User List
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
          if (_context.Users == null)
          {
              return NotFound();
          }
            return await _context.Users.ToListAsync();
        }

        /// <summary>
        /// Get User Data by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(int id)
        {
          if (_context.Users == null)
          {
              return NotFound();
          }
            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return user;
        }

        /// <summary>
        /// Add New User
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult<User>> PostUser(User user)
        {
            try
            {
                if (_context.Users == null)
                {
                    throw new Exception("Users does not exist.");
                }

                user.PIN = _userService.HashPIN(user.PIN);

                _context.Users.Add(user);

                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (_userService.IsExist(user.UserId))
                    {
                        throw new Exception("User already exist. User creation aborted.");
                    }
                }

                Account account = new Account();

                account.UserId = user.UserId;
                account.Balance = 0;
                account.IsActive = true;

                _context.Accounts.Add(account);

                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (_accountService.IsExist(account.AccountId))
                    {
                        throw new Exception("User Account already exist. User Account creation aborted.");
                    }
                }

                return Ok(user);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
