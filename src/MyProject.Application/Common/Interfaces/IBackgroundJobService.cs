using System.Linq.Expressions;

namespace MyProject.Application.Common.Interfaces;

public interface IBackgroundJobService
{
    string Enqueue<TJob>(Expression<Func<TJob, Task>> methodCall);
    string Schedule<TJob>(Expression<Func<TJob, Task>> methodCall, TimeSpan delay);
}
