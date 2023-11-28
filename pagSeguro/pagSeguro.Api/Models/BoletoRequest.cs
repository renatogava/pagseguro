namespace pagSeguro.Api.Models
{
    public class BoletoRequest
    {
        public Customer customer { get; set; }
        public decimal amount { get; set; }
        public string senderHash { get; set; }
    }
}
