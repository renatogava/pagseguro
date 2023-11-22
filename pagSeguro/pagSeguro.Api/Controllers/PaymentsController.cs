using Microsoft.AspNetCore.Mvc;
using pagSeguro.Api.Enums;
using pagSeguro.Api.Integrations;
using pagSeguro.Api.Models;
using pagSeguro.Api.Services;
using pagSeguro.Api.Services.Models;

namespace pagSeguro.Api.Controllers
{
    [Route("payments")]
    [ApiController]
    public class PaymentsController : ControllerBase
    {
        private readonly ILogger<PaymentsController> _logger;
        private readonly IPaymentService _paymentService;
        private readonly IPagSeguroService _pagSeguroService;

        public PaymentsController(ILogger<PaymentsController> logger, 
            IPaymentService paymentService, 
            IPagSeguroService pagSeguroService)
        {
            _logger = logger;
            _paymentService = paymentService;
            _pagSeguroService = pagSeguroService;
        }

        [HttpPost("creditcard")]
        public async Task<IActionResult> CreditCard(CreditCardRequest request)
        {
            try
            {
                var processPaymentRequest = BuildProcessPaymentRequest(request);

                var processPaymentResponse = await _paymentService.ProcessPayment(processPaymentRequest);

                var response = new CreditCardResponse();

                if (processPaymentResponse != null && string.IsNullOrEmpty(processPaymentResponse.ErrorMessage))
                {

                    return Ok(response);
                }
                else
                {
                    return UnprocessableEntity(response);
                }
            }
            catch (System.Exception ex)
            {
                _logger.LogError($"{ex.Message}");
                return UnprocessableEntity();
            }
        }

        [HttpPost("getsessionid")]
        public async Task<IActionResult> GetSessionId()
        {
            try
            {
                var sessionId = await _pagSeguroService.GetSessionId();

                var response = new GetSessionIdResponse();

                if (!string.IsNullOrEmpty(sessionId))
                {
                    response.sessionId = sessionId;

                    return Ok(response);
                }
                else
                {
                    return UnprocessableEntity(response);
                }
            }
            catch (System.Exception ex)
            {
                _logger.LogError($"{ex.Message}");
                return BadRequest(ex.Message);
            }
        }

        private ProcessPaymentRequest BuildProcessPaymentRequest(CreditCardRequest request)
        {
            var processPaymentRequest = new ProcessPaymentRequest();
            processPaymentRequest.PaymentMethodId = (int)PaymentMethod.CartaoCredito;
            processPaymentRequest.Customer = new Services.Models.Customer();
            processPaymentRequest.Customer.Email = request.customer.email;
            processPaymentRequest.Customer.Address = new Services.Models.Address
            {
                Street = request.customer.address.street,
                City = request.customer.address.city,
                Complement = request.customer.address.complement,
                Neighbourhood = request.customer.address.neighbourhood,
                Number = request.customer.address.number,
                State = request.customer.address.state,
                ZipPostalCode = request.customer.address.zipPostalCode
            };
            processPaymentRequest.CreditCard.CreditCardToken = request.creditCardInfo.creditCardToken;
            processPaymentRequest.CreditCard.HolderCpf = request.creditCardInfo.holderCpf;
            processPaymentRequest.CreditCard.HolderCodeArea = GetCodeArea(request.creditCardInfo.holderPhone);
            processPaymentRequest.CreditCard.HolderPhone = GetPhone(request.creditCardInfo.holderPhone);
            processPaymentRequest.CreditCard.HolderBirthDate = request.creditCardInfo.holderBirthDate;
            processPaymentRequest.CreditCard.HolderName = request.creditCardInfo.holderName;

            processPaymentRequest.CreditCard.NumberOfPayments = request.creditCardInfo.numberOfPayments;

            processPaymentRequest.TotalPrice = request.amount;
            processPaymentRequest.SenderHash = request.senderHash;

            return processPaymentRequest;
        }

        private string GetPhone(string phone)
        {
            return phone;
        }

        private string GetCodeArea(string phone)
        {
            return phone;
        }

    }
}
