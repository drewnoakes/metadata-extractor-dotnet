// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.IO;

namespace MetadataExtractor.Tests
{
    /// <summary>Utility functions for working with unit tests data files.</summary>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    internal static class TestDataUtil
    {
        /// <summary>
        /// Traditionally, NUnit and xUnit on desktops have run tests such that the current directory
        /// was the project folder. xUnit on .NET Core uses the bin/Debug folder. This method tries both.
        /// </summary>
        public static string GetPath(string filePath) => File.Exists(filePath) ? filePath : Path.Combine("../..", filePath);

        public static Stream OpenRead(string filePath) => new FileStream(GetPath(filePath), FileMode.Open, FileAccess.Read, FileShare.Read);

        public static byte[] GetBytes(string filePath) => File.ReadAllBytes(GetPath(filePath));
    }
}
