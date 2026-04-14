using Microsoft.Extensions.DependencyInjection;
using ServiceLocator;

public static class ScopeLocator
{
    private readonly static Dictionary<Type, Func<object>> services = new Dictionary<Type, Func<object>>();

    public static void Register<T>(Func<T> resolver) where T : class
    {
        services[typeof(T)] = resolver;
    }

    public static IServiceScopeFactory CreateServiceScopeFactory()
    {
        return new ServiceScopeFactory(services);
    }

    public static T Resolve<T>(IServiceScope scope) where T : class
    {
        return scope.ServiceProvider.GetService<T>();
    }
}