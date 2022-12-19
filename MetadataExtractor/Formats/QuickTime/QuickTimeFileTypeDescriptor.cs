// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace MetadataExtractor.Formats.QuickTime
{
    public sealed class QuickTimeFileTypeDescriptor : TagDescriptor<QuickTimeFileTypeDirectory>
    {
        public QuickTimeFileTypeDescriptor(QuickTimeFileTypeDirectory directory)
            : base(directory)
        {
        }

        public override string? GetDescription(int tagType)
        {
            return tagType switch
            {
                QuickTimeFileTypeDirectory.TagCompatibleBrands => GetCompatibleBrandsDescription(),
                _ => base.GetDescription(tagType)
            };
        }

        public string? GetCompatibleBrandsDescription()
        {
            var values = Directory.GetStringArray(QuickTimeFileTypeDirectory.TagCompatibleBrands);

            if (values is null || values.Length == 0)
                return null;

            var sb = new StringBuilder();

            foreach (var value in values)
            {
                var trimmed = value.Trim();
                if (trimmed.Length == 0)
                    continue;
                if (sb.Length != 0)
                    sb.Append(", ");
                sb.Append(trimmed);
            }

            return sb.ToString();
        }
    }
}
