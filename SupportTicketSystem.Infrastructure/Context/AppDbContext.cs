using Microsoft.EntityFrameworkCore;
using SupportTicketSystem.Core.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupportTicketSystem.Infrastructure.Context
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Customer> customers { get; set; }
        public DbSet<Agent> agents { get; set; }
        public DbSet<Ticket> tickets { get; set; }
        public DbSet<TicketNote> ticketNotes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Store enum values as strings in the database while keeping enum types in the entity
            // Took help from here https://learn.microsoft.com/en-us/ef/core/modeling/value-conversions?tabs=data-annotations

            modelBuilder.Entity<Ticket>(b =>
            {
                b.Property(t => t.priority)
                 .HasConversion<string>();

                b.Property(t => t.status)
                 .HasConversion<string>();
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}
