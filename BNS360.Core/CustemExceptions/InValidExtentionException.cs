using BNS360.Core.Helpers.Settings;
using Microsoft.Extensions.Options;

namespace BNS360.Core.CustemExceptions;

public class InValidExtentionException : Exception
{

    public InValidExtentionException(FileUploadSettings fileUploadSettings)

        : base($"Only {string.Join(':', fileUploadSettings.Extensions)} are allawed")
    {
        
    }
}
