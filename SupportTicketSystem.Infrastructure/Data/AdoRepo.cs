using Microsoft.Extensions.Configuration;
using SupportTicketSystem.Core.Dtos;
using SupportTicketSystem.Core.Interfaces;
using System;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SupportTicketSystem.Core.Enums;
using SupportTicketSystem.Core.Exceptions;

namespace SupportTicketSystem.Infrastructure.Data
{
    public class AdoRepo(IConfiguration _configuration) : IAdoRepo
    {
        private readonly IConfiguration configuration = _configuration;

        private readonly string connectionString = _configuration.GetConnectionString("DefaultConnection")!;

        public async Task<IEnumerable<ResponseTicketDto>> GetAllTicketsAsync()
        {
            List<ResponseTicketDto> tickets = new List<ResponseTicketDto>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                SqlCommand cmd = new SqlCommand();
                cmd.Connection = connection;
                cmd.CommandText = "Select * from ticketsDetails";


                using (SqlDataReader reader = cmd.ExecuteReader())
                {

                    while(await reader.ReadAsync())
                    {
                        ResponseTicketDto t = new();

                        t.id = reader.GetInt32(reader.GetOrdinal("ticketId"));
                        t.title = reader.GetString(reader.GetOrdinal("title"));
                        t.description = reader.IsDBNull(reader.GetOrdinal("description")) ? null : reader.GetString(reader.GetOrdinal("description"));
                        t.priority = Enum.Parse<PriorityValues>(reader.GetString(reader.GetOrdinal("priority")));
                        t.status = Enum.Parse<StatusValues>(reader.GetString(reader.GetOrdinal("status")));
                        t.customerId = reader.GetInt32(reader.GetOrdinal("customerId"));
                        t.customerName = reader.GetString(reader.GetOrdinal("customerName"));
                        t.agentId = reader.IsDBNull(reader.GetOrdinal("agentId")) ? null : reader.GetInt32(reader.GetOrdinal("agentId"));
                        t.agentName = reader.IsDBNull(reader.GetOrdinal("agentName")) ? null : reader.GetString(reader.GetOrdinal("agentName"));

                        tickets.Add(t);
                    }
                }

                return tickets;
            }

        }

        public async Task<IEnumerable<ResponseTicketDto>> GetTicketByCustomerIdAsync(int customerId)
        {
            List<ResponseTicketDto> tickets = new List<ResponseTicketDto>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();

                using SqlCommand cmd = new SqlCommand("getTicketsByCustomer", connection);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@customerId", customerId);

                using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        ResponseTicketDto t = new();

                        t.id = reader.GetInt32(reader.GetOrdinal("id"));
                        t.title = reader.GetString(reader.GetOrdinal("title"));
                        t.description = reader.IsDBNull(reader.GetOrdinal("description")) ? null : reader.GetString(reader.GetOrdinal("description"));

                        // priority and status are stored as strings in the DB; parse to enums
                        t.priority = Enum.Parse<PriorityValues>(reader.GetString(reader.GetOrdinal("priority")));
                        t.status = Enum.Parse<StatusValues>(reader.GetString(reader.GetOrdinal("status")));

                        t.customerId = reader.GetInt32(reader.GetOrdinal("customerId"));
                        t.agentId = reader.IsDBNull(reader.GetOrdinal("agentId")) ? null : reader.GetInt32(reader.GetOrdinal("agentId"));

                        t.createdAt = reader.GetDateTime(reader.GetOrdinal("createdAt"));

                        tickets.Add(t);
                    }
                }

                return tickets;
            }
        }

        public async Task<int> UpdateTicketStatusAsync(int ticketId, StatusValues newStatus)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();

                using SqlCommand cmd = new SqlCommand("updateTicketStatus", connection);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ticketId", ticketId);
                cmd.Parameters.AddWithValue("@newStatus", newStatus.ToString());

                SqlParameter response = cmd.Parameters.Add("@returnVal", SqlDbType.Int);
                response.Direction = ParameterDirection.ReturnValue;

                await cmd.ExecuteNonQueryAsync();

                return (int)response.Value;
            }
        }
    }
}
