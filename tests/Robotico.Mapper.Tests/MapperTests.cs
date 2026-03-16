using Robotico.Mapper;
using Xunit;

namespace Robotico.Mapper.Tests;

public sealed class MapperTests
{
    [Fact]
    public void IMapper_contract_exists()
    {
        // Placeholder: ensures the library and IMapper<TSource, TDestination> are loadable.
        Assert.True(typeof(IMapper<object, object>).IsInterface);
    }
}
