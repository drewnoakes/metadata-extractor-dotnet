/*
 * Copyright 2002-2015 Drew Noakes
 *
 *    Modified by Yakov Danilov <yakodani@gmail.com> for Imazen LLC (Ported from Java to C#)
 *    Licensed under the Apache License, Version 2.0 (the "License");
 *    you may not use this file except in compliance with the License.
 *    You may obtain a copy of the License at
 *
 *        http://www.apache.org/licenses/LICENSE-2.0
 *
 *    Unless required by applicable law or agreed to in writing, software
 *    distributed under the License is distributed on an "AS IS" BASIS,
 *    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 *    See the License for the specific language governing permissions and
 *    limitations under the License.
 *
 * More information about this project is available at:
 *
 *    https://drewnoakes.com/code/exif/
 *    https://github.com/drewnoakes/metadata-extractor
 */

using JetBrains.Annotations;

namespace MetadataExtractor.Formats.Exif.Makernotes
{
    /// <summary>
    /// Provides human-readable string representations of tag values stored in a <see cref="NikonType2MakernoteDirectory"/>.
    /// Type-2 applies to the E990 and D-series cameras such as the D1, D70 and D100.
    /// </summary>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public sealed class NikonType2MakernoteDescriptor : TagDescriptor<NikonType2MakernoteDirectory>
    {
        public NikonType2MakernoteDescriptor([NotNull] NikonType2MakernoteDirectory directory)
            : base(directory)
        {
        }

        public override string GetDescription(int tagType)
        {
            switch (tagType)
            {
                case NikonType2MakernoteDirectory.TagProgramShift:
                {
                    return GetProgramShiftDescription();
                }

                case NikonType2MakernoteDirectory.TagExposureDifference:
                {
                    return GetExposureDifferenceDescription();
                }

                case NikonType2MakernoteDirectory.TagLens:
                {
                    return GetLensDescription();
                }

                case NikonType2MakernoteDirectory.TagCameraHueAdjustment:
                {
                    return GetHueAdjustmentDescription();
                }

                case NikonType2MakernoteDirectory.TagCameraColorMode:
                {
                    return GetColorModeDescription();
                }

                case NikonType2MakernoteDirectory.TagAutoFlashCompensation:
                {
                    return GetAutoFlashCompensationDescription();
                }

                case NikonType2MakernoteDirectory.TagFlashExposureCompensation:
                {
                    return GetFlashExposureCompensationDescription();
                }

                case NikonType2MakernoteDirectory.TagFlashBracketCompensation:
                {
                    return GetFlashBracketCompensationDescription();
                }

                case NikonType2MakernoteDirectory.TagExposureTuning:
                {
                    return GetExposureTuningDescription();
                }

                case NikonType2MakernoteDirectory.TagLensStops:
                {
                    return GetLensStopsDescription();
                }

                case NikonType2MakernoteDirectory.TagColorSpace:
                {
                    return GetColorSpaceDescription();
                }

                case NikonType2MakernoteDirectory.TagActiveDLighting:
                {
                    return GetActiveDLightingDescription();
                }

                case NikonType2MakernoteDirectory.TagVignetteControl:
                {
                    return GetVignetteControlDescription();
                }

                case NikonType2MakernoteDirectory.TagIso1:
                {
                    return GetIsoSettingDescription();
                }

                case NikonType2MakernoteDirectory.TagDigitalZoom:
                {
                    return GetDigitalZoomDescription();
                }

                case NikonType2MakernoteDirectory.TagFlashUsed:
                {
                    return GetFlashUsedDescription();
                }

                case NikonType2MakernoteDirectory.TagAfFocusPosition:
                {
                    return GetAutoFocusPositionDescription();
                }

                case NikonType2MakernoteDirectory.TagFirmwareVersion:
                {
                    return GetFirmwareVersionDescription();
                }

                case NikonType2MakernoteDirectory.TagLensType:
                {
                    return GetLensTypeDescription();
                }

                case NikonType2MakernoteDirectory.TagShootingMode:
                {
                    return GetShootingModeDescription();
                }

                case NikonType2MakernoteDirectory.TagNefCompression:
                {
                    return GetNefCompressionDescription();
                }

                case NikonType2MakernoteDirectory.TagHighIsoNoiseReduction:
                {
                    return GetHighIsoNoiseReductionDescription();
                }

                case NikonType2MakernoteDirectory.TagPowerUpTime:
                {
                    return GetPowerUpTimeDescription();
                }

                default:
                {
                    return base.GetDescription(tagType);
                }
            }
        }

        [CanBeNull]
        public string GetPowerUpTimeDescription()
        {
            return GetEpochTimeDescription(NikonType2MakernoteDirectory.TagPowerUpTime);
        }

        [CanBeNull]
        public string GetHighIsoNoiseReductionDescription()
        {
            return GetIndexedDescription(NikonType2MakernoteDirectory.TagHighIsoNoiseReduction, "Off", "Minimal", "Low", null, "Normal", null, "High");
        }

        [CanBeNull]
        public string GetFlashUsedDescription()
        {
            return GetIndexedDescription(NikonType2MakernoteDirectory.TagFlashUsed, "Flash Not Used", "Manual Flash", null, "Flash Not Ready", null, null, null, "External Flash", "Fired, Commander Mode", "Fired, TTL Mode");
        }

        [CanBeNull]
        public string GetNefCompressionDescription()
        {
            return GetIndexedDescription(NikonType2MakernoteDirectory.TagNefCompression, 1, "Lossy (Type 1)", null, "Uncompressed", null, null, null, "Lossless", "Lossy (Type 2)");
        }

        [CanBeNull]
        public string GetShootingModeDescription()
        {
            return GetBitFlagDescription(NikonType2MakernoteDirectory.TagShootingMode, new[] { "Single Frame", "Continuous" }, "Delay", null, "PC Control", "Exposure Bracketing", "Auto ISO", "White-Balance Bracketing", "IR Control");
        }

        // LSB [low label, high label]
        [CanBeNull]
        public string GetLensTypeDescription()
        {
            return GetBitFlagDescription(NikonType2MakernoteDirectory.TagLensType, new[] { "AF", "MF" }, "D", "G", "VR");
        }

        // LSB [low label, high label]
        [CanBeNull]
        public string GetColorSpaceDescription()
        {
            return GetIndexedDescription(NikonType2MakernoteDirectory.TagColorSpace, 1, "sRGB", "Adobe RGB");
        }

        [CanBeNull]
        public string GetActiveDLightingDescription()
        {
            var value = Directory.GetInteger(NikonType2MakernoteDirectory.TagActiveDLighting);
            if (value == null)
            {
                return null;
            }
            switch (value)
            {
                case 0:
                {
                    return "Off";
                }

                case 1:
                {
                    return "Light";
                }

                case 3:
                {
                    return "Normal";
                }

                case 5:
                {
                    return "High";
                }

                case 7:
                {
                    return "Extra High";
                }

                case 65535:
                {
                    return "Auto";
                }

                default:
                {
                    return "Unknown (" + value + ")";
                }
            }
        }

        [CanBeNull]
        public string GetVignetteControlDescription()
        {
            var value = Directory.GetInteger(NikonType2MakernoteDirectory.TagVignetteControl);
            if (value == null)
            {
                return null;
            }
            switch (value)
            {
                case 0:
                {
                    return "Off";
                }

                case 1:
                {
                    return "Low";
                }

                case 3:
                {
                    return "Normal";
                }

                case 5:
                {
                    return "High";
                }

                default:
                {
                    return "Unknown (" + value + ")";
                }
            }
        }

        [CanBeNull]
        public string GetAutoFocusPositionDescription()
        {
            var values = Directory.GetIntArray(NikonType2MakernoteDirectory.TagAfFocusPosition);
            if (values == null)
            {
                return null;
            }
            if (values.Length != 4 || values[0] != 0 || values[2] != 0 || values[3] != 0)
            {
                return "Unknown (" + Directory.GetString(NikonType2MakernoteDirectory.TagAfFocusPosition) + ")";
            }
            switch (values[1])
            {
                case 0:
                {
                    return "Centre";
                }

                case 1:
                {
                    return "Top";
                }

                case 2:
                {
                    return "Bottom";
                }

                case 3:
                {
                    return "Left";
                }

                case 4:
                {
                    return "Right";
                }

                default:
                {
                    return "Unknown (" + values[1] + ")";
                }
            }
        }

        [CanBeNull]
        public string GetDigitalZoomDescription()
        {
            var value = Directory.GetRational(NikonType2MakernoteDirectory.TagDigitalZoom);
            if (value == null)
            {
                return null;
            }
            return value.ToInt32() == 1 ? "No digital zoom" : value.ToSimpleString(true) + "x digital zoom";
        }

        [CanBeNull]
        public string GetProgramShiftDescription()
        {
            return GetEvDescription(NikonType2MakernoteDirectory.TagProgramShift);
        }

        [CanBeNull]
        public string GetExposureDifferenceDescription()
        {
            return GetEvDescription(NikonType2MakernoteDirectory.TagExposureDifference);
        }

        [CanBeNull]
        public string GetAutoFlashCompensationDescription()
        {
            return GetEvDescription(NikonType2MakernoteDirectory.TagAutoFlashCompensation);
        }

        [CanBeNull]
        public string GetFlashExposureCompensationDescription()
        {
            return GetEvDescription(NikonType2MakernoteDirectory.TagFlashExposureCompensation);
        }

        [CanBeNull]
        public string GetFlashBracketCompensationDescription()
        {
            return GetEvDescription(NikonType2MakernoteDirectory.TagFlashBracketCompensation);
        }

        [CanBeNull]
        public string GetExposureTuningDescription()
        {
            return GetEvDescription(NikonType2MakernoteDirectory.TagExposureTuning);
        }

        [CanBeNull]
        public string GetLensStopsDescription()
        {
            return GetEvDescription(NikonType2MakernoteDirectory.TagLensStops);
        }

        [CanBeNull]
        private string GetEvDescription(int tagType)
        {
            var values = Directory.GetIntArray(tagType);
            if (values == null || values.Length < 3 || values[2] == 0)
                return null;
            return string.Format("{0:0.##} EV", values[0] * values[1] / (double)values[2]);
        }

        [CanBeNull]
        public string GetIsoSettingDescription()
        {
            var values = Directory.GetIntArray(NikonType2MakernoteDirectory.TagIso1);
            if (values == null)
            {
                return null;
            }
            if (values[0] != 0 || values[1] == 0)
            {
                return "Unknown (" + Directory.GetString(NikonType2MakernoteDirectory.TagIso1) + ")";
            }
            return "ISO " + values[1];
        }

        [CanBeNull]
        public string GetLensDescription()
        {
            var values = Directory.GetRationalArray(NikonType2MakernoteDirectory.TagLens);
            return values == null
                ? null
                : values.Length < 4
                    ? Directory.GetString(NikonType2MakernoteDirectory.TagLens)
                    : string.Format("{0}-{1}mm f/{2:0.#}-{3:0.#}",
                        values[0].ToInt32(),
                        values[1].ToInt32(),
                        values[2].ToSingle(),
                        values[3].ToSingle());
        }

        [CanBeNull]
        public string GetHueAdjustmentDescription()
        {
            return GetFormattedString(NikonType2MakernoteDirectory.TagCameraHueAdjustment, "{0} degrees");
        }

        [CanBeNull]
        public string GetColorModeDescription()
        {
            var value = Directory.GetString(NikonType2MakernoteDirectory.TagCameraColorMode);
            return value == null ? null : value.StartsWith("MODE1") ? "Mode I (sRGB)" : value;
        }

        [CanBeNull]
        public string GetFirmwareVersionDescription()
        {
            return GetVersionBytesDescription(NikonType2MakernoteDirectory.TagFirmwareVersion, 2);
        }
    }
}
