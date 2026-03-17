namespace Robotico.Mapper.Tests;

/// <summary>
/// Contract and law-style tests for <see cref="ConventionMapper{TSource, TDestination}"/>: mapping preserves matching property values; null source throws.
/// </summary>
public sealed class MapperLawsTests
{
    [Fact]
    public void Map_preserves_matching_property_values()
    {
        ConventionMapper<SampleEntity, SampleDto> mapper = new();
        SampleEntity source = new() { Id = 42, Name = "Test", InternalCode = "X" };
        SampleDto dest = mapper.Map(source);
        Assert.Equal(source.Id, dest.Id);
        Assert.Equal(source.Name, dest.Name);
        Assert.Equal(source.InternalCode, dest.InternalCode);
    }

    [Fact]
    public void Map_partial_destination_preserves_only_matching_properties()
    {
        ConventionMapper<SampleEntity, SampleDtoMinimal> mapper = new();
        SampleEntity source = new() { Id = 1, Name = "A", InternalCode = "ignored" };
        SampleDtoMinimal dest = mapper.Map(source);
        Assert.Equal(source.Id, dest.Id);
        Assert.Equal(source.Name, dest.Name);
    }

    [Fact]
    public void Map_null_source_throws_ArgumentNullException()
    {
        ConventionMapper<SampleEntity, SampleDto> mapper = new();
        Assert.Throws<ArgumentNullException>(() => mapper.Map(null!));
    }
}
