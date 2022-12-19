// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace MetadataExtractor.Formats.Tga
{
    /// <summary>Reads TGA image file extension area.</summary>
    /// <author>Dmitry Shechtman</author>
    internal sealed class TgaExtensionReader : TgaDirectoryReader<TgaExtensionDirectory>
    {
        private const int ExtensionSize = 495;

        protected override void Populate(Stream stream, int offset, TgaExtensionDirectory directory)
        {
            var reader = new SequentialStreamReader(stream, isMotorolaByteOrder: false);

            var size = reader.GetUInt16();
            if (size < ExtensionSize)
                throw new ImageProcessingException("Invalid TGA extension size");
            var authorName = GetString(41);
            if (authorName.Length > 0)
                directory.Set(TgaExtensionDirectory.TagAuthorName, authorName);
            var comments = GetString(324);
            if (comments.Length > 0)
                directory.Set(TgaExtensionDirectory.TagComments, comments);
            if (TryGetDateTime(out var dateTime))
                directory.Set(TgaExtensionDirectory.TagDateTime, dateTime);
            var jobName = GetString(41);
            if (jobName.Length > 0)
                directory.Set(TgaExtensionDirectory.TagJobName, jobName);
            if (TryGetTimeSpan(out var jobTime))
                directory.Set(TgaExtensionDirectory.TagJobTime, jobTime);
            var softwareName = GetString(41);
            if (softwareName.Length > 0)
                directory.Set(TgaExtensionDirectory.TagSoftwareName, softwareName);
            var softwareVersion = GetSoftwareVersion(softwareName);
            if (softwareVersion.Length > 0)
                directory.Set(TgaExtensionDirectory.TagSoftwareVersion, softwareVersion);
            var keyColor = reader.GetUInt32();
            if (keyColor != 0)
                directory.Set(TgaExtensionDirectory.TagKeyColor, keyColor);
            if (TryGetRational(out var aspectRatio))
                directory.Set(TgaExtensionDirectory.TagAspectRatio, aspectRatio);
            if (TryGetRational(out var gamma))
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

            string GetString(int length)
            {
                var buffer = new byte[length];
                reader.GetBytes(buffer, 0, length);
                int i = 0;
                while (i < buffer.Length && buffer[i] != '\0')
                    ++i;
                return Encoding.ASCII.GetString(buffer, 0, i).TrimEnd();
            }

            bool TryGetDateTime(out DateTime dateTime)
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

            bool TryGetTimeSpan(out TimeSpan timeSpan)
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

            string GetSoftwareVersion(string softwareName)
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

            bool TryGetRational(out Rational value)
            {
                var num = reader.GetUInt16();
                var denom = reader.GetUInt16();
                if (denom == 0)
                {
                    value = default;
                    return false;
                }
                value = new Rational(num, denom);
                return true;
            }
        }
    }
}
