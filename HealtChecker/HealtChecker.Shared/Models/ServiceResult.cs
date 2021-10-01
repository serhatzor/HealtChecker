using System;

namespace HealtChecker.Shared.Models
{
    public class ServiceResult<T>
    {
        public string ErrorMessage { get; set; }
        public T Data { get; set; }
        public bool IsSuccess
        {
            get
            {
                return string.IsNullOrWhiteSpace(ErrorMessage);
            }
        }


    }
}
