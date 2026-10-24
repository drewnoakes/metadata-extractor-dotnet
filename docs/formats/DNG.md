# Adobe Digital Negative (DNG)

DNG uses TIFF image file directories (IFDs) with Adobe-defined tags.

## .NET parsing behavior

The reader recognizes tags through DNG 1.4. Each image IFD containing recognized
DNG tags has a separate `DngDirectory`. Its `Parent` identifies the original TIFF
directory; the original directories and raw tag values remain available unchanged.

DNG interpretation requires a four-byte `DNGVersion` with a nonzero major version in
the first TIFF IFD, rather than a `.dng` filename. This also works with stream input.
DNG names are not added to ordinary Exif directories, and Exif, GPS, interoperability,
and maker-note IFDs are not interpreted as DNG image IFDs. Embedded Exif read through
`ExifReader` continues to use Exif interpretation only.

## Implementation mapping

- .NET: `MetadataExtractor/Formats/Tiff/TiffMetadataReader.cs` uses the handler in
  `MetadataExtractor/Formats/Dng/` for DNG interpretation.

This describes the .NET implementation, not verified cross-language parity.

## References

- [Adobe DNG specification](https://helpx.adobe.com/camera-raw/digital-negative.html)
