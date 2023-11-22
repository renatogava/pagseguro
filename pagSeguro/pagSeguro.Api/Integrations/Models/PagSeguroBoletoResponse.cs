using System;
using System.Collections.Generic;

namespace pagSeguro.Api.Integrations.Models
{
    public class PagSeguroBoletoResponse
    {
        public string id { get; set; }
        public string reference_id { get; set; }
        public string status { get; set; }
        public DateTime created_at { get; set; }
        public string description { get; set; }
        public AmountBoletoResponse amount { get; set; }
        public PaymentBoletoResponse payment_response { get; set; }
        public PaymentMethodBoletoResponse payment_method { get; set; }
        public List<string> notification_urls { get; set; }
        public List<LinkBoletoResponse> links { get; set; }
        public bool Success { get; set; }
        public string Message { get; set; }


    }

    public class AddressBoletoResponse
    {
        public string region { get; set; }
        public string city { get; set; }
        public string postal_code { get; set; }
        public string street { get; set; }
        public string number { get; set; }
        public string locality { get; set; }
        public string country { get; set; }
        public string region_code { get; set; }
    }

    public class AmountBoletoResponse
    {
        public int value { get; set; }
        public string currency { get; set; }
        public SummaryBoletoResponse summary { get; set; }
    }

    public class BoletoResponse
    {
        public string id { get; set; }
        public string barcode { get; set; }
        public string formatted_barcode { get; set; }
        public string due_date { get; set; }
        public InstructionLinesBoletoResponse instruction_lines { get; set; }
        public HolderBoletoResponse holder { get; set; }
    }

    public class HolderBoletoResponse
    {
        public string name { get; set; }
        public string tax_id { get; set; }
        public string email { get; set; }
        public AddressBoletoResponse address { get; set; }
    }

    public class InstructionLinesBoletoResponse
    {
        public string line_1 { get; set; }
        public string line_2 { get; set; }
    }

    public class LinkBoletoResponse
    {
        public string rel { get; set; }
        public string href { get; set; }
        public string media { get; set; }
        public string type { get; set; }
    }

    public class PaymentMethodBoletoResponse
    {
        public string type { get; set; }
        public BoletoResponse boleto { get; set; }
    }

    public class PaymentBoletoResponse
    {
        public string code { get; set; }
        public string message { get; set; }
    }

    public class SummaryBoletoResponse
    {
        public int total { get; set; }
        public int paid { get; set; }
        public int refunded { get; set; }
    }


}