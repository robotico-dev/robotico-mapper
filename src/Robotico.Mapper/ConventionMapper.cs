using System.Collections.Concurrent;
using System.Reflection;
using Robotico.Option;

namespace Robotico.Mapper;

/// <summary>
/// Convention-based mapper: maps by matching property names (source readable → destination writable). Uses reflection; cache per type pair for performance.
/// Use for development, tests, or simple Entity/DTO scenarios. For production consider manual <see cref="IMapper{TSource, TDestination}"/> implementations or a source generator.
/// </summary>
/// <remarks>
/// <para><b>Thread-safe</b>: Concurrent calls to <see cref="Map"/> are safe; the internal map cache is shared and populated in a thread-safe manner.</para>
/// <para><b>Convention</b>: For each readable property on <typeparamref name="TSource"/>, if <typeparamref name="TDestination"/> has a writable property with the same name (optionally case-insensitive), its value is copied. Value types and reference types are supported; destination must have a parameterless constructor or be a record.</para>
/// <para><b>Options</b>: Pass <see cref="MappingOptions"/> via <see cref="Robotico.Option.Option{T}"/> to customize case sensitivity and ignored members. The cache key includes a hash of <c>IgnoreSourceMembers</c>; use distinct option instances for distinct ignore sets to avoid rare cache key collisions.</para>
/// </remarks>
public sealed class ConventionMapper<TSource, TDestination> : IMapper<TSource, TDestination>
{
    private static readonly ConcurrentDictionary<(Type Source, Type Dest, bool IgnoreCase, int IgnoreHash), IReadOnlyList<PropertyMap<TSource>>> MapCache = new();
    private readonly Option<MappingOptions> _options;

    /// <summary>
    /// Creates a convention mapper with default options (case-sensitive, no ignores).
    /// </summary>
    public ConventionMapper() => _options = Option<MappingOptions>.None;

    /// <summary>
    /// Creates a convention mapper with the given options.
    /// </summary>
    /// <param name="options">Optional mapping options (case sensitivity, ignored members). Pass <see cref="Option{MappingOptions}.None"/> for defaults. Value type; default is valid and means no custom options.</param>
    public ConventionMapper(Option<MappingOptions> options) => _options = options;

    /// <inheritdoc />
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="source"/> is null.</exception>
    /// <exception cref="InvalidOperationException">Thrown when <typeparamref name="TDestination"/> has no parameterless constructor and is not a value type.</exception>
    public TDestination Map(TSource source)
    {
        ArgumentNullException.ThrowIfNull(source);

        IReadOnlyList<PropertyMap<TSource>> maps = GetOrCreateMaps();
        object dest = CreateDestination();

        foreach (PropertyMap<TSource> map in maps)
        {
            object? value = map.GetSourceValue(source);
            if (value is not null || map.DestinationProperty.PropertyType.IsClass)
            {
                map.SetDestinationValue(dest, value);
            }
        }

        return (TDestination)dest;
    }

    private static object CreateDestination()
    {
        Type destType = typeof(TDestination);
        ConstructorInfo? ctor = destType.GetConstructor(Type.EmptyTypes);
        if (ctor is not null)
        {
            return ctor.Invoke(null);
        }

        if (destType.IsValueType)
        {
            return Activator.CreateInstance(destType)!;
        }

        throw new InvalidOperationException($"Type {destType.FullName} has no parameterless constructor. ConventionMapper requires a default constructor or use a value type.");
    }

    private IReadOnlyList<PropertyMap<TSource>> GetOrCreateMaps()
    {
        bool ignoreCase = _options.TryGetValue(out MappingOptions? opt) && opt.IgnoreCase;
        IReadOnlySet<string>? ignore = opt?.IgnoreSourceMembers;
        int ignoreHash = GetIgnoreSetHash(ignore);

        (Type Source, Type Dest, bool IgnoreCase, int IgnoreHash) key = (typeof(TSource), typeof(TDestination), ignoreCase, ignoreHash);

        return MapCache.GetOrAdd(key, _ => BuildMaps(ignoreCase, ignore));
    }

    private static List<PropertyMap<TSource>> BuildMaps(bool ignoreCase, IReadOnlySet<string>? ignoreSourceMembers)
    {
        Type sourceType = typeof(TSource);
        Type destType = typeof(TDestination);

        PropertyInfo[] sourceProps = sourceType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.CanRead && p.GetMethod is not null).ToArray();

        Dictionary<string, PropertyInfo> destProps = destType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.CanWrite && p.SetMethod is not null)
            .ToDictionary(p => ignoreCase ? p.Name.ToUpperInvariant() : p.Name);

        StringComparison comparison = ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
        List<PropertyMap<TSource>> maps = [];

        foreach (PropertyInfo sp in sourceProps)
        {
            if (ignoreSourceMembers?.Contains(sp.Name) == true)
            {
                continue;
            }

            string key = ignoreCase ? sp.Name.ToUpperInvariant() : sp.Name;
            if (!destProps.TryGetValue(key, out PropertyInfo? dp))
            {
                continue;
            }

            if (!IsCompatible(sp.PropertyType, dp.PropertyType))
            {
                continue;
            }

            maps.Add(new PropertyMap<TSource>(sp, dp));
        }

        return maps;
    }

    private static int GetIgnoreSetHash(IReadOnlySet<string>? ignore)
    {
        if (ignore is null || ignore.Count == 0)
        {
            return 0;
        }

        int h = 0;
        foreach (string s in ignore.OrderBy(x => x, StringComparer.Ordinal))
        {
            h = HashCode.Combine(h, s.GetHashCode(StringComparison.Ordinal));
        }

        return h;
    }

    private static bool IsCompatible(Type sourcePropType, Type destPropType)
    {
        if (destPropType.IsAssignableFrom(sourcePropType))
        {
            return true;
        }

        Type src = Nullable.GetUnderlyingType(sourcePropType) ?? sourcePropType;
        Type dst = Nullable.GetUnderlyingType(destPropType) ?? destPropType;
        return dst.IsAssignableFrom(src);
    }
}
