# JSON Schema to POCO
The purpose of this tool is to convert JSON schemas based on the [official JSON schema standard](http://json-schema.org/) into C# POCOs. This tool uses JSON.net as a JSON deserializer, and currently supports up to the [v3 draft](http://tools.ietf.org/html/draft-zyp-json-schema-03).

Turn this JSON schema:
```json
{
  "$schema": "http://json-schema.org/draft-03/schema#",
  "title": "DataSet",
  "description": "A result set and description of measures and values",
  "type": "object",
  "properties": {
    "results": {
      "$ref": "data-result.json"
    },
    "dimensions": {
      "type": "array",
      "description": "An array of data dimensions included in a result set",
      "items": {
        "$ref": "data-dimension.json"
      },
      "uniqueItems": true
    },
    "measure": {
      "$ref": "data-measure.json",
      "description": "single measure represented in this data set."
    }
  }
}
```
Into this! (with all references generated in separate files)
```csharp
namespace generated
{
    using System;
    using generated;
    using com.cvent.board.core;
    using System.Collections.Generic;


    // A result set and description of measures and values
    public class DataSet
    {

        // A result list for data sets
        public List<DataResult> _results;

        // An array of data dimensions included in a result set
        public HashSet<DataDimension> _dimensions;

        // Information about a measure returned by a data-source
        public DataMeasure _measure;

        public DataSet()
        {
        }

        // A result list for data sets
        public virtual List<DataResult> Results
        {
            get
            {
                return _results;
            }
            set
            {
                _results = value;
            }
        }

        // An array of data dimensions included in a result set
        public virtual HashSet<DataDimension> Dimensions
        {
            get
            {
                return _dimensions;
            }
            set
            {
                _dimensions = value;
            }
        }

        // Information about a measure returned by a data-source
        public virtual DataMeasure Measure
        {
            get
            {
                return _measure;
            }
            set
            {
                _measure = value;
            }
        }
    }
}
```

## Usage

### Requirements
* [.NET Framework 4.5](http://www.microsoft.com/en-us/download/details.aspx?id=30653) or later

### Instructions
1. [Download the latest executable](https://github.com/cvent/json-schema-2-poco/releases/latest), build the solution in Visual Studio (Ctrl+Shift+B), or run `msbuild \Path\to\.sln`.
2. Run the following command:
```
Cvent.SchemaToPoco.Console.exe -s \Location\To\Json\Schema
```

### Optional Flags

```
-n name.for.namespace
```
Default: `generated`

```
-o \Location\To\Generate\Files
```
Default: `<exe location>\generated`

```
-v
```
Prints out generated code without generating files

## [Reference](https://github.com/cvent/json-schema-2-poco/wiki/Reference)
Current version: 1.0 (Alpha)

[View changelog](https://github.com/cvent/json-schema-2-poco/wiki/Changelog)

## [Troubleshooting](https://github.com/cvent/json-schema-2-poco/wiki/Troubleshooting)
