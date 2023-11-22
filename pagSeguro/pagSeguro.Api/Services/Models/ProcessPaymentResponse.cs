namespace pagSeguro.Api.Services.Models
{
    public class ProcessPaymentResponse
    {
        public int OrderId { get; set; }
        public bool Succeeded { get; set; }
        public CreditCardInfo CreditCardInfo { get; set; }
        public BoletoInfo BoletoInfo { get; set; }
        public PixInfo PixInfo { get; set; }
        public string ErrorMessage { get; set; }

    }

    public class BoletoInfo
    {
        public string Url { get; set; }
        public string BarCode { get; set; }
        public string Numero { get; set; }
        public string DataVencimento { get; set; }
        public string Cedente { get; set; }
        public string Amount { get; set; }
        public Guid TransactionId { get; set; }
        public string OrderId { get; set; }
    }

    public class CreditCardInfo
    {
        public string CaptureTransactionId { get; set; }
        public string CaptureTransactionResult { get; set; }
        public string AuthorizationTransactionId { get; set; }
        public string AuthorizationTransactionResult { get; set; }
        public string OrderIdentifier { get; set; }
    }

    public class PixInfo
    {
        public string ExpirationDate { get; set; }
        public string OrderId { get; set; }
        public string QrCode { get; set; }
        public string QrCodeText { get; set; }
    }
}
