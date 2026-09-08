using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupportTicketSystem.Core.Models
{
    public class PaginatedResponse<T>
    {
        public IEnumerable<T> data { get; set; } = Array.Empty<T>();
        public Pagination pagination { get; set; }

        public PaginatedResponse(IEnumerable<T> data, Pagination pagination)
        {
            this.data = data;
            this.pagination = pagination;
        }
    }
}
