using BenchmarkDotNet.Attributes;
using Robotico.Mapper;

namespace Robotico.Mapper.Benchmarks;

[MemoryDiagnoser]
[ShortRunJob]
public class MapperBenchmarks
{
    private static readonly ConventionMapper<SourceEntity, DestDto> Mapper = new();
    private static readonly SourceEntity Source = new() { Id = 1, Name = "Test" };

    [Benchmark(Baseline = true)]
    public DestDto Map_ConventionMapper()
    {
        return Mapper.Map(Source);
    }

    [Benchmark]
    public DestDto Map_ConventionMapper_Reuse()
    {
        DestDto dto = Mapper.Map(Source);
        return Mapper.Map(Source);
    }
}
