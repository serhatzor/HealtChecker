using System;

namespace HealtChecker.Shared.Models
{
    public class LogItem
    {
        public string LogType { get; set; }
        public DateTime ErrorTime { get; set; }
        public string Content { get; set; }
        public Guid Id { get; set; }
    }
}
