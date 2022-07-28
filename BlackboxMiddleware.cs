using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

namespace Ste4lthPr0xy.Applications.Blackbox.Web
{
    public class BlackboxMiddleware : IMiddleware
    {
        public Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            var request = context.Request;

            if (!request.Path.StartsWithSegments("/blackbox/rest", out PathString remainingPathString))
                return next != null ? next(context) : Task.CompletedTask;

            var config = context.RequestServices.GetService<IConfiguration>();
            var requestLogDirectory = new DirectoryInfo(config.GetValue<string>("RequestLogDirectory"));

            // ohne erstes "/"
            var remainingPath = remainingPathString.ToString()[1..];

            var environment = remainingPath.Remove(remainingPath.IndexOf("/"));

            JObject jBody = null;

            if (request.Body != null)
            {
                using (var requestBodyReader = new StreamReader(request.Body))
                {
                    var body = requestBodyReader.ReadToEndAsync().Result;

                    if (body?.Length > 0)
                        jBody = JObject.Parse(body);
                }
            }

            var jRequest = JObject.FromObject(new { request.Path, request.Method, Body = jBody });

            var workflowId = jRequest.SelectToken("body.header.workflowId")?.ToString() ?? "no_workflow";
            var requestId = jRequest.SelectToken("body.header.messageId")?.ToString() ?? $"no_request_id_{Guid.NewGuid()}";

            var targetDirectory = Path.Combine(requestLogDirectory.FullName, environment, workflowId);
            if (!Directory.Exists(targetDirectory))
                Directory.CreateDirectory(targetDirectory);

            using (var streamWriter = File.CreateText(Path.Combine(targetDirectory, $"{requestId}.json")))
            {
                streamWriter.Write(jRequest.ToString());
            }

            // header
            context.Response.ContentType = "text/plain";
            context.Response.StatusCode = (int)HttpStatusCode.NoContent;

            return Task.CompletedTask;
        }
    }
}
