﻿namespace pagSeguro.Api.Models
{
    public class CreditCardResponse
    {
        public bool succeeded { get; set; }
        public string errorMessage { get; set; }
        public string transactionid { get; set; }
        public int paymentstatus { get; set; }
    }
}
