namespace BeerLabelGenerator;

/// <summary>
/// Represents the data for a beer label
/// </summary>
public class LabelData
{
    public string BeerName { get; set; } = string.Empty;
    public string Brewery { get; set; } = string.Empty;
    public string Style { get; set; } = string.Empty;
    public string ABV { get; set; } = string.Empty;
    public string IBU { get; set; } = string.Empty;
    public string Packaged { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;
    public string BrewDate { get; set; } = string.Empty;
}
