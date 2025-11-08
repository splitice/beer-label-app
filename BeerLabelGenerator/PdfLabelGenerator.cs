using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using TableDescriptor = QuestPDF.Fluent.TableDescriptor;

namespace BeerLabelGenerator;

/// <summary>
/// Generates printable beer labels with bordered template design
/// </summary>
public class PdfLabelGenerator
{
    public PdfLabelGenerator()
    {
        // Configure QuestPDF license for community/open-source use
        QuestPDF.Settings.License = LicenseType.Community;
    }

    /// <summary>
    /// Generates a beer label PDF with bordered template design
    /// </summary>
    /// <param name="labelData">The beer label data</param>
    /// <param name="outputPath">The output path for the generated PDF</param>
    /// <param name="makeEditable">If true, highlights Style, Packaged, and Notes fields for easy editing</param>
    public void GenerateLabel(LabelData labelData, string outputPath, bool makeEditable = false)
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
                page.Margin(40);
                page.DefaultTextStyle(x => x.FontSize(11));

                page.Content().Element(c => CreateBorderedLabel(c, labelData, makeEditable));
            });
        }).GeneratePdf(outputPath);

        Console.WriteLine($"Label generated successfully: {outputPath}");
    }

    /// <summary>
    /// Generates multiple labels from a collection of label data
    /// </summary>
    /// <param name="labels">Collection of label data</param>
    /// <param name="outputDirectory">Directory where labels will be saved</param>
    /// <param name="makeEditable">If true, highlights editable fields</param>
    public void GenerateLabels(IEnumerable<LabelData> labels, string outputDirectory, bool makeEditable = false)
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
            GenerateLabel(label, outputPath, makeEditable);
        }

        Console.WriteLine($"Generated {count} label(s) in {outputDirectory}");
    }

    private void CreateBorderedLabel(IContainer container, LabelData data, bool makeEditable)
    {
        container.Column(column =>
        {
            column.Spacing(10);

            // Header - Beer Name
            column.Item().AlignCenter().Text(data.BeerName)
                .FontSize(24).Bold().FontColor(Colors.Blue.Darken3);

            // Brewery Name
            if (!string.IsNullOrEmpty(data.Brewery))
            {
                column.Item().AlignCenter().Text(data.Brewery)
                    .FontSize(14).FontColor(Colors.Grey.Darken1);
            }

            column.Item().PaddingVertical(10);

            // Main content table with borders
            column.Item().Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.ConstantColumn(150); // Label column
                    columns.RelativeColumn(); // Value column
                });

                // Style field - editable
                AddTableRow(table, "Style:", data.Style, makeEditable, true);

                // ABV
                AddTableRow(table, "ABV:", data.ABV, false, false);

                // IBU
                AddTableRow(table, "IBU:", data.IBU, false, false);

                // Packaged field - editable
                AddTableRow(table, "Packaged:", data.Packaged, makeEditable, true);

                // Brew Date
                AddTableRow(table, "Brew Date:", data.BrewDate, false, false);
            });

            column.Item().PaddingTop(15);

            // Notes section - editable
            column.Item().Column(notesColumn =>
            {
                notesColumn.Item().Text("Notes:")
                    .FontSize(12).Bold();

                notesColumn.Item().PaddingTop(5).Border(1).BorderColor(Colors.Grey.Medium)
                    .Padding(10)
                    .Background(makeEditable ? Colors.Yellow.Lighten4 : Colors.Grey.Lighten4)
                    .MinHeight(100)
                    .Text(data.Notes)
                    .FontSize(10)
                    .FontColor(makeEditable ? Colors.Blue.Darken2 : Colors.Black);
            });

            // Footer note if editable
            if (makeEditable)
            {
                column.Item().PaddingTop(20).AlignCenter()
                    .Text("Highlighted fields (Style, Packaged, Notes) can be edited in a PDF editor")
                    .FontSize(8).Italic().FontColor(Colors.Grey.Darken1);
            }
        });
    }

    private void AddTableRow(TableDescriptor table, string label, string value, bool makeEditable, bool isEditableField)
    {
        // Label cell
        table.Cell().Border(1).BorderColor(Colors.Black)
            .Background(Colors.Grey.Lighten3)
            .Padding(8)
            .AlignMiddle()
            .Text(text =>
            {
                text.Span(label).Bold().FontSize(11);
            });

        // Value cell - Highlight editable fields
        table.Cell().Border(1).BorderColor(Colors.Black)
            .Padding(8)
            .Background(makeEditable && isEditableField ? Colors.Yellow.Lighten4 : Colors.White)
            .AlignMiddle()
            .Text(text =>
            {
                if (makeEditable && isEditableField)
                {
                    text.Span(value).FontSize(11).FontColor(Colors.Blue.Darken2);
                }
                else
                {
                    text.Span(value).FontSize(11);
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
