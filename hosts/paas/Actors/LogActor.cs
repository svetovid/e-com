using Akka.Actor;
using btm.paas.Data;
using btm.paas.Models;
using btm.shared.Messages;

namespace btm.paas.Actors
{
    internal class LogActor : ReceiveActor
    {
        private readonly ApplicationDbContext _context;

        public LogActor(ApplicationDbContext context)
        {
            _context = context;

            Receive<Payment>(OnPaymentMessageReceived);

            Receive<PaymentStatus>(OnPaymentStatusMessageReceived);
        }

        private void OnPaymentMessageReceived(Payment payment)
        {
            var existingPayment = _context.Payments.Find(payment.PaymentReference);
            if (existingPayment == null)
            {
                PaymentModel paymentModel = new()
                {
                    Reference = payment.PaymentReference,
                    Amount = payment.Amount,
                    CurrencyCode = payment.CurrencyCode,
                    CustomerReference = payment.CustomerReference,
                    MethodActionId = payment.MethodActionId,
                    PublicPaymentId = payment.PublicPaymentId,
                    ProviderAccountName = payment.ProviderAccountName,
                    Status = payment.Status
                };

                _context.Payments.Add(paymentModel);
            }
            else
            {
                existingPayment.Status = payment.Status;
                existingPayment.PublicPaymentId = payment.PublicPaymentId;
                existingPayment.ProviderAccountName = payment.ProviderAccountName;
            }

            _context.SaveChanges();
        }

        private void OnPaymentStatusMessageReceived(PaymentStatus paymentStatus)
        {
            PaymentHistoryModel paymentHistoryModel = new()
            {
                PaymentReference = paymentStatus.PaymentReference,
                Status = paymentStatus.Status
            };

            _context.PaymentHistories.Add(paymentHistoryModel);
            _context.SaveChanges();
        }
    }
}
