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
        public string holderCpf { get; set; }
        public string holderPhone { get; set; }
        public string holderBirthDate { get; set; }
        public int numberOfPayments { get; set; }
    }

    public class Customer
    {
        public string email { get; set; }

        public Address address { get; set; }
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
