// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace MetadataExtractor
{
    /// <summary>
    /// Models a key/value pair, where both are non-null <see cref="string"/> objects.
    /// </summary>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public sealed class KeyValuePair
    {
        public KeyValuePair(string key, StringValue value)
        {
            Key = key;
            Value = value;
        }

        public string Key { get; }

        public StringValue Value { get; }

        public void Deconstruct(out string key, out StringValue value)
        {
            key = Key;
            value = Value;
        }
    }
}
