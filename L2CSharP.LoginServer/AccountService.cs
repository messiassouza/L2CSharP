using L2CSharP.DataBase;
using L2CSharP.Model;
using L2CSharP.Network.Cliente.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace L2CSharP.LoginServer
{
    public class AccountService : IAccountService
    {
        private readonly AppDbContext _dbContext;

        public AccountService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> CreateAccountAsync(string username, string passwordCrypted, string password)
        {
            if (await AccountExistsAsync(username))
                return false;

            var account = new Accounts
            {
                UserName = username,
                PassWordCrypted = passwordCrypted,
                PassWord = password,
                State = "Active" // Default state
            };

            _dbContext.Accounts.Add(account);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> CheckAccountCredentialsAsync(string username, string password)
        {
            return await _dbContext.Accounts
                .AnyAsync(a => a.UserName == username && a.PassWordCrypted == password);
        }

        public async Task<bool> AccountExistsAsync(string username)
        {
            return await _dbContext.Accounts
                .AnyAsync(a => a.UserName == username);
        }

        public async Task<bool> IsAccountBannedAsync(string username)
        {
            return await _dbContext.Accounts
                .AnyAsync(a => a.UserName == username && a.State == "Banned");
        }
    }
}
