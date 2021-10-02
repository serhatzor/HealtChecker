using System;

namespace HealtChecker.Shared.Exceptions
{
    public class SecurityException : BaseException
    {
        public string EntityName { get; set; }
        public Guid EntityId { get; set; }
        public Guid InvalidOperatorUserId { get; set; }
    }
}
