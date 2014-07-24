# JSON Schema to POCO
Converts JSON schema files into plain old CLR objects in C#.

# Usage
1. Build the solution in Visual Studio (Ctrl+Shift+B), or `msbuild \Path\to\sln`.
2. Run the following command:
```
\Source\Cvent.SchemaToPoco.Console\bin\Debug\Cvent.SchemaToPoco.Console.exe -s \Location\To\Json\Schema
```

### Optional Flags

`-n name.for.namespace`

`-o \Location\To\Generate\Files` (will generate directories relative to exe location unless a full directory path is given)

`-v` (Prints out generated code without generating files)
