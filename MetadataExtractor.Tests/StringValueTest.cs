// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace MetadataExtractor;

public sealed class StringValueTest
{
    [Theory]
    [MemberData(nameof(GetToStringVariations))]
    public void ToStringVariations(Encoding encoding, string expected, byte[] bytes)
    {
        StringValue stringValue = new(bytes, encoding);

        Assert.Equal(expected, stringValue.ToString());
    }

    public static IEnumerable<object[]> GetToStringVariations()
    {
        yield return [Encoding.ASCII, "NEUTRAL\0\0\0", new byte[] { 78, 69, 85, 84, 82, 65, 76, 0, 0, 0 }];
        yield return [Encoding.ASCII, "N\0N\0", new byte[] { 78, 0, 78, 0 }];
        yield return [Encoding.ASCII, "N\0N", new byte[] { 78, 0, 78 }];
    }
}
