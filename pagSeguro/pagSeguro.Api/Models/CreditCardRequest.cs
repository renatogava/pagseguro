using Newtonsoft.Json;

namespace pagSeguro.Api.Models
{
    public class CreditCardRequest
    {
        public Customer customer { get; set; }

        public CreditCardInfo creditCardInfo { get; set; }

        public decimal amount { get; set; }

        public string senderHash { get; set; }
    }

    public class CreditCardInfo
    {
        public string creditCardToken { get; set; }
        public string holderName { get; set; }
        public int numberOfPayments { get; set; }
        public decimal installmentValue { get; set; }
    }

    public class Customer
    {
        public string email { get; set; }
        public string name { get; set; }
        public string cpf { get; set; }
        public string phone { get; set; }
        public string birthDate { get; set; }
        public Address shippingaddress { get; set; }

        [JsonProperty(Required = Required.Default)]
        public Address? billingaddress { get; set; }

    }

    public class Address
    {
        public string zipPostalCode { get; set; }
        public string street { get; set; }
        public string number { get; set; }
        public string complement { get; set; }
        public string neighbourhood { get; set; }
        public string city { get; set; }
        public string state { get; set; }
    }
}
