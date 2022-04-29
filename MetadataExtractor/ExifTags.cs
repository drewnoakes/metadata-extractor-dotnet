// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.Text;
using MetadataExtractor.Util;

namespace MetadataExtractor.NewApi
{
    public static class ExifTags
    {
        public static readonly TiffStringTag InteropIndex = new TiffStringTag(0x0001, "Interoperability Index", describer: static (value, _) =>
        {
            return string.Equals("R98", value.Trim(), StringComparison.OrdinalIgnoreCase)
                ? "Recommended Exif Interoperability Rules (ExifR98)"
                : value;
        });

        // TODO the element type here is undefined -- may need coercion
        public static readonly TiffUInt16ArrayTag InteropVersion = new(0x0002, "Interoperability Version", 4, static (values, _) => DescribeVersion(values, 2));

        // TODO consider http://www.awaresystems.be/imaging/tiff/tifftags/newsubfiletype.html
        public static readonly TiffIndexedUInt16Tag NewSubfileType = new(0x00FE, "New Subfile Type", 1, new[]
        {
            "Full-resolution image",
            "Reduced-resolution image",
            "Single page of multi-page reduced-resolution image",
            "Transparency mask",
            "Transparency mask of reduced-resolution image",
            "Transparency mask of multi-page image",
            "Transparency mask of reduced-resolution multi-page image"
        });

        public static readonly TiffIndexedUInt16Tag SubfileType = new(0x00FF, "Subfile Type", 1, new[]
        {
            "Full-resolution image",
            "Reduced-resolution image",
            "Single page of multi-page image"
        });

        public static readonly TiffUInt16Tag ImageWidth = new(0x0100, "Image Width", DescribePixels);

        public static readonly TiffUInt16Tag ImageHeight = new(0x0101, "Image Height", DescribePixels);

        /// <summary>
        /// For uncompressed image data, this value shows the number of bits per component for each pixel.
        /// </summary>
        /// <remarks>
        /// Usually this value is '8,8,8'.
        /// </remarks>
        public static readonly TiffUInt16ArrayTag BitsPerSample = new(0x0102, "Bits Per Sample", expectedCount: 3, describer: static (ints, _) => ints.Length == 3 ? $"{string.Join(",", ints)} bits/component/pixel" : null);

        public static readonly TiffMappedUInt16Tag Compression = new(0x0103, "Compression", new Dictionary<int, string>
        {
            { 1, "Uncompressed" },
            { 2, "CCITT 1D" },
            { 3, "T4/Group 3 Fax" },
            { 4, "T6/Group 4 Fax" },
            { 5, "LZW" },
            { 6, "JPEG (old-style)" },
            { 7, "JPEG" },
            { 8, "Adobe Deflate" },
            { 9, "JBIG B&W" },
            { 10, "JBIG Color" },
            { 99, "JPEG" },
            { 262, "Kodak 262" },
            { 32766, "Next" },
            { 32767, "Sony ARW Compressed" },
            { 32769, "Packed RAW" },
            { 32770, "Samsung SRW Compressed" },
            { 32771, "CCIRLEW" },
            { 32772, "Samsung SRW Compressed 2" },
            { 32773, "PackBits" },
            { 32809, "Thunderscan" },
            { 32867, "Kodak KDC Compressed" },
            { 32895, "IT8CTPAD" },
            { 32896, "IT8LW" },
            { 32897, "IT8MP" },
            { 32898, "IT8BL" },
            { 32908, "PixarFilm" },
            { 32909, "PixarLog" },
            { 32946, "Deflate" },
            { 32947, "DCS" },
            { 34661, "JBIG" },
            { 34676, "SGILog" },
            { 34677, "SGILog24" },
            { 34712, "JPEG 2000" },
            { 34713, "Nikon NEF Compressed" },
            { 34715, "JBIG2 TIFF FX" },
            { 34718, "Microsoft Document Imaging (MDI) Binary Level Codec" },
            { 34719, "Microsoft Document Imaging (MDI) Progressive Transform Codec" },
            { 34720, "Microsoft Document Imaging (MDI) Vector" },
            { 34892, "Lossy JPEG" },
            { 65000, "Kodak DCR Compressed" },
            { 65535, "Pentax PEF Compressed" }
        });

        /// <summary>Color space of the image data components.</summary>
        public static readonly TiffMappedUInt16Tag PhotometricInterpretation = new(0x0106, "Photometric Interpretation", new Dictionary<int, string>
        {
            { 0, "WhiteIsZero" },
            { 1, "BlackIsZero" },
            { 2, "RGB" },
            { 3, "RGB Palette" },
            { 4, "Transparency Mask" },
            { 5, "CMYK" },
            { 6, "YCbCr" },
            { 8, "CIELab" },
            { 9, "ICCLab" },
            { 10, "ITULab" },
            { 32803, "Color Filter Array" },
            { 32844, "Pixar LogL" },
            { 32845, "Pixar LogLuv" },
            { 32892, "Linear Raw" }
        });

        public static readonly TiffIndexedUInt16Tag Thresholding = new(0x0107, "Thresholding", 1, new[]
        {
            "No dithering or halftoning",
            "Ordered dither or halftone",
            "Randomized dither"
        });

        public static readonly TiffIndexedUInt16Tag FillOrder = new(0x010A, "Fill Order", 1, new[]
        {
            "Normal",
            "Reversed"
        });

        public static readonly TiffStringTag DocumentName = new(0x010D, "Document Name");

        public static readonly TiffStringTag ImageDescription = new(0x010E, "Description");

        public static readonly TiffStringTag Make = new(0x010F, "Make");

        public static readonly TiffStringTag Model = new(0x0110, "Model");

        /// <summary>The position in the file of raster data.</summary>
        public static readonly TiffUInt16Tag StripOffsets = new(0x0111, "Strip Offsets");

        public static readonly TiffIndexedUInt16Tag Orientation = new(0x0112, "Orientation", 1, new[]
        {
            "Top, left side (Horizontal / normal)",
            "Top, right side (Mirror horizontal)",
            "Bottom, right side (Rotate 180)", "Bottom, left side (Mirror vertical)",
            "Left side, top (Mirror horizontal and rotate 270 CW)",
            "Right side, top (Rotate 90 CW)",
            "Right side, bottom (Mirror horizontal and rotate 90 CW)",
            "Left side, bottom (Rotate 270 CW)"
        });

        /// <summary>Each pixel is composed of this many samples.</summary>
        public static readonly TiffUInt16Tag SamplesPerPixel = new(0x0115, "Samples Per Pixel", static (value, format) => string.Format(format, "{0} sample{1}/pixel", value, value == 1 ? "" : "s"));

        /// <summary>The raster is codified by a single block of data holding this many rows.</summary>
        public static readonly TiffUInt16Tag RowsPerStrip = new(0x0116, "Rows Per Strip", static (value, format) => string.Format(format, "{0} row{1}/strip", value, value == 1 ? "" : "s"));

        /// <summary>The size of the raster data in bytes.</summary>
        public static readonly TiffUInt16Tag StripByteCounts = new(0x0117, "Strip Byte Counts", static (value, format) => string.Format(format, "{0} byte{1}", value, value == 1 ? "" : "s"));

        public static readonly TiffUInt16Tag MinSampleValue = new(0x0118, "Minimum Sample Value");

        public static readonly TiffUInt16Tag MaxSampleValue = new(0x0119, "Maximum Sample Value");

        // TODO the descriptor function for these two tags attempts to read ResolutionUnit from the directory, which is not yet available via this new API -- would need to pass it a way to resolve questions about other values in the same directory, or potentially even in other directories
        public static readonly TiffUInt16Tag XResolution = new(0x011A, "X Resolution");
        public static readonly TiffUInt16Tag YResolution = new(0x011B, "Y Resolution");

        /// <summary>
        /// When image is uncompressed YCbCr, this value shows byte aligns of YCbCr data.
        /// </summary>
        public static readonly TiffIndexedUInt16Tag PlanarConfiguration = new(0x011C, "Planar Configuration", 1, new[]
        {
            "Chunky (contiguous for each subsampling pixel)",
            "Separate (Y-plane/Cb-plane/Cr-plane format)"
        });

        public static readonly TiffStringTag PageName = new(0x011D, "Page Name");

        public static readonly TiffIndexedUInt16Tag ResolutionUnit = new(0x0128, "Resolution Unit", 1, new[]
        {
            "(No unit)",
            "Inch",
            "cm"
        });

        public static readonly TiffUInt16Tag TransferFunction = new(0x012D, "Transfer Function");

        public static readonly TiffStringTag Software = new(0x0131, "Software");

        public static readonly TiffStringTag DateTime = new(0x0132, "DateTime");

        public static readonly TiffStringTag Artist = new(0x013B, "Artist");

        public static readonly TiffStringTag HostComputer = new(0x013C, "Host Computer");

        public static readonly TiffIndexedUInt16Tag Predictor = new(0x013D, "Predictor", 1, new []
        {
            "No prediction scheme used before coding",
            "Horizontal differencing",
            "Floating point horizontal differencing"
        });

        public static readonly TiffURationalArrayTag WhitePoint = new(0x013E, "White Point", expectedCount: 2);

        public static readonly TiffURationalArrayTag PrimaryChromaticities = new(0x013F, "PrimaryChromaticities", expectedCount: 6);

        public static readonly TiffUInt16Tag TileWidth = new(0x0142, "Tile Width");

        public static readonly TiffUInt16Tag TileLength = new(0x0143, "Tile Width");

        // TODO is this an array?
        public static readonly TiffUInt16Tag TileOffsets = new(0x0144, "Tile Offsets");

        // TODO is this an array?
        public static readonly TiffUInt16Tag TileByteCounts = new(0x0145, "Tile Byte Counts");

        // TODO should this be public?
        public static readonly TiffUInt16Tag SubIfdOffset = new(0x014a, "Sub IFD Offset");

        public static readonly TiffUInt16Tag TransferRange = new(0x0156, "Transfer Range");

        public static readonly TiffUInt16Tag JpegTables = new(0x015B, "JPEG Tables");

        public static readonly TiffMappedUInt16Tag JpegProc = new(0x0200, "JPEG Proc", new Dictionary<int, string>
        {
            { 1, "Baseline" },
            { 14, "Lossless" }
        });

        /// <summary>The matrix coefficients used for transformation from RGB to YCbCr image data.</summary>
        public static readonly TiffURationalArrayTag YCbCrCoefficients = new(0x0211, "YCbCr Coefficients", 3);

        public static readonly TiffUInt16ArrayTag YCbCrSubsampling = new(0x0212, "YCbCr Subsampling", 2, (ints, provider) =>
        {
            if (ints.Length == 2)
            {
                if (ints[0] == 2 && ints[1] == 1)
                    return "YCbCr4:2:2";
                if (ints[0] == 2 && ints[1] == 2)
                    return "YCbCr4:2:0";
            }
            return null;
        });

        public static readonly TiffIndexedUInt16Tag YCbCrPositioning = new(0x0213, "YCbCr Positioning", 1, new[]
        {
            "Center of pixel array",
            "Datum point"
        });

        public static readonly TiffURationalArrayTag ReferenceBlackWhite = new(0x0214, "Reference Black White", 6,
            (values, provider) => values.Length == 6
                ? string.Format(provider, "[{0},{1},{2}] [{3},{4},{5}]", values[0], values[1], values[2], values[3], values[4], values[5])
                : null);

        public static readonly TiffUInt16Tag RelatedImageFileFormat = new(0x1000, "Related Image File Format");

        public static readonly TiffUInt16Tag RelatedImageWidth = new(0x1001, "Related Image Width");

        public static readonly TiffUInt16Tag RelatedImageHeight = new(0x1002, "Related Image Height");

        public static readonly TiffUInt16Tag Rating = new(0x4746, "Rating");

        public static readonly TiffUInt16Tag CfaRepeatPatternDim = new(0x828D, "CFA Repeat Pattern Dim");

        /// <summary>There are two definitions for CFA pattern, I don't know the difference...</summary>
        // TODO interpreting this tag requires the value of CfaRepeatPatternDim too
        public static readonly TiffUInt16Tag CfaPattern2 = new(0x828E, "CFA Pattern 2");

        public static readonly TiffUInt16Tag BatteryLevel = new(0x828F, "Battery Level");

        public static readonly TiffStringTag Copyright = new(0x8298, "Copyright");

        /// <summary>Exposure time (reciprocal of shutter speed).</summary>
        /// <remarks>Unit is second.</remarks>
        public static readonly TiffURationalTag ExposureTime = new(0x829A, "Exposure Time", (value, format) => $"{value.ToSimpleString()} sec");

        /// <summary>The actual F-number(F-stop) of lens when the image was taken.</summary>
        public static readonly TiffURationalTag FNumber = new(0x829D, "F-Number",
            (value, format) => DescribeFStop(PhotographicConversions.ApertureToFStop(value.ToDouble()), format));

        // TODO what type is this?
        public static readonly TiffUInt16Tag IptcNaa = new(0x83BB, "Iptc Naa");

        // TODO what type is this?
        public static readonly TiffUInt16Tag InterColorProfile = new(0x8773, "Inter Color Profile");

        public static readonly TiffIndexedUInt16Tag ExposureProgram = new(0x8822, "Exposure Program", baseIndex: 1, new[]
        {
            "Manual control",
            "Program normal",
            "Aperture priority",
            "Shutter priority",
            "Program creative (slow program)",
            "Program action (high-speed program)",
            "Portrait mode",
            "Landscape mode"
        });

        // TODO what type is this?
        public static readonly TiffUInt16Tag SpectralSensitivity = new(0x8824, "Spectral Sensitivity");

        // TODO what type is this?
        public static readonly TiffUInt16Tag IsoEquivalent = new(0x8827, "ISO Equivalent");

        /// <summary>Indicates the Opto-Electric Conversion Function (OECF) specified in ISO 14524.</summary>
        /// <remarks>
        /// OECF is the relationship between the camera optical input and the image values.
        /// <para />
        /// The values are:
        /// <list type="bullet">
        /// <item>Two shorts, indicating respectively number of columns, and number of rows.</item>
        /// <item>For each column, the column name in a null-terminated ASCII string.</item>
        /// <item>For each cell, an SRATIONAL value.</item>
        /// </list>
        /// </remarks>
        public static readonly TiffUInt16Tag OptoElectricConversionFunction = new(0x8828, "Opto-Electric Conversion Function");

        public static readonly TiffUInt16Tag Interlace = new(0x8829, "Interlace");

        public static readonly TiffUInt16Tag TimeZoneOffsetTiffEp = new(0x882A, "Time Zone Offset Tiff EP");

        public static readonly TiffUInt16Tag SelfTimerModeTiffEp = new(0x882B, "Self Timer Mode Tiff Ep");

        /// <summary>Applies to ISO tag.</summary>
        public static readonly TiffIndexedUInt16Tag SensitivityType = new(0x8830, "Sensitivity Type", baseIndex: 0, new[]
        {
            "Unknown",
            "Standard Output Sensitivity",
            "Recommended Exposure Index",
            "ISO Speed",
            "Standard Output Sensitivity and Recommended Exposure Index",
            "Standard Output Sensitivity and ISO Speed",
            "Recommended Exposure Index and ISO Speed",
            "Standard Output Sensitivity, Recommended Exposure Index and ISO Speed"
        });

        public static readonly TiffUInt32Tag StandardOutputSensitivity = new(0x8831, "Standard Output Sensitivity");

        public static readonly TiffUInt32Tag RecommendedExposureIndex = new(0x8832, "Recommended Exposure Index");

        /// <summary>Non-standard, but in use.</summary>
        public static readonly TiffUInt16Tag TimeZoneOffset = new(0x882A, "Time Zone Offset");

        public static readonly TiffUInt16Tag SelfTimerMode = new(0x882B, "Self Timer Mode");

        // TODO need to convert this, and may need to coerce element values as they're of unspecified type in the spec
        public static readonly TiffUInt16ArrayTag ExifVersion = new(0x9000, "Exif Version", 4, (value, _) => DescribeVersion(value, majorDigits: 2));

        public static readonly TiffStringTag DateTimeOriginal = new(0x9003, "Date Time Original");

        public static readonly TiffStringTag DateTimeDigitized = new(0x9004, "Date Time Digitized");

        private static readonly string[] _componentsConfigurationElements = { string.Empty, "Y", "Cb", "Cr", "R", "G", "B" };

        // TODO this has undefined type in the spec
        public static readonly TiffUInt16ArrayTag ComponentsConfiguration = new(0x9101, "Components Configuration", 4, static (value, _) =>
        {
            var sb = new StringBuilder();
            for (var i = 0; i < Math.Min(4, value.Length); i++)
            {
                var j = value[i];
                if (j > 0 && j < _componentsConfigurationElements.Length)
                    sb.Append(_componentsConfigurationElements[j]);
            }
            return sb.ToString();
        });

        // TODO is this URational
        /// <summary>Average (rough estimate) compression level in JPEG bits per pixel.</summary>
        public static readonly TiffRationalTag CompressedAverageBitsPerPixel = new(0x9102, "Compressed Average Bits Per Pixel",
            (r, p) => $"{r.ToSimpleString(provider: p)} bit{(r.IsInteger && r.ToInt32() == 1 ? "" : "s")}/pixel");

        /// <summary>Shutter speed by APEX value.</summary>
        public static readonly TiffSingleTag ShutterSpeed = new(0x9201, "Shutter Speed", (apexValue, p) =>
        {
            // I believe this method to now be stable, but am leaving some alternative snippets of
            // code in here, to assist anyone who's looking into this (given that I don't have a public CVS).
            //        float apexValue = _directory.getFloat(ExifSubIFDDirectory.TAG_SHUTTER_SPEED);
            //        int apexPower = (int)Math.pow(2.0, apexValue);
            //        return "1/" + apexPower + " sec";
            // TODO test this method
            // thanks to Mark Edwards for spotting and patching a bug in the calculation of this
            // description (spotted bug using a Canon EOS 300D)
            // thanks also to Gli Blr for spotting this bug

            if (apexValue <= 1)
            {
                var apexPower = (float)(1/Math.Exp(apexValue*Math.Log(2)));
                var apexPower10 = (long)Math.Round(apexPower*10.0);
                var fApexPower = apexPower10/10.0f;
                return fApexPower + " sec";
            }
            else
            {
                var apexPower = (int)Math.Exp(apexValue*Math.Log(2));
                return "1/" + apexPower + " sec";
            }
        });

        /// <summary>Lens aperture used in an image.</summary>
        public static readonly TiffURationalTag Aperture = new(0x9202, "Aperture",
            (value, p) => DescribeFStop(PhotographicConversions.ApertureToFStop(value.ToDouble()), p));

        public static readonly TiffRationalTag BrightnessValue = new(0x9203, "Brightness");

        public static readonly TiffRationalTag ExposureBias = new(0x9204, "Exposure Bias", (value, format) => string.Format(format, "{0} EV", value.ToSimpleString()));

        /// <summary>Maximum aperture of lens.</summary>
        public static readonly TiffRationalTag MaxAperture = new(0x9205, "Max Aperture", (value, format) => DescribeFStop(PhotographicConversions.ApertureToFStop(value.ToDouble()), format));

        /// <summary>The distance autofocus focused to.</summary>
        /// <remarks>Tends to be less accurate as distance increases.</remarks>
        public static readonly TiffRationalTag SubjectDistance = new(0x9206, "Subject Distance", (value, format) => $"{value.ToDouble():0.0##} metres");

        /// <summary>Exposure metering method.</summary>
        public static readonly TiffMappedUInt16Tag MeteringMode = new(0x9207, "Metering Mode", new Dictionary<int, string>
        {
            { 0, "Unknown" },
            { 1, "Average" },
            { 2, "Center weighted average" },
            { 3, "Spot" },
            { 4, "Multi-spot" },
            { 5, "Multi-segment" },
            { 6, "Partial" },
            { 255, "(Other)" }
        });

        /// <summary>White balance (aka light source).</summary>
        public static readonly TiffMappedUInt16Tag WhiteBalance = new(0x9208, "White Balance", new Dictionary<int, string>
        {
            { 0, "Unknown" },
            { 1, "Daylight" },
            { 2, "Florescent" },
            { 3, "Tungsten" },
            { 4, "Flash" },
            { 9, "Fine Weather" },
            { 10, "Cloudy" },
            { 11, "Shade" },
            { 12, "Daylight Fluorescent" },
            { 13, "Day White Fluorescent" },
            { 14, "Cool White Fluorescent" },
            { 15, "White Fluorescent" },
            { 16, "Warm White Fluorescent" },
            { 17, "Standard light" },
            { 18, "Standard light (B)" },
            { 19, "Standard light (C)" },
            { 20, "D55" },
            { 21, "D65" },
            { 22, "D75" },
            { 23, "D50" },
            { 24, "Studio Tungsten" },
            { 255, "(Other)" }
        });

        public static readonly TiffUInt16Tag Flash = new(0x9209, "Flash", (value, p) =>
        {
            /*
             * This is a bit mask.
             * 0 = flash fired
             * 1 = return detected
             * 2 = return able to be detected
             * 3 = unknown
             * 4 = auto used
             * 5 = unknown
             * 6 = red eye reduction used
             */
            var sb = new StringBuilder();
            sb.Append((value & 0x1) != 0 ? "Flash fired" : "Flash did not fire");
            // check if we're able to detect a return, before we mention it
            if ((value & 0x4) != 0)
                sb.Append((value & 0x2) != 0 ? ", return detected" : ", return not detected");
            if ((value & 0x10) != 0)
                sb.Append(", auto");
            if ((value & 0x40) != 0)
                sb.Append(", red-eye reduction");
            return sb.ToString();
        });

        /// <summary>Focal length of lens used to take image.</summary>
        /// <remarks>
        /// Unit is millimeter.
        /// Nice digital cameras actually save the focal length as a function of how far they are zoomed in.
        /// </remarks>
        public static readonly TiffURationalTag FocalLength = new(0x920A, "Focal Length", (rational, provider) => DescribeFocalLength(rational.ToDouble(), provider));

        // TODO what type is this?
        public static readonly TiffUInt16Tag FlashEnergyTiffEp = new(0x920B, "TIFF/EP Flash Energy");

        // TODO what type is this?
        public static readonly TiffUInt16Tag SpatialFreqResponseTiffEp = new(0x920C, "TIFF/EP Spatial Freq Response");

        // TODO what type is this?
        public static readonly TiffUInt16Tag Noise = new(0x920D, "Noise");

        // TODO what type is this?
        public static readonly TiffUInt16Tag FocalPlaneXResolutionTiffEp = new(0x920E, "TIFF/EP Focal Plane X Resolution");

        // TODO what type is this?
        public static readonly TiffUInt16Tag FocalPlaneYResolutionTiffEp = new(0x920F, "TIFF/EP Focal Plane Y Resolution");

        // TODO what type is this?
        public static readonly TiffUInt16Tag ImageNumber = new(0x9211, "Image Number");

        // TODO what type is this?
        public static readonly TiffUInt16Tag SecurityClassification = new(0x9212, "Security Classification");

        // TODO what type is this?
        public static readonly TiffUInt16Tag ImageHistory = new(0x9213, "Image History");

        // TODO what type is this?
        public static readonly TiffUInt16Tag SubjectLocationTiffEp = new(0x9214, "TIFF/EP Subject Location");

        // TODO what type is this?
        public static readonly TiffUInt16Tag ExposureIndexTiffEp = new(0x9215, "TIFF/EP Exposure Index");

        // TODO what type is this?
        public static readonly TiffUInt16Tag StandardIdTiffEp = new(0x9216, "TIFF/EP Standard ID");

        /// <summary>This tag holds the Exif Makernote.</summary>
        /// <remarks>
        /// Makernotes are free to be in any format, though they are often IFDs.
        /// To determine the format, we consider the starting bytes of the makernote itself and sometimes the
        /// camera model and make.
        /// <para />
        /// The component count for this tag includes all of the bytes needed for the makernote.
        /// </remarks>
        // TODO should this be public? probably shouldn't be storing the byte array for makernotes, unless it's unknown (in which case it's in a makernote directory)
        public static readonly TiffByteArrayTag Makernote = new TiffByteArrayTag(0x927C, "Makernote");

        public static readonly TiffStringTag UserComment = new(0x9286, "User Comment", DescribeUserComment);

        public static readonly TiffUInt16Tag SubsecondTime = new(0x9290, "Subsecond Time");

        public static readonly TiffUInt16Tag SubsecondTimeOriginal = new(0x9291, "Subsecond Time Original");

        public static readonly TiffUInt16Tag SubsecondTimeDigitized = new(0x9292, "Subsecond Time Digitized");

        /// <summary>The image title, as used by Windows XP.</summary>
        public static readonly TiffStringTag WinTitle = new(0x9C9B, "Windows XP Title", Encoding.Unicode);

        /// <summary>The image comment, as used by Windows XP.</summary>
        public static readonly TiffStringTag WinComment = new(0x9C9C, "Windows XP Comment", Encoding.Unicode);

        /// <summary>The image author, as used by Windows XP (called Artist in the Windows shell).</summary>
        public static readonly TiffStringTag WinAuthor = new(0x9C9D, "Windows XP Author", Encoding.Unicode);

        /// <summary>The image keywords, as used by Windows XP.</summary>
        public static readonly TiffStringTag WinKeywords = new(0x9C9E, "Windows XP Keywords", Encoding.Unicode);

        /// <summary>The image subject, as used by Windows XP.</summary>
        public static readonly TiffStringTag WinSubject = new(0x9C9F, "Windows XP Subject", Encoding.Unicode);

        public static readonly TiffUInt16ArrayTag FlashpixVersion = new(0xA000, "Flashpix Version", expectedCount: 4, describer: (values, p) => DescribeVersion(values, 2));

        /// <summary>Defines Color Space.</summary>
        /// <remarks>
        /// DCF image must use sRGB color space so value is
        /// always '1'. If the picture uses the other color space, value is
        /// '65535':Uncalibrated.
        /// </remarks>
        public static readonly TiffMappedUInt16Tag ColorSpace = new(0xA001, "Color Space", new Dictionary<int, string>
        {
            { 1, "sRGB" },
            { 65535, "Undefined" }
        });

        public static readonly TiffUInt16Tag ExifImageWidth = new(0xA002, "Exif Image Width", DescribePixels);

        public static readonly TiffUInt16Tag ExifImageHeight = new(0xA003, "Exif Image Height", DescribePixels);

        public static readonly TiffUInt16Tag RelatedSoundFile = new(0xA004, "Related Sound File");

        public static readonly TiffURationalTag FlashEnergy = new(0xA20B, "Flash Energy");

        public static readonly TiffUInt16Tag SpatialFreqResponse = new(0xA20C, "Spatial Freq Response");

        // TODO the descriptor function for these two tags attempts to read units from the directory, which is not yet available via this new API
        public static readonly TiffURationalTag FocalPlaneXResolution = new(0xA20E, "Focal Plane X Resolution");
        public static readonly TiffURationalTag FocalPlaneYResolution = new(0xA20F, "Focal Plane Y Resolution");

        /// <summary>Unit of <see cref="FocalPlaneXResolution"/> and <see cref="FocalPlaneYResolution"/>.</summary>
        /// <remarks>
        /// '1' means no-unit, '2' inch, '3' centimeter.
        /// Note: Some of Fujifilm's digicam(e.g.FX2700,FX2900,Finepix4700Z/40i etc)
        /// uses value '3' so it must be 'centimeter', but it seems that they use a
        /// '8.3mm?'(1/3in.?) to their ResolutionUnit. Fuji's BUG? Finepix4900Z has
        /// been changed to use value '2' but it doesn't match to actual value also.
        /// </remarks>
        public static readonly TiffIndexedUInt16Tag FocalPlaneResolutionUnit = new(0xA210, "Focal Plane Resolution Unit", 1, new []
        {
            "(No unit)",
            "Inches",
            "cm"
        });

        public static readonly TiffUInt16ArrayTag SubjectLocation = new(0xA214, "Subject Location", 2);

        public static readonly TiffURationalTag ExposureIndex = new(0xA215, "Exposure Index");

        public static readonly TiffIndexedUInt16Tag SensingMethod = new(0xA217, "Sensing Method", 1, new[]
        {
            "(Not defined)",
            "One-chip color area sensor",
            "Two-chip color area sensor",
            "Three-chip color area sensor",
            "Color sequential area sensor",
            null,
            "Trilinear sensor",
            "Color sequential linear sensor"
        });

        public static readonly TiffIndexedUInt16Tag FileSource = new(0xA300, "File Source", 1, new[]
        {
            "Film Scanner",
            "Reflection Print Scanner",
            "Digital Still Camera (DSC)"
        });

        public static readonly TiffIndexedUInt16Tag SceneType = new(0xA301, "Scene Type", 1, new [] { "Directly photographed image" });

        // TODO this is undefined, with any number of components
        public static readonly TiffUInt16Tag CfaPattern = new(0xA302, "CFA Pattern");

        /// <summary>
        /// This tag indicates the use of special processing on image data, such as rendering
        /// geared to output.
        /// </summary>
        /// <remarks>
        /// When special processing is performed, the reader is expected to
        /// disable or minimize any further processing.
        /// </remarks>
        public static readonly TiffIndexedUInt16Tag CustomRendered = new(0xA401, "Custom Rendered", 0, new []
        {
            "Normal process",
            "Custom process"
        });

        /// <remarks>
        /// In auto-bracketing mode, the camera shoots a series of frames of the
        /// same scene at different exposure settings.
        /// </remarks>
        public static readonly TiffIndexedUInt16Tag ExposureMode = new(0xA402, "Exposure Mode", 0, new []
        {
            "Auto exposure",
            "Manual exposure",
            "Auto bracket"
        });

        public static readonly TiffIndexedUInt16Tag WhiteBalanceMode = new(0xA403, "White Balance Mode", 0, new []
        {
            "Auto white balance",
            "Manual white balance"
        });

        /// <remarks>The digital zoom ratio, or zero if digital zoom was not used.</remarks>
        public static readonly TiffURationalTag DigitalZoomRatio = new(0xA404, "Digital Zoom Ratio",
            (ratio, format) => ratio.Numerator == 0 ? "Digital zoom not used" : ratio.ToSimpleString(provider: format));

        /// <summary>
        /// The equivalent focal length assuming a 35mm film camera, in millimetres.
        /// A value of zero means the focal length is unknown.
        /// </summary>
        /// <remarks>Note that this tag differs from <see cref="FocalLength"/>.</remarks>
        public static readonly TiffUInt16Tag Equivalent35MMFocalLength = new(0xA405, "35mm Equivalent Focal Length",
            (value, format) => value == 0 ? "Unknown" : DescribeFocalLength(value, format));

        /// <summary>The type of scene that was shot, or the mode in which the image was shot.</summary>
        /// <remarks>Note that this differs from <see cref="SceneType"/>.</remarks>
        public static readonly TiffIndexedUInt16Tag SceneCaptureType = new(0xA406, "Scene Capture Type", 0, new []
        {
            "Standard",
            "Landscape",
            "Portrait",
            "Night scene"
        });

        /// <summary>The degree of overall image gain adjustment.</summary>
        public static readonly TiffIndexedUInt16Tag GainControl = new(0xA407, "Gain Control", 0, new []
        {
            "None",
            "Low gain up",
            "Low gain down",
            "High gain up",
            "High gain down"
        });

        /// <summary>The direction of contrast processing applied by the camera when the image was shot.</summary>
        public static readonly TiffIndexedUInt16Tag Contrast = new(0xA408, "Contrast", 0, new []
        {
            "None",
            "Soft",
            "Hard"
        });

        /// <summary>The direction of saturation processing applied by the camera when the image was shot.</summary>
        public static readonly TiffIndexedUInt16Tag Saturation = new(0xA409, "Saturation", 0, new []
        {
            "None",
            "Low saturation",
            "High saturation"
        });

        /// <summary>The direction of sharpness processing applied by the camera when the image was shot.</summary>
        public static readonly TiffIndexedUInt16Tag Sharpness = new(0xA40A, "Sharpness", 0, new []
        {
            "None",
            "Low",
            "Hard"
        });

        /// <summary>
        /// This tag indicates information on the picture-taking conditions of a particular
        /// camera model.
        /// </summary>
        /// <remarks>
        /// The tag is used only to indicate the picture-taking conditions in the reader.
        /// Tag = 41995 (A40B.H)
        /// Type = UNDEFINED
        /// Count = Any
        /// Default = none
        /// The information is recorded in the format shown below. The data is recorded
        /// in Unicode using SHORT type for the number of display rows and columns and
        /// UNDEFINED type for the camera settings. The Unicode (UCS-2) string including
        /// Signature is NULL terminated. The specifics of the Unicode string are as given
        /// in ISO/IEC 10464-1.
        /// Length  Type        Meaning
        /// ------+-----------+------------------
        /// 2       SHORT       Display columns
        /// 2       SHORT       Display rows
        /// Any     UNDEFINED   Camera setting-1
        /// Any     UNDEFINED   Camera setting-2
        /// :       :           :
        /// Any     UNDEFINED   Camera setting-n
        /// </remarks>
        public static readonly TiffUInt16Tag DeviceSettingDescription = new(0xA40B, "Device Setting Description");

        public static readonly TiffIndexedUInt16Tag SubjectDistanceRange = new(0xA40C, "Subject Distance Range", 0, new []
        {
            "Unknown",
            "Macro",
            "Close view",
            "Distant view"
        });

        /// <summary>An identifier assigned uniquely to each image.</summary>
        /// <remarks>
        /// It is recorded as an ASCII string equivalent to hexadecimal notation and 128-bit
        /// fixed length.
        /// </remarks>
        public static readonly TiffStringTag ImageUniqueId = new(0xA420, "Image Unique ID");

        public static readonly TiffStringTag CameraOwnerName = new(0xA430, "Camera Owner Name");

        public static readonly TiffStringTag BodySerialNumber = new(0xA431, "Body Serial Number");

        /// <summary>Describes aperture and focal ranges of the lens.</summary>
        /// <remarks>
        /// Represented as an array of four URational values holding:
        /// <list type="bullet">
        ///     <item>Minimum focal length (in mm)</item>
        ///     <item>Maximum focal length (in mm)</item>
        ///     <item>Minimum f-number in the minimum focal length</item>
        ///     <item>Minimum f-number in the maximum focal length</item>
        /// </list>
        /// </remarks>
        public static readonly TiffURationalArrayTag LensSpecification = new(0xA432, "Lens Specification", 4, (values, format) =>
        {
            if (values.Length != 4 || (values[0].IsZero && values[2].IsZero))
                return null;

            var sb = new StringBuilder();

            sb.AppendFormat(
                format,
                values[0] == values[1] ? "{0}mm" : "{0}-{1}mm",
                values[0].ToSimpleString(provider: format),
                values[1].ToSimpleString(provider: format));

            if (!values[2].IsZero)
            {
                sb.Append(' ');
                sb.AppendFormat(
                    format,
                    values[2] == values[3] ? "f/{0:0.0}" : "f/{0:0.0}-{1:0.0}",
                    values[2].ToDouble(),
                    values[3].ToDouble());
            }

            return sb.ToString();
        });

        public static readonly TiffStringTag LensMake = new(0xA433, "Lens Make");

        public static readonly TiffStringTag LensModel = new(0xA434, "Lens Model");

        public static readonly TiffStringTag LensSerialNumber = new(0xA435, "Lens Serial Number");

        public static readonly TiffURationalTag Gamma = new(0xA500, "Gamma");

        // TODO what type is this?
        public static readonly TiffUInt16Tag PrintIm = new(0xC4A5, "Print IM");

        // TODO what type is this?
        public static readonly TiffUInt16Tag PanasonicTitle = new(0xC6D2, "Panasonic Title");

        // TODO what type is this?
        public static readonly TiffUInt16Tag PanasonicTitle2 = new(0xC6D3, "Panasonic Title 2");

        // TODO what type is this?
        public static readonly TiffUInt16Tag Padding = new(0xEA1C, "Padding");

        // TODO what type is this?
        public static readonly TiffUInt16Tag Lens = new(0xFDEA, "Lens");

        private static string DescribeFStop(double fStop, IFormatProvider? format) => string.Format(format, "f/{0:0.0}", fStop);

        private static string DescribeFocalLength(double mm, IFormatProvider? format) => string.Format(format, "{0:0.#} mm", mm);

        private static string DescribePixels(int i, IFormatProvider? format) => string.Format(format, "{0} pixel{1}", i, i == 1 ? "" : "s");

        private static string? DescribeVersion(IReadOnlyList<int>? components, int majorDigits)
        {
            if (components == null)
                return null;

            var version = new StringBuilder();
            for (var i = 0; i < 4 && i < components.Count; i++)
            {
                if (i == majorDigits)
                    version.Append('.');
                var c = (char)components[i];
                if (c < '0')
                    c += '0';
                if (i == 0 && c == '0')
                    continue;
                version.Append(c);
            }
            return version.ToString();
        }

        private static string? DescribeUserComment(byte[] bytes)
        {
            if (bytes.Length == 0)
                return string.Empty;

            // TODO use ByteTrie here
            // Someone suggested "ISO-8859-1".
            var encodingMap = new Dictionary<string, Encoding>
            {
                ["ASCII"] = Encoding.ASCII,
                ["UTF8"] = Encoding.UTF8,
                ["UTF7"] = Encoding.UTF7,
                ["UTF32"] = Encoding.UTF32,
                ["UNICODE"] = Encoding.Unicode,
                ["JIS"] = Encoding.GetEncoding("Shift-JIS")
            };

            try
            {
                if (bytes.Length >= 10)
                {
                    // TODO no guarantee bytes after the UTF8 name are valid UTF8 -- only read as many as needed
                    var firstTenBytesString = Encoding.UTF8.GetString(bytes, 0, 10);
                    // try each encoding name
                    foreach (var pair in encodingMap)
                    {
                        var encodingName = pair.Key;
                        var encoding = pair.Value;
                        if (firstTenBytesString.StartsWith(encodingName))
                        {
                            // skip any null or blank characters commonly present after the encoding name, up to a limit of 10 from the start
                            for (var j = encodingName.Length; j < 10; j++)
                            {
                                var b = bytes[j];
                                if (b != '\0' && b != ' ')
                                {
                                    return encoding.GetString(bytes, j, bytes.Length - j).Trim('\0', ' ');
                                }
                            }
                            return encoding.GetString(bytes, 10, bytes.Length - 10).Trim('\0', ' ');
                        }
                    }
                }
                // special handling fell through, return a plain string representation
                return Encoding.UTF8.GetString(bytes, 0, bytes.Length).Trim('\0', ' ');
            }
            catch
            {
                return null;
            }
        }
    }
}