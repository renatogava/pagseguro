namespace pagSeguro.Api.Services.Models
{
    public class ProcessPaymentResponse
    {
        public ProcessPaymentResponse() 
        {
            CreditCardInfo = new CreditCardInfo();
            BoletoInfo = new BoletoInfo();
            PixInfo = new PixInfo();
        }

        public bool Succeeded { get; set; }
        public int PaymentStatus { get; set; }
        public CreditCardInfo CreditCardInfo { get; set; }
        public BoletoInfo BoletoInfo { get; set; }
        public PixInfo PixInfo { get; set; }
        public string ErrorMessage { get; set; }
    }

    public class BoletoInfo
    {
        public string Url { get; set; }
        public string TransactionId { get; set; }
    }

    public class CreditCardInfo
    {
        public string TransactionId { get; set; }
    }

    public class PixInfo
    {
        public string ExpirationDate { get; set; }
        public string OrderId { get; set; }
        public string QrCode { get; set; }
        public string QrCodeText { get; set; }
    }
}
