# millify-dotnet 📊

[![Build, Test & Coverage](https://github.com/bolorundurowb/millify-dotnet/actions/workflows/build-and-test.yml/badge.svg)](https://github.com/bolorundurowb/millify-dotnet/actions/workflows/build-and-test.yml)
[![codecov](https://codecov.io/gh/bolorundurowb/millify-dotnet/graph/badge.svg?token=8PIHY52HJS)](https://codecov.io/gh/bolorundurowb/millify-dotnet)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)
![NuGet Version](https://img.shields.io/nuget/v/millify)

`millify-dotnet` turns large numbers into short, human-readable strings for dashboards, analytics UIs, reports, and logs.

Inspired by [millify (Node.js)](https://www.npmjs.com/package/millify).

## Install

### .NET CLI
```powershell
dotnet add package millify
```

### Package Manager
```powershell
Install-Package millify
```

### PackageReference
```xml
<PackageReference Include="millify" />
```

## Quick start

```csharp
using MillifyDotnet;

var a = Millify.Shorten(2500);      // 2.5K
var b = Millify.Shorten(1024000);   // 1.0M
var c = Millify.Shorten(-5300000);  // -5.3M
```

## API overview

### `Millify.Shorten`

Overloads:
- `Shorten(long number, MillifyOptions? options = null)`
- `Shorten(double number, MillifyOptions? options = null)`
- `Shorten(decimal number, MillifyOptions? options = null)`

Each returns a formatted string with a unit suffix (for example: `K`, `M`, `B`).

## Customization with `MillifyOptions`

```csharp
using MillifyDotnet;

var options = new MillifyOptions(
    precision: 2,
    lowercase: true,
    spaceBeforeUnit: true,
    units: new[] { "", "k", "m", "b", "t" });

var result = Millify.Shorten(1440000, options); // 1.44 m
```

| Option | Type | Default | Description |
|---|---|---|---|
| `Precision` | `int` | `1` | Number of decimal places. Must be `>= 1`. |
| `Lowercase` | `bool` | `false` | Outputs suffix in lowercase. |
| `SpaceBeforeUnit` | `bool` | `false` | Adds a space between number and unit. |
| `Units` | `string[]` | `{"", "K", "M", "B", "T", "P", "E"}` | Custom suffix list. |

## Target frameworks

Library targets:
- `netstandard1.6`
- `netstandard2.0`
- `netstandard2.1`

## For contributors

Contributions are welcome.

### Local development
1. Clone the repository.
2. Restore/build from the solution in Rider or with `dotnet`.
3. Run tests in `src\millify.Tests`.

### Project layout
- `src\millify` — library source
- `src\millify.Tests` — test project

## License

MIT. See [LICENSE](LICENSE).