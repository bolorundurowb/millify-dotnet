# millify-dotnet

[![Coverage Status](https://coveralls.io/repos/github/bolorundurowb/millify-dotnet/badge.svg)](https://coveralls.io/github/bolorundurowb/millify-dotnet) [![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE) [![NuGet Badge](https://buildstats.info/nuget/millify)](https://www.nuget.org/packages/millify)

A dotnet library to convert long numbers to pretty, human-readable strings.

This library is heavily inspired by the [millify](https://www.npmjs.com/package/millify) NodeJS library.

## Install

Get it on nuget

#### Package Manager

```
Install-Package millify
```

#### .NET CLI
```
> dotnet add package millify
```

#### PackageReference
```csharp
<PackageReference Include="millify" />
```

## Usage

Add the following import to the top of your csharp code file or your global `Usings.cs` file:

```csharp
using MillifyDotnet;
```

To shorten any number, call the `Shorten` method on the `Millify` class.

```csharp
var result = Millify.Shorten(2500);
// 2.5K

result = Millify.Shorten(1024000);
// 1.0M
```

The  `MillifyOptions` class offers configurable options for the output. Further details as well as defaults can be seen in the table below.

```csharp
var options = new MillifyOptions(precision: 3, lowercase: true);
var result = Millify.Shorten(1024000, options);
// 1.024m

options = new MillifyOptions(space: true, units: new[] {"B", "KB", "MB", "GB", "TB"});
result = Millify.Shorten(1440000, options);
// 1.44 MB
```

## Options

Name | Type | Default | Description
--- | --- | --- | ---
`Precision` | `number` | `1` | Number of decimal places to use
`Lowercase` | `boolean` | `false` | Use lowercase abbreviations
`SpaceBeforeUnit` | `boolean` | `false` | Add a space between number and abbreviation
`units` | `Array<string>` | `['', 'K', 'M', 'B', 'T', 'P', 'E']` | Optional custom unit abbreviations