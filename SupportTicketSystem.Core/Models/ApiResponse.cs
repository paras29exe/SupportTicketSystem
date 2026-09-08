using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupportTicketSystem.Core.Responses
{
    public class ApiResponse<T>
    {
        public bool success { get; }
        public int status { get; }
        public string message { get; } = string.Empty;
        public T? data { get; }

        public ApiResponse(int status, string message, T? data)
        {
            this.success = true;
            this.status = status;
            this.message = message;
            this.data = data;
        }
    }

    public class ApiResponse
    {
        public bool success { get; }
        public int status { get; }
        public string message { get; } = string.Empty;

        public ApiResponse(int status, string message)
        {
            this.success = true;
            this.status = status;
            this.message = message;
        }
    }

    public class DataWrapper<T>
    {
        public T data { get; }

        public DataWrapper(T data)
        {
            this.data = data;
        }
    }
}
