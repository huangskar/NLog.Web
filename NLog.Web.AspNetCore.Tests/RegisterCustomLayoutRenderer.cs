using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using NLog.Layouts;
#if ASP_NET_CORE
using Microsoft.AspNetCore.Http;
using NLog.Web.DependencyInjection;
#else
using System.Web;
#endif
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
#if ASP_NET_CORE
            httpcontext.Connection.LocalPort.Returns(123);
#else
            httpcontext.Request.RawUrl.Returns("123");
#endif

            // Act
            AspNetLayoutRendererBase.Register("test-web",
                (logEventInfo, httpContextAccessor, loggingConfiguration) =>
#if ASP_NET_CORE
                    httpContextAccessor.HttpContext.Connection.LocalPort);
#else
                    httpContextAccessor.HttpContext.Request.RawUrl);
#endif
            Layout l = "${test-web}";
            var restult = l.Render(LogEventInfo.CreateNullEvent());

            // Assert
            Assert.Equal("123", restult);
        }

        private static 
            #if ASP_NET_CORE
            HttpContext 
#else
            HttpContextBase
            #endif
            
            SetupHttpAccessorWithHttpContext()
        {
            var httpContextAccessorMock = Substitute.For<IHttpContextAccessor>();
            var serviceProviderMock = Substitute.For<IServiceProvider>();
            serviceProviderMock.GetService(typeof(IHttpContextAccessor)).Returns(httpContextAccessorMock);

#if ASP_NET_CORE
            var httpcontext = Substitute.For<HttpContext>();
#else
            var httpcontext = Substitute.For<HttpContextBase>();
#endif

            ServiceLocator.ServiceProvider = serviceProviderMock;
            httpContextAccessorMock.HttpContext.Returns(httpcontext);
            return httpcontext;
        }
    }
}
