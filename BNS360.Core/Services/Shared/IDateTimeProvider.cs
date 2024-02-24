using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BNS360.Core.Services.Shared
{
    public interface IDateTimeProvider
    {
        DateTime Now { get; }
        TimeOnly CurrentTime { get; }
    }
}
