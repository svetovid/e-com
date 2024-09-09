using System;

namespace btm.shared.Messages
{
    public class PaymentInformation
    {
        public PaymentInformation(
            string currencyCode,
            decimal amount,
            string paymentMethod,
            DateTime created,
            DateTime changed,
            string paymentReference,
            string publicPaymentId,
            long paymentId,
            int methodActionId)
        {
            PaymentMethod = paymentMethod;
            Created = created;
            Changed = changed;
            PaymentReference = paymentReference;
            PublicPaymentId = publicPaymentId;
            PaymentId = paymentId;
            Amount = amount;
            CurrencyCode = currencyCode;
            MethodActionId = methodActionId;
        }

        public decimal Amount { get; }
        public string PaymentMethod { get; }
        public decimal BaseAmount { get; }
        public decimal PaymentAmount { get; }
        public DateTime Created { get; }
        public DateTime Changed { get; }
        public string PaymentReference { get; }
        public string PublicPaymentId { get; }
        public long PaymentId { get; }
        public string CurrencyCode { get; }
        public int MethodActionId { get; }
    }
}