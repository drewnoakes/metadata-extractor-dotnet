// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Diagnostics.CodeAnalysis;

namespace MetadataExtractor.Formats.Exif.Makernotes
{
    /// <summary>
    /// Provides human-readable string representations of tag values stored in a <see cref="SonyType6MakernoteDirectory"/>.
    /// </summary>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public sealed class SanyoMakernoteDescriptor : TagDescriptor<SanyoMakernoteDirectory>
    {
        public SanyoMakernoteDescriptor(SanyoMakernoteDirectory directory)
            : base(directory)
        {
        }

        public override string? GetDescription(int tagType)
        {
            return tagType switch
            {
                SanyoMakernoteDirectory.TagSanyoQuality => GetSanyoQualityDescription(),
                SanyoMakernoteDirectory.TagMacro => GetMacroDescription(),
                SanyoMakernoteDirectory.TagDigitalZoom => GetDigitalZoomDescription(),
                SanyoMakernoteDirectory.TagSequentialShot => GetSequentialShotDescription(),
                SanyoMakernoteDirectory.TagWideRange => GetWideRangeDescription(),
                SanyoMakernoteDirectory.TagColorAdjustmentMode => GetColorAdjustmentModeDescription(),
                SanyoMakernoteDirectory.TagQuickShot => GetQuickShotDescription(),
                SanyoMakernoteDirectory.TagSelfTimer => GetSelfTimerDescription(),
                SanyoMakernoteDirectory.TagVoiceMemo => GetVoiceMemoDescription(),
                SanyoMakernoteDirectory.TagRecordShutterRelease => GetRecordShutterDescription(),
                SanyoMakernoteDirectory.TagFlickerReduce => GetFlickerReduceDescription(),
                SanyoMakernoteDirectory.TagOpticalZoomOn => GetOptimalZoomOnDescription(),
                SanyoMakernoteDirectory.TagDigitalZoomOn => GetDigitalZoomOnDescription(),
                SanyoMakernoteDirectory.TagLightSourceSpecial => GetLightSourceSpecialDescription(),
                SanyoMakernoteDirectory.TagResaved => GetResavedDescription(),
                SanyoMakernoteDirectory.TagSceneSelect => GetSceneSelectDescription(),
                SanyoMakernoteDirectory.TagSequenceShotInterval => GetSequenceShotIntervalDescription(),
                SanyoMakernoteDirectory.TagFlashMode => GetFlashModeDescription(),
                _ => base.GetDescription(tagType),
            };
        }

        public string? GetSanyoQualityDescription()
        {
            if (!Directory.TryGetInt32(SanyoMakernoteDirectory.TagSanyoQuality, out int value))
                return null;

            return value switch
            {
                0x0 => "Normal/Very Low",
                0x1 => "Normal/Low",
                0x2 => "Normal/Medium Low",
                0x3 => "Normal/Medium",
                0x4 => "Normal/Medium High",
                0x5 => "Normal/High",
                0x6 => "Normal/Very High",
                0x7 => "Normal/Super High",
                0x100 => "Fine/Very Low",
                0x101 => "Fine/Low",
                0x102 => "Fine/Medium Low",
                0x103 => "Fine/Medium",
                0x104 => "Fine/Medium High",
                0x105 => "Fine/High",
                0x106 => "Fine/Very High",
                0x107 => "Fine/Super High",
                0x200 => "Super Fine/Very Low",
                0x201 => "Super Fine/Low",
                0x202 => "Super Fine/Medium Low",
                0x203 => "Super Fine/Medium",
                0x204 => "Super Fine/Medium High",
                0x205 => "Super Fine/High",
                0x206 => "Super Fine/Very High",
                0x207 => "Super Fine/Super High",
                _ => "Unknown (" + value + ")",
            };
        }

        private string? GetMacroDescription()
        {
            return GetIndexedDescription(SanyoMakernoteDirectory.TagMacro,
                "Normal", "Macro", "View", "Manual");
        }

        private string? GetDigitalZoomDescription()
        {
            return GetDecimalRational(SanyoMakernoteDirectory.TagDigitalZoom, 3);
        }

        private string? GetSequentialShotDescription()
        {
            return GetIndexedDescription(SanyoMakernoteDirectory.TagSequentialShot,
                "None", "Standard", "Best", "Adjust Exposure");
        }

        private string? GetWideRangeDescription()
        {
            return GetIndexedDescription(SanyoMakernoteDirectory.TagWideRange,
                "Off", "On");
        }

        private string? GetColorAdjustmentModeDescription()
        {
            return GetIndexedDescription(SanyoMakernoteDirectory.TagColorAdjustmentMode,
                "Off", "On");
        }

        private string? GetQuickShotDescription()
        {
            return GetIndexedDescription(SanyoMakernoteDirectory.TagQuickShot,
                "Off", "On");
        }

        private string? GetSelfTimerDescription()
        {
            return GetIndexedDescription(SanyoMakernoteDirectory.TagSelfTimer,
                "Off", "On");
        }

        private string? GetVoiceMemoDescription()
        {
            return GetIndexedDescription(SanyoMakernoteDirectory.TagVoiceMemo,
                "Off", "On");
        }

        private string? GetRecordShutterDescription()
        {
            return GetIndexedDescription(SanyoMakernoteDirectory.TagRecordShutterRelease,
                "Record while down", "Press start, press stop");
        }

        private string? GetFlickerReduceDescription()
        {
            return GetIndexedDescription(SanyoMakernoteDirectory.TagFlickerReduce,
                "Off", "On");
        }

        private string? GetOptimalZoomOnDescription()
        {
            return GetIndexedDescription(SanyoMakernoteDirectory.TagOpticalZoomOn,
                "Off", "On");
        }

        private string? GetDigitalZoomOnDescription()
        {
            return GetIndexedDescription(SanyoMakernoteDirectory.TagDigitalZoomOn,
                "Off", "On");
        }

        private string? GetLightSourceSpecialDescription()
        {
            return GetIndexedDescription(SanyoMakernoteDirectory.TagLightSourceSpecial,
                "Off", "On");
        }

        private string? GetResavedDescription()
        {
            return GetIndexedDescription(SanyoMakernoteDirectory.TagResaved,
                "No", "Yes");
        }

        private string? GetSceneSelectDescription()
        {
            return GetIndexedDescription(SanyoMakernoteDirectory.TagSceneSelect,
                "Off", "Sport", "TV", "Night", "User 1", "User 2", "Lamp");
        }

        private string? GetSequenceShotIntervalDescription()
        {
            return GetIndexedDescription(SanyoMakernoteDirectory.TagSequenceShotInterval,
                "5 frames/sec", "10 frames/sec", "15 frames/sec", "20 frames/sec");
        }

        private string? GetFlashModeDescription()
        {
            return GetIndexedDescription(SanyoMakernoteDirectory.TagFlashMode,
                "Auto", "Force", "Disabled", "Red eye");
        }
    }
}
