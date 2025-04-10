using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace L2CSharP.Network.Cliente.Interfaces
{
    public interface IAccountService
    {
        Task<bool> CreateAccountAsync(string username, string passwordCrypted, string password);
        Task<bool> CheckAccountCredentialsAsync(string username, string password);
        Task<bool> AccountExistsAsync(string username);
        Task<bool> IsAccountBannedAsync(string username);
    }
}
