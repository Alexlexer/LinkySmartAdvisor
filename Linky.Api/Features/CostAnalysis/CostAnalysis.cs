namespace Linky.Api.Features.CostAnalysis;

public record DailyCostReport(
    DateTime Date,
    decimal TotalKWh,
    decimal TotalCostEuro,
    List<HourlyCostBreakdown> Details);

public record HourlyCostBreakdown(
    DateTime Timestamp,
    decimal KWh,
    decimal PricePerKWh,
    decimal CostEuro);