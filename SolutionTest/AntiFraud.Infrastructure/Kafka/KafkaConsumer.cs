using AntiFraud.Core.Entities;
using AntiFraud.Core.Enums;
using AntiFraud.Core.Interface;
using AntiFraud.Infrastructure.Config;
using Confluent.Kafka;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text.Json;


namespace AntiFraud.Infrastructure.Kafka
{
    public class KafkaConsumer : BackgroundService
    {
        private readonly string _bootstrapServers;
        private readonly string _topic = "transaction-validation";
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ILogger<KafkaConsumer> _logger;

        public KafkaConsumer(IOptions<KafkaSettings> kafkaSettings, IServiceScopeFactory serviceScopeFactory, ILogger<KafkaConsumer> logger)
        {
            _bootstrapServers = kafkaSettings.Value.BootstrapServers;
            _serviceScopeFactory = serviceScopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var config = new ConsumerConfig
            {
                BootstrapServers = _bootstrapServers,
                GroupId = "anti-fraud-group",
                AutoOffsetReset = AutoOffsetReset.Earliest
            };

            using var consumer = new ConsumerBuilder<Ignore, string>(config).Build();
            consumer.Subscribe(_topic);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var consumeResult = consumer.Consume(stoppingToken);
                    var message = consumeResult.Message.Value;

                    var transaction = JsonSerializer.Deserialize<Transaction>(message);

                    if (transaction != null)
                    {
                        await ValidateAndUpdateTransactionAsync(transaction);
                    }
                }
                catch (ConsumeException ex)
                {
                    Console.WriteLine($"Error consuming message: {ex.Error.Reason}");
                }
            }

            consumer.Close();
        }

        private async Task ValidateAndUpdateTransactionAsync(Transaction transaction)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var transactionService = scope.ServiceProvider.GetRequiredService<ITransactionService>();
            
            var customerTransactionsToday = await transactionService.GetTransactionsByCustomerAndDateAsync(
                transaction.TransactionExternalId, transaction.CreatedAt.Date);
            
            var totalAmountToday = customerTransactionsToday.Sum(t => t.Value);
            
            if (transaction.Value > 2000 || (totalAmountToday + transaction.Value) > 20000)
            {
                transaction.Status = TransactionStatus.Rejected;
            }
            else
            {
                transaction.Status = TransactionStatus.Approved;
            }

            await transactionService.UpdateTransactionStatusAsync(transaction.Id, transaction.Status);
        }

    }
}
