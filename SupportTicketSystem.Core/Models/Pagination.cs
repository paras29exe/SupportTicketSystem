using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupportTicketSystem.Core.Models
{
    public class Pagination
    {
        public int page {  get; set; }
        public int totalPages { get; set; }
        public int pageSize { get; set; }
        public int maxPageSize { get; set; }

        public Pagination(int  page, int totalPages, int pageSize, int maxPageSize)
        {
            this.page = page;
            this.totalPages = totalPages; 
            this.pageSize = pageSize;
            this.maxPageSize = maxPageSize;
        }
    }
}
