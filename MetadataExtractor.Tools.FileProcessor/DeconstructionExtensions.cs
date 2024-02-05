// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

#if NETFRAMEWORK

namespace System.Collections.Generic;

internal static class DeconstructionExtensions
{
    [DebuggerStepThrough]
    public static void Deconstruct<TKey, TValue>(this KeyValuePair<TKey, TValue> pair, out TKey key, out TValue value)
    {
        key = pair.Key;
        value = pair.Value;
    }
}

#endif
