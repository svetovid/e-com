using System;
using Akka.Actor;
using Akka.TestKit;
using Akka.TestKit.TestActors;
using Akka.TestKit.Xunit2;
using btm.paas.Actors;
using btm.paas.Messages;
using btm.shared.Messages;
using Xunit;

namespace btm.tests.Actors
{
    public class PaymentActorTests : TestKit
    {
        [Fact]
        public void ShouldSendPaymentStatusCreated()
        {
            var paymentReference = "abc123";

            var validationTestProbe = CreateTestProbe();
            validationTestProbe.SetAutoPilot(new Delegate​Auto​Pilot((sender, message) =>
            {
                sender.Tell(new ValidationResponse(0,
                    string.Empty, paymentReference),
                    ActorRefs.NoSender);

                return AutoPilot.KeepRunning;
            }));

            var paasSender = CreateTestProbe();
            var paymentLogger = CreateTestProbe();

            var actor = ActorOf(()=> new PaymentActor(
                paasSender,
                validationTestProbe,
                paymentLogger,
                ActorOf(BlackHoleActor.Props)));

            var paymentRequest = new PaymentRequest(
                "test.com",
                new PaymentInformation("EUR",2m,"VISA",DateTime.UtcNow,DateTime.UtcNow,paymentReference,string.Empty,0,1),
                new CustomerInformation("EUR", "custRef"), null);

            actor.Tell(paymentRequest);

            var received = paasSender.ExpectMsg<PaymentStatus>();

            Assert.Equal(paymentReference, received.PaymentReference);
            Assert.Equal("Created", received.Status);
        }

        [Fact]
        public void ShouldSendPaymentStatusInProcess()
        {
            var paymentReference = "abc123";

            var validationTestProbe = CreateTestProbe();
            validationTestProbe.SetAutoPilot(new Delegate​Auto​Pilot((sender, message) =>
            {
                sender.Tell(new ValidationResponse(0,
                    string.Empty, paymentReference),
                    ActorRefs.NoSender);

                return AutoPilot.KeepRunning;
            }));

            var paasSender = CreateTestProbe();
            var paymentLogger = CreateTestProbe();

            var actor = ActorOf(()=> new PaymentActor(
                paasSender,
                validationTestProbe,
                paymentLogger,
                ActorOf(BlackHoleActor.Props)));

            var paymentRequest = new PaymentRequest(
                "test.com",
                new PaymentInformation("EUR", 2m, "VISA", DateTime.UtcNow, DateTime.UtcNow, paymentReference, string.Empty, 0, 1),
                new CustomerInformation("EUR", "custRef"), null);

            actor.Tell(paymentRequest);

            var received = paasSender.FishForMessage<PaymentStatus>(m => m.Status == "InProcess");

            Assert.Equal(paymentReference, received.PaymentReference);
            Assert.Equal("InProcess", received.Status);
        }

        [Fact]
        public void ShouldSendPaymentStatusCaptured()
        {
            var paymentReference = "abc123";

            var validationTestProbe = CreateTestProbe();
            validationTestProbe.SetAutoPilot(new Delegate​Auto​Pilot((sender, message) =>
            {
                sender.Tell(new ValidationResponse(0,
                    string.Empty, paymentReference),
                    ActorRefs.NoSender);

                return AutoPilot.KeepRunning;
            }));

            var pspTestProbe = CreateTestProbe();
            pspTestProbe.SetAutoPilot(new Delegate​Auto​Pilot((sender, message) =>
            {
                sender.Tell(new PspResponse(paymentReference, "Captured",
                    "xyz123"),
                    ActorRefs.NoSender);

                return AutoPilot.KeepRunning;
            }));

            var paasSender = CreateTestProbe();
            var paymentLogger = CreateTestProbe();

            var actor = ActorOf(()=> new PaymentActor(
                paasSender,
                validationTestProbe,
                pspTestProbe, 
                paymentLogger));

            var paymentRequest = new PaymentRequest(
                "test.com",
                new PaymentInformation("EUR", 2m, "VISA", DateTime.UtcNow, DateTime.UtcNow, paymentReference, string.Empty, 0, 1),
                new CustomerInformation("EUR", "custRef"), null);

            actor.Tell(paymentRequest);

            var received = paasSender.FishForMessage<PaymentStatus>(m => m.Status == "Captured");

            Assert.Equal(paymentReference, received.PaymentReference);
            Assert.Equal("Captured", received.Status);
        }
    }
}
