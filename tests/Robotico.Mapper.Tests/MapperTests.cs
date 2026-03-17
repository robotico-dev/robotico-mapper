namespace Robotico.Mapper.Tests;

/// <summary>
/// Tests for <see cref="IMapper{TSource, TDestination}"/> and <see cref="ConventionMapper{TSource, TDestination}"/>: property mapping, options, null/exception behavior, and contract.
/// </summary>
public sealed class MapperTests
{
    [Fact]
    public void IMapper_contract_exists()
    {
        Assert.True(typeof(IMapper<object, object>).IsInterface);
    }

    [Fact]
    public void ConventionMapper_maps_by_property_name()
    {
        ConventionMapper<SampleEntity, SampleDto> mapper = new();
        SampleEntity source = new() { Id = 42, Name = "Test" };
        SampleDto dest = mapper.Map(source);
        Assert.Equal(42, dest.Id);
        Assert.Equal("Test", dest.Name);
    }

    [Fact]
    public void ConventionMapper_partial_destination_only_maps_matching_properties()
    {
        ConventionMapper<SampleEntity, SampleDtoMinimal> mapper = new();
        SampleEntity source = new() { Id = 1, Name = "A", InternalCode = "x" };
        SampleDtoMinimal dest = mapper.Map(source);
        Assert.Equal(1, dest.Id);
        Assert.Equal("A", dest.Name);
    }

    [Fact]
    public void ConventionMapper_with_IgnoreSourceMembers_skips_specified_members()
    {
        MappingOptions options = new(IgnoreCase: false, IgnoreSourceMembers: new HashSet<string> { "InternalCode" });
        ConventionMapper<SampleEntity, SampleDto> mapper = new(Option<MappingOptions>.Some(options));
        SampleEntity source = new() { Id = 2, Name = "B", InternalCode = "skip" };
        SampleDto dest = mapper.Map(source);
        Assert.Equal(2, dest.Id);
        Assert.Equal("B", dest.Name);
        Assert.Null(dest.InternalCode);
    }

    [Fact]
    public void ConventionMapper_with_Option_None_uses_default_options()
    {
        ConventionMapper<SampleEntity, SampleDto> mapper = new(Option<MappingOptions>.None);
        SampleEntity source = new() { Id = 3, Name = "C" };
        SampleDto dest = mapper.Map(source);
        Assert.Equal(3, dest.Id);
        Assert.Equal("C", dest.Name);
    }

    [Fact]
    public void ConventionMapper_throws_on_null_source()
    {
        ConventionMapper<SampleEntity, SampleDto> mapper = new();
        Assert.Throws<ArgumentNullException>(() => mapper.Map(null!));
    }

    [Fact]
    public void ConventionMapper_with_IgnoreCase_maps_case_insensitively()
    {
        MappingOptions options = new(IgnoreCase: true, IgnoreSourceMembers: null);
        ConventionMapper<SampleEntity, SampleDtoCaseInsensitive> mapper = new(Option<MappingOptions>.Some(options));
        SampleEntity source = new() { Id = 10, Name = "CaseTest" };
        SampleDtoCaseInsensitive dest = mapper.Map(source);
        Assert.Equal(10, dest.id);
        Assert.Equal("CaseTest", dest.NAME);
    }

    [Fact]
    public void ConventionMapper_throws_InvalidOperationException_when_destination_has_no_parameterless_constructor()
    {
        ConventionMapper<SampleEntity, NoDefaultCtorDto> mapper = new();
        SampleEntity source = new() { Id = 1, Name = "X" };
        InvalidOperationException ex = Assert.Throws<InvalidOperationException>(() => mapper.Map(source));
        Assert.Contains("parameterless constructor", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void ConventionMapper_maps_to_value_type_destination()
    {
        ConventionMapper<SampleEntity, SampleStructDto> mapper = new();
        SampleEntity source = new() { Id = 7, Name = "Struct" };
        SampleStructDto dest = mapper.Map(source);
        Assert.Equal(7, dest.Id);
        Assert.Equal("Struct", dest.Name);
    }

    [Fact]
    public void ConventionMapper_maps_to_record_destination()
    {
        ConventionMapper<SampleEntity, SampleRecordDto> mapper = new();
        SampleEntity source = new() { Id = 99, Name = "Record" };
        SampleRecordDto dest = mapper.Map(source);
        Assert.Equal(99, dest.Id);
        Assert.Equal("Record", dest.Name);
    }

    [Fact]
    public void ConventionMapper_empty_maps_returns_default_instance()
    {
        ConventionMapper<NoMatchingSource, NoMatchingDest> mapper = new();
        NoMatchingSource source = new() { OnlyOnSource = 42 };
        NoMatchingDest dest = mapper.Map(source);
        Assert.NotNull(dest);
        Assert.Equal(0, dest.OnlyOnDest);
    }
}
