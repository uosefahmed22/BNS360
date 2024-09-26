using BNS360.Core.Dto;
using BNS360.Core.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BNS360.Core.IRepository
{
    public interface IFeedbackRepository
    {
        Task<ApiResponse> AddFeedbackForBusiness(string userId, int businessId,string feedback, int rating);
        Task<ApiResponse> AddFeedbackForCraftsMen(string userId, int craftsMenId, string feedback, int rating);
        Task<ApiResponse> GetFeedbackForBusiness(int businessId);
        Task<ApiResponse> GetFeedbackForCraftsMen(int craftsMenId);
        Task<ApiResponse> DeleteBusinessFeedback(string userId, int feedbackId);
        Task<ApiResponse> DeleteCraftsMenFeedback(string userId, int feedbackId);
    }
}
