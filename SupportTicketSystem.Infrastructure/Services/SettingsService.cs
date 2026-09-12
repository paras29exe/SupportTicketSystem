using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;
using System.Text;
using System.Threading.Tasks;

namespace SupportTicketSystem.Infrastructure.Services
{
    public class SettingsService(IConfiguration configuration)
    {
        public object getSetting()
        {
            object config = new
            {
                defaultPageSize = configuration["Pagination:DefaultPageSize"],
                maxPageSize = configuration["Pagination:MaxPageSize"],

                frontendOrigins = configuration["FrontendOrigins"]?.Split(",").Select(origin => origin.Trim()).ToList() ?? new List<string>()
            };
            return config;
        }
    }
}
