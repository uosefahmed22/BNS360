using BNS360.Core.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BNS360.Core.IRepository
{
    public interface ISavedJobsRepository
    {
        Task<ApiResponse> SaveJob(int jobId, string userId);
        Task<ApiResponse> UnSaveJob(int jobId, string userId);
        Task<ApiResponse> GetSavedJobs(string userId);  
    }
}
