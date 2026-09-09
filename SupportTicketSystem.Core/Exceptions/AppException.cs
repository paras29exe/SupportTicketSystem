using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupportTicketSystem.Core.Exceptions
{
    public class AppException : Exception
    {
        public int status { get; }
        public string message { get; } = "";
        public object? loggingDetails { get; } = null;
        public object? errorInfo { get; } = null;

        public AppException(int status, string message, object? loggingDetails = null, object? errorInfoForFrontend = null)
        {
            this.status = status;
            this.message = message;
            this.loggingDetails = loggingDetails;
            this.errorInfo = errorInfoForFrontend;
        }
    }
}
