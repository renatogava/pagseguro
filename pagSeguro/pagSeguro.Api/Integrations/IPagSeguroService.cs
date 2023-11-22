using pagSeguro.Api.Integrations.Models;
using pagSeguro.Api.Models;

namespace pagSeguro.Api.Integrations
{
    public interface IPagSeguroService
    {
        Task<PagSeguroBoletoResponse> ChargeBoleto(PagSeguroBoletoRequest request);
        Task<PagSeguroCardResponse> ChargeCreditCard(PagSeguroCardRequest request);
        Task<bool> Notification(PagSeguroCardResponse request);
        Task<string> GetSessionId();
        Task<PagSeguroTransactionResponse> CreatePayment(PagSeguroTransactionRequest request);
        Task<PagSeguroTransactionResponse> CheckStatus(string code);
    }
}