namespace pagSeguro.Api.Integrations.Models
{
    public class PagSeguroTransactionRequest
    {
        public string PaymentMethod { get; set; }
        public string ItemId1 { get; set; }
        public string ItemDescription1 { get; set; }
        public string ItemAmount1 { get; set; }
        public string ItemQuantity1 { get; set; }
        public string NotificationURL { get; set; }
        public string Reference { get; set; }
        public string SenderName { get; set; }
        public string SenderCPF { get; set; }
        public string SenderAreaCode { get; set; }
        public string SenderPhone { get; set; }
        public string SenderEmail { get; set; }
        public string SenderHash { get; set; }
        public string ShippingAddressStreet { get; set; }
        public string ShippingAddressNumber { get; set; }
        public string ShippingAddressComplement { get; set; }
        public string ShippingAddressDistrict { get; set; }
        public string ShippingAddressPostalCode { get; set; }
        public string ShippingAddressCity { get; set; }
        public string ShippingAddressState { get; set; }
        public string ShippingAddressCountry { get; set; }
        public string ShippingType { get; set; }
        public string ShippingCost { get; set; }
        public string CreditCardToken { get; set; }
        public string InstallmentQuantity { get; set; }
        public string InstallmentValue { get; set; }
        public string NoInterestInstallmentQuantity { get; set; }
        public string CreditCardHolderName { get; set; }
        public string CreditCardHolderCPF { get; set; }
        public string CreditCardHolderBirthDate { get; set; }
        public string CreditCardHolderAreaCode { get; set; }
        public string CreditCardHolderPhone { get; set; }
        public string BillingAddressStreet { get; set; }
        public string BillingAddressNumber { get; set; }
        public string BillingAddressComplement { get; set; }
        public string BillingAddressDistrict { get; set; }
        public string BillingAddressPostalCode { get; set; }
        public string BillingAddressCity { get; set; }
        public string BillingAddressState { get; set; }
        public string BillingAddressCountry { get; set; }
    }
}