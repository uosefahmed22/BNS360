using BNS360.Core.Dto;
using BNS360.Core.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BNS360.Core.IRepository
{
    public interface IJobRepository
    {
        Task<ApiResponse> AddJob(JobModelDto model);
        Task<ApiResponse> UpdateJob(int jobId, string userId, JobModelDto model);
        Task<ApiResponse> DeleteJob(int jobId, string userId);
        Task<ApiResponse> GetJobById(int JobId);
        Task<ApiResponse> GetAllJobs();
    }
}
