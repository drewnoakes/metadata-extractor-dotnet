// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace MetadataExtractor.Formats.Mkv;

/// <summary>
/// One SimpleTag, parented to its target group or containing SimpleTag.
/// Repeated names are separate directories, never overwritten.
/// </summary>
public sealed class MkvSimpleTagDirectory : Directory
{
    public const int TagName = ElementIds.TagName;
    public const int TagString = ElementIds.TagString;
    public const int TagBinary = ElementIds.TagBinary;
    public const int TagLanguage = ElementIds.TagLanguage;
    public const int TagLanguageBcp47 = ElementIds.TagLanguageBcp47;
    public const int TagDefault = ElementIds.TagDefault;

    private static readonly Dictionary<int, string> _tagNameMap = new()
    {
        { TagName, "Name" },
        { TagString, "String Value" },
        { TagBinary, "Binary Value" },
        { TagLanguage, "Language" },
        { TagLanguageBcp47, "Language BCP47" },
        { TagDefault, "Default Language" }
    };

    public MkvSimpleTagDirectory() : base(_tagNameMap) => SetDescriptor(new MkvSimpleTagDescriptor(this));

    /// <summary>Gets the BCP47 language, falling back to the legacy language or "und".</summary>
    public string GetLanguage() => this.GetString(TagLanguageBcp47) ?? this.GetString(TagLanguage) ?? "und";

    /// <summary>Gets the targets inherited through any containing SimpleTag directories.</summary>
    public MkvTagDirectory? GetTargets()
    {
        for (var parent = Parent; parent is not null; parent = parent.Parent)
        {
            if (parent is MkvTagDirectory targets)
                return targets;
        }
        return null;
    }

    public override string Name => "Matroska Simple Tag";
}
