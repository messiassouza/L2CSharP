using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace L2CSharP.LoggerApi.Logger.Interfaces
{
    public interface IGameLogger : ILogger
    {
        void Debug(string message);
        void Warn(string message);
        void Error(string message, Exception ex = null);
        void NetworkLog(string message);
        void GameServerLog(string message);
        void LoginServerLog(string message);
        void ErrorTrace(string message, int framesToSkip = 2);
        void Info(string v);
    }
}
