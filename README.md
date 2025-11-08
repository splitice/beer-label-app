# Beer Label App

A simple C# console application for generating printable beer labels as PDF files. This application allows you to create professional-looking beer labels with brewery information, beer styles, ABV, IBU, descriptions, and more. The fields Style, Packaged, and Notes can be highlighted for easy editing in PDF editors.

## Features

- Generate printable PDF beer labels with bordered template design
- Customizable label fields including:
  - Beer Name
  - Brewery
  - Style (can be highlighted as editable)
  - ABV (Alcohol by Volume)
  - IBU (International Bitterness Units)
  - Packaged Date (can be highlighted as editable)
  - Notes (can be highlighted as editable)
  - Brew Date
- Generate single labels or batch process multiple labels
- Optional highlighting of editable fields (Style, Packaged, Notes) for easy identification in PDF editors
- Clean, professional PDF output with bordered table layout
- Cross-platform (Windows, macOS, Linux)

## Requirements

- .NET 9.0 SDK or later

## Installation

1. Clone the repository:
   ```bash
   git clone https://github.com/splitice/beer-label-app.git
   cd beer-label-app
   ```

2. Build the application:
   ```bash
   dotnet build
   ```

## Usage

### Run the Demo

The easiest way to see the application in action is to run the demo, which generates three sample beer labels:

```bash
cd BeerLabelGenerator
dotnet run -- demo
```

This will create a `demo-output` directory with three sample PDF labels:
- `Hoppy_IPA.pdf`
- `Dark_Stout.pdf`
- `Golden_Lager.pdf`

### Generate a Single Label

To generate a custom beer label:

```bash
dotnet run -- generate <output-path> [beer-name] [brewery] [style] [abv] [ibu] [packaged] [notes] [--editable]
```

**Example:**
```bash
dotnet run -- generate my-beer.pdf "West Coast IPA" "Hop Paradise Brewery" "IPA" "7.2%" "72" "2024-11-08" "An aggressively hopped IPA with tropical fruit flavors"
```

**With editable fields highlighted:**
```bash
dotnet run -- generate my-beer.pdf "West Coast IPA" "Hop Paradise Brewery" "IPA" "7.2%" "72" "2024-11-08" "An aggressively hopped IPA" --editable
```

**Parameters:**
- `output-path`: Path where the PDF will be saved (required)
- `beer-name`: Name of the beer (optional, default: "Sample Beer")
- `brewery`: Name of the brewery (optional, default: "Sample Brewery")
- `style`: Beer style (optional, default: "IPA")
- `abv`: Alcohol by Volume (optional, default: "6.5%")
- `ibu`: International Bitterness Units (optional, default: "45")
- `packaged`: Packaging date (optional, default: current date)
- `notes`: Beer notes/description (optional, default: "A delicious craft beer")
- `--editable`: Highlights the Style, Packaged, and Notes fields in yellow for easy identification when editing in a PDF editor

### Batch Processing from JSON

You can generate multiple labels at once by providing a JSON file with beer data:

```bash
dotnet run -- batch <json-file> <output-directory> [--editable]
```

**Example:**
```bash
dotnet run -- batch ../examples/sample-beers.json batch-output/
```

**With editable fields:**
```bash
dotnet run -- batch ../examples/sample-beers.json batch-output/ --editable
```

**JSON Format:**
```json
[
  {
    "BeerName": "Tropical Thunder IPA",
    "Brewery": "Island Brewing Company",
    "Style": "New England IPA",
    "ABV": "6.5%",
    "IBU": "50",
    "Packaged": "2024-11-05",
    "Notes": "A hazy, juicy IPA bursting with tropical fruit flavors...",
    "BrewDate": "2024-11-01"
  }
]
```

See `examples/sample-beers.json` for a complete example.

### Building a Standalone Executable

To create a standalone executable:

```bash
dotnet publish -c Release -r win-x64 --self-contained
```

Replace `win-x64` with your target platform:
- `win-x64` for Windows 64-bit
- `linux-x64` for Linux 64-bit
- `osx-x64` for macOS 64-bit
- `osx-arm64` for macOS ARM (M1/M2)

The executable will be in `bin/Release/net9.0/[runtime]/publish/`

## Project Structure

```
beer-label-app/
├── BeerLabelGenerator/          # Main application
│   ├── Program.cs               # Entry point and CLI
│   ├── LabelData.cs             # Data model for beer labels
│   ├── PdfLabelGenerator.cs     # PDF generation logic
│   └── BeerLabelGenerator.csproj
├── examples/                     # Example files
│   ├── sample-beers.json         # Sample JSON with 3 beers
│   └── README.md                 # Examples documentation
├── BeerLabelApp.sln             # Solution file
└── README.md                     # This file
```

## Technology Stack

- **Language**: C# 12
- **Framework**: .NET 9.0
- **PDF Library**: QuestPDF 2024.10.3 (Community License)
- **JSON**: System.Text.Json 9.0.10

## Editable Fields

When you use the `--editable` flag, the following fields are highlighted in yellow with blue text:
- **Style**: The beer style
- **Packaged**: The packaging date
- **Notes**: The beer description/notes

These highlighted fields make it easy to identify which content can be modified in a PDF editor after generation.

## License

This project is open source and uses the QuestPDF Community License for non-commercial use.

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

## Examples

### Creating Labels for a Homebrew Batch

```bash
# Create labels for different beers in your brewing lineup
dotnet run -- generate american-ipa.pdf "American IPA" "My Home Brewery" "IPA" "6.8%" "65" "2024-11-08" "Classic American IPA with Cascade hops"
dotnet run -- generate irish-stout.pdf "Irish Dry Stout" "My Home Brewery" "Stout" "4.2%" "35" "2024-11-08" "Smooth and creamy Irish-style stout"
dotnet run -- generate wheat-beer.pdf "Hefeweizen" "My Home Brewery" "Wheat Beer" "5.4%" "12" "2024-11-08" "Traditional German wheat beer with banana and clove notes"
```

### Creating Editable Templates

```bash
# Create a template with highlighted editable fields that you can customize later
dotnet run -- generate template.pdf "Beer Name Here" "Your Brewery" "Style Here" "0.0%" "0" "YYYY-MM-DD" "Add your tasting notes here" --editable
```

## Support

If you encounter any issues or have questions, please file an issue on the GitHub repository.