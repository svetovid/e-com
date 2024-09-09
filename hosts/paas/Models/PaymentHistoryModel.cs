namespace btm.paas.Models
{
    public class PaymentHistoryModel
    {
        public long Id { get; set; }

        public string PaymentReference { get; set; }

        public PaymentModel Payment { get; set; }

        public string Status { get; set; }
    }
}
