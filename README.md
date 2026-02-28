# millify-dotnet 📊

[![Build, Test & Coverage](https://github.com/bolorundurowb/millify-dotnet/actions/workflows/build-and-test.yml/badge.svg)](https://github.com/bolorundurowb/millify-dotnet/actions/workflows/build-and-test.yml) [![codecov](https://codecov.io/gh/bolorundurowb/millify-dotnet/graph/badge.svg?token=8PIHY52HJS)](https://codecov.io/gh/bolorundurowb/millify-dotnet)  [![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)  ![NuGet Version](https://img.shields.io/nuget/v/millify)

---

## About Millify-Dotnet 📜

**Millify-Dotnet** is a lightweight and easy-to-use .NET library designed to convert long, unwieldy numbers into **pretty, human-readable strings**. Inspired by the popular [millify](https://www.npmjs.com/package/millify) NodeJS library, it simplifies the process of displaying large numbers in a clean and concise format. 🎯

Whether you're working with thousands, millions, or even trillions, **Millify-Dotnet** makes your numbers look neat and professional. Perfect for dashboards, reports, or any application where readability matters! ✨

---

## Installation 📦

You can install **Millify-Dotnet** via NuGet using one of the following methods:

#### **Package Manager**
```cmd
Install-Package millify
```

#### **.NET CLI**
```bash
dotnet add package millify
```

#### **PackageReference**
```xml
<PackageReference Include="millify" />
```

---

## Usage 🛠️

### Add the Namespace
Start by including the **MillifyDotnet** namespace in your code:
```csharp
using MillifyDotnet;
```

### Shorten Numbers
To convert a long number into a human-readable format, use the `Shorten` method:
```csharp
var result = Millify.Shorten(2500);
// Output: 2.5K

result = Millify.Shorten(1024000);
// Output: 1.0M
```

---

## Customize Output 🎨

**Millify-Dotnet** provides flexible options to tailor the output to your needs. Use the `MillifyOptions` class to configure the formatting:

### Example 1: Custom Precision and Lowercase
```csharp
var options = new MillifyOptions(precision: 3, lowercase: true);
var result = Millify.Shorten(1024000, options);
// Output: 1.024m
```

### Example 2: Add Space and Custom Units
```csharp
var options = new MillifyOptions(spaceBeforeUnit: true, units: new[] {"B", "KB", "MB", "GB", "TB"});
var result = Millify.Shorten(1440000, options);
// Output: 1.44 MB
```

---

## Options ⚙️

The `MillifyOptions` class allows you to customize the output with the following properties:

| Name              | Type            | Default                          | Description                                                                 |
|-------------------|-----------------|----------------------------------|-----------------------------------------------------------------------------|
| `Precision`       | `int`           | `1`                              | Number of decimal places to display.                                        |
| `Lowercase`       | `bool`          | `false`                          | Use lowercase abbreviations (e.g., `1.5k` instead of `1.5K`).               |
| `SpaceBeforeUnit` | `bool`          | `false`                          | Add a space between the number and the abbreviation (e.g., `1.5 K`).        |
| `Units`           | `string[]`      | `['', 'K', 'M', 'B', 'T', 'P', 'E']` | Custom unit abbreviations to use for formatting.                           |

---

## Why Use Millify-Dotnet? 🌟

- **Simple and Intuitive**: Convert numbers with just one method call.
- **Customizable**: Tailor the output to fit your needs with flexible options.
- **Lightweight**: Minimal overhead, maximum performance.
- **Human-Readable**: Make large numbers easy to understand at a glance.

---

## License 📜

**Millify-Dotnet** is licensed under the MIT License. See the [LICENSE](LICENSE) file for more details.

---

## Get Started Today! 🎉

Whether you're building a dashboard, generating reports, or just need to display large numbers in a clean format, **Millify-Dotnet** is here to help. Install the package, follow the examples, and start simplifying your numbers today! ⏱️

**Happy Coding!** 🚀