#region License
//
// Copyright 2002-2017 Drew Noakes
// Ported from Java to C# by Yakov Danilov for Imazen LLC in 2014
//
//    Licensed under the Apache License, Version 2.0 (the "License");
//    you may not use this file except in compliance with the License.
//    You may obtain a copy of the License at
//
//        http://www.apache.org/licenses/LICENSE-2.0
//
//    Unless required by applicable law or agreed to in writing, software
//    distributed under the License is distributed on an "AS IS" BASIS,
//    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//    See the License for the specific language governing permissions and
//    limitations under the License.
//
// More information about this project is available at:
//
//    https://github.com/drewnoakes/metadata-extractor-dotnet
//    https://drewnoakes.com/code/exif/
//
#endregion

using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;

namespace MetadataExtractor.Formats.Exif.Makernotes
{
    /// <summary>
    /// Provides human-readable string representations of tag values stored in a <see cref="SonyType6MakernoteDirectory"/>.
    /// </summary>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public sealed class SanyoMakernoteDescriptor : TagDescriptor<SanyoMakernoteDirectory>
    {
        public SanyoMakernoteDescriptor([NotNull] SanyoMakernoteDirectory directory)
            : base(directory)
        {
        }

        public override string GetDescription(int tagType)
        {
            switch (tagType)
            {
                case SanyoMakernoteDirectory.TagSanyoQuality:
                    return GetSanyoQualityDescription();
                case SanyoMakernoteDirectory.TagMacro:
                    return GetMacroDescription();
                case SanyoMakernoteDirectory.TagDigitalZoom:
                    return GetDigitalZoomDescription();
                case SanyoMakernoteDirectory.TagSequentialShot:
                    return GetSequentialShotDescription();
                case SanyoMakernoteDirectory.TagWideRange:
                    return GetWideRangeDescription();
                case SanyoMakernoteDirectory.TagColorAdjustmentMode:
                    return GetColorAdjustmentModeDescription();
                case SanyoMakernoteDirectory.TagQuickShot:
                    return GetQuickShotDescription();
                case SanyoMakernoteDirectory.TagSelfTimer:
                    return GetSelfTimerDescription();
                case SanyoMakernoteDirectory.TagVoiceMemo:
                    return GetVoiceMemoDescription();
                case SanyoMakernoteDirectory.TagRecordShutterRelease:
                    return GetRecordShutterDescription();
                case SanyoMakernoteDirectory.TagFlickerReduce:
                    return GetFlickerReduceDescription();
                case SanyoMakernoteDirectory.TagOpticalZoomOn:
                    return GetOptimalZoomOnDescription();
                case SanyoMakernoteDirectory.TagDigitalZoomOn:
                    return GetDigitalZoomOnDescription();
                case SanyoMakernoteDirectory.TagLightSourceSpecial:
                    return GetLightSourceSpecialDescription();
                case SanyoMakernoteDirectory.TagResaved:
                    return GetResavedDescription();
                case SanyoMakernoteDirectory.TagSceneSelect:
                    return GetSceneSelectDescription();
                case SanyoMakernoteDirectory.TagSequenceShotInterval:
                    return GetSequenceShotIntervalDescription();
                case SanyoMakernoteDirectory.TagFlashMode:
                    return GetFlashModeDescription();
                default:
                    return base.GetDescription(tagType);
            }
        }

        [CanBeNull]
        public string GetSanyoQualityDescription()
        {
            if (!Directory.TryGetInt32(SanyoMakernoteDirectory.TagSanyoQuality, out int value))
                return null;

            switch (value)
            {
                case 0x0:
                    return "Normal/Very Low";
                case 0x1:
                    return "Normal/Low";
                case 0x2:
                    return "Normal/Medium Low";
                case 0x3:
                    return "Normal/Medium";
                case 0x4:
                    return "Normal/Medium High";
                case 0x5:
                    return "Normal/High";
                case 0x6:
                    return "Normal/Very High";
                case 0x7:
                    return "Normal/Super High";
                case 0x100:
                    return "Fine/Very Low";
                case 0x101:
                    return "Fine/Low";
                case 0x102:
                    return "Fine/Medium Low";
                case 0x103:
                    return "Fine/Medium";
                case 0x104:
                    return "Fine/Medium High";
                case 0x105:
                    return "Fine/High";
                case 0x106:
                    return "Fine/Very High";
                case 0x107:
                    return "Fine/Super High";
                case 0x200:
                    return "Super Fine/Very Low";
                case 0x201:
                    return "Super Fine/Low";
                case 0x202:
                    return "Super Fine/Medium Low";
                case 0x203:
                    return "Super Fine/Medium";
                case 0x204:
                    return "Super Fine/Medium High";
                case 0x205:
                    return "Super Fine/High";
                case 0x206:
                    return "Super Fine/Very High";
                case 0x207:
                    return "Super Fine/Super High";
                default:
                    return "Unknown (" + value + ")";
            }
        }

        [CanBeNull]
        private string GetMacroDescription()
        {
            return GetIndexedDescription(SanyoMakernoteDirectory.TagMacro,
                "Normal", "Macro", "View", "Manual");
        }

        [CanBeNull]
        private string GetDigitalZoomDescription()
        {
            return GetDecimalRational(SanyoMakernoteDirectory.TagDigitalZoom, 3);
        }

        [CanBeNull]
        private string GetSequentialShotDescription()
        {
            return GetIndexedDescription(SanyoMakernoteDirectory.TagSequentialShot,
                "None", "Standard", "Best", "Adjust Exposure");
        }

        [CanBeNull]
        private string GetWideRangeDescription()
        {
            return GetIndexedDescription(SanyoMakernoteDirectory.TagWideRange,
                "Off", "On");
        }

        [CanBeNull]
        private string GetColorAdjustmentModeDescription()
        {
            return GetIndexedDescription(SanyoMakernoteDirectory.TagColorAdjustmentMode,
                "Off", "On");
        }

        [CanBeNull]
        private string GetQuickShotDescription()
        {
            return GetIndexedDescription(SanyoMakernoteDirectory.TagQuickShot,
                "Off", "On");
        }

        [CanBeNull]
        private string GetSelfTimerDescription()
        {
            return GetIndexedDescription(SanyoMakernoteDirectory.TagSelfTimer,
                "Off", "On");
        }

        [CanBeNull]
        private string GetVoiceMemoDescription()
        {
            return GetIndexedDescription(SanyoMakernoteDirectory.TagVoiceMemo,
                "Off", "On");
        }

        [CanBeNull]
        private string GetRecordShutterDescription()
        {
            return GetIndexedDescription(SanyoMakernoteDirectory.TagRecordShutterRelease,
                "Record while down", "Press start, press stop");
        }

        [CanBeNull]
        private string GetFlickerReduceDescription()
        {
            return GetIndexedDescription(SanyoMakernoteDirectory.TagFlickerReduce,
                "Off", "On");
        }

        [CanBeNull]
        private string GetOptimalZoomOnDescription()
        {
            return GetIndexedDescription(SanyoMakernoteDirectory.TagOpticalZoomOn,
                "Off", "On");
        }

        [CanBeNull]
        private string GetDigitalZoomOnDescription()
        {
            return GetIndexedDescription(SanyoMakernoteDirectory.TagDigitalZoomOn,
                "Off", "On");
        }

        [CanBeNull]
        private string GetLightSourceSpecialDescription()
        {
            return GetIndexedDescription(SanyoMakernoteDirectory.TagLightSourceSpecial,
                "Off", "On");
        }

        [CanBeNull]
        private string GetResavedDescription()
        {
            return GetIndexedDescription(SanyoMakernoteDirectory.TagResaved,
                "No", "Yes");
        }

        [CanBeNull]
        private string GetSceneSelectDescription()
        {
            return GetIndexedDescription(SanyoMakernoteDirectory.TagSceneSelect,
                "Off", "Sport", "TV", "Night", "User 1", "User 2", "Lamp");
        }

        [CanBeNull]
        private string GetSequenceShotIntervalDescription()
        {
            return GetIndexedDescription(SanyoMakernoteDirectory.TagSequenceShotInterval,
                "5 frames/sec", "10 frames/sec", "15 frames/sec", "20 frames/sec");
        }

        [CanBeNull]
        private string GetFlashModeDescription()
        {
            return GetIndexedDescription(SanyoMakernoteDirectory.TagFlashMode,
                "Auto", "Force", "Disabled", "Red eye");
        }
    }
}
