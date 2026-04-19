# Millify for .NET

[![Build, Test & Coverage](https://github.com/bolorundurowb/millify-dotnet/actions/workflows/build-and-test.yml/badge.svg)](https://github.com/bolorundurowb/millify-dotnet/actions/workflows/build-and-test.yml)
[![codecov](https://codecov.io/gh/bolorundurowb/millify-dotnet/graph/badge.svg?token=8PIHY52HJS)](https://codecov.io/gh/bolorundurowb/millify-dotnet)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)
[![NuGet](https://img.shields.io/nuget/v/millify)](https://www.nuget.org/packages/millify/)

**Millify** is a small .NET library that turns large counts and measurements into short, readable labels (`2.5K`, `1.44M`, `1Mi`, …). Use it anywhere you would otherwise hand-roll suffix logic: analytics tiles, tables, logs, charts, and mobile or web API payloads.

The [NuGet package](https://www.nuget.org/packages/millify/) is named **millify**. This repository is **millify-dotnet**; the API lives in the `MillifyDotnet` namespace.

Conceptually similar to the [millify](https://www.npmjs.com/package/millify) package for JavaScript.

## What you get

- **One-call formatting** via `Millify.Shorten` for `long`, `double`, `decimal`, and `BigInteger`.
- **Decimal (1000) or binary (1024)** scaling with sensible default suffixes, including IEC-style units for byte-style values.
- **Split “scale then format”** via `Millify.Decompose` and `Millify.FormatScaled` (or `MillifiedNumber.ToFormattedString`) when you need the numeric magnitude for tooltips, charts, tests, or accessibility text.
- **Culture-aware decimal separators** (separator only; digit grouping stays off so labels stay compact).
- **Optional `Span<char>` output** via `Millify.TryFormat` for fixed buffers and reduced allocations.
- **Tunable presentation**: precision, “smart” precision by magnitude, trimming of insignificant fractional zeros, custom unit lists, spacing before the suffix, and casing.

## Install

**Package ID:** `millify`

```powershell
dotnet add package millify
```

Package Manager Console:

```powershell
Install-Package millify
```

Or add a `PackageReference` in your project file (pin a version in production apps):

```xml
<PackageReference Include="millify" Version="1.1.0" />
```

## Quick start

```csharp
using MillifyDotnet;

Millify.Shorten(2500);              // 2.5K
Millify.Shorten(1_500_000);         // 1.5M
Millify.Shorten(-5_300_000);        // -5.3M
Millify.Shorten(1234.5m);           // 1.2K (default precision is 1)
```

Default options trim trailing fractional zeros (for example `1.0M` becomes `1M`). Set `TrimInsignificantZeros = false` on `MillifyOptions` if you want a fixed-width fractional part.

## Common uses

### Custom suffixes and layout

```csharp
var options = new MillifyOptions(
    precision: 2,
    lowercase: true,
    spaceBeforeUnit: true,
    units: new[] { "", "k", "m", "b", "t" });

Millify.Shorten(1_440_000, options); // "1.44 m"
```

### Byte-style values (1024 steps, IEC-style defaults)

When `ScaleBase` is `Binary` and you do not pass `units`, suffixes default to IEC-style (`Ki`, `Mi`, `Gi`, …) with appropriate casing.

```csharp
var bytes = new MillifyOptions(precision: 2, scaleBase: MillifyScaleBase.Binary);
Millify.Shorten(1_048_576, bytes); // 1Mi
```

### Localized decimal separator

`Culture` affects the **decimal separator only** (for example `1,2K` with `fr-FR`). Thousands separators are not inserted.

```csharp
using System.Globalization;
using MillifyDotnet;

var fr = new MillifyOptions(precision: 1, culture: CultureInfo.GetCultureInfo("fr-FR"));
Millify.Shorten(1234.5m, fr); // "1,2K" — comma as decimal separator
```

### Decompose for UI logic

```csharp
using System.Globalization;
using MillifyDotnet;

var options = new MillifyOptions(precision: 2, culture: CultureInfo.GetCultureInfo("fr-FR"));
var parts = Millify.Decompose(9_990, options);

var label = parts.ToFormattedString(options);   // same formatting rules as Shorten
var same = Millify.FormatScaled(parts, options);
// parts.ScaledValue — signed magnitude after scaling
// parts.UnitIndex   — index into options.Units for the active suffix
```

### `TryFormat` into a buffer

```csharp
Span<char> buffer = stackalloc char[32];
if (Millify.TryFormat(1234.5m, buffer, out var written))
{
    var text = buffer[..written].ToString();
}
```

Overloads exist for `decimal`, `long`, `double`, and `BigInteger`. On `netstandard1.6` and `netstandard2.0`, the library references [System.Memory](https://www.nuget.org/packages/System.Memory/) so `Span<char>` is available.

### Smarter fractions on large magnitudes

Enable `SmartPrecision` to reduce fractional digits when the scaled value is large (for denser dashboards), while keeping finer detail when the scaled magnitude is small. Behavior is still capped by `Precision`.

```csharp
var dense = new MillifyOptions(precision: 2, smartPrecision: true);
Millify.Shorten(9_990, dense);       // 9.99K — full precision while scaled value < 10
Millify.Shorten(125_000_000, dense); // 125M — no fractional digits once scaled value ≥ 100
```

## API at a glance

| Member | Role |
|--------|------|
| `Millify.Shorten(...)` | Format a number to a single string with suffix. |
| `Millify.Decompose(...)` | Return `MillifiedNumber` (scaled value + unit index) without final string formatting. |
| `Millify.FormatScaled(MillifiedNumber, ...)` | Format a decomposed value with the same rules as `Shorten`. |
| `MillifiedNumber.ToFormattedString(...)` | Instance helper equivalent to `FormatScaled`. |
| `Millify.TryFormat(..., Span<char>, ...)` | Write the same string as `Shorten` into a buffer; returns `false` if the buffer is too small. |
| `MillifyOptions` | Precision, casing, spacing, custom `Units`, `ScaleBase`, trimming, smart precision, `Culture`. |
| `MillifyScaleBase` | `Decimal` (1000) or `Binary` (1024) steps between tiers. |

## `MillifyOptions` reference

| Option | Type | Default | Description |
|--------|------|---------|-------------|
| `Precision` | `int` | `1` | Maximum fractional digits (`F` precision). Must be `>= 1`. |
| `Lowercase` | `bool` | `false` | When `true`, suffixes are lowercased. When `false`, decimal single-letter units are uppercased; binary `ki`-style units become `Ki`, `Mi`, etc. |
| `SpaceBeforeUnit` | `bool` | `false` | Inserts a space between the number and the suffix. |
| `Units` | `string[]` | See below | Suffix per magnitude, index `0` for the unscaled tier. Must be non-empty and must not contain null entries. |
| `ScaleBase` | `MillifyScaleBase` | `Decimal` (`1000`) | `Binary` (`1024`) for IEC-style scaling. |
| `TrimInsignificantZeros` | `bool` | `true` | Removes trailing fractional zeros (for example `2.50K` → `2.5K`). |
| `SmartPrecision` | `bool` | `false` | When `true`, fractional digits are reduced for larger scaled magnitudes: scaled absolute value `>= 100` → `0` decimals; `[10, 100)` → up to `1` (still capped by `Precision`); below `10` (including values `< 1`) → full `Precision`. |
| `Culture` | `CultureInfo?` | `null` | Supplies the **decimal separator only** (invariant when `null`). Digit grouping is disabled so only the separator changes (for example `1,2K` in `fr-FR`). |

Constructor parameter order (all have defaults except where noted):  
`precision`, `lowercase`, `spaceBeforeUnit`, `units`, `scaleBase`, `trimInsignificantZeros`, `smartPrecision`, `culture`.

### Default `Units`

When you do not pass `units`:

- **`ScaleBase.Decimal`**: `{ "", "k", "m", "g", "t", "p", "e", "z", "y" }` (shown as `K`, `M`, `G`, … when `Lowercase` is `false`).
- **`ScaleBase.Binary`**: `{ "", "ki", "mi", "gi", "ti", "pi", "ei", "zi", "yi" }` (shown as `Ki`, `Mi`, `Gi`, … when `Lowercase` is `false`).

## `BigInteger` behavior

`Shorten` and `Decompose` compute the scaled value as `(decimal)abs(n) / (decimal)pow(scaleBase, unitIndex)` when that fits in `decimal`. If the cast overflows, a double ratio is used; if the value is still not representable, an `OverflowException` is thrown with guidance to add more `Units` tiers so the value can be scaled further.

## Validation

`Units` is validated when options are constructed and whenever the `Units` property is assigned: the array must contain at least one entry, and no element may be `null`.

## Target frameworks

The library targets:

- `netstandard1.6`
- `netstandard2.0`
- `netstandard2.1`

`netstandard1.6` and `netstandard2.0` builds reference [System.Memory](https://www.nuget.org/packages/System.Memory/) for `Span<char>` support in `TryFormat`.

Tests in this repository target a current .NET SDK (see [CI](.github/workflows/build-and-test.yml)); your app only needs a TFM compatible with the standards above.

---

## Contributing

Issues and pull requests are welcome. The **millify** package ships this README from the repository root.

**Build and test** (from the repository root):

```powershell
cd src
dotnet restore
dotnet build --configuration Release
dotnet test --configuration Release
```

CI runs the same steps under Ubuntu with the .NET 10 SDK. The solution file is `src/millify.slnx`.

**Layout**

- `src/millify` — library source (`MillifyDotnet` namespace)
- `src/millify.Tests` — xUnit tests

## License

MIT. See [LICENSE](LICENSE).
