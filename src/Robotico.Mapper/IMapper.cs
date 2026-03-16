namespace Robotico.Mapper;

/// <summary>
/// Maps a source instance to a destination type. Placeholder for convention/config-based or source-generated mapping.
/// </summary>
/// <typeparam name="TSource">Source type.</typeparam>
/// <typeparam name="TDestination">Destination type.</typeparam>
public interface IMapper<in TSource, TDestination>
{
    /// <summary>
    /// Maps the source to an instance of the destination type.
    /// </summary>
    TDestination Map(TSource source);
}
