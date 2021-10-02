using System;

namespace HealtChecker.Shared.Exceptions
{
    public class BaseException : Exception
    {
        public string ServiceName { get; set; }
        public string MethodName { get; set; }

    }
}
