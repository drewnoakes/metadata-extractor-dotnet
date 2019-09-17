// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace MetadataExtractor.Tests
{
    /// <summary>A mock implementation of Directory used in unit testing.</summary>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public class MockDirectory : Directory
    {
        public override string Name => string.Empty;

        protected override bool TryGetTagName(int tagType, out string? tagName)
        {
            tagName = null;
            return false;
        }
    }
}
