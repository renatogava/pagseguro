using Microsoft.AspNetCore.Mvc;
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

        public PaymentsController(ILogger<PaymentsController> logger, IPaymentService paymentService)
        {
            _logger = logger;
            _paymentService = paymentService;
        }

        [HttpPost("creditcard")]
        public async Task<IActionResult> CreditCard(CreditCardRequest request)
        {
            try
            {
                var response = new CreditCardResponse();

                var processPaymentRequest = new ProcessPaymentRequest();

                var processPaymentResponse = await _paymentService.ProcessPayment(processPaymentRequest);

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
    }
}
