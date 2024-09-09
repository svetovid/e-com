using Akka.TestKit.Xunit2;
using btm.paas.Actors;
using NSubstitute;
using System;
using Xunit;

namespace btm.tests.Actors
{
    public class PaasActorTests : TestKit
    {
        [Fact]
        public void ShouldLogUpAndRunnning()
        {
            var serviceProvider = Substitute.For<IServiceProvider>();

            EventFilter.Info("Paas up and running...")
                .ExpectOne(() => ActorOf(() => new PaasActor(serviceProvider)));
        }
    }
}