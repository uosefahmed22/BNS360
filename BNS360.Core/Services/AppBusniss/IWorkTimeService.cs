using BNS360.Core.Dtos.Request;
using BNS360.Core.Entities;
using System.Linq.Expressions;

namespace BNS360.Core.Services.AppBusniss;

public interface IWorkTimeService
{
    Task Add(WorkTimeDto workTime,Guid busnissId);
    Expression<Func<WorkTime, bool>> IsActive { get; }
    Task Remove(WorkTime workTime);
    Task Update(WorkTime workTime);
}
