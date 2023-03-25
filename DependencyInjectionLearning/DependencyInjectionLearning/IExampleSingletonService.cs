using Microsoft.Extensions.DependencyInjection;

namespace DependencyInjectionLearning;

public interface IExampleSingletonService : IReportServiceLifetime
{
    ServiceLifetime IReportServiceLifetime.Lifetime => ServiceLifetime.Singleton;
}