// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace MetadataExtractor.Tools.FileProcessor;

/// <summary>
/// Keeps track of unknown tags.
/// </summary>
internal sealed class UnknownTagHandler : FileHandlerBase
{
    private readonly record struct Key(string DirectoryName, int TagType);

    private readonly Dictionary<Key, int> _occurrenceByKey = [];

    public override void OnExtractionSuccess(string filePath, IList<Directory> directories, string relativePath, TextWriter log, long streamPosition)
    {
        base.OnExtractionSuccess(filePath, directories, relativePath, log, streamPosition);

        foreach (Directory directory in directories)
        {
            foreach (Tag tag in directory.Tags)
            {
                // Only interested in unknown tags (those without names)
                if (tag.HasName)
                    continue;

                Key key = new(directory.Name, tag.Type);

                _occurrenceByKey.TryGetValue(key, out int count);
                _occurrenceByKey[key] = count + 1;
            }
        }
    }

    public override void OnScanCompleted(TextWriter log)
    {
        base.OnScanCompleted(log);

        var results = _occurrenceByKey
            .OrderByDescending(pair => pair.Value)
            .ThenBy(pair => pair.Key.DirectoryName)
            .ThenBy(pair => pair.Key.TagType);

        foreach ((Key key, int count) in results)
        {
            log.WriteLine($"{key.DirectoryName}, 0x{key.TagType:X4}, {count}");
        }
    }
}
