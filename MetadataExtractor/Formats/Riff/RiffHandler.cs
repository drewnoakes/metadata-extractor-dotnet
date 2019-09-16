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

using System;
using System.Collections.Generic;

namespace MetadataExtractor.Formats.Riff
{
    /// <summary>
    /// Implementation of <see cref="IRiffHandler"/> using a dictionary of
    /// <see cref="IRiffChunkHandler"/> factory delegates.
    /// </summary>
    /// <author>Dmitry Shechtman</author>
    public abstract class RiffHandler : IRiffHandler
    {
        private readonly List<Directory> _directories;
        private readonly Dictionary<string, Func<List<Directory>, IRiffChunkHandler>> _handlers;

        protected RiffHandler(List<Directory> directories, Dictionary<string, Func<List<Directory>, IRiffChunkHandler>> handlers)
        {
            _directories = directories;
            _handlers = handlers;
        }

        public void ProcessChunk(string fourCc, byte[] payload)
        {
            if (!_handlers.TryGetValue(fourCc, out Func<List<Directory>, IRiffChunkHandler> createHandler))
                return;
            var handler = createHandler(_directories);
            handler.ProcessChunk(fourCc, payload);
        }

        public bool ShouldAcceptChunk(string fourCc) => _handlers.ContainsKey(fourCc);

        public abstract bool ShouldAcceptRiffIdentifier(string identifier);

        public abstract bool ShouldAcceptList(string fourCc);
    }
}
