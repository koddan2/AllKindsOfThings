
# General

Leverage Json2CSharpCodeGenerator (https://github.com/Json2CSharp/Json2CSharpCodeGenerator) via the command-line.

## Usage

```bash
J2CS.exe -d /my/dir/with/json-files -i **/*.json
```

or

```bash
J2CS.exe -c my-config.json -d /my/dir/with/json-files -i **/*.json
# where my-config.json is a JSON file that is serializable into a CSharpCodeWriterConfig object
```

example `my-config.json`

```json
{
  "UsePascalCase": true,
  "UseNestedClasses": false,
  "AttributeLibrary": "SystemTextJson",
  "NullValueHandlingIgnore": true,
  "AttributeUsage": "Always",
  "OutputType": "MutableClass",
  "OutputMembers": "AsProperties",
  "ReadOnlyCollectionProperties": false,
  "AlwaysUseNullables": true,
  "Namespace": "Generated",
  "HasNamespace": true,
  "SecondaryNamespace": null,
  "ExamplesInDocumentation": false,
  "ApplyObfuscationAttributes": false,
  "MainClass": null,
  "InternalVisibility": true,
  "CollectionType": "Array"
}
```