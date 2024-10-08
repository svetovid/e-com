using System;
using Akka.Actor;
using Akka.TestKit.Xunit2;
using btm.paas.Actors;
using btm.paas.Messages;
using btm.shared.Messages;
using Xunit;

namespace btm.tests.Actors
{
    public class PspActorTests : TestKit
    {
        [Fact]
        public void ShouldReturnSuccess()
        {
            var paymentReference = "abc123";
            var paymentRequest = new PaymentRequest(
                "test.com",
                new PaymentInformation("EUR", 42m, "VISA", DateTime.UtcNow, DateTime.UtcNow, paymentReference, string.Empty, 0, 1),
                null, null);

            var actor = ActorOf<PspActor>();
            actor.Tell(paymentRequest);

            var received = ExpectMsg<PspResponse>();

            Assert.Equal(paymentReference, received.PaymentReference);
            Assert.Equal("Captured", received.Status);
            Assert.Equal("qwertyy123", received.PublicPaymentId);
        }

        [Fact]
        public void ShouldReturnErrorCodeOnAmountMoreThan100()
        {
            var paymentReference = "abc123";
            var paymentRequest = new PaymentRequest(
                "test.com",
                new PaymentInformation("EUR", 142m, "VISA", DateTime.UtcNow, DateTime.UtcNow, paymentReference, string.Empty, 0, 1),
                null, null);

            var actor = ActorOf<PspActor>();
            actor.Tell(paymentRequest);

            var received = ExpectMsg<PspResponse>();

            Assert.Equal(paymentReference, received.PaymentReference);
            Assert.Equal("Refused", received.Status);
            Assert.Equal("qwertyy123", received.PublicPaymentId);
        }
    }
}