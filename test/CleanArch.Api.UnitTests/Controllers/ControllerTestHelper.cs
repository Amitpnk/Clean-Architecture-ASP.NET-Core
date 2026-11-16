using CleanArch.Application.CQRS;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace CleanArch.Api.UnitTests.Controllers;

internal static class ControllerTestHelper
{
    /// <summary>
    /// BaseController resolves <see cref="IDispatcher"/> from HttpContext.RequestServices,
    /// so give the controller a context whose service provider returns the mock.
    /// </summary>
    public static T WithDispatcher<T>(this T controller, Mock<IDispatcher> dispatcher) where T : ControllerBase
    {
        var services = new ServiceCollection().AddSingleton(dispatcher.Object).BuildServiceProvider();
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { RequestServices = services }
        };
        return controller;
    }
}
