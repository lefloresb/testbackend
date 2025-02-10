namespace AntiFraud.Api.Endpoints
{
    public class CreateTransactionRequest
    {
        public const string Route = "/transactions";
        public BodyTransaction bodyTransaction { get; set; } = new ();
    }
    public class BodyTransaction
    {
        public Guid SourceAccountId { get; set; } 
        public Guid TargetAccountId { get; set; } 
        public int TransferTypeId { get; set; } 
        public decimal Value { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
