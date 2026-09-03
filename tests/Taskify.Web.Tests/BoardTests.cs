using System.Net;
using System.Text;
using Bunit;
using Microsoft.Extensions.DependencyInjection;
using Taskify.Web.Components.Pages;
using Taskify.Web.Services;

namespace Taskify.Web.Tests;

/// <summary>bUnit component tests for the Kanban board.</summary>
public class BoardTests : BunitContext
{
    [Fact]
    public void Renders_Four_Kanban_Columns()
    {
        var http = new HttpClient(new EmptyListHandler()) { BaseAddress = new Uri("http://localhost") };
        var identity = new IdentityState();
        Services.AddSingleton<IdentityState>(identity);
        Services.AddSingleton<ApiClient>(new ApiClient(http, identity));
        Services.AddSingleton<RealtimeBus>(new RealtimeBus());

        var cut = Render<Board>(parameters => parameters.Add(p => p.ProjectId, Guid.NewGuid()));

        Assert.Contains("To Do", cut.Markup);
        Assert.Contains("In Progress", cut.Markup);
        Assert.Contains("In Review", cut.Markup);
        Assert.Contains("Done", cut.Markup);
    }

    /// <summary>Returns an empty JSON array for any request so the board renders with no data.</summary>
    private sealed class EmptyListHandler : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("[]", Encoding.UTF8, "application/json")
            };
            return Task.FromResult(response);
        }
    }
}
