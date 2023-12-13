using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace BNS360.Core.Helpers.Extintions
{
    public static class ModelValidationHandler
    {
        public static List<string> GetModelErrors(this ModelStateDictionary modelState)
        {
            var errors = new List<string>();
            foreach (var entry in modelState)
            {
                var entryErrors = entry.Value.Errors.ToList();
                var singleEntryErrors = string.Empty;
                // get errors for each entry value
                foreach (var error in entryErrors)
                {
                    singleEntryErrors += error.ErrorMessage + ".\n";
                }
                //singleEntryErrors += "_"; // seprate between eantry errors message with _
                errors.Add(singleEntryErrors);
            }
            return errors;
        }
    }
}