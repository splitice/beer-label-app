# Examples

This directory contains example files for the Beer Label Generator.

## sample-beers.json

This file demonstrates the JSON format for batch processing beer labels. Each entry in the array represents one beer label with the following fields:

- `BeerName`: The name of the beer
- `Brewery`: The brewery that produces the beer
- `Style`: The style of beer (e.g., IPA, Stout, Lager) - **can be highlighted as editable**
- `ABV`: Alcohol by Volume percentage
- `IBU`: International Bitterness Units
- `Packaged`: The date the beer was packaged (YYYY-MM-DD format) - **can be highlighted as editable**
- `Notes`: Description and tasting notes for the beer - **can be highlighted as editable**
- `BrewDate`: The date the beer was brewed (YYYY-MM-DD format)

## Usage

To generate labels from this example file:

```bash
cd BeerLabelGenerator
dotnet run -- batch ../examples/sample-beers.json output/
```

To generate labels with editable fields highlighted (Style, Packaged, Notes):

```bash
cd BeerLabelGenerator
dotnet run -- batch ../examples/sample-beers.json output/ --editable
```

This will create PDF labels for all beers defined in the JSON file in the `output/` directory.

## Editable Fields

When you use the `--editable` flag, the following fields will be highlighted in yellow with blue text to indicate they can be easily edited in a PDF editor:
- **Style**: The beer style
- **Packaged**: The packaging date
- **Notes**: The beer description and tasting notes

## Creating Your Own

You can create your own JSON file by copying `sample-beers.json` and modifying the values. All fields are required for best results, though the application will handle missing fields gracefully.

### Example Entry

```json
{
  "BeerName": "Holy Water Pale Ale",
  "Brewery": "Heard's Brewery",
  "Style": "Pale Ale",
  "ABV": "5.2%",
  "IBU": "35",
  "Packaged": "2024-11-08",
  "Notes": "A refreshing pale ale with citrus and floral hop character. Perfect for any occasion.",
  "BrewDate": "2024-10-25"
}
```
