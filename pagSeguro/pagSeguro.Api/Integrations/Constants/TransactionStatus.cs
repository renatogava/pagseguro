namespace pagSeguro.Api.Integrations.Constants
{
    public static class TransactionStatus
    {
        public const int Initiated = 0;
        public const int WaitingPayment = 1;
        public const int InAnalysis = 2;
        public const int Paid = 3;
        public const int Available = 4;
        public const int InDispute = 5;
        public const int Refunded = 6;
        public const int Cancelled = 7;
    }
}
