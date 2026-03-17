namespace Robotico.Mapper.Tests;

/// <summary>
/// Property-style and parameterized tests for mapper contract and laws using [Theory] and [InlineData].
/// Aligns with robotico-results-csharp ResultLawsTheoryTests quality.
/// </summary>
public sealed class MapperLawsTheoryTests
{
    [Theory]
    [InlineData(1, "A")]
    [InlineData(42, "Test")]
    [InlineData(0, "")]
    public void Map_preserves_matching_property_values(int id, string name)
    {
        ConventionMapper<SampleEntity, SampleDto> mapper = new();
        SampleEntity source = new() { Id = id, Name = name, InternalCode = "x" };
        SampleDto dest = mapper.Map(source);
        Assert.Equal(id, dest.Id);
        Assert.Equal(name, dest.Name);
    }

    [Theory]
    [InlineData(10, "CaseTest")]
    [InlineData(0, "")]
    public void Map_with_IgnoreCase_preserves_values_case_insensitive(int id, string name)
    {
        MappingOptions options = new(IgnoreCase: true, IgnoreSourceMembers: null);
        ConventionMapper<SampleEntity, SampleDtoCaseInsensitive> mapper = new(Option<MappingOptions>.Some(options));
        SampleEntity source = new() { Id = id, Name = name };
        SampleDtoCaseInsensitive dest = mapper.Map(source);
        Assert.Equal(id, dest.id);
        Assert.Equal(name, dest.NAME);
    }

    [Theory]
    [InlineData(2, "B", "skip")]
    [InlineData(3, "C", "ignored")]
    public void Map_with_IgnoreSourceMembers_omits_specified_member(int id, string name, string internalCode)
    {
        MappingOptions options = new(IgnoreCase: false, IgnoreSourceMembers: new HashSet<string> { "InternalCode" });
        ConventionMapper<SampleEntity, SampleDto> mapper = new(Option<MappingOptions>.Some(options));
        SampleEntity source = new() { Id = id, Name = name, InternalCode = internalCode };
        SampleDto dest = mapper.Map(source);
        Assert.Equal(id, dest.Id);
        Assert.Equal(name, dest.Name);
        Assert.Null(dest.InternalCode);
    }

    [Theory]
    [InlineData(7, "Struct")]
    [InlineData(0, "")]
    public void Map_to_value_type_destination_preserves_matching_properties(int id, string name)
    {
        ConventionMapper<SampleEntity, SampleStructDto> mapper = new();
        SampleEntity source = new() { Id = id, Name = name };
        SampleStructDto dest = mapper.Map(source);
        Assert.Equal(id, dest.Id);
        Assert.Equal(name, dest.Name);
    }

    [Theory]
    [InlineData(99, "Record")]
    [InlineData(1, "R")]
    public void Map_to_record_destination_preserves_matching_properties(int id, string name)
    {
        ConventionMapper<SampleEntity, SampleRecordDto> mapper = new();
        SampleEntity source = new() { Id = id, Name = name };
        SampleRecordDto dest = mapper.Map(source);
        Assert.Equal(id, dest.Id);
        Assert.Equal(name, dest.Name);
    }
}
