namespace ServiceLocator;

public static class SingletonLocator
{
    private readonly static Dictionary<Type, object> services = new Dictionary<Type, object>();

    public static void Register<T>(T service) where T : class
    {
        services[typeof(T)] = service;
    }

    public static T Resolve<T>()
    {
        return (T)services[typeof(T)];
    }

    public static void Reset()
    {
        services.Clear();
    }
}
