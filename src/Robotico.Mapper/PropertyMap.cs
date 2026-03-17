using System.Reflection;

namespace Robotico.Mapper;

internal sealed class PropertyMap<TSource>
{
    private readonly MethodInfo _sourceGet;
    private readonly MethodInfo _destSet;

    internal PropertyMap(PropertyInfo sourceProperty, PropertyInfo destinationProperty)
    {
        SourceProperty = sourceProperty;
        DestinationProperty = destinationProperty;
        _sourceGet = sourceProperty.GetMethod!;
        _destSet = destinationProperty.SetMethod!;
    }

    internal PropertyInfo SourceProperty { get; }
    internal PropertyInfo DestinationProperty { get; }

    internal object? GetSourceValue(TSource source) => _sourceGet.Invoke(source, null);

    internal void SetDestinationValue(object dest, object? value) => _destSet.Invoke(dest, [value]);
}
