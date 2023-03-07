using Microsoft.Extensions.DependencyInjection;

namespace DependencyInjectionLearning;

public interface IExampleScopedService : IReportServiceLifetime
{
    ServiceLifetime IReportServiceLifetime.Lifetime => ServiceLifetime.Scoped;
}