using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Api.Middlewares;
using Xunit;
using FluentAssertions;
using NSubstitute;

namespace Api.Tests.Middlewares
{
    public class CorrelationIdHeaderMiddlewareTests
    {
        private readonly IDummyRequestDelegate _innerFunction;
        private readonly RequestDelegate _next;
        private readonly CorrelationIdHeaderMiddleware _middleware;

        public CorrelationIdHeaderMiddlewareTests()
        {
            _innerFunction = Substitute.For<IDummyRequestDelegate>();
            _next = async (context) =>
            {
                _innerFunction.CallMe(context);
                await Task.CompletedTask;
            };

            _middleware = new CorrelationIdHeaderMiddleware(_next);
        }

        [Fact]
        public async void InvokeAsync_Should_Add_CorrelationId_Request_Header()
        {
            var context = new DefaultHttpContext();

            await _middleware.InvokeAsync(context);

            context.Request.Headers.Should().ContainKey(CorrelationIdHeaderMiddleware.CORRELATION_ID);
            context.Request.Headers[CorrelationIdHeaderMiddleware.CORRELATION_ID].Should().HaveCount(1);
            context.Request.Headers[CorrelationIdHeaderMiddleware.CORRELATION_ID].Single().Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async void InvokeAsync_Should_Not_Override_Existing_CorrelationId_Request_Header()
        {
            var context = new DefaultHttpContext();

            var correlationId = "new";
            context.Request.Headers[CorrelationIdHeaderMiddleware.CORRELATION_ID] = correlationId;

            await _middleware.InvokeAsync(context);

            context.Request.Headers.Should().ContainKey(CorrelationIdHeaderMiddleware.CORRELATION_ID);
            context.Request.Headers[CorrelationIdHeaderMiddleware.CORRELATION_ID].Should().HaveCount(1);
            context.Request.Headers[CorrelationIdHeaderMiddleware.CORRELATION_ID].Should().BeEquivalentTo(correlationId);
        }

        [Fact]
        public async void InvokeAsync_Should_Add_AccessControlExposeHeaders_Response_Header()
        {
            var context = new DefaultHttpContext();

            await _middleware.InvokeAsync(context);

            var headerName = "Access-Control-Expose-Headers";
            context.Response.Headers.Should().ContainKey(headerName);
            context.Response.Headers[headerName].Should().HaveCount(1);
            context.Response.Headers[headerName].Should().BeEquivalentTo(CorrelationIdHeaderMiddleware.CORRELATION_ID);
        }

        [Fact]
        public async void InvokeAsync_Should_Append_CorrelationId_To_AccessControlExposeHeaders_Response_Header()
        {
            var context = new DefaultHttpContext();

            var headerName = "Access-Control-Expose-Headers";
            var exposedHeader = "exposedHeader";
            context.Response.Headers[headerName] = exposedHeader;

            await _middleware.InvokeAsync(context);

            context.Response.Headers.Should().ContainKey(headerName);
            context.Response.Headers[headerName].Should().HaveCount(2);
            context.Response.Headers[headerName].Should().BeEquivalentTo(exposedHeader, CorrelationIdHeaderMiddleware.CORRELATION_ID);
        }

        [Fact]
        public async void InvokeAsync_Should_Not_Override_Existing_CorrelationId_In_AccessControlExposeHeaders_Response_Header()
        {
            var context = new DefaultHttpContext();

            var headerName = "Access-Control-Expose-Headers";
            var exposedHeaders = new StringValues(new [] { "exposedHeader", CorrelationIdHeaderMiddleware.CORRELATION_ID });
            context.Response.Headers[headerName] = exposedHeaders;

            await _middleware.InvokeAsync(context);

            context.Response.Headers.Should().ContainKey(headerName);
            context.Response.Headers[headerName].Should().HaveCount(2);
            context.Response.Headers[headerName].Should().BeEquivalentTo(exposedHeaders);
        }

        [Fact]
        public async void InvokeAsync_Should_Add_CorrelationId_Request_Header_To_Response_Header()
        {
            var context = new DefaultHttpContext();

            var correlationId = "any";
            context.Request.Headers[CorrelationIdHeaderMiddleware.CORRELATION_ID] = correlationId;

            await _middleware.InvokeAsync(context);

            context.Response.Headers.Should().ContainKey(CorrelationIdHeaderMiddleware.CORRELATION_ID);
            context.Response.Headers[CorrelationIdHeaderMiddleware.CORRELATION_ID].Should().HaveCount(1);
            context.Response.Headers[CorrelationIdHeaderMiddleware.CORRELATION_ID].Should().BeEquivalentTo(correlationId);
        }

        [Fact]
        public async void InvokeAsync_Should_Call_Next_Middleware()
        {
            var context = new DefaultHttpContext();

            await _middleware.InvokeAsync(context);

            _innerFunction.Received(1).CallMe(context);
        }

        public interface IDummyRequestDelegate { void CallMe(HttpContext context); }
    }
}