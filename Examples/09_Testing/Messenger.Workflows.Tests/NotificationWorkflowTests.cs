using System.Threading.Tasks;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using DomainWorkflows.Hosting;
using DomainWorkflows.Testing;

using Messenger.Services;
using Messenger.Events;

namespace Messenger.Workflows.Tests
{
    [TestClass]
    public class NotificationWorkflowTests
    {
        [TestMethod]
        public async Task BasicTimeout_OneMessageReceived_SendsNewMessagesEmail()
        {
            // ARRANGE
            string userId = "user1";

            ITestHost testHost = new TestHostBuilder()
             .ConfigureEndpoints(endpoint =>
             {
                 // attach event raising proxy/mock for the interface
                 endpoint.WireUp<IEmailService>();
             })
             .ConfigureTesting(testing =>
             {
                 // add workflow(s) for testing
                 testing.AddWorkflow<NotificationWorkflow>();
             })
             .ConfigureTestPlan(plan =>
             {
                 MessageReceived messageReceived = new MessageReceived() { UserId = userId };
                 plan.AddEvent(messageReceived); // add event at 0 time stampt
             })
             .Build();


            // ACT
            await testHost.Run();


            // ASSERT

            // assert that IEmailService.SendNewMessages was called in 10 minutes for userId
            var ev = testHost.EventLog.Get<IEmailService>("SendNewMessages", "10 min", (e) => e.UserId == userId);
            Assert.IsNotNull(ev, "New Mesages Email is not sent on basic timeout.");
        }

        [TestMethod]
        public async Task SlidingBasicTimeout_TwoMessageReceived_SendsNewMessagesEmailWithDelay()
        {
            // ARRANGE
            string userId = "user1";

            ITestHost testHost = new TestHostBuilder<NotificationWorkflow>()
             .ConfigureEndpoints(endpoint =>
             {
                 endpoint.WireUp<IEmailService>();
             })
             .ConfigureTestPlan(plan =>
             {
                 MessageReceived messageReceived = new MessageReceived() { UserId = userId };
                 plan.AddEvent(messageReceived);
                 plan.AddEvent(messageReceived, "6 min"); // raise MessageReceived on 6th min and shift timeout
             })
             .Build();


            // ACT
            await testHost.Run();


            // ASSERT
            var ev = testHost.EventLog.Get<IEmailService>("SendNewMessages", "16 min", (e) => e.UserId == userId);
            Assert.IsNotNull(ev, "New Mesages Email is not sent on sliding basic timeout.");
        }

        [TestMethod]
        public async Task LimitingTimeout_ManyMessageReceived_RaisesMessagesUnread()
        {
            // ARRANGE
            string userId = "user1";

            ITestHost testHost = new TestHostBuilder<NotificationWorkflow>()
             .ConfigureEndpoints(endpoint =>
             {
                 endpoint.WireUp<IEmailService>();
             })
             .ConfigureTestPlan(plan =>
             {
                 MessageReceived messageReceived = new MessageReceived() { UserId = userId };
                 plan.AddEvent(messageReceived);
                 plan.AddEvent(messageReceived, "7 min");
                 plan.AddEvent(messageReceived, "15 min");
                 plan.AddEvent(messageReceived, "24 min");  // slide MessageReceived until limitig timeout
             })
             .Build();

            // ACT
            await testHost.Run();


            // ASSERT
            var ev = testHost.EventLog.Get<MessagesUnread>("30 min", (e) => e.UserId == userId);
            Assert.IsNotNull(ev, "Messages Unread event is absent on limiting timeout.");
        }

        [TestMethod]
        public async Task SingleBasicTimeout_ManyMessageReceived_RaisesMessagesUnreadOnlyOnce()
        {
            // ARRANGE
            string userId = "user1";

            ITestHost testHost = new TestHostBuilder<NotificationWorkflow>()
             .ConfigureEndpoints(endpoint =>
             {
                 endpoint.WireUp<IEmailService>();
             })
             .ConfigureTestPlan(plan =>
             {
                 MessageReceived messageReceived = new MessageReceived() { UserId = userId };
                 plan.AddEvent(messageReceived);
                 plan.AddEvent(messageReceived, "7 min");   // raise on 7th min
                 plan.AddEvent(messageReceived, "+12 min"); // raise after 12 min from prev time stamp 7 min + 12 min
             })
             .Build();

            // ACT
            await testHost.Run();


            // ASSERT

            // assert first basic timeout
            var ev = testHost.EventLog.Get<MessagesUnread>("17 min", e => e.UserId == userId);
            Assert.IsNotNull(ev, "Messages Unread event is absent on sliding timeout.");

            // asset no more basic timeout
            Assert.IsTrue(
                testHost.EventLog.Single<MessagesUnread>(e => e.UserId == userId),
                "Messages Unread event is rised more than once.");
        }

        [TestMethod]
        public async Task CancelBasicTimeout_MessageReceivedThenMessagesRead_DoesNotRaiseMessagesUnread()
        {
            // ARRANGE
            string userId = "user1";

            ITestHost testHost = new TestHostBuilder<NotificationWorkflow>()
             .ConfigureEndpoints(endpoint =>
             {
                 endpoint.WireUp<IEmailService>();
             })
             .ConfigureTestPlan(plan =>
             {
                 MessageReceived messageReceived = new MessageReceived() { UserId = userId };
                 MessagesRead messagesRead = new MessagesRead() { UserId = userId };

                 plan.AddEvent(messageReceived);
                 plan.AddEvent(messageReceived, "+6 min");  // raise on 6 min - slide basic timeout
                 plan.AddEvent(messagesRead, "+8 min");     // raise on 14 min - cancel basic timeout
             })
             .Build();

            // ACT
            await testHost.Run();


            // ASSERT
            var ev = testHost.EventLog.Get<MessagesUnread>("16 min", e => e.UserId == userId);
            Assert.IsNull(ev, "Messages Unread event is not canceled.");           
        }

        [TestMethod]
        public async Task ResetBasicTimeout_MessageReceivedThenMessagesRead_DoNotRaiseMessagesUnread()
        {
            // ARRANGE
            string userId = "user1";

            ITestHost testHost = new TestHostBuilder<NotificationWorkflow>()
             .ConfigureEndpoints(endpoint =>
             {
                 endpoint.WireUp<IEmailService>();
             })
             .ConfigureTestPlan(plan =>
             {
                 MessageReceived messageReceived = new MessageReceived() { UserId = userId };
                 MessagesRead messagesRead = new MessagesRead() { UserId = userId };

                 plan.AddEvent(messageReceived);
                 plan.AddEvent(messageReceived, "+15 min");
                 plan.AddEvent(messagesRead, "+5 min");     // reset basic timeout
                 plan.AddEvent(messageReceived, "+12 min"); // start new basic timeout on 32 min
             })
             .Build();

            // ACT
            await testHost.Run();


            // ASSERT
            var ev1 = testHost.EventLog.Get<MessagesUnread>("10 min", e => e.UserId == userId);
            Assert.IsNotNull(ev1, "Messages Unread event is not raised first time.");

            Assert.IsTrue(
                testHost.EventLog.Many<MessagesUnread>(e => e.UserId == userId),
                "Expected many Messages Unread events.");            

            // just for demo
            var ev2 = testHost.EventLog.Get<MessagesUnread>("42 min", e => e.UserId == userId);
            Assert.IsNotNull(ev2, "Messages Unread event is not raised second time.");            
        }

        [TestMethod]
        public async Task ResetLimitingTimeout_ManyMessageReceivedThenMessagesRead_DoNotSendNewMessagesEmail()
        {
            // ARRANGE
            string userId = "user1";

            ITestHost testHost = new TestHostBuilder<NotificationWorkflow>()
             .ConfigureEndpoints(endpoint =>
             {
                 endpoint.WireUp<IEmailService>();
             })
             .ConfigureTestPlan(plan =>
             {
                 MessageReceived messageReceived = new MessageReceived() { UserId = userId };
                 MessagesRead messagesRead = new MessagesRead() { UserId = userId };

                 plan.AddEvent(messageReceived);
                 plan.AddEvent(messageReceived, "+7 min");
                 plan.AddEvent(messageReceived, "+8 min");
                 plan.AddEvent(messageReceived, "+9 min");  // raise on 24 min
                 plan.AddEvent(messagesRead, "+5 min");     // reset limiting timeout on 29 min
             })
             .Build();

            // ACT
            await testHost.Run();


            // ASSERT            
            var ev = testHost.EventLog.Get<IEmailService>("SendNewMessages", "30 min", e => e.UserId == userId);
            Assert.IsNull(ev, "New Mesages Email sending is not canceled.");

            Assert.IsTrue(
                testHost.EventLog.None<IEmailService>("SendNewMessages", e => e.UserId == userId),
                "No New Mesages Emails should be sent.");
        }
    }
}
