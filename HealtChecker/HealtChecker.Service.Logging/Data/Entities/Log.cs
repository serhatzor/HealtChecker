using HealtChecker.Shared.Models;
using System;

namespace HealtChecker.Service.Logging.Data.Entities
{
    public class Log
    {
        public string LogType { get; set; }
        public DateTime ErrorTime { get; set; }
        public string Content { get; set; }
        public Guid Id { get; set; }
        public string ErrorMessage { get; set; }
        public Channel Channel { get; set; }
    }
}
