using AntiFraud.Core.Entities;
using AntiFraud.Core.Enums;
using AntiFraud.Core.Interface;
using AntiFraud.Infrastructure.Kafka;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace AntiFraud.Infrastructure.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly AntiFraudDbContext _context;
        private readonly KafkaProducer _kafkaProducer;
        public TransactionService(AntiFraudDbContext context, KafkaProducer kafkaProducer)
        {
            _context = context;
            _kafkaProducer = kafkaProducer;
        }
        public async Task<Transaction> GetTransactionByIdAsync(Guid transactionExternalId)
        {
            return await _context.Transactions
                .FirstOrDefaultAsync(t => t.Id == transactionExternalId);
        }

        public async Task AddTransactionAsync(Transaction transaction)
        {
            await _context.Transactions.AddAsync(transaction);
            await _context.SaveChangesAsync();            
        }

        public async Task SendTransactionForValidationAsync(Transaction transaction)
        {
            await _kafkaProducer.SendTransactionAsync(transaction);
        }
        
        public async Task UpdateTransactionStatusAsync(Guid transactionId, TransactionStatus newStatus)
        {
            var transaction = await _context.Transactions.FindAsync(transactionId);
            if (transaction != null)
            {
                transaction.Status = newStatus;
                await _context.SaveChangesAsync();
            }
        }
       
        public async Task<decimal> GetDailyAccumulatedAmountAsync(Guid userId, DateTime date)
        {
            return await _context.Transactions
                .Where(t => t.SourceAccountId == userId && t.CreatedAt.Date == date.Date)
                .SumAsync(t => t.Value);
        }

        public async Task<List<Transaction>> GetTransactionsByCustomerAndDateAsync(Guid transactionExternalId, DateTime date)
        {
            return await _context.Transactions
                .Where(t => t.TransactionExternalId == transactionExternalId && t.CreatedAt.Date == date)
                .ToListAsync();
        }

    }
}
