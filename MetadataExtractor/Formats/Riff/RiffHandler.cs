// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

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
