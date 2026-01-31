using Linky.Api.Features.SyncConsumption;

using Xunit;

namespace Linky.Tests.Features.SyncConsumption;

public class EnedisMapperTests
{
    private readonly EnedisMapper _mapper;

    public EnedisMapperTests()
    {
        _mapper = new EnedisMapper();
    }

    [Fact]
    public void Map_ShouldTransformResponseToEntriesCorrectly()
    {
        // Arrange (Prepare data)
        var response = new EnedisLoadCurveResponse
        {
            MeterReading = new MeterReading
            {
                UsagePointId = "12345678901234",
                IntervalReadings = new List<IntervalReading>
                {
                    new() { Value = "1200", Date = new DateTime(2026, 1, 30, 10, 0, 0) },
                    new() { Value = "850", Date = new DateTime(2026, 1, 30, 10, 30, 0) }
                }
            }
        };

        // Act (Action)
        var result = _mapper.Map(response);

        // Assert (Check result)
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.Equal(1200m, result[0].Watts);
        Assert.Equal("12345678901234", result[0].Prm);
        Assert.Equal(new DateTime(2026, 1, 30, 10, 0, 0), result[0].Timestamp);
    }

    [Theory]
    [InlineData("invalid", 0)] // Check for invalid string
    [InlineData("", 0)]        // Check for empty string
    public void Map_ShouldHandleInvalidWattsGracefully(string invalidValue, decimal expected)
    {
        // Arrange
        var response = new EnedisLoadCurveResponse();
        response.MeterReading.IntervalReadings.Add(new IntervalReading { Value = invalidValue });

        // Act
        var result = _mapper.Map(response);

        // Assert
        Assert.Equal(expected, result[0].Watts);
    }
}