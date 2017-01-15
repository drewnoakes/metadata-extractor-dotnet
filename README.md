![metadata-extractor logo](https://cdn.rawgit.com/drewnoakes/metadata-extractor/master/Resources/metadata-extractor-logo.svg)

[![Build status](https://ci.appveyor.com/api/projects/status/90hfuleg8wj8r956?svg=true)](https://ci.appveyor.com/project/drewnoakes/metadata-extractor-dotnet)
[![MetadataExtractor NuGet version](https://img.shields.io/nuget/v/MetadataExtractor.svg)](https://www.nuget.org/packages/MetadataExtractor/)
[![MetadataExtractor NuGet pre-release version](https://img.shields.io/nuget/vpre/MetadataExtractor.svg)](https://www.nuget.org/packages/MetadataExtractor/)

_MetadataExtractor_ is a straightforward .NET library for reading metadata from image and movie files.

## Installation

The easiest way to use this library is via its [NuGet package](https://www.nuget.org/packages/MetadataExtractor/):

    PM> Install-Package MetadataExtractor

Alternatively, search for `MetadataExtractor` in the Visual Studio NuGet Package Manager.

## Usage

```csharp
IEnumerable<Directory> directories = ImageMetadataReader.ReadMetadata(imagePath);
```

The resulting `directories` sequence holds potentially many different directories of metadata, depending upon the input image.

To print out all values from all directories:

```csharp
foreach (var directory in directories)
foreach (var tag in directory.Tags)
    Console.WriteLine($"{directory.Name} - {tag.Name} = {tag.Description}");
```

Producing:

```text
Exif SubIFD - Exposure Time = 1/60 sec
Exif SubIFD - F-Number = f/8.0
...
Exif IFD0 - Make = NIKON CORPORATION
Exif IFD0 - Model = NIKON D70
...
IPTC - Credit = Drew Noakes
IPTC - City = London
...
```

Access a specific value, in this case the Exif DateTime tag:

```csharp
var subIfdDirectory = directories.OfType<ExifSubIfdDirectory>().FirstOrDefault();
var dateTime = subIfdDirectory?.GetDescription(ExifDirectoryBase.TagDateTime);
```

## Features

The library understands several formats of metadata, many of which may be present in a single image:

* [Exif](http://en.wikipedia.org/wiki/Exchangeable_image_file_format)
* [IPTC](http://en.wikipedia.org/wiki/IPTC)
* [XMP](http://en.wikipedia.org/wiki/Extensible_Metadata_Platform)
* [JFIF / JFXX](http://en.wikipedia.org/wiki/JPEG_File_Interchange_Format)
* [ICC Profiles](http://en.wikipedia.org/wiki/ICC_profile)
* [Photoshop](http://en.wikipedia.org/wiki/Photoshop) fields
* [WebP](http://en.wikipedia.org/wiki/WebP) properties
* [Netpbm](https://en.wikipedia.org/wiki/Netpbm_format) properties
* [PNG](http://en.wikipedia.org/wiki/Portable_Network_Graphics) properties
* [BMP](http://en.wikipedia.org/wiki/BMP_file_format) properties
* [GIF](http://en.wikipedia.org/wiki/Graphics_Interchange_Format) properties
* [ICO](https://en.wikipedia.org/wiki/ICO_(file_format)) properties
* [PCX](http://en.wikipedia.org/wiki/PCX) properties

It will process files of type:

* JPEG
* TIFF
* WebP
* PSD
* PNG
* BMP
* GIF
* ICO
* PCX
* Netpbm
* Camera Raw
  * NEF (Nikon)
  * CR2 (Canon)
  * ORF (Olympus)
  * ARW (Sony)
  * RW2 (Panasonic)
  * RWL (Leica)
  * SRW (Samsung)

Camera-specific "makernote" data is decoded for cameras manufactured by:

* Agfa
* Apple
* Canon
* Casio
* Epson
* Fujifilm
* Kodak
* Kyocera
* Leica
* Minolta
* Nikon
* Olympus
* Panasonic
* Pentax
* Reconyx
* Sanyo
* Sigma/Foveon
* Sony

## Supported Frameworks

This library targets:

- .NET Framework 3.5 (`net35`)
- .NET Framework 4.5 (`net45`)
- .NET Standard 1.3 (`netstandard1.3`)

All target frameworks are provided via the [one NuGet package](https://www.nuget.org/packages/MetadataExtractor).

`net35` and `net45` target the full .NET Framework. `net45` uses the newer `IReadOnlyList<>` on some public APIs where `net35` uses `IList<>`. Internally `net45` also uses some newer library features for slightly improved performance.

`netstandard1.3` implements version 1.3 of the [.NET Standard](https://docs.microsoft.com/en-us/dotnet/articles/standard/library) which covers .NET Core, Mono, Xamarin platforms, UWP, and future platforms.

A PCL build was supported until [version 1.5.3](https://www.nuget.org/packages/MetadataExtractor/1.5.3) which supported Silverlight 5.0, Windows 8.0, Windows Phone 8.1 and Windows Phone Silverlight 8.0. PCL versions did not support file-system metadata due to restricted IO APIs.

## Questions & Feedback

The quickest way to have your questions answered is via [Stack Overflow](http://stackoverflow.com/questions/tagged/metadata-extractor).
Check whether your question has already been asked, and if not, ask a new one tagged with both `metadata-extractor` and `.net`.

Bugs and feature requests should be provided via the project's [issue tracker](https://github.com/drewnoakes/metadata-extractor-dotnet/issues).
Please attach sample images where possible as most issues cannot be investigated without an image.

## Contributing

If you want to get your hands dirty, making a pull request is a great way to enhance the library.
In general it's best to create an issue first that captures the problem you want to address.
You can discuss your proposed solution in that issue.
This gives others a chance to provide feedback before you spend your valuable time working on it.

An easier way to help is to contribute to the [sample image file library](https://github.com/drewnoakes/metadata-extractor-images/wiki) used for research and testing.

## Credits

This library is developed by [Drew Noakes](https://drewnoakes.com/code/exif/) and contributors.

Thanks are due to the many [users](https://github.com/drewnoakes/metadata-extractor/wiki/UsedBy) who sent in suggestions, bug reports,
[sample images](https://github.com/drewnoakes/metadata-extractor-images/wiki) from their cameras as well as encouragement.
Wherever possible, they have been credited in the source code and commit logs.

This library was [originally written in Java](https://github.com/drewnoakes/metadata-extractor/) in 2002.
In 2014, Yakov Danilov (for Imazen LLC) converted the code to C# using Sharpen.
The code has subsequently been edited to provide a more idiomatic .NET API.
Both projects are now developed in unison and aim to be functionally equivalent.

## Other languages

- Java  [metadata-extractor](https://github.com/drewnoakes/metadata-extractor) is the original implementation of this project, from which this .NET version was ported
- PHP [php-metadata-extractor](https://github.com/gomoob/php-metadata-extractor) wraps the Java project, making it available to users of PHP

---

More information about this project is available at:

* https://drewnoakes.com/code/exif/
* https://github.com/drewnoakes/metadata-extractor-dotnet/
