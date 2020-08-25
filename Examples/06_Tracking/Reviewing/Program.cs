using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;

using DomainWorkflows.Hosting;
using DomainWorkflows.Workflows;
using DomainWorkflows.Workflows.DependencyInjection;
using DomainWorkflows.Tracking;
using DomainWorkflows.Utils.CommandProcessing;

using Reviewing.Services;
using Reviewing.Events;


namespace Reviewing
{
    class Program
    {
        static async Task Main(string[] args)
        {
            IHost workflowHost = new HostBuilder()
              .UseWorkflows()
              .UseTracking()    // attach tracking client interface ITrackingService
              .ConfigureLogging((logging) =>
              {
                  logging.AddConsole();
              })              
              .ConfigureServices(services =>
              {
                  services.AddTransient<IReviewPolicyService, ReviewPolicyService>();
                  services.AddTransient<IUserService, UserService>();
                  services.AddTransient<INotificationService, NotificationService>();
              })
              .Build();

            await workflowHost.StartAsync();

            Console.WriteLine("Workflow host started.");
            Console.WriteLine();            

            ReviewingCommandProcessor commandProcessor = new ReviewingCommandProcessor(workflowHost);
            commandProcessor.Run();

            using (workflowHost)
            {
                await workflowHost.StopAsync();
            }
        }
    }


    internal class ReviewingCommandProcessor : CommandProcessor
    {
        private readonly IHost _workflowHost;

        public ReviewingCommandProcessor(IHost workflowHost)
        {
            _workflowHost = workflowHost;
        }

        [Command("start", Description = "Start article review")]
        public async Task StartReviewing(int artId)
        {
            string trackingId = MakeTrackingId(artId);
            var context = new Dictionary<string, object>
                    {
                        { WorkflowEventData.Workflow_TrackingId, trackingId }
                    };

            // start ChainReview workflow and attach tracking context with trackingId
            await _workflowHost.SendEvent(new ReviewRequested() { ArticleId = artId }, context);
        }

        [Command("accept", Description = "Accept article")]
        public async Task AcceptArticle(int artId)
        {
            await _workflowHost.SendEvent(new ArticleReviwed() { ArticleId = artId, Accepted = true });
        }
        
        [Command("reject", Description = "Reject article")]
        public async Task RejectArticle(int artId)
        {
            await _workflowHost.SendEvent(new ArticleReviwed() { ArticleId = artId, Accepted = false });
        }

        [Command("check", Description = "Check review result")]
        public async Task CheckStatus(int artId)
        {
            string trackingId = MakeTrackingId(artId);
            // checking review result by trackingId
            string workflowResult = await _workflowHost.GetWorkflowResult(trackingId);

            Console.WriteLine("Workflow Result for article(id={0}) is {1}", artId, workflowResult);
        }

        [Command("track", Description = "Track article review workflow")]
        public async Task TrackWorkflow(int artId)
        {
            string trackingId = MakeTrackingId(artId);
            // retrive executing workflow details by trackingId
            var tracks = await _workflowHost.GetWorkflowTracks(trackingId);
            foreach (var track in tracks)
            {
                Console.WriteLine("WORKFLOW: {0} | {1} | {2} | {3}", track.Name, track.Status, track.State, track.ResultCode ?? "(none)");
            }
        }

        [Command("trackx", Description = "Track (detailed) article review workflow")]
        public async Task TrackWorkflowDetailed(int artId)
        {
            string trackingId = MakeTrackingId(artId);
            // retrive executing workflow details by trackingId
            var tracks = await _workflowHost.GetWorkflowTracks(trackingId);
            foreach (var track in tracks)
            {
                Console.WriteLine("WORKFLOW: {0} | {1} | {2} | {3}", track.Name, track.Status, track.State, track.ResultCode ?? "(none)");
                // retrive executing workflow entries for related workflow track
                var entries = await _workflowHost.GetTrackEntries(track.Id);
                foreach (var entry in entries)
                {
                    Console.WriteLine(" --> {0:G} - {1} - {2}", entry.TimeStamp, entry.Message, entry.State);
                }
            }
        }

        private static string MakeTrackingId(int artId)
        {
            return "art-" + artId;
        }
    }

    static class TrackingExtensions
    {
        public static async Task<string> GetWorkflowResult(this IHost host, string trackingId)
        {
            using (ExternalService<ITrackingService> externalService = host.GetExternalService<ITrackingService>())
            {
                ITrackingService trackingService = externalService.Value;

                // get all workflow tracks and combine workflow results
                var tracks = await trackingService.GetTracks(trackingId);
                string result = string.Join(";", tracks.Select(t => t.ResultCode));
                if (string.IsNullOrEmpty(result))
                    return "(none)";

                return result;
            }
        }

        public static async Task<IEnumerable<WorkflowTrack>> GetWorkflowTracks(this IHost host, string trackingId)
        {
            using (ExternalService<ITrackingService> externalService = host.GetExternalService<ITrackingService>())
            {
                // get workflow tracks by provided trackingId
                ITrackingService trackingService = externalService.Value;
                return await trackingService.GetTracks(trackingId);
            }
        }

        public static async Task<IEnumerable<TrackingEntry>> GetTrackEntries(this IHost host, string trackId)
        {
            using (ExternalService<ITrackingService> externalService = host.GetExternalService<ITrackingService>())
            {
                // get workflow track entries by provided trackId
                ITrackingService trackingService = externalService.Value;
                return await trackingService.GetEntries(trackId);
            }
        }
    }
}
