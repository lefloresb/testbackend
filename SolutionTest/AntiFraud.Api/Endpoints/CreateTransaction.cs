using AntiFraud.Core.Entities;
using AntiFraud.Core.Interface;



namespace AntiFraud.Api.Endpoints
{    
    using FastEndpoints;
    using Microsoft.AspNetCore.Http.HttpResults;    
    using Microsoft.Extensions.Logging;
    using System.Threading;
    using System.Threading.Tasks;

    public class CreateTransaction : Endpoint<CreateTransactionRequest, Results<Ok<Transaction>, ProblemDetails>>
    {
        private readonly ITransactionService _transactionService;
        private readonly ILogger<CreateTransaction> _logger;
        private readonly AutoMapper.IMapper _mapper;

        public CreateTransaction(ITransactionService transactionService, ILogger<CreateTransaction> logger, AutoMapper.IMapper mapper)
        {
            _transactionService = transactionService;
            _logger = logger;
            _mapper = mapper;
        }

        public override void Configure()
        {
            Version(1);
            Post(CreateTransactionRequest.Route);
            AllowAnonymous();
            Summary(s =>
            {
                s.Summary = "Create a new transaction.";
                s.Description = "This endpoint allows you to create a new transaction.";
            });
            Description(builder => builder.ProducesProblemDetails(400, "application/json"));
        }

        public override async Task<Results<Ok<Transaction>, ProblemDetails>> ExecuteAsync(
            CreateTransactionRequest request,
            CancellationToken cancellationToken)
        {
            try
            {
                var transaction = _mapper.Map<Transaction>(request);
                await _transactionService.AddTransactionAsync(transaction);
                _logger.LogInformation("Transaction created successfully.");
                await _transactionService.SendTransactionForValidationAsync(transaction);
                _logger.LogInformation("send Transaction successfully.");
                return TypedResults.Ok(transaction);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating transaction.");

                AddError("Error al crear la transaccion", "CreateTransaction_error");
                return new ProblemDetails(ValidationFailures);                
            }
        }
    }

}
