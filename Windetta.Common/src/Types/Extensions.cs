using Autofac;
using System.Reflection;

namespace Windetta.Common.Types;

public static class Extensions
{
    /// <summary>
    /// Resolves all dependencies from calling assembly.
    /// Interfaces are used to obtain the service lifecycle : IScopedService, ISingletonService, ITransientService
    /// In general, this code automates the process
    /// of registering services in the dependency injection container, 
    /// determining the lifetime of the service based on the interface implemented by it.
    /// </summary>
    public static void ResolveDependenciesFromAssembly(this ContainerBuilder builder)
    {
        var typesFromCallingAssembly = Assembly.GetCallingAssembly().GetTypes();
        var typesFromCommonAssembly = Assembly.GetExecutingAssembly().GetTypes();

        var allTypes = new List<Type>();
        allTypes.AddRange(typesFromCallingAssembly);
        allTypes.AddRange(typesFromCommonAssembly);

        var implementationServices = allTypes
            .Where(x => x.IsClass)
            .Where(x => !x.CustomAttributes.Any(
                x => x.AttributeType == typeof(AutoInjectExcludeAttribute)))
            .Where(x => x.IsAssignableTo(typeof(IServiceLifetime)))
            .Where(x => x.IsAbstract == false)
            .ToList();

        var assignableFilter = delegate (Type typeObj, object? criteriaObj)
        {
            return typeObj.IsAssignableTo(typeof(IServiceLifetime));
        };

        var filter = new TypeFilter(assignableFilter);

        foreach (var s in implementationServices)
        {
            var serviceType = GetServiceType(s, filter);
            var lifetimeType = GetLifeTimeType(s, filter);

            AddToContainer(builder, lifetimeType, s, serviceType);
        }
    }

    private static Type GetLifeTimeType(Type root, TypeFilter filter)
    {
        var cachedType = root;
        var type = root.FindInterfaces(filter, null).First();

        if (type.FindInterfaces(filter, null).FirstOrDefault() is null)
            return cachedType;

        return GetLifeTimeType(type, filter);
    }

    private static Type GetServiceType(Type root, TypeFilter filter)
        => root.FindInterfaces(filter, null).First();

    private static void AddToContainer(
        ContainerBuilder builder,
        Type? lifetimeType,
        Type implementationType,
        Type serviceType)
    {
        var serviceTypeAttr = implementationType
            .GetCustomAttribute(typeof(AutoInjectServiceTypeAttribute<>));

        if (serviceTypeAttr is not null)
        {
            var serviceTypeReplacement = serviceTypeAttr
                .GetType()
                .GetProperty(AutoInjectServiceTypeAttribute<object>.NameOfProperty)?
                .GetValue(serviceTypeAttr);

            serviceType = (Type)serviceTypeReplacement!;
        }

        var registered = builder
            .RegisterType(implementationType);

        if (lifetimeType == serviceType)
        {
            registered = registered.AsSelf();
        }
        else
        {
            registered = registered
                .IfNotRegistered(serviceType)
                .As(serviceType);
        }

        // pattern matching
        Action serviceLifeTimeApply = lifetimeType switch
        {
            Type type when type.Equals(typeof(ITransientService)) => () => registered.InstancePerDependency(),
            Type type when type.Equals(typeof(IScopedService)) => () => registered.InstancePerLifetimeScope(),
            Type type when type.Equals(typeof(ISingletonService)) => () => registered.SingleInstance(),
            _ => throw new Exception("Invalid lifetime type")
        };

        serviceLifeTimeApply();
    }
}
