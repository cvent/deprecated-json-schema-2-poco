# JSON Schema to POCO
The purpose of this tool is to convert JSON schemas based on the [official JSON schema standard](http://json-schema.org/) into C# POCOs. This tool uses JSON.net as a JSON deserializer, and currently supports up to the [v3 draft](http://tools.ietf.org/html/draft-zyp-json-schema-03).

## Usage

### Requirements
* [.NET Framework 4.5](http://www.microsoft.com/en-us/download/details.aspx?id=30653) or later

### Instructions
1. Build the solution in Visual Studio (Ctrl+Shift+B), or run `msbuild \Path\to\.sln`.
2. Run the following command:
```
\Source\Cvent.SchemaToPoco.Console\bin\Debug\Cvent.SchemaToPoco.Console.exe -s \Location\To\Json\Schema
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

## [Troubleshooting](https://github.com/cvent/json-schema-2-poco/wiki/Troubleshooting)
