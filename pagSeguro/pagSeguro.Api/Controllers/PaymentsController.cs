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
        private readonly IPaymentService _paymentService;
        private readonly IPagSeguroService _pagSeguroService;
        private readonly ILogService _logService;

        public PaymentsController(ILogger<PaymentsController> logger, 
            IPaymentService paymentService, 
            IPagSeguroService pagSeguroService,
            ILogService logService)
        {
            _paymentService = paymentService;
            _pagSeguroService = pagSeguroService;
            _logService = logService;
        }

        [HttpPost("creditcard")]
        [BasicAuthentication]
        public async Task<IActionResult> CreditCard(CreditCardRequest request)
        {
            try
            {
                var processPaymentRequest = BuildProcessPaymentRequestCartaoCredito(request);

                var processPaymentResponse = await _paymentService.ProcessPayment(processPaymentRequest);

                var response = new CreditCardResponse();
                response.succeeded = processPaymentResponse.Succeeded;
                response.errorMessage = processPaymentResponse.ErrorMessage;
                response.transactionid = processPaymentResponse.CreditCardInfo.TransactionId;
                response.paymentstatus = processPaymentResponse.PaymentStatus;

                return Ok(response);
            }
            catch (System.Exception ex)
            {
                _logService.LogError(ex.ToString());
                return UnprocessableEntity(ex.Message);
            }
        }

        [HttpPost("boleto")]
        [BasicAuthentication]
        public async Task<IActionResult> Boleto(BoletoRequest request)
        {
            try
            {
                var processPaymentRequest = BuildProcessPaymentRequestBoleto(request);

                var processPaymentResponse = await _paymentService.ProcessPayment(processPaymentRequest);

                var response = new BoletoResponse();
                response.succeeded = processPaymentResponse.Succeeded;
                response.errorMessage = processPaymentResponse.ErrorMessage;
                response.boletourl = processPaymentResponse.BoletoInfo.Url;
                response.transactionid = processPaymentResponse.BoletoInfo.TransactionId;
                response.paymentstatus = processPaymentResponse.PaymentStatus;

                return Ok(response);
            }
            catch (System.Exception ex)
            {
                _logService.LogError(ex.ToString());
                return UnprocessableEntity(ex.Message);
            }
        }

        [HttpPost("status")]
        [BasicAuthentication]
        public async Task<IActionResult> Status(StatusRequest request)
        {
            try
            {
                var checkStatusRequest = new CheckStatusRequest
                {
                    TransactionId = request.transactionid
                };

                var checkStatusResponse = await _paymentService.CheckStatus(checkStatusRequest);

                var response = new StatusResponse();
                response.paymentstatus = checkStatusResponse.PaymentStatus;

                return Ok(response);
            }
            catch (System.Exception ex)
            {
                _logService.LogError(ex.ToString());
                return UnprocessableEntity(ex.Message);
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
                _logService.LogError(ex.ToString());
                return UnprocessableEntity(ex.Message);
            }
        }

        private ProcessPaymentRequest BuildProcessPaymentRequestCartaoCredito(CreditCardRequest request)
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

            processPaymentRequest.CreditCard = new CreditCard
            {
                CreditCardToken = request.creditCardInfo.creditCardToken,
                HolderCpf = FormatCpf(request.customer.cpf),
                HolderCodeArea = GetCodeArea(request.customer.phone),
                HolderPhone = GetPhone(request.customer.phone),
                HolderBirthDate = request.customer.birthDate,
                HolderName = request.creditCardInfo.holderName,

                NumberOfPayments = request.creditCardInfo.numberOfPayments,
                InstallmentValue = request.creditCardInfo.installmentValue
            };

            processPaymentRequest.TotalPrice = request.amount;
            processPaymentRequest.SenderHash = request.senderHash;

            return processPaymentRequest;
        }

        private ProcessPaymentRequest BuildProcessPaymentRequestBoleto(BoletoRequest request)
        {
            var processPaymentRequest = new ProcessPaymentRequest();
            processPaymentRequest.PaymentMethodId = (int)PaymentMethod.Boleto;
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
