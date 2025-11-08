# Beer Label App

A simple C# console application for generating printable beer labels by filling in a PDF template. The template is a standard label sheet with a 3x6 grid layout (18 labels per page on A4), with fields for Style, Packaged date, and Notes.

## Features

- Uses a provided PDF template (3 columns x 6 rows = 18 labels per sheet)
- Fills in the 3 editable fields per label:
  - **Style** (on page 1)
  - **Packaged** (on page 1)  
  - **Notes** (on page 2)
- Generate single label sheets or batch process multiple sheets from JSON
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

The application uses a PDF template file (`templates/beer-label-template.pdf`) with a 3x6 grid layout for 18 labels per page. The template has two pages:
- **Page 1**: Contains Style and Packaged fields for each label
- **Page 2**: Contains Notes field for each label

## Usage

### Run the Demo

The easiest way to see the application in action is to run the demo:

```bash
cd BeerLabelGenerator
dotnet run -- demo
```

This will create a `demo-output` directory with `label-sheet-1.pdf` containing 3 sample labels.

### Generate a Single Label Sheet

To generate a label sheet with one label:

```bash
dotnet run -- generate <output-path> [beer-name] [brewery] [style] [abv] [ibu] [packaged] [notes]
```

**Example:**
```bash
dotnet run -- generate my-label.pdf "West Coast IPA" "Hop Paradise Brewery" "IPA" "7.2%" "72" "2024-11-08" "An aggressively hopped IPA with tropical fruit flavors"
```

**Parameters:**
- `output-path`: Path where the PDF will be saved (required)
- `beer-name`: Name of the beer (optional)
- `brewery`: Name of the brewery (optional)
- `style`: Beer style - **fills the Style field** (optional, default: "IPA")
- `abv`: Alcohol by Volume (optional)
- `ibu`: International Bitterness Units (optional)
- `packaged`: Packaging date - **fills the Packaged field** (optional, default: current date)
- `notes`: Beer notes/description - **fills the Notes field on page 2** (optional)

### Batch Processing from JSON

You can generate multiple label sheets at once by providing a JSON file with beer data. The application will create sheets with up to 18 labels each.

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
│   └── beer-label-template.pdf  # 3x6 label sheet template (2 pages)
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

## How It Works

The application:
1. Loads the PDF template (`beer-label-template.pdf`)
2. Calculates positions for each label in the 3x6 grid
3. Overlays text for the 3 fields (Style, Packaged, Notes) at the appropriate positions
4. Saves the filled template as a new PDF

Each label sheet can contain up to 18 labels. When batch processing, if you have more than 18 labels, multiple sheets will be generated.

## License

This project is open source and uses the PDFsharp MIT License.

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

## Support

If you encounter any issues or have questions, please file an issue on the GitHub repository.