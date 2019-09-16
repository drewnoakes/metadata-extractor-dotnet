#region License
//
// Copyright 2002-2019 Drew Noakes
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

using System.Collections.Generic;
using System.IO;

namespace MetadataExtractor.Formats.Riff
{
    /// <summary>
    /// Base class for <see cref="IRiffChunkHandler"/> implementations.
    /// </summary>
    /// <typeparam name="T">Directory type.</typeparam>
    /// <author>Dmitry Shechtman</author>
    public abstract class RiffChunkHandler<T> : IRiffChunkHandler
        where T : Directory, new()
    {
        private readonly List<Directory> _directories;

        public RiffChunkHandler(List<Directory> directories)
        {
            _directories = directories;
        }

        public void ProcessChunk(string fourCc, byte[] payload)
        {
            if (payload.Length < MinSize)
                return;

            var directory = new T();
            try
            {
                Populate(directory, payload);
            }
            catch (IOException e)
            {
                directory.AddError($"Exception reading chunk '{fourCc}' : {e.Message}");
            }
            _directories.Add(directory);
        }

        protected abstract int MinSize { get; }

        protected abstract void Populate(T directory, byte[] payload);
    }
}
