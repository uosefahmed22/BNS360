using System.Collections;
using System.ComponentModel.DataAnnotations;

namespace BNS360.Core.Helpers
{
    [AttributeUsage(AttributeTargets.Property 
        | AttributeTargets.Field 
        | AttributeTargets.Parameter,
        AllowMultiple = false)]
    public class ListSizeAttribute : ValidationAttribute
    {
        private readonly int _maxSize;
        private readonly int _minSize;
        private string? _errorMessage;
        public ListSizeAttribute(
            int maxSize,
            int minSize = 0,
            string? errorMessage = null)
        {
            _maxSize = maxSize;
            _errorMessage = errorMessage ??
                $"The {{0}} size must be in range {_minSize}. and {_maxSize}";
            _minSize = minSize;
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is IList list && (list.Count > _maxSize || list.Count < _minSize))
            {
                _errorMessage = string.Format(_errorMessage!, validationContext.MemberName);
                return new ValidationResult(_errorMessage);
            }
            return ValidationResult.Success;
        }
    }

}
