using System.Collections.Generic;
using Microsoft.AspNet.Http.Internal;
using Microsoft.AspNet.Mvc.Abstractions;
using Microsoft.AspNet.Mvc.ActionConstraints;
using Microsoft.AspNet.Routing;
using Microsoft.Extensions.Primitives;
using Xunit;

namespace Microsoft.AspNet.Mvc.Core.Test
{
    public class HttpMethodConstraintTest
    {
        [Theory]
        [InlineData("gEt")]
        [InlineData("get;GeT")]
        [InlineData("get")]
        [InlineData("GET")]
        public void HttpMethodConstraint_Accept_Preflight_CaseInsensitive(string httpMethodStr)
        {
            // Arrange
            var httpMethods = httpMethodStr.Split(';');
            var constraint = new HttpMethodConstraint(httpMethods);

            var context = new ActionConstraintContext();
            var routeContext = CreateRouteContext("oPtIoNs", true);
            
            var action = new ActionDescriptor();
            var constraints = new List<IActionConstraint> {
                new HttpMethodConstraint(httpMethods)
            };
            var actionSelectorCandidate = new ActionSelectorCandidate(action, constraints);

            context.Candidates = new List<ActionSelectorCandidate> { actionSelectorCandidate };
            context.CurrentCandidate = context.Candidates[0];
            context.RouteContext = routeContext;

            // Act
            var result = constraint.Accept(context);

            // Assert
            Assert.True(result);
        }

        [Theory]
        [InlineData("get;Get;GET;GEt", "gEt")]
        [InlineData("POST;PoSt;GEt", "GET")]
        [InlineData("get", "get")]
        [InlineData("post", "POST")]
        public void HttpMethodConstraint_Accept_CaseInsensitive(string httpMethodStr, string expectedMethod)
        {
            // Arrange
            var httpMethods = httpMethodStr.Split(';');
            var constraint = new HttpMethodConstraint(httpMethods);

            var context = new ActionConstraintContext();
            var routeContext = CreateRouteContext(expectedMethod);

            var action = new ActionDescriptor();
            var constraints = new List<IActionConstraint> {
                new HttpMethodConstraint(httpMethods)
            };
            var actionSelectorCandidate = new ActionSelectorCandidate(action, constraints);

            context.Candidates = new List<ActionSelectorCandidate> { actionSelectorCandidate};
            context.CurrentCandidate = context.Candidates[0];
            context.RouteContext = routeContext;

            // Act
            var result = constraint.Accept(context);

            // Assert
            Assert.True(result);
        }

        private static RouteContext CreateRouteContext(string requestedMethod, bool preflight = false)
        {
            var httpContext = new DefaultHttpContext();

            httpContext.Request.Method = requestedMethod;

            if(preflight)
            {
                httpContext.Request.Headers.Add("Origin", StringValues.Empty);
                httpContext.Request.Headers.Add("Access-Control-Request-Method", "GET");
            }

            var routeContext = new RouteContext(httpContext);
            routeContext.RouteData = new RouteData();

            return routeContext;
        }
    }
}
