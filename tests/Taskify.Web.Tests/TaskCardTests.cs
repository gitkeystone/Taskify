using Bunit;
using Microsoft.Extensions.DependencyInjection;
using Taskify.Web.Components;
using Taskify.Web.Services;

namespace Taskify.Web.Tests;

/// <summary>bUnit component tests for task-card rendering.</summary>
public class TaskCardTests : BunitContext
{
    [Fact]
    public void Renders_TaskTitle_And_Columns()
    {
        var identity = new IdentityState();
        Services.AddSingleton<IdentityState>(identity);
        Services.AddSingleton<ApiClient>(new ApiClient(new HttpClient { BaseAddress = new Uri("http://localhost") }, identity));
        Services.AddSingleton<RealtimeBus>(new RealtimeBus());

        var task = new TaskDto(
            Guid.NewGuid(), Guid.NewGuid(), "Set up CI", null,
            KanbanColumn.ToDo, null, Guid.NewGuid(),
            DateTimeOffset.UtcNow, DateTimeOffset.UtcNow);

        var cut = Render<TaskCard>(parameters => parameters
            .Add(p => p.Task, task)
            .Add(p => p.Users, new List<UserDto>()));

        Assert.Contains("Set up CI", cut.Markup);
        Assert.Contains("To Do", cut.Markup);
        Assert.Contains("Unassigned", cut.Markup);
    }
}
