using Microsoft.Extensions.DependencyInjection;

namespace DependencyInjectionLearning;

public interface IReportServiceLifetime
{
    Guid Id { get; }

    ServiceLifetime Lifetime { get; }
}