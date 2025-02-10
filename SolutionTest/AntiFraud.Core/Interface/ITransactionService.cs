using AntiFraud.Core.Entities;
using AntiFraud.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntiFraud.Core.Interface
{
    public interface ITransactionService
    {
        Task AddTransactionAsync(Transaction transaction);
        Task<Transaction> GetTransactionByIdAsync(Guid transactionExternalId);
        Task SendTransactionForValidationAsync(Transaction transaction);
        Task UpdateTransactionStatusAsync(Guid transactionId, TransactionStatus newStatus);
        Task<decimal> GetDailyAccumulatedAmountAsync(Guid userId, DateTime date);
        Task<List<Transaction>> GetTransactionsByCustomerAndDateAsync(Guid transactionExternalId, DateTime date);
    }
}
