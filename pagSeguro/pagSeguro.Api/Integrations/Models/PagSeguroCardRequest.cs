using System.Collections.Generic;

namespace pagSeguro.Api.Integrations.Models
{
    public class PagSeguroCardRequest
    {
        public string reference_id { get; set; }
        public string description { get; set; }
        public AmountCardRequest amount { get; set; }
        public PaymentMethodCardRequest payment_method { get; set; }
        public List<string> notification_urls { get; set; }
        public MetadataCardRequest metadata { get; set; }
    }

    public class AmountCardRequest
    {
        public int value { get; set; }
        public string currency { get; set; }
    }

    public class CardRequest
    {
        public string number { get; set; }
        public string exp_month { get; set; }
        public string exp_year { get; set; }
        public string security_code { get; set; }
        public HolderCardRequest holder { get; set; }
    }

    public class HolderCardRequest
    {
        public string name { get; set; }
    }

    public class MetadataCardRequest
    {
        public string Exemplo { get; set; }
        public string NotaFiscal { get; set; }
        public string idComprador { get; set; }
    }

    public class PaymentMethodCardRequest
    {
        public string type { get; set; }
        public int installments { get; set; }
        public bool capture { get; set; }
        public string soft_descriptor { get; set; }
        public CardRequest card { get; set; }
    }
}