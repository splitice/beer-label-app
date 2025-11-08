# Beer Label App

A simple C# console application for generating printable beer labels by filling in a PDF template. This application uses the provided beer label template and overlays your custom data (beer name, brewery, style, ABV, IBU, packaged date, and notes) to create professional-looking printable labels.

## Features

- Uses a provided PDF template for consistent label design
- Fills in label data by overlaying text on the template
- Customizable label fields including:
  - Beer Name
  - Brewery
  - Style (user-editable field)
  - ABV (Alcohol by Volume)
  - IBU (International Bitterness Units)
  - Packaged Date (user-editable field)
  - Notes (user-editable field)
  - Brew Date
- Generate single labels or batch process multiple labels from JSON
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

## Template

The application uses a PDF template file located in the `templates/` directory. The template (`beer-label-template.pdf`) provides the base design and layout for the beer labels. The application fills in the blank fields with your data.

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
dotnet run -- generate <output-path> [beer-name] [brewery] [style] [abv] [ibu] [packaged] [notes]
```

**Example:**
```bash
dotnet run -- generate my-beer.pdf "West Coast IPA" "Hop Paradise Brewery" "IPA" "7.2%" "72" "2024-11-08" "An aggressively hopped IPA with tropical fruit flavors"
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

### Batch Processing from JSON

You can generate multiple labels at once by providing a JSON file with beer data:

```bash
dotnet run -- batch <json-file> <output-directory>
```

**Example:**
```bash
dotnet run -- batch ../examples/sample-beers.json batch-output/
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
├── templates/                    # PDF template files
│   └── beer-label-template.pdf  # Base template for labels
├── examples/                     # Example files
│   ├── sample-beers.json         # Sample JSON with 3 beers
│   └── README.md                 # Examples documentation
├── BeerLabelApp.sln             # Solution file
└── README.md                     # This file
```

## Technology Stack

- **Language**: C# 12
- **Framework**: .NET 9.0
- **PDF Library**: PDFsharp 6.2.0 (MIT License)
- **JSON**: System.Text.Json 9.0.10

## User-Editable Fields

The following fields are designated as user-editable in the template:
- **Style**: The beer style
- **Packaged**: The packaging date
- **Notes**: The beer description and tasting notes

These fields can be filled in with your data through the command-line interface or JSON file.

## License

This project is open source and uses the PDFsharp MIT License.

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

## Support

If you encounter any issues or have questions, please file an issue on the GitHub repository.