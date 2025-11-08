using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using PdfSharp.Drawing;
using PdfSharp.Fonts;
using PdfSharp.Snippets.Font;

namespace BeerLabelGenerator;

/// <summary>
/// Generates printable beer labels by filling in a PDF template
/// The template is a 3x6 grid (18 labels per page) with fields on page 1 (style, packaged) and page 2 (notes)
/// </summary>
public class PdfLabelGenerator
{
    private const string TEMPLATE_FILENAME = "beer-label-template.pdf";
    
    // Template layout: 3 columns x 6 rows = 18 labels per page
    private const int COLUMNS = 3;
    private const int ROWS = 6;
    private const int LABELS_PER_PAGE = COLUMNS * ROWS;
    
    // A4 page size: 595.28 x 841.89 points
    // These values need to be calibrated based on actual template
    private const double PAGE_WIDTH = 595.28;
    private const double PAGE_HEIGHT = 841.89;
    
    // Calculate label dimensions (approximate, may need adjustment)
    private readonly double labelWidth = PAGE_WIDTH / COLUMNS;
    private readonly double labelHeight = PAGE_HEIGHT / ROWS;

    static PdfLabelGenerator()
    {
        // Setup font resolver for PDFsharp
        if (GlobalFontSettings.FontResolver == null)
        {
            GlobalFontSettings.FontResolver = new FailsafeFontResolver();
        }
    }

    /// <summary>
    /// Generates a beer label sheet PDF by filling in the template with provided data
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

        // For single label, generate a sheet with just this one label
        var labels = new[] { labelData };
        GenerateSheetInternal(labels, outputPath);
    }

    /// <summary>
    /// Generates a label sheet with multiple labels
    /// </summary>
    private void GenerateSheetInternal(IEnumerable<LabelData> labels, string outputPath)
    {
        if (labels == null)
            throw new ArgumentNullException(nameof(labels));
        
        if (string.IsNullOrEmpty(outputPath))
            throw new ArgumentException("Output path cannot be null or empty", nameof(outputPath));

        // Create output directory if it doesn't exist
        var outputDir = Path.GetDirectoryName(outputPath);
        if (!string.IsNullOrEmpty(outputDir) && !Directory.Exists(outputDir))
        {
            Directory.CreateDirectory(outputDir);
        }

        // Get template path
        var executableDir = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) ?? "";
        var templatePath = Path.Combine(executableDir, "templates", TEMPLATE_FILENAME);

        if (!File.Exists(templatePath))
        {
            throw new FileNotFoundException($"Template file not found: {templatePath}. Please ensure the template PDF is in the templates folder.");
        }

        try
        {
            // Open the template PDF
            using var templateDocument = PdfReader.Open(templatePath, PdfDocumentOpenMode.Import);
            
            // Create a new PDF document
            using var outputDocument = new PdfDocument();
            
            // Copy both pages from the template
            var page1 = outputDocument.AddPage(templateDocument.Pages[0]);
            var page2 = templateDocument.Pages.Count > 1 ? outputDocument.AddPage(templateDocument.Pages[1]) : null;
            
            // Create graphics objects for both pages
            using var gfx1 = XGraphics.FromPdfPage(page1);
            using var gfx2 = page2 != null ? XGraphics.FromPdfPage(page2) : null;
            
            // Set up font (smaller size for label fields)
            var font = new XFont("Helvetica", 8, XFontStyleEx.Regular);
            
            // Fill in labels
            var labelList = labels.Take(LABELS_PER_PAGE).ToList();
            for (int i = 0; i < labelList.Count; i++)
            {
                var label = labelList[i];
                int row = i / COLUMNS;
                int col = i % COLUMNS;
                
                FillLabelAtPosition(gfx1, gfx2, label, row, col, font);
            }
            
            // Save the output document
            outputDocument.Save(outputPath);
            
            Console.WriteLine($"Label sheet generated successfully: {outputPath} ({labelList.Count} label(s) filled)");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error generating label: {ex.Message}");
            throw;
        }
    }

    /// <summary>
    /// Generates multiple label sheets from a collection of label data
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

        var labelList = labels.ToList();
        int sheetNumber = 1;
        
        // Process labels in batches of 18 (one sheet at a time)
        for (int i = 0; i < labelList.Count; i += LABELS_PER_PAGE)
        {
            var batchLabels = labelList.Skip(i).Take(LABELS_PER_PAGE);
            var outputPath = Path.Combine(outputDirectory, $"label-sheet-{sheetNumber}.pdf");
            
            GenerateSheetInternal(batchLabels, outputPath);
            sheetNumber++;
        }

        Console.WriteLine($"Generated {sheetNumber - 1} label sheet(s) for {labelList.Count} label(s) in {outputDirectory}");
    }

    private void FillLabelAtPosition(XGraphics? gfx1, XGraphics? gfx2, LabelData label, int row, int col, XFont font)
    {
        // Calculate the top-left corner of this label position
        double x = col * labelWidth;
        double y = row * labelHeight;
        
        // Field positions within each label (these are estimates and will need calibration)
        // Based on typical label sheet layouts, fields are usually centered or at specific positions
        
        // Style field offset (on page 1)
        double styleX = x + labelWidth * 0.5;  // Center horizontally
        double styleY = y + labelHeight * 0.3; // Upper third of label
        
        // Packaged field offset (on page 1)
        double packagedX = x + labelWidth * 0.5;
        double packagedY = y + labelHeight * 0.5; // Middle of label
        
        // Notes field offset (on page 2)
        double notesX = x + labelWidth * 0.5;
        double notesY = y + labelHeight * 0.7; // Lower third of label
        
        // Fill style on page 1
        if (gfx1 != null && !string.IsNullOrEmpty(label.Style))
        {
            DrawCenteredText(gfx1, label.Style, font, XBrushes.Black, styleX, styleY, labelWidth * 0.8);
        }
        
        // Fill packaged on page 1
        if (gfx1 != null && !string.IsNullOrEmpty(label.Packaged))
        {
            DrawCenteredText(gfx1, label.Packaged, font, XBrushes.Black, packagedX, packagedY, labelWidth * 0.8);
        }
        
        // Fill notes on page 2
        if (gfx2 != null && !string.IsNullOrEmpty(label.Notes))
        {
            DrawCenteredText(gfx2, label.Notes, font, XBrushes.Black, notesX, notesY, labelWidth * 0.8);
        }
    }

    private void DrawCenteredText(XGraphics gfx, string text, XFont font, XBrush brush, double centerX, double y, double maxWidth)
    {
        // Measure text
        var size = gfx.MeasureString(text, font);
        
        // If text is too wide, we might need to wrap or truncate
        if (size.Width > maxWidth)
        {
            // For now, just draw at the position (could implement wrapping here)
            double x = centerX - maxWidth / 2;
            gfx.DrawString(text, font, brush, new XRect(x, y, maxWidth, size.Height), 
                XStringFormats.TopLeft);
        }
        else
        {
            // Center the text
            double x = centerX - size.Width / 2;
            gfx.DrawString(text, font, brush, new XPoint(x, y));
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
