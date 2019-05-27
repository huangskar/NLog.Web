using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using NLog.Layouts;
using NLog.Web.DependencyInjection;
using NLog.Web.LayoutRenderers;
using NLog.Web.Tests.LayoutRenderers;
using NSubstitute;
using Xunit;

namespace NLog.Web.AspNetCore.Tests
{
    public class RegisterCustomLayoutRenderer : TestBase
    {
        [Fact]
        public void RegisterLayoutRendererTest()
        {
            var httpcontext = SetupHttpAccessorWithHttpContext();
            httpcontext.Connection.LocalPort.Returns(123);

            // Act
            AspNetLayoutRendererBase.Register("test-web", 
                (logEventInfo, httpContextAccessor, loggingConfiguration) => httpContextAccessor.HttpContext.Connection.LocalPort);
            Layout l = "${test-web}";
            var restult = l.Render(LogEventInfo.CreateNullEvent());

            // Assert
            Assert.Equal("123", restult);
        }
      
        private static HttpContext SetupHttpAccessorWithHttpContext()
        {
            var httpContextAccessorMock = Substitute.For<IHttpContextAccessor>();
            var serviceProviderMock = Substitute.For<IServiceProvider>();
            serviceProviderMock.GetService(typeof(IHttpContextAccessor)).Returns(httpContextAccessorMock);

            var httpcontext = Substitute.For<HttpContext>();

            ServiceLocator.ServiceProvider = serviceProviderMock;
            httpContextAccessorMock.HttpContext.Returns(httpcontext);
            return httpcontext;
        }
    }
}
