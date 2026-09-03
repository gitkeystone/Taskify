using Taskify.Api.Contracts;
using Taskify.Api.Models;
using Taskify.Api.Validation;

namespace Taskify.Api.Tests;

/// <summary>Unit tests for the server-side input-validation rules (constitution Principle II).</summary>
public class ValidatorsTests
{
    private readonly CreateProjectRequestValidator _projectValidator = new();
    private readonly CreateTaskRequestValidator _taskValidator = new();
    private readonly UpdateTaskRequestValidator _updateValidator = new();
    private readonly AddCommentRequestValidator _commentValidator = new();

    [Fact]
    public void CreateProject_EmptyName_Fails() =>
        Assert.False(_projectValidator.Validate(new CreateProjectRequest("")).IsValid);

    [Fact]
    public void CreateProject_WhitespaceName_Fails() =>
        Assert.False(_projectValidator.Validate(new CreateProjectRequest("   ")).IsValid);

    [Fact]
    public void CreateProject_TooLongName_Fails() =>
        Assert.False(_projectValidator.Validate(new CreateProjectRequest(new string('a', 121))).IsValid);

    [Fact]
    public void CreateProject_ValidName_Passes() =>
        Assert.True(_projectValidator.Validate(new CreateProjectRequest("Sprint 1")).IsValid);

    [Fact]
    public void CreateTask_EmptyTitle_Fails() =>
        Assert.False(_taskValidator.Validate(new CreateTaskRequest("", null)).IsValid);

    [Fact]
    public void CreateTask_TooLongTitle_Fails() =>
        Assert.False(_taskValidator.Validate(new CreateTaskRequest(new string('a', 201), null)).IsValid);

    [Fact]
    public void CreateTask_Valid_Passes() =>
        Assert.True(_taskValidator.Validate(new CreateTaskRequest("Set up CI", "optional")).IsValid);

    [Fact]
    public void AddComment_EmptyText_Fails() =>
        Assert.False(_commentValidator.Validate(new AddCommentRequest("")).IsValid);

    [Fact]
    public void AddComment_Valid_Passes() =>
        Assert.True(_commentValidator.Validate(new AddCommentRequest("Looks good")).IsValid);

    [Fact]
    public void UpdateTask_WhitespaceTitle_Fails() =>
        Assert.False(_updateValidator.Validate(new UpdateTaskRequest("  ", null, null, null, false)).IsValid);

    [Fact]
    public void UpdateTask_InvalidStatus_Fails() =>
        Assert.False(_updateValidator.Validate(new UpdateTaskRequest(null, null, null, (KanbanColumn)999, false)).IsValid);

    [Fact]
    public void UpdateTask_ValidStatus_Passes() =>
        Assert.True(_updateValidator.Validate(new UpdateTaskRequest(null, null, null, KanbanColumn.Done, false)).IsValid);
}
