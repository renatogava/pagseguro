namespace pagSeguro.Api.Models
{
    public class CreditCardRequest
    {
        public Customer Customer { get; set; }

        public CreditCardInfo CreditCardInfo { get; set; }

        public decimal Amount { get; set; }
    }

    public class CreditCardInfo
    {
        public string holderName { get; set; }
        public string cardNumber { get; set; }
        public string expirationDate { get; set; }
        public string verificationCode { get; set; }
    }

    public class Customer
    {

        public string fullName { get; set; }

        public string cpf { get; set; }

        public string phone { get; set; }

        public string email { get; set; }

        public Address address { get; set; }
    }

    public class Address
    {
        public string postalCode { get; set; }
        public string street { get; set; }
        public string number { get; set; }
        public string complement { get; set; }
        public string neighbourhood { get; set; }
        public string city { get; set; }
        public string state { get; set; }
    }
}
