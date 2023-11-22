using pagSeguro.Api.Services.Models;

namespace pagSeguro.Api.Services
{
    public interface IPaymentService
    {
        Task<ProcessPaymentResponse> ProcessPayment(ProcessPaymentRequest request);
    }
}