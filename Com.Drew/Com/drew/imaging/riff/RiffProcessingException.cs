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
using Com.Drew.Imaging;
using JetBrains.Annotations;
using Sharpen;

namespace Com.Drew.Imaging.Riff
{
    /// <summary>An exception class thrown upon unexpected and fatal conditions while processing a RIFF file.</summary>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    [System.Serializable]
    public class RiffProcessingException : ImageProcessingException
    {
        private const long serialVersionUID = -1658134596321487960L;

        public RiffProcessingException([CanBeNull] string message)
            : base(message)
        {
        }

        public RiffProcessingException([CanBeNull] string message, [CanBeNull] Exception cause)
            : base(message, cause)
        {
        }

        public RiffProcessingException([CanBeNull] Exception cause)
            : base(cause)
        {
        }
    }
}
