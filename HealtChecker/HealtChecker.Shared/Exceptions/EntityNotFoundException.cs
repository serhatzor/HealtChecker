using System;

namespace HealtChecker.Shared.Exceptions
{
    public class EntityNotFoundException : BaseException
    {
        public string EntityName { get; set; }
        public Guid EntityId { get; set; }
    }
}
