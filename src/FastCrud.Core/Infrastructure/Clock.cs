using FastCrud.Abstractions;

namespace FastCrud.Core.Infrastructure;
public sealed class SystemClock : IFastCrudClock
{
    public DateTime UtcNow => DateTime.UtcNow;
}
