using AntiFraud.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntiFraud.Core.Entities
{
    public class Transaction
    {
        public Guid Id { get; set; }
        public Guid SourceAccountId { get; set; }
        public Guid TargetAccountId { get; set; } 
        public int TransferTypeId { get; set; }
        public decimal Value { get; set; } 
        public TransactionStatus Status { get; set; }
        public Guid TransactionExternalId { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
