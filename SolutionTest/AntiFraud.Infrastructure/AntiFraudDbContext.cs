using AntiFraud.Core.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntiFraud.Infrastructure
{
    public class AntiFraudDbContext : DbContext
    {
        public DbSet<Transaction> Transactions { get; set; }
        public AntiFraudDbContext(DbContextOptions<AntiFraudDbContext> options) : base(options) { }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Transaction>().ToTable("Transactions");
        }
    }
}
