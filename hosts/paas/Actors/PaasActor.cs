using Akka.Actor;
using Akka.Event;
using btm.paas.Data;
using btm.paas.Messages;
using btm.shared.Messages;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;

namespace btm.paas.Actors
{
    public class PaasActor : ReceiveActor
    {
        private readonly ILoggingAdapter _log = Logging.GetLogger(Context);

        private readonly IActorRef _validationActor;
        private readonly IActorRef _pspActor;
        private readonly IServiceProvider _serviceProvider;

        private readonly Dictionary<string, IActorRef> _payments = [];

        public PaasActor(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;

            _validationActor = Context.ActorOf(Props.Create(() => new ValidationActor()), "validator");
            _pspActor = Context.ActorOf(Props.Create(() => new PspActor()), "psp");

            _log.Info("Paas up and running...");

            Receive<PaymentRequest>(OnPaymentRequestMessageReceived);

            Receive<TerminationResponse>(OnTerminationResponseMessageReceived);

            // HealthCheck / debug endpoint
            Receive<string>(OnDebugMessageReceived);
        }

        private void OnPaymentRequestMessageReceived(PaymentRequest paymentRequest)
        {
            _log.Info("PaasActor: {0}, sender: {1}", Self.Path, Sender.Path);

            if (!_payments.ContainsKey(paymentRequest.Payment.PaymentReference))
            {
                IActorRef paymentLogger = CreatePaymentLoggerActor();

                IActorRef paymentActor = Context.ActorOf(Props.Create(() =>
                    new PaymentActor(Sender, _validationActor, _pspActor, paymentLogger)));
                paymentActor.Tell(paymentRequest);

                _payments.Add(paymentRequest.Payment.PaymentReference, paymentActor);
            }
            else
            {
                _log.Info("Payment {0} is already being processed", paymentRequest.Payment.PaymentReference);
            }
        }

        private void OnTerminationResponseMessageReceived(TerminationResponse terminationResponse)
        {
            _log.Info("Actor {0} can be disposed", terminationResponse.PaymentReference);

            Sender.GracefulStop(TimeSpan.FromSeconds(20)).ContinueWith((t) =>
            {
                if (t.Result)
                    _payments.Remove(terminationResponse.PaymentReference);
            });
        }

        private void OnDebugMessageReceived(string message)
        {
            _log.Info("Message was received: {0}", message);
        }

        private IActorRef CreatePaymentLoggerActor()
        {
            var dataContext = _serviceProvider.GetRequiredService<ApplicationDbContext>();
            return Context.ActorOf(Props.Create(() => new LogActor(dataContext)));
        }
    }
}