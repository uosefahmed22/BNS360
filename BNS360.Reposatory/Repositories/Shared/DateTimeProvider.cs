using BNS360.Core.Services.Shared;

namespace BNS360.Reposatory.Repositories.Shared;

public class DateTimeProvider : IDateTimeProvider
{
    public DateTime Now => DateTime.Now;

    public TimeOnly CurrentTime => TimeOnly.FromDateTime(Now);
}
