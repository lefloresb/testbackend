using AntiFraud.Core.Entities;
using AntiFraud.Core.Enums;
using Confluent.Kafka;
using System.Text.Json;

namespace AntiFraud.Infrastructure.Kafka
{
    public class KafkaProducer
    {
        private readonly IProducer<Null, string> _producer;
        private readonly string _topic = "transaction-validation";

        public KafkaProducer(string bootstrapServers)
        {
            var config = new ProducerConfig { BootstrapServers = bootstrapServers };
            _producer = new ProducerBuilder<Null, string>(config).Build();
        }

        public async Task SendTransactionAsync(Transaction transaction)
        {
            var message = new TransactionUpdateMessage
            {
                TransactionExternalId = transaction.Id,
                Status = transaction.Status
            };

            var jsonMessage = JsonSerializer.Serialize(message);
            try
            {
                var result = await _producer.ProduceAsync(_topic, new Message<Null, string> { Value = jsonMessage });
                Console.WriteLine($"Message sent to {result.TopicPartitionOffset}");
            }
            catch (ProduceException<Null, string> e)
            {
                Console.WriteLine($"Error producing message: {e.Error.Reason}");
            }
        }
    }

    public class TransactionUpdateMessage
    {
        public Guid TransactionExternalId { get; set; }
        public TransactionStatus Status { get; set; }
    }
}
