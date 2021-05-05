﻿using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using NUnit.Framework;
using Serilog;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using WireMock.Matchers;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;

namespace TinyFp.Complex.Setup
{
    [ExcludeFromCodeCoverage]
    public class BaseIntegrationTest : IDisposable
    {
        protected WireMockServer SearchServer { get; private set; }

        protected IntegrationTestServer TestServer;
        protected HttpClient Client => TestServer.Client;

        public IWebHostBuilder GetWebHostBuilder()
            => WebHost.CreateDefaultBuilder(Array.Empty<string>())
                .ConfigureKestrel(options => options.AddServerHeader = false)
                .UseStartup<TestStartup>()
                .UseSerilog();

        [OneTimeSetUp]
        public void GlobalSetup()
        {
            SearchServer = WireMockServer.Start(port: 5001);
        }

        [OneTimeTearDown]
        public void GlobalTeardown()
        {
            SearchServer.Stop();
            TestStartup.InMemoryRedisCache.ClearCache();
        }

        [TearDown]
        public void Teardown()
        {
            SearchServer.Reset();
        }

        protected BaseIntegrationTest()
        {
            InitTestServer();
        }

        public void InitTestServer()
        {
            TestServer = new IntegrationTestServer(GetWebHostBuilder());
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            TestServer.TestServer.Dispose();
            TestServer.Client.Dispose();            
        }

        protected void StubProducts(string forName, int statusCode, string responseBody, int delayInMilliseconds = 1)
            => SearchServer
                .Given(
                //Request.Create()
                //    .WithPath("/products")
                //    .WithParam("forName", new ExactMatcher(forName))
                //Request.Create()
                //    .WithUrl("http://localhost:5001/products?forName=prd")
                    Request.Create()
                        .WithPath($"/products/{forName}")
                        .UsingGet()
                )
                .RespondWith(
                    Response.Create()
                        .WithStatusCode(statusCode)
                        .WithBody(responseBody)
                        .WithDelay(TimeSpan.FromMilliseconds(delayInMilliseconds))
                );
    }
}