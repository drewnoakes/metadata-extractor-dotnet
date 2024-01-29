// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

#if NETFRAMEWORK || NETSTANDARD1_3

internal static class EncodingExtensions
{
    /// <summary>
    /// Converts a span of bytes into a string, following the specified encoding's rules.
    /// </summary>
    /// <param name="encoding">The encoding to follow.</param>
    /// <param name="bytes">The bytes to convert.</param>
    /// <returns>The decoded string.</returns>
    public unsafe static string GetString(this Encoding encoding, ReadOnlySpan<byte> bytes)
    {
        // Poly-fill for method available in newer versions of .NET

        fixed (byte* pBytes = bytes)
        {
            return encoding.GetString(pBytes, bytes.Length);
        }
    }
}

#endif
