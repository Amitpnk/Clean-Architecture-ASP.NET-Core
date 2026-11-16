using CleanArch.Application.Mappings;
using Mapster;
using MapsterMapper;

namespace CleanArch.Application.UnitTests.Common;

internal static class MapperFactory
{
    /// <summary>A mapper built from the real <see cref="MappingConfig"/>, isolated from global settings.</summary>
    public static IMapper Create()
    {
        var config = new TypeAdapterConfig();
        config.Scan(typeof(MappingConfig).Assembly);
        config.Compile();
        return new Mapper(config);
    }
}
