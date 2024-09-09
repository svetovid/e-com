using Akka.Actor;
using Akka.Event;
using btm.paas.Messages;
using btm.shared.Messages;

namespace btm.paas.Actors
{
    public class PaymentActor : ReceiveActor
    {
        private readonly ILoggingAdapter _log = Logging.GetLogger(Context);

        private readonly Payment _payment;
        private PaymentRequest _paymentRequest;

        private readonly IActorRef _validationActor;
        private readonly IActorRef _pspActor;
        private readonly IActorRef _sender;
        private readonly IActorRef _paymentLogger;

        public PaymentActor(IActorRef sender, IActorRef validationActor, IActorRef pspActor, IActorRef paymentLogger)
        {
            _sender = sender;
            _validationActor = validationActor;
            _pspActor = pspActor;
            _paymentLogger = paymentLogger;

            _payment = new Payment();

            Become(Initiated);
        }

        private void Initiated()
        {
            _log.Info("PaymentActor before Initiated {0}", Self.Path);

            Receive<PaymentRequest>(msg =>
            {
                _log.Info("PaymentActor Initiated {0}, sender: {1}", msg.Payment.PaymentReference, Sender.Path);

                _paymentRequest = msg;

                _payment.PaymentReference = msg.Payment.PaymentReference;
                _payment.CustomerReference = msg.Customer.CustomerReference;
                _payment.Amount = msg.Payment.PaymentAmount;
                _payment.CurrencyCode = msg.Payment.CurrencyCode;
                _payment.MethodActionId = msg.Payment.MethodActionId;

                // Save to db
                _paymentLogger.Tell(_payment);

                //ValidationActor tell payment request
                _validationActor.Tell(msg);

                Become(Created);
            });
        }

        private void Created()
        {
            Receive<ValidationResponse>(msg => 
            {
                _log.Info("PaymentActor Created {0}, sender: {1}", msg.PaymentReference, Sender.Path);

                if (msg.StatusCode == 0) 
                {
                    _payment.ProviderAccountName = msg.StatusMessage;

                    var paymentStatus = new PaymentStatus(_payment.PaymentReference, "Created");
                    _sender.Tell(paymentStatus);
                    _paymentLogger.Tell(paymentStatus);

                    //PSPActor tell payment request
                    _pspActor.Tell(_paymentRequest);

                    Become(Pending);
                }
                else
                {
                    var paymentStatus = new PaymentStatus(_payment.PaymentReference, "Invalid");
                    _sender.Tell(paymentStatus);
                    _paymentLogger.Tell(paymentStatus);
                }
            });
        }

        private void Pending()
        {
            var paymentStatus = new PaymentStatus(_payment.PaymentReference, "InProcess");
            _sender.Tell(paymentStatus);
            _paymentLogger.Tell(paymentStatus);

            Receive<PspResponse>(msg => 
            {

                _log.Info("PaymentActor Pending {0}, sender: {1}", msg.PaymentReference, Sender.Path);

                _payment.PublicPaymentId = msg.PublicPaymentId;
                _payment.Status = msg.Status;

                _paymentLogger.Tell(new PaymentStatus(_payment.PaymentReference, _payment.Status));

                //Send notification to Wallet in order to update customer balance
                Become(Final);
            });
        }

        private void Final()
        {
            _log.Info("PaymentActor Final {0}", _payment.PaymentReference);

            var paymentStatus = new PaymentStatus(_payment.PaymentReference, _payment.Status);
            _sender.Tell(paymentStatus);
            _paymentLogger.Tell(_payment);

            Sender.Tell(new TerminationResponse(_payment.PaymentReference), Self);
        }
    }
}