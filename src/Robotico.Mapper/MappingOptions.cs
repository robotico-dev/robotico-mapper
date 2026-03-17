namespace Robotico.Mapper;

/// <summary>
/// Optional configuration for convention-based mapping. Use with <see cref="Robotico.Option.Option{T}"/> when you need to customize behavior.
/// </summary>
/// <param name="IgnoreCase">When true, property names are matched case-insensitively. Default is false (case-sensitive).</param>
/// <param name="IgnoreSourceMembers">Optional set of source member names to ignore (e.g. "InternalId"). Null means no members ignored. Not modified by the mapper; read during mapping setup.</param>
/// <remarks>
/// <para><b>When to use</b>: Use <see cref="Robotico.Option.Option{T}.Some"/> with <see cref="MappingOptions"/> when creating a <c>ConventionMapper&lt;TSource, TDestination&gt;</c> with custom behavior; use <see cref="Robotico.Option.Option{T}.None"/> for default convention (case-sensitive, no ignores).</para>
/// <para><b>IgnoreSourceMembers</b>: The set is read during mapping setup (when building the property map cache) and is not modified. Null means no ignores. Callers may pass a mutable set; if the set is mutated elsewhere, prefer an immutable set (e.g. <c>ImmutableHashSet</c>) to avoid surprises.</para>
/// </remarks>
public sealed record MappingOptions(bool IgnoreCase = false, IReadOnlySet<string>? IgnoreSourceMembers = null);
