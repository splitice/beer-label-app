using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using PdfSharp.Drawing;
using PdfSharp.Fonts;
using PdfSharp.Snippets.Font;

namespace BeerLabelGenerator;

/// <summary>
/// Generates printable beer labels by filling in a PDF template
/// </summary>
public class PdfLabelGenerator
{
    private const string TEMPLATE_FILENAME = "beer-label-template.pdf";

    static PdfLabelGenerator()
    {
        // Setup font resolver for PDFsharp
        if (GlobalFontSettings.FontResolver == null)
        {
            GlobalFontSettings.FontResolver = new FailsafeFontResolver();
        }
    }

    /// <summary>
    /// Generates a beer label PDF by filling in the template with provided data
    /// </summary>
    /// <param name="labelData">The beer label data</param>
    /// <param name="outputPath">The output path for the generated PDF</param>
    /// <param name="makeEditable">Not used - kept for compatibility</param>
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

        // Get template path - look in templates folder relative to executable
        var executableDir = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) ?? "";
        var templatePath = Path.Combine(executableDir, "templates", TEMPLATE_FILENAME);

        if (!File.Exists(templatePath))
        {
            throw new FileNotFoundException($"Template file not found: {templatePath}. Please ensure the template PDF is in the templates folder.");
        }

        try
        {
            // Open the template PDF
            using var document = PdfReader.Open(templatePath, PdfDocumentOpenMode.Import);
            
            // Create a new PDF document
            using var outputDocument = new PdfDocument();
            
            // Copy the first page from the template
            var page = outputDocument.AddPage(document.Pages[0]);
            
            // Create graphics object to draw on the page
            using var gfx = XGraphics.FromPdfPage(page);
            
            // Set up fonts
            var font = new XFont("Helvetica", 11, XFontStyleEx.Regular);
            var boldFont = new XFont("Helvetica", 18, XFontStyleEx.Bold);
            var smallFont = new XFont("Helvetica", 10, XFontStyleEx.Regular);
            
            // Get page dimensions
            var pageWidth = page.Width.Point;
            var pageHeight = page.Height.Point;
            
            // Define positions for overlaying text (these are estimates and may need adjustment)
            // Coordinates in PDF are from bottom-left, but XGraphics uses top-left
            double leftMargin = 100;
            double topMargin = 200;
            
            // Beer Name
            if (!string.IsNullOrEmpty(labelData.BeerName))
            {
                gfx.DrawString(labelData.BeerName, boldFont, XBrushes.Black, 
                    new XPoint(leftMargin, topMargin));
            }
            
            // Brewery
            if (!string.IsNullOrEmpty(labelData.Brewery))
            {
                gfx.DrawString(labelData.Brewery, font, XBrushes.Black,
                    new XPoint(leftMargin, topMargin + 30));
            }
            
            // Style (editable field)
            if (!string.IsNullOrEmpty(labelData.Style))
            {
                gfx.DrawString(labelData.Style, font, XBrushes.Black,
                    new XPoint(leftMargin + 100, topMargin + 70));
            }
            
            // ABV
            if (!string.IsNullOrEmpty(labelData.ABV))
            {
                gfx.DrawString(labelData.ABV, font, XBrushes.Black,
                    new XPoint(leftMargin + 100, topMargin + 100));
            }
            
            // IBU
            if (!string.IsNullOrEmpty(labelData.IBU))
            {
                gfx.DrawString(labelData.IBU, font, XBrushes.Black,
                    new XPoint(leftMargin + 100, topMargin + 130));
            }
            
            // Packaged (editable field)
            if (!string.IsNullOrEmpty(labelData.Packaged))
            {
                gfx.DrawString(labelData.Packaged, font, XBrushes.Black,
                    new XPoint(leftMargin + 100, topMargin + 160));
            }
            
            // Brew Date
            if (!string.IsNullOrEmpty(labelData.BrewDate))
            {
                gfx.DrawString(labelData.BrewDate, font, XBrushes.Black,
                    new XPoint(leftMargin + 100, topMargin + 190));
            }
            
            // Notes (editable field - multiline)
            if (!string.IsNullOrEmpty(labelData.Notes))
            {
                DrawMultilineText(gfx, labelData.Notes, smallFont, XBrushes.Black,
                    new XRect(leftMargin, topMargin + 250, pageWidth - 2 * leftMargin, 80));
            }
            
            // Save the output document
            outputDocument.Save(outputPath);
            
            Console.WriteLine($"Label generated successfully: {outputPath}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error generating label: {ex.Message}");
            throw;
        }
    }

    /// <summary>
    /// Generates multiple labels from a collection of label data
    /// </summary>
    /// <param name="labels">Collection of label data</param>
    /// <param name="outputDirectory">Directory where labels will be saved</param>
    /// <param name="makeEditable">Not used - kept for compatibility</param>
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

    private void DrawMultilineText(XGraphics gfx, string text, XFont font, XBrush brush, XRect rect)
    {
        var words = text.Split(' ');
        var lines = new List<string>();
        var currentLine = "";
        
        foreach (var word in words)
        {
            var testLine = string.IsNullOrEmpty(currentLine) ? word : $"{currentLine} {word}";
            var size = gfx.MeasureString(testLine, font);
            
            if (size.Width > rect.Width && !string.IsNullOrEmpty(currentLine))
            {
                lines.Add(currentLine);
                currentLine = word;
            }
            else
            {
                currentLine = testLine;
            }
        }
        
        if (!string.IsNullOrEmpty(currentLine))
        {
            lines.Add(currentLine);
        }
        
        // Draw each line
        double y = rect.Y;
        double lineHeight = font.Height + 2;
        
        foreach (var line in lines)
        {
            if (y > rect.Y + rect.Height)
                break; // Stop if we exceed the available height
                
            gfx.DrawString(line, font, brush, new XPoint(rect.X, y));
            y += lineHeight;
        }
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
