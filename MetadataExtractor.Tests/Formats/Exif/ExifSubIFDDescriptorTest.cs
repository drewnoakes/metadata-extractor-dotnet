// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace MetadataExtractor.Formats.Exif;

/// <summary>Unit tests for <see cref="ExifSubIfdDescriptor"/>.</summary>
/// <author>Drew Noakes https://drewnoakes.com</author>
public sealed class ExifSubIfdDescriptorTest
{
    [Theory]
    [InlineData(1, 25, "1/25 sec")]
    [InlineData(4, 100, "1/25 sec")]
    [InlineData(1, 2, "1/2 sec")]
    [InlineData(1, 125, "1/125 sec")]
    [InlineData(3, 10, "3/10 sec")]
    public void ExposureTimeDescription_Subsecond(int numerator, int denominator, string expected)
    {
        var directory = new ExifSubIfdDirectory();
        directory.Set(ExifDirectoryBase.TagExposureTime, new Rational(numerator, denominator));

        Assert.Equal(expected, directory.GetDescription(ExifDirectoryBase.TagExposureTime));
    }

    [Theory, UseCulture("en-GB")]
    [InlineData(1, 1, "1 sec")]
    [InlineData(2, 1, "2 sec")]
    [InlineData(3, 2, "1.5 sec")]
    public void ExposureTimeDescription_OneSecondOrLonger(int numerator, int denominator, string expected)
    {
        var directory = new ExifSubIfdDirectory();
        directory.Set(ExifDirectoryBase.TagExposureTime, new Rational(numerator, denominator));

        Assert.Equal(expected, directory.GetDescription(ExifDirectoryBase.TagExposureTime));
    }

    [Fact]
    public void ExposureTimeDescription_Missing()
    {
        var directory = new ExifSubIfdDirectory();

        Assert.Null(directory.GetDescription(ExifDirectoryBase.TagExposureTime));
    }

    [Fact, UseCulture("en-GB")]
    public void ExposureTimeRawString()
    {
        var directory = new ExifSubIfdDirectory();
        directory.Set(ExifDirectoryBase.TagExposureTime, new Rational(1, 25));

        Assert.Equal("0.04", directory.GetString(ExifDirectoryBase.TagExposureTime));
    }

    [Fact, UseCulture("en-GB")]
    public void LensSpecificationDescription_DecimalFocalLength()
    {
        var directory = new ExifSubIfdDirectory();
        directory.Set(ExifDirectoryBase.TagLensSpecification, new[]
        {
            new Rational(17, 4), new Rational(17, 4), new Rational(9, 5), new Rational(9, 5)
        });

        Assert.Equal("4.25mm f/1.8", directory.GetDescription(ExifDirectoryBase.TagLensSpecification));
    }

    [Fact]
    public void UserCommentDescription_EmptyEncoding()
    {
        var commentBytes = "\x0\x0\x0\x0\x0\x0\x0\x0This is a comment"u8.ToArray();
        var directory = new ExifSubIfdDirectory();
        directory.Set(ExifDirectoryBase.TagUserComment, commentBytes);
        var descriptor = new ExifSubIfdDescriptor(directory);
        Assert.Equal("This is a comment", descriptor.GetDescription(ExifDirectoryBase.TagUserComment));
    }

    [Fact]
    public void UserCommentDescription_BlankAscii()
    {
        var commentBytes = "ASCII\x0\x0\x0          "u8.ToArray();
        var directory = new ExifSubIfdDirectory();
        directory.Set(ExifDirectoryBase.TagUserComment, commentBytes);
        var descriptor = new ExifSubIfdDescriptor(directory);
        Assert.Equal("", descriptor.GetDescription(ExifDirectoryBase.TagUserComment));
    }

    [Fact]
    public void UserCommentDescription_ZeroLengthAscii1()
    {
        // the 10-byte encoding region is only partially full
        var commentBytes = "ASCII\x0\x0\x0"u8.ToArray();
        var directory = new ExifSubIfdDirectory();
        directory.Set(ExifDirectoryBase.TagUserComment, commentBytes);
        var descriptor = new ExifSubIfdDescriptor(directory);
        Assert.Equal("", descriptor.GetDescription(ExifDirectoryBase.TagUserComment));
    }

    [Fact]
    public void UserCommentDescription_ZeroLengthAscii2()
    {
        // fill the 10-byte encoding region
        var commentBytes = "ASCII\x0\x0\x0\x0\x0"u8.ToArray();
        var directory = new ExifSubIfdDirectory();
        directory.Set(ExifDirectoryBase.TagUserComment, commentBytes);
        var descriptor = new ExifSubIfdDescriptor(directory);
        Assert.Equal("", descriptor.GetDescription(ExifDirectoryBase.TagUserComment));
    }

    [Fact]
    public void UnicodeComment_Unicode()
    {
        var commentBytes = new byte[] {
            85, 78, 73, 67, 79, 68, 69, 0, // "UNICODE\0"
            0, 84, 0, 104, 0, 105, 0, 115, 0, 32, 0, 109, 0, 97, 0, 114, 0, 109, 0, 111, 0, 116, 0, 32, 0, 105, 0, 115, 0, 32, 0, 103, 0, 101, 0, 116, 0, 116, 0, 105, 0, 110, 0, 103, 0,
            32, 0, 99, 0, 108, 0, 111, 0, 115, 0, 101, 0, 46, 0, 46, 0, 46, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32, 0, 32 };
        var directory = new ExifSubIfdDirectory();
        directory.Set(ExifDirectoryBase.TagUserComment, commentBytes);
        var descriptor = new ExifSubIfdDescriptor(directory);
        Assert.Equal("This marmot is getting close...         ", descriptor.GetDescription(ExifDirectoryBase.TagUserComment));
    }

    [Fact]
    public void UnicodeComment_Ascii()
    {
        var commentBytes = new byte[] { 65, 83, 67, 73, 73, 0, 0, 0, 73, 32, 97, 109, 32, 97, 32, 99, 111, 109, 109, 101, 110, 116, 46, 32, 89, 101, 121, 46, 0 };
        var directory = new ExifSubIfdDirectory();
        directory.Set(ExifDirectoryBase.TagUserComment, commentBytes);
        var descriptor = new ExifSubIfdDescriptor(directory);
        Assert.Equal("I am a comment. Yey.", descriptor.GetDescription(ExifDirectoryBase.TagUserComment));
    }
}
