using BNS360.Core.Dto;
using BNS360.Core.Models;
using BNS360.Repository.Data;
using BNS360.Repository.Repository;
using Microsoft.EntityFrameworkCore;

namespace BNS360.Tests;

public sealed class JobRepositoryOwnershipTests
{
    [Fact]
    public async Task UpdateJob_DoesNotAllowAnotherUserToTakeOwnership()
    {
        await using var context = CreateContext();
        var job = CreateJob("owner-id");
        context.Jobs.Add(job);
        await context.SaveChangesAsync();

        var repository = new JobRepository(context);
        var response = await repository.UpdateJob(job.Id, "attacker-id", CreateDto("attacker-id"));

        Assert.Equal(404, response.StatusCode);
        Assert.Equal("owner-id", job.UserId);
        Assert.Equal("Original title", job.JobTitleArabic);
    }

    [Fact]
    public async Task DeleteJob_DoesNotAllowAnotherUserToDeleteIt()
    {
        await using var context = CreateContext();
        var job = CreateJob("owner-id");
        context.Jobs.Add(job);
        await context.SaveChangesAsync();

        var repository = new JobRepository(context);
        var response = await repository.DeleteJob(job.Id, "attacker-id");

        Assert.Equal(404, response.StatusCode);
        Assert.NotNull(await context.Jobs.FindAsync(job.Id));
    }

    [Fact]
    public async Task DeleteJob_AllowsTheOwnerToDeleteIt()
    {
        await using var context = CreateContext();
        var job = CreateJob("owner-id");
        context.Jobs.Add(job);
        await context.SaveChangesAsync();

        var repository = new JobRepository(context);
        var response = await repository.DeleteJob(job.Id, "owner-id");

        Assert.Equal(200, response.StatusCode);
        Assert.Null(await context.Jobs.FindAsync(job.Id));
    }

    private static AppDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new AppDbContext(options);
    }

    private static JobModel CreateJob(string userId) => new()
    {
        JobTitleArabic = "Original title",
        JobDescriptionArabic = "Description",
        AddreesInArabic = "Address",
        Numbers = ["01000000000"],
        Requirements = ["Requirement"],
        UserId = userId
    };

    private static JobModelDto CreateDto(string userId) => new()
    {
        JobTitleArabic = "Changed title",
        JobDescriptionArabic = "Changed description",
        AddreesInArabic = "Changed address",
        Numbers = ["01111111111"],
        Requirements = ["Changed requirement"],
        UserId = userId
    };
}
