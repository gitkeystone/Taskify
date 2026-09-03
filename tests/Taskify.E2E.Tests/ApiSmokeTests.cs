using System.Net.Http.Json;

namespace Taskify.E2E.Tests;

/// <summary>End-to-end smoke tests that boot the Aspire AppHost (requires Docker for Postgres).</summary>
public class ApiSmokeTests
{
    [Fact]
    public async Task Users_Endpoint_Returns_Five_Seeded_Users()
    {
        var appHost = await DistributedApplicationTestingBuilder.CreateAsync<Projects.Taskify_AppHost>();
        await using var app = await appHost.BuildAsync();
        await app.StartAsync();

        var httpClient = app.CreateHttpClient("apiservice", "http");
        var users = await httpClient.GetFromJsonAsync<List<UserSummary>>("/api/users");

        Assert.NotNull(users);
        Assert.Equal(5, users.Count);
    }

    [Fact]
    public async Task CreateProject_With_Valid_Identity_Returns_Created()
    {
        var appHost = await DistributedApplicationTestingBuilder.CreateAsync<Projects.Taskify_AppHost>();
        await using var app = await appHost.BuildAsync();
        await app.StartAsync();

        var httpClient = app.CreateHttpClient("apiservice", "http");
        httpClient.DefaultRequestHeaders.Add("X-Taskify-User-Id", "11111111-1111-1111-1111-111111111111");

        var response = await httpClient.PostAsJsonAsync("/api/projects", new { name = "E2E Project" });

        Assert.Equal(System.Net.HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task Web_Service_Responds_On_Http_Endpoint()
    {
        var appHost = await DistributedApplicationTestingBuilder.CreateAsync<Projects.Taskify_AppHost>();
        await using var app = await appHost.BuildAsync();
        await app.StartAsync();

        using var handler = new HttpClientHandler { AllowAutoRedirect = false };
        using var httpClient = new HttpClient(handler)
        {
            BaseAddress = app.GetEndpoint("web", "http")
        };

        var response = await httpClient.GetAsync("/");

        // 2xx (served) or 3xx (HTTPS redirect) both mean the Blazor app is up and responding.
        Assert.InRange((int)response.StatusCode, 200, 399);
    }

    private record UserSummary(Guid Id, string Name, string Role);
}
