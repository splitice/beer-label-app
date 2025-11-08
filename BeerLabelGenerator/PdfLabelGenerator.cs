using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace BeerLabelGenerator;

/// <summary>
/// Generates printable beer labels from data
/// </summary>
public class PdfLabelGenerator
{
    public PdfLabelGenerator()
    {
        // Configure QuestPDF license for community/open-source use
        QuestPDF.Settings.License = LicenseType.Community;
    }

    /// <summary>
    /// Generates a beer label PDF with the provided data
    /// </summary>
    /// <param name="labelData">The beer label data</param>
    /// <param name="outputPath">The output path for the generated PDF</param>
    public void GenerateLabel(LabelData labelData, string outputPath)
    {
        if (labelData == null)
            throw new ArgumentNullException(nameof(labelData));
        
        if (string.IsNullOrEmpty(outputPath))
            throw new ArgumentException("Output path cannot be null or empty", nameof(outputPath));

        // Create output directory if it doesn't exist
        var outputDir = Path.GetDirectoryName(outputPath);
        if (!string.IsNullOrEmpty(outputDir) && !Directory.Exists(outputDir))
        {
            Directory.CreateDirectory(outputDir);
        }

        // Generate the PDF
        Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(50);
                page.DefaultTextStyle(x => x.FontSize(11));

                page.Header().Element(Header);
                page.Content().Element(c => Content(c, labelData));
                page.Footer().AlignCenter().Text(x =>
                {
                    x.Span("Generated on ");
                    x.Span(DateTime.Now.ToString("yyyy-MM-dd")).SemiBold();
                });
            });
        }).GeneratePdf(outputPath);

        Console.WriteLine($"Label generated successfully: {outputPath}");
    }

    /// <summary>
    /// Generates multiple labels from a collection of label data
    /// </summary>
    /// <param name="labels">Collection of label data</param>
    /// <param name="outputDirectory">Directory where labels will be saved</param>
    public void GenerateLabels(IEnumerable<LabelData> labels, string outputDirectory)
    {
        if (labels == null)
            throw new ArgumentNullException(nameof(labels));

        if (!Directory.Exists(outputDirectory))
        {
            Directory.CreateDirectory(outputDirectory);
        }

        int count = 0;
        foreach (var label in labels)
        {
            count++;
            var filename = SanitizeFilename(label.BeerName) ?? $"label_{count}";
            var outputPath = Path.Combine(outputDirectory, $"{filename}.pdf");
            GenerateLabel(label, outputPath);
        }

        Console.WriteLine($"Generated {count} label(s) in {outputDirectory}");
    }

    private void Header(IContainer container)
    {
        container.Row(row =>
        {
            row.RelativeItem().Column(column =>
            {
                column.Item().AlignCenter().Text("BEER LABEL")
                    .FontSize(24).Bold().FontColor(Colors.Blue.Darken3);
                
                column.Item().PaddingTop(5).AlignCenter().Text("Craft Beverage Information")
                    .FontSize(12).Italic().FontColor(Colors.Grey.Darken1);
            });
        });
    }

    private void Content(IContainer container, LabelData data)
    {
        container.PaddingVertical(20).Column(column =>
        {
            column.Spacing(15);

            // Beer Name - Prominently displayed
            column.Item().BorderBottom(2).BorderColor(Colors.Blue.Darken3)
                .PaddingBottom(10).Text(data.BeerName)
                .FontSize(20).Bold().FontColor(Colors.Blue.Darken3);

            // Brewery
            if (!string.IsNullOrEmpty(data.Brewery))
            {
                column.Item().Row(row =>
                {
                    row.ConstantItem(120).Text("Brewery:").Bold();
                    row.RelativeItem().Text(data.Brewery);
                });
            }

            // Style
            if (!string.IsNullOrEmpty(data.Style))
            {
                column.Item().Row(row =>
                {
                    row.ConstantItem(120).Text("Style:").Bold();
                    row.RelativeItem().Text(data.Style);
                });
            }

            // ABV
            if (!string.IsNullOrEmpty(data.ABV))
            {
                column.Item().Row(row =>
                {
                    row.ConstantItem(120).Text("ABV:").Bold();
                    row.RelativeItem().Text(data.ABV);
                });
            }

            // IBU
            if (!string.IsNullOrEmpty(data.IBU))
            {
                column.Item().Row(row =>
                {
                    row.ConstantItem(120).Text("IBU:").Bold();
                    row.RelativeItem().Text(data.IBU);
                });
            }

            // Brew Date
            if (!string.IsNullOrEmpty(data.BrewDate))
            {
                column.Item().Row(row =>
                {
                    row.ConstantItem(120).Text("Brew Date:").Bold();
                    row.RelativeItem().Text(data.BrewDate);
                });
            }

            // Description
            if (!string.IsNullOrEmpty(data.Description))
            {
                column.Item().PaddingTop(10).Column(descColumn =>
                {
                    descColumn.Item().Text("Description:").Bold().FontSize(12);
                    descColumn.Item().PaddingTop(5).Background(Colors.Grey.Lighten4)
                        .Padding(10).Text(data.Description).FontSize(10);
                });
            }
        });
    }

    private static string? SanitizeFilename(string filename)
    {
        if (string.IsNullOrEmpty(filename))
            return null;

        var invalidChars = Path.GetInvalidFileNameChars();
        var sanitized = string.Join("_", filename.Split(invalidChars));
        return sanitized.Replace(" ", "_");
    }
}
