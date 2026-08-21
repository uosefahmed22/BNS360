using BNS360.Core.Dto;
using BNS360.Core.Errors;
using BNS360.Core.IRepository;
using BNS360.Core.Models;
using BNS360.Repository.Data;
using BNS360.Repository.Mapping;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BNS360.Repository.Repository
{
    public class JobRepository : IJobRepository
    {
        private readonly AppDbContext _dbContext;
        public JobRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<ApiResponse> AddJob(JobModelDto model)
        {
            try
            {
                var job = model.ToEntity();
                await _dbContext.Jobs.AddAsync(job);
                await _dbContext.SaveChangesAsync();
                return new ApiResponse(200, "تمت الاضافة بنجاح");
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<ApiResponse> DeleteJob(int jobId, string userId)
        {
            try
            {
                var job = await _dbContext.Jobs
                    .FirstOrDefaultAsync(x => x.Id == jobId && x.UserId == userId);
                if (job == null)
                    return new ApiResponse(404, "الوظيفة غير موجودة");
                _dbContext.Jobs.Remove(job);
                await _dbContext.SaveChangesAsync();
                return new ApiResponse(200, "تم الحذف بنجاح");
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<ApiResponse> GetAllJobs()
        {
            try
            {
                var jobs =await _dbContext
                    .Jobs
                    .Select(x=> new
                    {
                        x.Id,
                        x.JobTitleArabic,
                        x.JobTitleEnglish,
                        x.JobDescriptionArabic,
                        x.JobDescriptionEnglish,
                        x.AddreesInArabic,
                        x.AddreesInEnglish,
                        x.Numbers,
                        x.Type,
                        x.WorkHours,
                        x.Salary,
                        x.Requirements,
                        x.TimeAddedjob
                    })
                    .ToListAsync();
                return new ApiResponse(200, jobs);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<ApiResponse> GetJobById(int JobId)
        {
            try
            {
                var job = await _dbContext
                    .Jobs
                    .Select(x => new
                    {
                        x.Id,
                        x.JobTitleArabic,
                        x.JobTitleEnglish,
                        x.JobDescriptionArabic,
                        x.JobDescriptionEnglish,
                        x.AddreesInArabic,
                        x.AddreesInEnglish,
                        x.Numbers,
                        x.Type,
                        x.WorkHours,
                        x.Salary,
                        x.Requirements,
                        x.TimeAddedjob
                    })

                    .FirstOrDefaultAsync(x => x.Id == JobId);
                if (job == null)
                    return new ApiResponse(404, "الوظيفة غير موجودة");
                return new ApiResponse(200, job);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<ApiResponse> UpdateJob(int jobId, string userId, JobModelDto model)
        {
            try
            {
                var job = await _dbContext.Jobs
                    .FirstOrDefaultAsync(x => x.Id == jobId && x.UserId == userId);
                if (job == null)
                    return new ApiResponse(404, "الوظيفة غير موجودة");

                model.ApplyTo(job);
                await _dbContext.SaveChangesAsync();
                return new ApiResponse(200, "تم التعديل بنجاح");
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
