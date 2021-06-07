// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using MetadataExtractor.Formats.Flir;
using MetadataExtractor.Formats.Jpeg;
using MetadataExtractor.Util;

namespace MetadataExtractor.Samples
{
    public static class FlirSamples
    {
        public static void Main()
        {
            var inputFile = "my-input.jpg";   // path to a FLIR JPEG file
            var outputFile = "my-output.png"; // path to the output thermal image

            if (TryGetThermalImageBytesFromJpeg(inputFile, out byte[]? bytes, out int width, out int height))
            {
                WritePng(bytes, width, height, outputFile);
            }
        }

        public static bool TryGetThermalImageBytesFromJpeg(
            string jpegFile,
            [NotNullWhen(returnValue: true)] out byte[]? imageBytes,
            out int width,
            out int height)
        {
            var readers = JpegMetadataReader
                .AllReaders
                .Where(reader => reader is not FlirReader)
                .Concat(new[] { new FlirReader { ExtractRawThermalImage = true } })
                .ToList();

            var directories = JpegMetadataReader.ReadMetadata(jpegFile, readers);

            var flirRawDataDirectory = directories.OfType<FlirRawDataDirectory>().FirstOrDefault();

            if (flirRawDataDirectory is null)
            {
                imageBytes = null;
                width = 0;
                height = 0;
                return false;
            }

            width = flirRawDataDirectory.GetInt32(FlirRawDataDirectory.TagRawThermalImageWidth);
            height = flirRawDataDirectory.GetInt32(FlirRawDataDirectory.TagRawThermalImageHeight);
            imageBytes = flirRawDataDirectory.GetByteArray(FlirRawDataDirectory.TagRawThermalImage);

            return imageBytes != null;
        }

        public static void WritePng(byte[] thermalImageBytes, int width, int height, string outputFile)
        {
            var fileType = FileTypeDetector.DetectFileType(new MemoryStream(thermalImageBytes));

            if (fileType == FileType.Png)
            {
                // Data is already in PNG format.
                // It is likely already coloured and ready for presentation to a human.

                File.WriteAllBytes(outputFile, thermalImageBytes);
                return;
            }

            // Assume data is in uint16 grayscale.
            //
            // It is "raw" meaning uncoloured but, more importantly, the levels are unadjusted.
            // For example, if the scene did not have much temperature variation relative to the
            // range of the sensor, most values may be within a very narrow band of the 16-bit spectrum,
            // making the image appear a flat gray. Opening it in Photoshop/Gimp/etc and adjusting levels
            // will reveal the image.
            //
            // There is other metadata in the image that may inform this process (untested -- please
            // share if you find a good process here).

            var pixelFormats = PixelFormats.Gray16;

            var bitmap = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format16bppGrayScale);

            var data = bitmap.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, bitmap.PixelFormat);

            Marshal.Copy(thermalImageBytes, 0, data.Scan0, thermalImageBytes.Length);

            var source = BitmapSource.Create(
                width,
                height,
                bitmap.HorizontalResolution,
                bitmap.VerticalResolution,
                pixelFormats,
                null,
                data.Scan0,
                data.Stride * height,
                data.Stride);

            bitmap.UnlockBits(data);

            using var stream = new FileStream(outputFile, FileMode.Create);

            var encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(source));
            encoder.Save(stream);
        }
    }
}
