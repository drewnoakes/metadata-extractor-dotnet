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
using System;
using Sharpen;

namespace Com.Drew.Imaging
{
    /// <summary>Contains helper methods that perform photographic conversions.</summary>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public sealed class PhotographicConversions
    {
        public static readonly double RootTwo = Math.Sqrt(2);

        /// <exception cref="System.Exception"/>
        private PhotographicConversions()
        {
            throw new Exception("Not intended for instantiation.");
        }

        /// <summary>Converts an aperture value to its corresponding F-stop number.</summary>
        /// <param name="aperture">the aperture value to convert</param>
        /// <returns>the F-stop number of the specified aperture</returns>
        public static double ApertureToFStop(double aperture)
        {
            return Math.Pow(RootTwo, aperture);
        }

        // NOTE jhead uses a different calculation as far as i can tell...  this confuses me...
        // fStop = (float)Math.exp(aperture * Math.log(2) * 0.5));
        /// <summary>Converts a shutter speed to an exposure time.</summary>
        /// <param name="shutterSpeed">the shutter speed to convert</param>
        /// <returns>the exposure time of the specified shutter speed</returns>
        public static double ShutterSpeedToExposureTime(double shutterSpeed)
        {
            return (float)(1 / Math.Exp(shutterSpeed * Math.Log(2)));
        }
    }
}
