namespace Robotico.Mapper;

/// <summary>
/// Maps a source instance to a destination type. Use for Entity-to-DTO, request-to-command, or any object-to-object mapping.
/// Implement manually, use <c>ConventionMapper&lt;TSource, TDestination&gt;</c> for convention-based mapping by property name, or use a source generator for production.
/// </summary>
/// <typeparam name="TSource">Source type (e.g. <see cref="Robotico.Domain.IEntity{TId}"/> or domain model).</typeparam>
/// <typeparam name="TDestination">Destination type (e.g. DTO or API response).</typeparam>
/// <remarks>
/// <para><b>When to use</b>: Use in API/application layer to map domain entities or value objects to DTOs. Compose with <see cref="Robotico.Option.Option{T}"/> for optional mapping configuration.</para>
/// </remarks>
public interface IMapper<in TSource, TDestination>
{
    /// <summary>
    /// Maps the source to an instance of the destination type.
    /// </summary>
    /// <param name="source">The source instance (e.g. entity or domain object).</param>
    /// <returns>A new instance of <typeparamref name="TDestination"/>.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="source"/> is null (implementations may throw).</exception>
    TDestination Map(TSource source);
}
