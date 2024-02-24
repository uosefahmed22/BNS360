using BNS360.Core.Dtos.Request;
using BNS360.Core.Entities;
using BNS360.Core.Services.AppBusniss;
using BNS360.Core.Services.Shared;
using BNS360.Reposatory.Data.AppBusniss;
using System.Linq.Expressions;

namespace BNS360.Reposatory.Repositories.Repositories
{
    public class WorkTimeService : IWorkTimeService
    {
        private readonly AppBusnissDbContext _dbContext;
        private readonly IDateTimeProvider _dateTimeProvider;
        public WorkTimeService(AppBusnissDbContext dbContext, IDateTimeProvider dateTimeProvider)
        {
            _dbContext = dbContext;
            _dateTimeProvider = dateTimeProvider;
        }
        
        public Expression<Func<WorkTime, bool>> IsActive => 
            wt => wt.Day == _dateTimeProvider.Now.DayOfWeek &&
            wt.Start <= _dateTimeProvider.CurrentTime && 
            wt.End > _dateTimeProvider.CurrentTime;

        public async Task Add(WorkTimeDto workTime, Guid busnissId)
        {
            var wt = new WorkTime
            {
                Day = workTime.Day,
                Start = workTime.Strart,
                End = workTime.End,
                BusnissId = busnissId
            };

            _dbContext.WorkTime.Add(wt);
            await _dbContext.SaveChangesAsync();
        }

        public async Task Remove(WorkTime workTime)
        {
           _dbContext.WorkTime.Remove(workTime);    
            await _dbContext.SaveChangesAsync();
        }

        public async Task Update(WorkTime workTime)
        {
            _dbContext.WorkTime.Update(workTime);
            await _dbContext.SaveChangesAsync();
        }

    }
}
