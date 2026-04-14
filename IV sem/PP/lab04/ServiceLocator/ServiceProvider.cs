public class ServiceProvider : IServiceProvider
{
    private Dictionary<Type, Func<object>> services;
    private Dictionary<Type, object> scopeservices;

    public ServiceProvider(Dictionary<Type, Func<object>> services, Dictionary<Type, object> scopeservices)
    {
        this.services = services;
        this.scopeservices = scopeservices;
    }

    public object? GetService(Type serviceType)
    {
        object? obj = this.scopeservices.GetValueOrDefault(serviceType);

        if (obj == null && this.services.TryGetValue(serviceType, out var factory))
        {
            obj = factory();
            this.scopeservices[serviceType] = obj;
        }

        return obj;
    }
}