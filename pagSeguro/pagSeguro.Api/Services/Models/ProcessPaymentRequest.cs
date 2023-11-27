using System.ComponentModel.DataAnnotations;

namespace pagSeguro.Api.Services.Models
{
    public class ProcessPaymentRequest
    {
        public Customer Customer { get; set; }
        public int PaymentMethodId { get; set; }
        public CreditCard CreditCard { get; set; }
        public string SenderHash { get; set; }
        public decimal ShippingPrice { get; set; }
        public decimal TotalPrice { get; set; }
    }

    public class CreditCard
    {
        public string HolderName { get; set; }
        public int NumberOfPayments { get; set; }
        public string HolderBirthDate { get; set; }
        public string HolderCpf { get; set; }
        public string HolderCodeArea { get; set; }
        public string HolderPhone { get; set; }
        public string CreditCardToken { get; set; }
        public decimal InstallmentValue { get; set; }
    }

    public class Customer
    {
        public string Email { get; set; }
        public string CodeArea { get; set; }
        public string Phone { get; set; }
        public string BirthDate { get; set; }
        public string Name { get; set; }
        public string CPF { get; set; }
        public Address ShippingAddress { get; set; }
        public Address BillingAddress { get; set; }
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
