# Benchmarks

For the purposes of approximate comparison between _MetadataExtractor_ and .NET Framework alternatives,
multiple scenarios are benchmarked. These focus on retrieving the "DateTimeOriginal" Exif value,
as that is a common use case for this library.

## Running

From the directory containing this README file:

    dotnet run -c Release

## Results

Results resemble (as of November 2016):

```plain
Host Process Environment Information:
BenchmarkDotNet.Core=v0.9.9.0
OS=Microsoft Windows NT 6.2.9200.0
Processor=Intel(R) Core(TM) i7-6700HQ CPU 2.60GHz, ProcessorCount=8
Frequency=2531248 ticks, Resolution=395.0620 ns, Timer=TSC
CLR=MS.NET 4.0.30319.42000, Arch=64-bit RELEASE [RyuJIT]
GC=Concurrent Workstation
JitModules=clrjit-v4.6.1586.0

Type=JpegBenchmark  Mode=Throughput

                                    Method |      Median |     StdDev | Scaled | Scaled-SD |
------------------------------------------ |------------ |----------- |------- |---------- |
 JpegMetadataReaderOnlyExifReaderBenchmark |  30.1141 us |  0.7083 us |   1.00 |      0.00 |
               JpegMetadataReaderBenchmark |  49.0014 us |  2.9799 us |   1.64 |      0.10 |
              ImageMetadataReaderBenchmark |  48.3592 us |  1.2577 us |   1.60 |      0.05 |
                    SystemDrawingBenchmark | 322.4140 us |  7.2245 us |  10.62 |      0.33 |
             WpfBitmapFrameCreateBenchmark | 678.8931 us | 13.3292 us |  22.36 |      0.65 |
             WpfJpegBitmapDecoderBenchmark | 934.4390 us | 18.2268 us |  30.75 |      0.89 |
```

The most indicative measure of throughput comes from the _Median_ column, where lower values are better.
Note that `us` indicates µs (microseconds, thousandths of milliseconds, or 10<sup>-6</sup> seconds).

For these benchmarks, image data is loaded into a `MemoryStream` which is reused between test runs.
In general, IO will be the major limiting factor, and that is excluded by pre-loading data, however
it does minimise external influences on the results. _MetadataExtractor_ is careful to avoid
unnecessary IO.

---

#### JpegMetadataReaderOnlyExifReaderBenchmark

If you know you have JPEG data, and you only want Exif data, this is a fast option:

    var directories = JpegMetadataReader.ReadMetadata(_stream, new[] { new ExifReader() });
    var subIfdDirectory = directories.OfType<ExifSubIfdDirectory>().First();
    var dateTime = subIfdDirectory.GetDateTime(ExifDirectoryBase.TagDateTimeOriginal);

Other kinds of metadata in the file will be ignored. Throws if the file does not actually contain JPEG data.

#### JpegMetadataReaderBenchmark

Retrieves all metadata from a JPEG file.

    var directories = JpegMetadataReader.ReadMetadata(_stream);
    var subIfdDirectory = directories.OfType<ExifSubIfdDirectory>().First();
    var dateTime = subIfdDirectory.GetDateTime(ExifDirectoryBase.TagDateTimeOriginal);

Throws if the file does not contain JPEG data.

#### ImageMetadataReaderBenchmark

Determines the file type automatically, then proceeds to return all found metadata.

    var directories = ImageMetadataReader.ReadMetadata(_stream);
    var subIfdDirectory = directories.OfType<ExifSubIfdDirectory>().First();
    var dateTime = subIfdDirectory.GetDateTime(ExifDirectoryBase.TagDateTimeOriginal);

This is the most general solution, and is recommended for most use cases. Throws if the file does not contain a supported file type.

#### SystemDrawingBenchmark

Using `System.Drawing` is ~1,000% slower.

    using (var myImage = Image.FromStream(_stream, useEmbeddedColorManagement: false, validateImageData: false))
    {
        const int PropertyTagExifDTOrig = ExifDirectoryBase.TagDateTimeOriginal;
        var propItem = myImage.GetPropertyItem(PropertyTagExifDTOrig);
        var dateTakenStr = Encoding.UTF8.GetString(propItem.Value);
        var dateTime = DateTime.Parse(_dateTimeRegex.Replace(dateTakenStr, "-", count: 2));
    }

#### WpfBitmapFrameCreateBenchmark

Using WPF's `BitmapFrame` is ~2,200% slower.

    var frame = BitmapFrame.Create(_stream, BitmapCreateOptions.DelayCreation, BitmapCacheOption.None);
    var metadata = (BitmapMetadata)frame.Metadata;
    var dateTakenStr = (string)metadata.GetQuery("/app1/ifd/exif/subifd:{uint=36867}");
    var dateTime DateTime.Parse(_dateTimeRegex.Replace(dateTakenStr, "-", count: 2));

#### WpfJpegBitmapDecoderBenchmark

Using WPF's `JpegBitmapDecoder` is ~3,000% slower.

    var decoder = new JpegBitmapDecoder(_stream, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.None);
    var metadata = (BitmapMetadata)decoder.Frames[0].Metadata;
    var dateTakenStr = (string)metadata.GetQuery("/app1/ifd/exif/subifd:{uint=36867}");
    var dateTime DateTime.Parse(_dateTimeRegex.Replace(dateTakenStr, "-", count: 2));

