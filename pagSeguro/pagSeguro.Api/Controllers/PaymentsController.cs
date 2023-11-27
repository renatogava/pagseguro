using Microsoft.AspNetCore.Mvc;
using pagSeguro.Api.Authentication;
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
        [BasicAuthentication]
        public async Task<IActionResult> CreditCard(CreditCardRequest request)
        {
            try
            {
                var processPaymentRequest = BuildProcessPaymentRequest(request);

                var processPaymentResponse = await _paymentService.ProcessPayment(processPaymentRequest);

                var response = new CreditCardResponse();
                response.succeeded = processPaymentResponse.Succeeded;
                response.errorMessage = processPaymentResponse.ErrorMessage;

                return Ok(response);
            }
            catch (System.Exception ex)
            {
                _logger.LogError($"{ex.Message}");
                return UnprocessableEntity();
            }
        }

        [HttpPost("getsessionid")]
        [BasicAuthentication]
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
            processPaymentRequest.Customer.CodeArea = GetCodeArea(request.customer.phone);
            processPaymentRequest.Customer.Phone = GetPhone(request.customer.phone);
            processPaymentRequest.Customer.BirthDate = request.customer.birthDate;
            processPaymentRequest.Customer.Name = request.customer.name;
            processPaymentRequest.Customer.CPF = FormatCpf(request.customer.cpf);

            processPaymentRequest.Customer.ShippingAddress = new Services.Models.Address
            {
                Street = request.customer.shippingaddress.street,
                City = request.customer.shippingaddress.city,
                Complement = request.customer.shippingaddress.complement,
                Neighbourhood = request.customer.shippingaddress.neighbourhood,
                Number = request.customer.shippingaddress.number,
                State = request.customer.shippingaddress.state,
                ZipPostalCode = FormatPostalCode(request.customer.shippingaddress.zipPostalCode)
            };

            if (request.customer.billingaddress != null)
            {
                processPaymentRequest.Customer.BillingAddress = new Services.Models.Address
                {
                    Street = request.customer.billingaddress.street,
                    City = request.customer.billingaddress.city,
                    Complement = request.customer.billingaddress.complement,
                    Neighbourhood = request.customer.billingaddress.neighbourhood,
                    Number = request.customer.billingaddress.number,
                    State = request.customer.billingaddress.state,
                    ZipPostalCode = FormatPostalCode(request.customer.billingaddress.zipPostalCode)
                };
            }

            processPaymentRequest.CreditCard = new CreditCard();
            processPaymentRequest.CreditCard.CreditCardToken = request.creditCardInfo.creditCardToken;
            processPaymentRequest.CreditCard.HolderCpf = FormatCpf(request.customer.cpf);
            processPaymentRequest.CreditCard.HolderCodeArea = GetCodeArea(request.customer.phone);
            processPaymentRequest.CreditCard.HolderPhone = GetPhone(request.customer.phone);
            processPaymentRequest.CreditCard.HolderBirthDate = request.customer.birthDate;
            processPaymentRequest.CreditCard.HolderName = request.creditCardInfo.holderName;

            processPaymentRequest.CreditCard.NumberOfPayments = request.creditCardInfo.numberOfPayments;
            processPaymentRequest.CreditCard.InstallmentValue = request.creditCardInfo.installmentValue;

            processPaymentRequest.TotalPrice = request.amount;
            processPaymentRequest.SenderHash = request.senderHash;

            return processPaymentRequest;
        }

        private string FormatPostalCode(string zipPostalCode)
        {
            return zipPostalCode.Replace("-", "").Replace(" ", "");
        }

        private string FormatCpf(string cpf)
        {
            return cpf.Replace(".", "").Replace("-", "").Replace(" ", "");
        }

        private string GetPhone(string phone)
        {
            phone = phone.Replace("(", "").Replace(")", "").Replace("-", "").Replace(" ", "");

            phone = phone.Substring(2, phone.Length - 2);

            return phone;
        }

        private string GetCodeArea(string phone)
        {
            phone = phone.Replace("(", "").Replace(")", "").Replace("-", "").Replace(" ", "");

            var code = phone.Substring(0, 2);

            return code;
        }

    }
}
