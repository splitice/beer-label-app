using BeerLabelGenerator;
using System.Text.Json;

Console.WriteLine("=== Beer Label Generator ===\n");

// Check command line arguments
if (args.Length == 0)
{
    ShowUsage();
    return;
}

var command = args[0].ToLower();

try
{
    switch (command)
    {
        case "generate":
            GenerateSingleLabel(args);
            break;

        case "batch":
            GenerateBatchFromJson(args);
            break;

        case "demo":
            RunDemo();
            break;

        default:
            Console.WriteLine($"Unknown command: {command}");
            ShowUsage();
            break;
    }
}
catch (Exception ex)
{
    Console.WriteLine($"Error: {ex.Message}");
    Environment.Exit(1);
}

static void ShowUsage()
{
    Console.WriteLine("Usage:");
    Console.WriteLine("  BeerLabelGenerator generate <output-path> [options]");
    Console.WriteLine("    Generates a label with provided data using the PDF template");
    Console.WriteLine("    Options: [beer-name] [brewery] [style] [abv] [ibu] [packaged] [notes]");
    Console.WriteLine();
    Console.WriteLine("  BeerLabelGenerator batch <json-file> <output-directory>");
    Console.WriteLine("    Generates labels from a JSON file containing multiple beer entries");
    Console.WriteLine();
    Console.WriteLine("  BeerLabelGenerator demo");
    Console.WriteLine("    Runs a demo that creates sample labels");
    Console.WriteLine();
    Console.WriteLine("Examples:");
    Console.WriteLine("  BeerLabelGenerator generate output.pdf \"IPA\" \"My Brewery\"");
    Console.WriteLine("  BeerLabelGenerator batch beers.json output/");
    Console.WriteLine("  BeerLabelGenerator demo");
}

static void GenerateSingleLabel(string[] args)
{
    if (args.Length < 2)
    {
        Console.WriteLine("Error: Output path required");
        Console.WriteLine("Usage: BeerLabelGenerator generate <output-path> [beer-name] [brewery] [style] [abv] [ibu] [packaged] [notes]");
        return;
    }

    var outputPath = args[1];

    var labelData = new LabelData
    {
        BeerName = args.Length > 2 ? args[2] : "Sample Beer",
        Brewery = args.Length > 3 ? args[3] : "Sample Brewery",
        Style = args.Length > 4 ? args[4] : "IPA",
        ABV = args.Length > 5 ? args[5] : "6.5%",
        IBU = args.Length > 6 ? args[6] : "45",
        Packaged = args.Length > 7 ? args[7] : DateTime.Now.ToString("yyyy-MM-dd"),
        Notes = args.Length > 8 ? args[8] : "A delicious craft beer",
        BrewDate = DateTime.Now.ToString("yyyy-MM-dd")
    };

    var generator = new PdfLabelGenerator();
    generator.GenerateLabel(labelData, outputPath);
}

static void GenerateBatchFromJson(string[] args)
{
    if (args.Length < 3)
    {
        Console.WriteLine("Error: JSON file path and output directory required");
        Console.WriteLine("Usage: BeerLabelGenerator batch <json-file> <output-directory>");
        return;
    }

    var jsonPath = args[1];
    var outputDir = args[2];

    if (!File.Exists(jsonPath))
    {
        Console.WriteLine($"Error: JSON file not found: {jsonPath}");
        return;
    }

    var jsonContent = File.ReadAllText(jsonPath);
    var labels = JsonSerializer.Deserialize<LabelData[]>(jsonContent);

    if (labels == null || labels.Length == 0)
    {
        Console.WriteLine("Error: No labels found in JSON file");
        return;
    }

    Console.WriteLine($"Found {labels.Length} beer(s) in JSON file");
    var generator = new PdfLabelGenerator();
    generator.GenerateLabels(labels, outputDir);
}

static void RunDemo()
{
    Console.WriteLine("Running demo...\n");

    var outputDir = "demo-output";
    if (!Directory.Exists(outputDir))
    {
        Directory.CreateDirectory(outputDir);
    }

    // Create sample labels
    var sampleLabels = new[]
    {
        new LabelData
        {
            BeerName = "Hoppy IPA",
            Brewery = "Craft Masters Brewery",
            Style = "India Pale Ale",
            ABV = "6.8%",
            IBU = "65",
            Packaged = "2024-10-20",
            Notes = "A bold and hoppy IPA with citrus and pine notes. Dry-hopped with Cascade and Centennial hops for maximum hop flavor and aroma.",
            BrewDate = "2024-10-15"
        },
        new LabelData
        {
            BeerName = "Dark Stout",
            Brewery = "Craft Masters Brewery",
            Style = "Imperial Stout",
            ABV = "9.2%",
            IBU = "40",
            Packaged = "2024-09-15",
            Notes = "Rich and roasty with notes of chocolate, coffee, and dark fruit. Aged in bourbon barrels for 6 months.",
            BrewDate = "2024-09-01"
        },
        new LabelData
        {
            BeerName = "Golden Lager",
            Brewery = "Summit Brewing Co.",
            Style = "German Pilsner",
            ABV = "4.8%",
            IBU = "32",
            Packaged = "2024-11-05",
            Notes = "Crisp and refreshing lager with a clean finish. Brewed with noble hops and German pilsner malt.",
            BrewDate = "2024-11-01"
        }
    };

    Console.WriteLine("Generating sample labels...");
    var generator = new PdfLabelGenerator();
    generator.GenerateLabels(sampleLabels, outputDir);

    Console.WriteLine($"\nDemo complete! Check the '{outputDir}' directory for:");
    Console.WriteLine($"  - Hoppy_IPA.pdf");
    Console.WriteLine($"  - Dark_Stout.pdf");
    Console.WriteLine($"  - Golden_Lager.pdf");
}
