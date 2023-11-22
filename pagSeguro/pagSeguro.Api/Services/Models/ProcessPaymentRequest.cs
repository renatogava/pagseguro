using System.ComponentModel.DataAnnotations;

namespace pagSeguro.Api.Services.Models
{
    public class ProcessPaymentRequest
    {
        [Required]
        public Customer Customer { get; set; }
        [Required]
        public int PaymentMethodId { get; set; }
        [Required]
        public CreditCard CreditCard { get; set; }
        public decimal? ShippingOptionRate { get; set; }
        public string SenderHash { get; set; }

        public decimal ShippingPrice { get; set; }

        public decimal TotalPrice { get; set; }
    }

    public class CreditCard
    {
        public string Number { get; set; }
        public string Brand { get; set; }
        public int Type { get; set; }
        public string HolderName { get; set; }
        public string ExpirationMonth { get; set; }
        public string ExpirationYear { get; set; }
        public string SecurityCodeCVV { get; set; }
        public int NumberOfPayments { get; set; }
        public string PagSeguroSenderHash { get; set; }
        public string PagSeguroTokenCard { get; set; }
        public string HolderBirthDate { get; set; }
        public string HolderCpf { get; set; }
        public string HolderCodeArea { get; set; }
        public string HolderPhone { get; set; }
        public string CreditCardToken { get; set; }
    }

    public class Customer
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string CPF { get; set; }

        public Address Address { get; set; }
    }

    public class Address
    {
        public string Street { get; set; }
        public string Number { get; set; }
        public string Neighbourhood { get; set; }
        public string Complement { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipPostalCode { get; set; }
    }
}
