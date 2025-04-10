using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace L2CSharP.Config
{
    public class LoggerConfig
    {
        public string LogDirectory { get; set; }
        public string FilePrefix { get; set; }
        public int MaxArchiveFiles { get; set; }
        public bool EnableConsoleLogging { get; set; } = true;
        public bool EnableFileLogging { get; set; } = true;
        public NLog.LogLevel ConsoleLogLevel { get; set; }
    }
}
