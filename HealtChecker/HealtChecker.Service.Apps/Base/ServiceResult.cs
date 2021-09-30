﻿using System;

namespace HealtChecker.Service.Metrics.Base
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
