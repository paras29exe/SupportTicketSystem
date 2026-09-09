using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using SupportTicketSystem.Core.Entity;
using SupportTicketSystem.Core.Enums;
using SupportTicketSystem.Core.Models;

// Set the string in the environment variable ConnectionStrings__DefaultConnection to your database connection string before running the program.
string? connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection");

if (string.IsNullOrWhiteSpace(connectionString))
{
    Console.WriteLine("Please set environment variable ConnectionStrings__DefaultConnection with your DB connection string.");
    return;
}

List<Ticket> tickets = new();
List<Agent> agents = new();
List<Customer> customers = new();

async Task LoadAllAsync()
{
    tickets = await LoadTicketsAsync(connectionString);
    agents = await LoadAgentsAsync(connectionString);
    customers = await LoadCustomersAsync(connectionString);

    Console.WriteLine($"\nLoaded {tickets.Count} tickets, {agents.Count} agents, {customers.Count} customers from database.\n");
}

async Task<List<Ticket>> LoadTicketsAsync(string conn)
{
    List<Ticket> list = new List<Ticket>();

    try
    {
        using (SqlConnection connection = new SqlConnection(conn))
        {
            connection.Open();

            SqlCommand cmd = new SqlCommand();
            cmd.Connection = connection;
            cmd.CommandText = "SELECT id, title, description, priority, status, customerId, agentId, createdAt, closedAt FROM tickets";

            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    Ticket t = new Ticket();

                    t.id = reader.GetInt32(reader.GetOrdinal("id"));
                    t.title = reader.IsDBNull(reader.GetOrdinal("title")) ? string.Empty : reader.GetString(reader.GetOrdinal("title"));
                    t.description = reader.IsDBNull(reader.GetOrdinal("description")) ? null : reader.GetString(reader.GetOrdinal("description"));

                    // priority and status are stored as strings in the DB
                    if (!reader.IsDBNull(reader.GetOrdinal("priority")))
                    {
                        string s = reader.GetString(reader.GetOrdinal("priority"));
                        t.priority = Enum.TryParse<PriorityValues>(s, true, out PriorityValues pv) ? pv : PriorityValues.low;
                    }

                    if (!reader.IsDBNull(reader.GetOrdinal("status")))
                    {
                        string s = reader.GetString(reader.GetOrdinal("status"));
                        t.status = Enum.TryParse<StatusValues>(s, true, out StatusValues sv) ? sv : StatusValues.open;
                    }

                    t.customerId = reader.IsDBNull(reader.GetOrdinal("customerId")) ? 0 : reader.GetInt32(reader.GetOrdinal("customerId"));
                    t.agentId = reader.IsDBNull(reader.GetOrdinal("agentId")) ? null : reader.GetInt32(reader.GetOrdinal("agentId"));
                    t.createdAt = reader.IsDBNull(reader.GetOrdinal("createdAt")) ? DateTime.UtcNow : reader.GetDateTime(reader.GetOrdinal("createdAt"));
                    t.closedAt = reader.IsDBNull(reader.GetOrdinal("closedAt")) ? null : reader.GetDateTime(reader.GetOrdinal("closedAt"));

                    list.Add(t);
                }
            }
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine();
        Console.WriteLine($"Error loading tickets: {ex}");
    }

    return list;
}

async Task<List<Agent>> LoadAgentsAsync(string conn)
{
    List<Agent> list = new List<Agent>();

    try
    {
        using (SqlConnection connection = new SqlConnection(conn))
        {
            connection.Open();

            SqlCommand cmd = new SqlCommand();
            cmd.Connection = connection;
            cmd.CommandText = "SELECT id, name, email, department, isActive, createdAt FROM agents";

            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    Agent a = new Agent();
                    a.id = reader.GetInt32(reader.GetOrdinal("id"));
                    a.name = reader.IsDBNull(reader.GetOrdinal("name")) ? string.Empty : reader.GetString(reader.GetOrdinal("name"));
                    a.email = reader.IsDBNull(reader.GetOrdinal("email")) ? string.Empty : reader.GetString(reader.GetOrdinal("email"));
                    a.department = reader.IsDBNull(reader.GetOrdinal("department")) ? null : reader.GetString(reader.GetOrdinal("department"));
                    a.isActive = reader.IsDBNull(reader.GetOrdinal("isActive")) ? true : reader.GetBoolean(reader.GetOrdinal("isActive"));
                    a.createdAt = reader.IsDBNull(reader.GetOrdinal("createdAt")) ? DateTime.UtcNow : reader.GetDateTime(reader.GetOrdinal("createdAt"));

                    list.Add(a);
                }
            }
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine();
        Console.WriteLine($"Error loading agents: {ex.Message}");
    }

    return list;
}

async Task<List<Customer>> LoadCustomersAsync(string conn)
{
    List<Customer> list = new List<Customer>();

    try
    {
        using (SqlConnection connection = new SqlConnection(conn))
        {
            connection.Open();

            SqlCommand cmd = new SqlCommand();
            cmd.Connection = connection;
            cmd.CommandText = "SELECT id, name, email, phone, createdAt FROM customers";

            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    Customer c = new Customer();
                    c.id = reader.GetInt32(reader.GetOrdinal("id"));
                    c.name = reader.IsDBNull(reader.GetOrdinal("name")) ? string.Empty : reader.GetString(reader.GetOrdinal("name"));
                    c.email = reader.IsDBNull(reader.GetOrdinal("email")) ? string.Empty : reader.GetString(reader.GetOrdinal("email"));
                    c.phone = reader.IsDBNull(reader.GetOrdinal("phone")) ? null : reader.GetString(reader.GetOrdinal("phone"));
                    c.createdAt = reader.IsDBNull(reader.GetOrdinal("createdAt")) ? DateTime.UtcNow : reader.GetDateTime(reader.GetOrdinal("createdAt"));

                    list.Add(c);
                }
            }
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine();
        Console.WriteLine($"Error loading customers: {ex.Message}");
    }

    return list;
}

// Menu loop
await LoadAllAsync();

while (true)
{
    Console.WriteLine("Menu:");
    Console.WriteLine("1) List all tickets");
    Console.WriteLine("2) Show open tickets");
    Console.WriteLine("3) Search tickets by title");
    Console.WriteLine("4) Group count by status");
    Console.WriteLine("5) Count tickets per agent");
    Console.WriteLine("6) Find ticket by exact title");
    Console.WriteLine("7) Any high-priority open tickets?");
    Console.WriteLine("8) Reload data from database");
    Console.WriteLine("0) Exit");
    Console.Write("Choose an option: ");

    var opt = Console.ReadLine();
    Console.WriteLine();

    if (opt == "0") return;

    if (opt == "8")
    {
        await LoadAllAsync();
        continue;
    }

    switch (opt)
    {
        case "1":
        {
            foreach (Ticket t in tickets)
            {
                Console.WriteLine($"[{t.id}] {t.title} (Status: {t.status}, Priority: {t.priority})");
            }

            break;
        }

        case "2":
        {
            List<Ticket> open = tickets.Where(t => t.status == StatusValues.open).ToList();
            foreach (Ticket t in open)
            {
                Console.WriteLine($"[{t.id}] {t.title} (Priority: {t.priority})");
            }

            break;
        }

        case "3":
        {
            Console.Write("Enter search term: ");
            string term = Console.ReadLine() ?? string.Empty;
            List<Ticket> founds = tickets.Where(t => !string.IsNullOrEmpty(t.title) && t.title.Contains(term, StringComparison.OrdinalIgnoreCase)).ToList();
            foreach (Ticket t in founds)
            {
                Console.WriteLine($"[{t.id}] {t.title}");
            }

            break;
        }

        case "4":
        {
            List<StatusCount> groups = tickets.GroupBy(t => t.status).Select(g => new StatusCount { status = g.Key, count = g.Count() }).ToList();

            foreach (StatusCount g in groups)
            {
                Console.WriteLine($"{g.status}: {g.count}");
            }

            break;
        }

        case "5":
        {
            List<AgentTicketCount> perAgent = tickets.Where(t => t.agentId.HasValue).GroupBy(t => t.agentId).Select(g => new AgentTicketCount { agentId = g.Key, count = g.Count() }).ToList();

            foreach (AgentTicketCount data in perAgent)
            {
                Agent? ag = agents.FirstOrDefault(x => x.id == data.agentId);

                Console.WriteLine($"[{data.agentId}] {ag.name} : {data.count}");
            }

            break;
        }

        case "6":
        {
            Console.Write("Enter exact title: ");
            string exactTitle = Console.ReadLine() ?? string.Empty;

            Ticket? ex = tickets.FirstOrDefault(t => string.Equals(t.title, exactTitle, StringComparison.OrdinalIgnoreCase));
            Console.WriteLine(ex != null ? $"Found: [{ex.id}] {ex.title} (Status: {ex.status})" : "Not found");

            break;
        }

        case "7":
        {
            bool any = tickets.Any(t => t.priority == PriorityValues.high && t.status == StatusValues.open);
            Console.WriteLine(any ? "There is at least one high-priority open ticket." : "No high-priority open tickets.");

            break;
        }

        default:
        {
            Console.WriteLine("Invalid option");
            break;
        }
    }

    Console.WriteLine();
}

