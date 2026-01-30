namespace Linky.Api.Features.Common;

/// <summary>
/// Manual mapping.
/// </summary>
public interface IManualMapper<in TSource, out TDestination>
{
    TDestination Map(TSource source);
}