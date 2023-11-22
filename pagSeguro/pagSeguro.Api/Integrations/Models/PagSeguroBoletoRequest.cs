using System.Collections.Generic;

namespace pagSeguro.Api.Integrations.Models
{
    public class PagSeguroBoletoRequest
    {
        public string reference_id { get; set; }
        public string description { get; set; }
        public AmountBoletoRequest amount { get; set; }
        public PaymentMethodBoletoRequest payment_method { get; set; }
        public List<string> notification_urls { get; set; }
    }

    public class AddressBoletoRequest
    {
        public string street { get; set; }
        public string number { get; set; }
        public string locality { get; set; }
        public string city { get; set; }
        public string region { get; set; }
        public string region_code { get; set; }
        public string country { get; set; }
        public string postal_code { get; set; }
    }

    public class AmountBoletoRequest
    {
        public int value { get; set; }
        public string currency { get; set; }
    }

    public class BoletoRequest
    {
        public string due_date { get; set; }
        public InstructionLinesBoletoRequest instruction_lines { get; set; }
        public HolderBoletoRequest holder { get; set; }
    }

    public class HolderBoletoRequest
    {
        public string name { get; set; }
        public string tax_id { get; set; }
        public string email { get; set; }
        public AddressBoletoRequest address { get; set; }
    }

    public class InstructionLinesBoletoRequest
    {
        public string line_1 { get; set; }
        public string line_2 { get; set; }
    }

    public class PaymentMethodBoletoRequest
    {
        public string type { get; set; }
        public BoletoRequest boleto { get; set; }
    }
}