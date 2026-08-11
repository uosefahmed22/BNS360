using BNS360.Core.Errors;
using BNS360.Core.IRepository;
using BNS360.Core.Models;
using BNS360.Repository.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BNS360.Repository.Repository
{
    public class SavedJobsRepository : ISavedJobsRepository
    {
        private readonly AppDbContext _dbContext;

        public SavedJobsRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<ApiResponse> GetSavedJobs(string userId)
        {
            try
            {
                var savedJobs = await _dbContext
                    .SavedJobs
                    .Where(x => x.UserId == userId)
                    .Include(x => x.JobModel)
                    .Select(x => new
                    {
                        x.JobModel.Id,
                        x.JobModel.JobTitleArabic,
                        x.JobModel.JobTitleEnglish,
                        x.JobModel.JobDescriptionArabic,
                        x.JobModel.JobDescriptionEnglish,
                        x.JobModel.AddreesInArabic,
                        x.JobModel.AddreesInEnglish,
                        x.JobModel.Numbers,
                        x.JobModel.Type,
                        x.JobModel.WorkHours,
                        x.JobModel.Salary,
                        x.JobModel.Requirements,
                        x.JobModel.TimeAddedjob
                    }
                    ).ToListAsync();
                return new ApiResponse(200, savedJobs);
            }
            catch (Exception ex)
            {
                return new ApiResponse(400, ex.Message);
            }
        }
        public async Task<ApiResponse> SaveJob(int jobId, string userId)
        {
            try
            {
                var jobExists = await _dbContext.Jobs.AnyAsync(x => x.Id == jobId);
                if (!jobExists)
                    return new ApiResponse(404, "الوظيفة غير موجودة");

                var alreadySaved = await _dbContext.SavedJobs
                    .AnyAsync(x => x.JobId == jobId && x.UserId == userId);
                if (alreadySaved)
                    return new ApiResponse(400, "الوظيفة محفوظة بالفعل");

                var savedJob = new SavedJobsModel
                {
                    JobId = jobId,
                    UserId = userId
                };
                await _dbContext.SavedJobs.AddAsync(savedJob);
                await _dbContext.SaveChangesAsync();
                return new ApiResponse(200, "تم الحفظ بنجاح");
            }
            catch (Exception ex)
            {
                return new ApiResponse(400, ex.Message);
            }
        }
        public async Task<ApiResponse> UnSaveJob(int jobId, string userId)
        {
            try
            {
                var savedJob = await _dbContext
                    .SavedJobs
                    .Where(x => x.JobId == jobId && x.UserId == userId)
                    .FirstOrDefaultAsync();
                if (savedJob == null)
                    return new ApiResponse(404, "الوظيفة غير موجودة");
                _dbContext.SavedJobs.Remove(savedJob);
                await _dbContext.SaveChangesAsync();
                return new ApiResponse(200, "تم الحذف بنجاح");
            }
            catch (Exception ex)
            {
                return new ApiResponse(400, ex.Message);
            }
        }
    }
}