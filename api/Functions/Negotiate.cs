using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.SignalRService;

namespace Api.Functions
{
    public static class Negotiate
    {
        [FunctionName("negotiate")]
        public static SignalRConnectionInfo Run(
            [HttpTrigger("post")] HttpRequest req,
            [SignalRConnectionInfo(HubName = "templates")] SignalRConnectionInfo connectionInfo)
        {
            return connectionInfo;
        }
    }
}