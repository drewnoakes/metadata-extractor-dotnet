// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using MetadataExtractor.IO;
using System;
using System.IO;
using System.Text;

namespace MetadataExtractor.Formats.Tga
{
    /// <summary>Reads TGA image file extension area.</summary>
    /// <author>Dmitry Shechtman</author>
    sealed class TgaExtensionReader : TgaDirectoryReader<TgaExtensionDirectory, SequentialReader>
    {
        private const int ExtensionSize = 495;

        public static readonly TgaExtensionReader Instance = new TgaExtensionReader();

        private TgaExtensionReader()
        {
        }

        protected override SequentialReader CreateReader(Stream stream)
        {
            return new SequentialStreamReader(stream, isMotorolaByteOrder: false);
        }

        protected override void Populate(Stream stream, int offset, SequentialReader reader, TgaExtensionDirectory directory)
        {
            var size = reader.GetUInt16();
            if (size < ExtensionSize)
                throw new ImageProcessingException("Invalid TGA extension size");
            var authorName = GetString(reader, 41);
            if (authorName.Length > 0)
                directory.Set(TgaExtensionDirectory.TagAuthorName, authorName);
            var comments = GetString(reader, 324);
            if (comments.Length > 0)
                directory.Set(TgaExtensionDirectory.TagComments, comments);
            if (TryGetDateTime(reader, out var dateTime))
                directory.Set(TgaExtensionDirectory.TagDateTime, dateTime);
            var jobName = GetString(reader, 41);
            if (jobName.Length > 0)
                directory.Set(TgaExtensionDirectory.TagJobName, jobName);
            if (TryGetTimeSpan(reader, out var jobTime))
                directory.Set(TgaExtensionDirectory.TagJobTime, jobTime);
            var softwareName = GetString(reader, 41);
            if (softwareName.Length > 0)
                directory.Set(TgaExtensionDirectory.TagSoftwareName, softwareName);
            var softwareVersion = GetSoftwareVersion(reader, softwareName);
            if (softwareVersion.Length > 0)
                directory.Set(TgaExtensionDirectory.TagSoftwareVersion, softwareVersion);
            var keyColor = reader.GetUInt32();
            if (keyColor != 0)
                directory.Set(TgaExtensionDirectory.TagKeyColor, keyColor);
            if (TryGetRational(reader, out var aspectRatio))
                directory.Set(TgaExtensionDirectory.TagAspectRatio, aspectRatio);
            if (TryGetRational(reader, out var gamma))
                directory.Set(TgaExtensionDirectory.TagGamma, gamma);
            var colorCorrectionOffset = reader.GetInt32();
            if (colorCorrectionOffset != 0)
                directory.Set(TgaExtensionDirectory.TagColorCorrectionOffset, colorCorrectionOffset);
            var thumbnailOffset = reader.GetInt32();
            if (thumbnailOffset != 0)
                directory.Set(TgaExtensionDirectory.TagThumbnailOffset, thumbnailOffset);
            var scanLineOffset = reader.GetInt32();
            if (scanLineOffset != 0)
                directory.Set(TgaExtensionDirectory.TagScanLineOffset, scanLineOffset);
            var attributesType = reader.GetByte();
            directory.Set(TgaExtensionDirectory.TagAttributesType, attributesType);
        }

        private static string GetString(SequentialReader reader, int length)
        {
            var buffer = new byte[length];
            reader.GetBytes(buffer, 0, length);
            int i = 0;
            while (i < buffer.Length && buffer[i] != '\0')
                ++i;
            return Encoding.ASCII.GetString(buffer, 0, i).TrimEnd();
        }

        private static bool TryGetDateTime(SequentialReader reader, out DateTime dateTime)
        {
            var month = reader.GetInt16();
            var day = reader.GetInt16();
            var year = reader.GetInt16();
            var hour = reader.GetInt16();
            var minute = reader.GetInt16();
            var second = reader.GetInt16();
            if (month == 0 && day == 0 && year == 0)
            {
                dateTime = DateTime.MinValue;
                return false;
            }
            dateTime = new DateTime(year, month, day, hour, minute, second);
            return true;
        }

        private static bool TryGetTimeSpan(SequentialReader reader, out TimeSpan timeSpan)
        {
            var hours = reader.GetInt16();
            var minutes = reader.GetInt16();
            var seconds = reader.GetInt16();
            if (hours == 0 && minutes == 0 && seconds == 0)
            {
                timeSpan = TimeSpan.Zero;
                return false;
            }
            timeSpan = new TimeSpan(hours, minutes, seconds);
            return true;
        }

        private static string GetSoftwareVersion(SequentialReader reader, string softwareName)
        {
            var number = reader.GetUInt16();
            var letter = reader.GetByte();
            if (number == 0)
                return string.Empty;
            var sb = new StringBuilder();
            var denom = softwareName != "Paint Shop Pro" ? 100 : 0x100;
            sb.Append(number / denom);
            sb.Append('.');
            sb.Append(number % denom);
            if (letter != 0 && letter != 0x20)
                sb.Append((char)letter);
            return sb.ToString();
        }

        private static bool TryGetRational(SequentialReader reader, out Rational value)
        {
            var ratioNum = reader.GetUInt16();
            var ratioDenom = reader.GetUInt16();
            if (ratioDenom == 0)
            {
                value = default;
                return false;
            }
            value = new Rational(ratioNum, ratioDenom);
            return true;
        }

    }
}
