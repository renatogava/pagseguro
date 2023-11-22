using System;
using System.Collections.Generic;

namespace pagSeguro.Api.Integrations.Models
{
    public class PagSeguroCardResponse
    {
        public string id { get; set; }
        public string reference_id { get; set; }
        public string status { get; set; }
        public DateTime created_at { get; set; }
        public string description { get; set; }
        public AmountCardResponse amount { get; set; }
        public PaymentCardResponse payment_response { get; set; }
        public PaymentMethodCardResponse payment_method { get; set; }
        public List<string> notification_urls { get; set; }
        public MetadataCardResponse metadata { get; set; }
        public List<LinkCardResponse> links { get; set; }
        public bool Success { get; set; }
        public string Message { get; set; }
    }

    public class AmountCardResponse
    {
        public int value { get; set; }
        public string currency { get; set; }
        public SummaryCardResponse summary { get; set; }
    }

    public class CardResponse
    {
        public string brand { get; set; }
        public string first_digits { get; set; }
        public string last_digits { get; set; }
        public string exp_month { get; set; }
        public string exp_year { get; set; }
        public HolderCardResponse holder { get; set; }
    }

    public class HolderCardResponse
    {
        public string name { get; set; }
    }

    public class LinkCardResponse
    {
        public string rel { get; set; }
        public string href { get; set; }
        public string media { get; set; }
        public string type { get; set; }
    }

    public class MetadataCardResponse
    {
        public string Exemplo { get; set; }
        public string NotaFiscal { get; set; }
        public string idComprador { get; set; }
    }

    public class PaymentMethodCardResponse
    {
        public string type { get; set; }
        public int installments { get; set; }
        public bool capture { get; set; }
        public DateTime capture_before { get; set; }
        public CardResponse card { get; set; }
        public string soft_descriptor { get; set; }
    }

    public class PaymentCardResponse
    {
        public string code { get; set; }
        public string message { get; set; }
        public string reference { get; set; }
    }



    public class SummaryCardResponse
    {
        public int total { get; set; }
        public int paid { get; set; }
        public int refunded { get; set; }
    }


}