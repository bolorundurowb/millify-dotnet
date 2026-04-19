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

var a = Millify.Shorten(2500);             // 2.5K
var b = Millify.Shorten(1_500_000);        // 1.5M
var c = Millify.Shorten(-5_300_000);       // -5.3M
```

## API overview

### `Millify.Shorten`

Overloads:

- `Shorten(long number, MillifyOptions? options = null)`
- `Shorten(double number, MillifyOptions? options = null)`
- `Shorten(decimal number, MillifyOptions? options = null)`
- `Shorten(BigInteger number, MillifyOptions? options = null)`

Each returns a formatted string with a unit suffix (for example: `K`, `M`, `G`).

### `Millify.Decompose` and `Millify.FormatScaled`

Split scaling from formatting when you need the numeric magnitude for tooltips, charts, or accessibility text:

```csharp
using System.Globalization;
using MillifyDotnet;

var options = new MillifyOptions(precision: 2, culture: CultureInfo.GetCultureInfo("fr-FR"));
var parts = Millify.Decompose(9_990, options); // ScaledValue ≈ 9.99, UnitIndex for "K"

var label = parts.ToFormattedString(options);  // same rules as Shorten
var same = Millify.FormatScaled(parts, options);
```

`MillifiedNumber` exposes:

- `ScaledValue` (`decimal`, sign preserved)
- `UnitIndex` (index into `MillifyOptions.Units`)

### `Millify.TryFormat`

Writes the same string as `Shorten` into a `Span<char>` when the buffer is large enough:

```csharp
Span<char> buffer = stackalloc char[32];
if (Millify.TryFormat(1_234.5m, buffer, out var written, options))
{
    var text = buffer.Slice(0, written).ToString();
}
```

Overloads exist for `decimal`, `long`, `double`, and `BigInteger`.

## Customization with `MillifyOptions`

```csharp
using MillifyDotnet;

var options = new MillifyOptions(
    precision: 2,
    lowercase: true,
    spaceBeforeUnit: true,
    units: new[] { "", "k", "m", "b", "t" });

var result = Millify.Shorten(1_440_000, options); // 1.44 m
```

### `MillifyScaleBase` (decimal vs binary)

Use `ScaleBase` to switch between decimal steps (1000) and binary steps (1024), for example for byte counts. When `units` is omitted, binary mode defaults to IEC-style suffixes (`Ki`, `Mi`, `Gi`, …).

```csharp
var bytes = new MillifyOptions(precision: 2, scaleBase: MillifyScaleBase.Binary);
Millify.Shorten(1_048_576, bytes); // 1Mi
```

### Option reference

| Option | Type | Default | Description |
|---|---|---|---|
| `Precision` | `int` | `1` | Maximum fractional digits (`F` precision). Must be `>= 1`. |
| `Lowercase` | `bool` | `false` | When `true`, suffixes are lowercased. When `false`, decimal-style single-letter units are uppercased; binary `ki`-style units become `Ki`, `Mi`, etc. |
| `SpaceBeforeUnit` | `bool` | `false` | Inserts a space between the number and the suffix. |
| `Units` | `string[]` | See below | Suffix per magnitude, index `0` for the unscaled tier. Must be non-empty and must not contain null entries. |
| `ScaleBase` | `MillifyScaleBase` | `Decimal` (`1000`) | `Binary` (`1024`) for IEC-style scaling. |
| `TrimInsignificantZeros` | `bool` | `true` | Removes trailing fractional zeros (for example `2.50K` → `2.5K`). |
| `SmartPrecision` | `bool` | `false` | When `true`, fractional digits are reduced for larger scaled magnitudes: scaled absolute value `>= 100` → `0` decimals; `[10, 100)` → up to `1` (still capped by `Precision`); below `10` (including values `< 1`) → full `Precision`. |
| `Culture` | `CultureInfo?` | `null` | Supplies the **decimal separator only** (invariant when `null`). Digit grouping is disabled so only the separator changes (for example `1,2K` in `fr-FR`). |

### Default `Units`

When you do not pass `units`:

- **`ScaleBase.Decimal`**: `{ "", "k", "m", "g", "t", "p", "e", "z", "y" }` (shown as `K`, `M`, `G`, … when `Lowercase` is `false`).
- **`ScaleBase.Binary`**: `{ "", "ki", "mi", "gi", "ti", "pi", "ei", "zi", "yi" }` (shown as `Ki`, `Mi`, `Gi`, … when `Lowercase` is `false`).

### Constructor parameters

The `MillifyOptions` constructor mirrors the table above, in order:

`precision`, `lowercase`, `spaceBeforeUnit`, `units`, `scaleBase`, `trimInsignificantZeros`, `smartPrecision`, `culture`.

## `BigInteger` notes

`Shorten` and `Decompose` compute the scaled value as `(decimal)abs(n) / (decimal)pow(scaleBase, unitIndex)` when that fits in `decimal`. If the cast overflows, a double ratio is used; if the value is still not representable, an `OverflowException` is thrown suggesting more `Units` tiers so the value can be scaled further.

## Validation

`Units` is validated when options are constructed and whenever the `Units` property is assigned: the array must contain at least one entry, and no element may be `null`.

## Target frameworks

Library targets:

- `netstandard1.6`
- `netstandard2.0`
- `netstandard2.1`

`netstandard1.6` and `netstandard2.0` builds reference [System.Memory](https://www.nuget.org/packages/System.Memory/) so `Span<char>` is available for `TryFormat`.

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
