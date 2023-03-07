
using Microsoft.Extensions.DependencyInjection;

namespace DependencyInjectionLearning;

public interface IExampleTransientService : IReportServiceLifetime
{
    ServiceLifetime IReportServiceLifetime.Lifetime => ServiceLifetime.Transient;
}
