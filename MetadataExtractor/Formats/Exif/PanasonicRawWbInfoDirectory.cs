// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace MetadataExtractor.Formats.Exif;

/// <remarks>These tags can be found in Panasonic/Leica RAW, RW2 and RWL images. The index values are 'fake' but
/// chosen specifically to make processing easier</remarks>
/// <author>Kevin Mott https://github.com/kwhopper</author>
/// <author>Drew Noakes https://drewnoakes.com</author>
public class PanasonicRawWbInfoDirectory : Directory
{
    public const int TagNumWbEntries = 0;

    public const int TagWbType1 = 1;
    public const int TagWbRbLevels1 = 2;

    public const int TagWbType2 = 4;
    public const int TagWbRbLevels2 = 5;

    public const int TagWbType3 = 7;
    public const int TagWbRbLevels3 = 8;

    public const int TagWbType4 = 10;
    public const int TagWbRbLevels4 = 11;

    public const int TagWbType5 = 13;
    public const int TagWbRbLevels5 = 14;

    public const int TagWbType6 = 16;
    public const int TagWbRbLevels6 = 17;

    public const int TagWbType7 = 19;
    public const int TagWbRbLevels7 = 20;

    private static readonly Dictionary<int, string> _tagNameMap = new()
    {
        { TagNumWbEntries, "Num WB Entries" },
        { TagWbType1, "WB Type 1" },
        { TagWbRbLevels1, "WB RGB Levels 1" },
        { TagWbType2, "WB Type 2" },
        { TagWbRbLevels2, "WB RGB Levels 2" },
        { TagWbType3, "WB Type 3" },
        { TagWbRbLevels3, "WB RGB Levels 3" },
        { TagWbType4, "WB Type 4" },
        { TagWbRbLevels4, "WB RGB Levels 4" },
        { TagWbType5, "WB Type 5" },
        { TagWbRbLevels5, "WB RGB Levels 5" },
        { TagWbType6, "WB Type 6" },
        { TagWbRbLevels6, "WB RGB Levels 6" },
        { TagWbType7, "WB Type 7" },
        { TagWbRbLevels7, "WB RGB Levels 7" }
    };

    public PanasonicRawWbInfoDirectory() : base(_tagNameMap)
    {
        SetDescriptor(new PanasonicRawWbInfoDescriptor(this));
    }

    public override string Name => "PanasonicRaw WbInfo";
}
