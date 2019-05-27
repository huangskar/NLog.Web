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


            var httpContextAccessorMock = Substitute.For<IHttpContextAccessor>();
            var serviceProviderMock = Substitute.For<IServiceProvider>();
            serviceProviderMock.GetService(typeof(IHttpContextAccessor)).Returns(httpContextAccessorMock);
            ServiceLocator.ServiceProvider = serviceProviderMock;

            // Act
            AspNetLayoutRendererBase.Register("test-web", (info, accessor, arg3) => accessor.HttpContext.Connection.LocalPort);
            Layout l = "${test-web}";
            var restult = l.Render(LogEventInfo.CreateNullEvent());

            // Assert
            Assert.Equal("something", restult);

        }
    }
}
