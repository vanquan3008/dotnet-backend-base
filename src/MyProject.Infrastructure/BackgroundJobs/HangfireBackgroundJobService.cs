using System.Linq.Expressions;
using Hangfire;
using MyProject.Application.Common.Interfaces;

namespace MyProject.Infrastructure.BackgroundJobs;

internal sealed class HangfireBackgroundJobService(IBackgroundJobClient client)
    : IBackgroundJobService
{
    public string Enqueue<TJob>(Expression<Func<TJob, Task>> methodCall)
        => client.Enqueue(methodCall);

    public string Schedule<TJob>(Expression<Func<TJob, Task>> methodCall, TimeSpan delay)
        => client.Schedule(methodCall, delay);
}
