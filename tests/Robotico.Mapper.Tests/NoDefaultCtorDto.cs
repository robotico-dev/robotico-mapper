namespace Robotico.Mapper.Tests;

public sealed class NoDefaultCtorDto(int id)
{
    public int Id { get; set; } = id;
}
