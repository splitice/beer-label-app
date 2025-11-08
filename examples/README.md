# Examples

This directory contains example files for the Beer Label Generator.

## sample-beers.json

This file demonstrates the JSON format for batch processing beer labels. Each entry in the array represents one beer label with the following fields:

- `BeerName`: The name of the beer
- `Brewery`: The brewery that produces the beer
- `Style`: The style of beer (e.g., IPA, Stout, Lager)
- `ABV`: Alcohol by Volume percentage
- `IBU`: International Bitterness Units
- `Description`: A description of the beer's characteristics
- `BrewDate`: The date the beer was brewed (YYYY-MM-DD format)

## Usage

To generate labels from this example file:

```bash
cd BeerLabelGenerator
dotnet run -- batch ../examples/sample-beers.json output/
```

This will create PDF labels for all beers defined in the JSON file in the `output/` directory.

## Creating Your Own

You can create your own JSON file by copying `sample-beers.json` and modifying the values. All fields are required for best results, though the application will handle missing fields gracefully.
