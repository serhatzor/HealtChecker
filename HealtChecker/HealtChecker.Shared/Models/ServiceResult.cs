using System;

namespace HealtChecker.Shared.Models
{
    public class ServiceResult<T>
    {
        public Exception Exception { get; set; }
        public T Data { get; set; }
        public bool IsSuccess
        {
            get
            {
                return Exception == null;
            }
        }
    }
}
