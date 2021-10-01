using HealtChecker.Shared.Exceptions;
using Newtonsoft.Json;
using System;

namespace HealtChecker.Shared.Models
{
    public class LogItem
    {
        public string LogType { get; set; }
        public DateTime ErrorTime { get; set; }
        public string Content { get; set; }
        public Guid Id { get; set; }

        public string ErrorMessage { get; set; }

        public static LogItem CreateLogItemFromException(Exception ex)
        {
            Guid logId = Guid.NewGuid();

            string errorMessage = $"Unexpected error occured with Reference Id : {logId}";

            if(ex is EntityNotFoundException)
            {
                errorMessage = $"Entity {(ex as EntityNotFoundException).EntityName} not found with Id : {(ex as EntityNotFoundException).EntityId}";
            }

            LogItem logItem = new LogItem()
            {
                Content = JsonConvert.SerializeObject(ex),
                ErrorTime = DateTime.UtcNow,
                LogType = ex.GetType().FullName,
                Id = logId,
                ErrorMessage = errorMessage
            };

            return logItem;
        }

    }
}
