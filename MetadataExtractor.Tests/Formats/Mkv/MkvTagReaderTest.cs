// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using static MetadataExtractor.Formats.Mkv.EbmlTestUtil;

namespace MetadataExtractor.Formats.Mkv;

public sealed class MkvTagReaderTest
{
    [Fact]
    public void StandaloneTagHasUndeterminedLanguageAndNoTargets()
    {
        var tag = new MkvSimpleTagDirectory();

        Assert.Equal("und", tag.GetLanguage());
        Assert.Null(tag.GetTargets());
        Assert.False(tag.ContainsTag(MkvSimpleTagDirectory.TagLanguage));
    }

    [Fact]
    public void PreservesRepeatedNamesAndValuesInFileOrder()
    {
        var metadata = Extract(Document(Element(Id.Tags, Element(Id.Tag, Element(Id.Targets),
            Simple("ARTIST", "First"), Simple("ARTIST", "Second"), Simple("ARTIST", "First")))));

        var tags = metadata.OfType<MkvSimpleTagDirectory>().ToArray();
        Assert.Equal(new[] { "First", "Second", "First" }, tags.Select(tag => tag.GetString(MkvSimpleTagDirectory.TagString)));
        Assert.All(tags, tag =>
        {
            Assert.Equal("ARTIST", tag.GetString(MkvSimpleTagDirectory.TagName));
            Assert.Same(tags[0].Parent, tag.Parent);
        });
        Assert.IsType<MkvTagDirectory>(tags[0].Parent);
        NoErrors(metadata);
    }

    [Fact]
    public void ResolvesNestedTagsToTargetsThatFollowTheirValues()
    {
        var metadata = Extract(Document(Element(Id.Tags, Element(Id.Tag,
            Simple("ARTIST", "Band", Simple("LEAD_PERFORMER", "Singer", Simple("DATE_STARTED", "1981-08"))),
            Element(Id.Targets, Unsigned(Id.TargetTypeValue, 30), Text(Id.TargetType, "TRACK"),
                Unsigned(Id.TagTrackUid, 1), Unsigned(Id.TagTrackUid, ulong.MaxValue), Unsigned(Id.TagTrackUid, 1),
                Unsigned(Id.TagEditionUid, 9), Unsigned(Id.TagChapterUid, 7), Unsigned(Id.TagAttachmentUid, 8))))));

        var tags = metadata.OfType<MkvSimpleTagDirectory>().ToArray();
        var target = Assert.Single(metadata.OfType<MkvTagDirectory>());
        Assert.Equal(new[] { "ARTIST", "LEAD_PERFORMER", "DATE_STARTED" },
            tags.Select(tag => tag.GetString(MkvSimpleTagDirectory.TagName)));
        Assert.Equal(new[] { "Band", "Singer", "1981-08" },
            tags.Select(tag => tag.GetString(MkvSimpleTagDirectory.TagString)));
        Assert.Same(target, tags[0].Parent);
        Assert.Same(tags[0], tags[1].Parent);
        Assert.Same(tags[1], tags[2].Parent);
        Assert.All(tags, tag => Assert.Same(target, tag.GetTargets()));
        Assert.Equal(new[] { 1UL, ulong.MaxValue, 1UL }, Assert.IsType<ulong[]>(target.GetObject(MkvTagDirectory.TagTrackUids)));
        Assert.Equal(new[] { 9UL }, Assert.IsType<ulong[]>(target.GetObject(MkvTagDirectory.TagEditionUids)));
        Assert.Equal(new[] { 7UL }, Assert.IsType<ulong[]>(target.GetObject(MkvTagDirectory.TagChapterUids)));
        Assert.Equal(new[] { 8UL }, Assert.IsType<ulong[]>(target.GetObject(MkvTagDirectory.TagAttachmentUids)));
        Assert.Equal("TRACK", target.GetString(MkvTagDirectory.TagTargetType));
        Assert.Equal(30UL, UInt(target, MkvTagDirectory.TagTargetTypeValue));
        var collection = Assert.IsType<MkvTagsDirectory>(target.Parent);
        Assert.IsType<MkvSegmentDirectory>(collection.Parent);
        NoErrors(metadata);
    }

    [Fact]
    public void PreservesEveryRepeatedTargetListIncludingDuplicatesAndHighBitUids()
    {
        var metadata = Extract(Document(Element(Id.Tags, Element(Id.Tag,
            Element(Id.Targets,
                Unsigned(Id.TagTrackUid, 0x8000000000000000), Unsigned(Id.TagTrackUid, 0),
                Unsigned(Id.TagEditionUid, 9), Unsigned(Id.TagEditionUid, 9),
                Unsigned(Id.TagChapterUid, ulong.MaxValue), Unsigned(Id.TagChapterUid, 7),
                Unsigned(Id.TagAttachmentUid, 8), Unsigned(Id.TagAttachmentUid, 8)),
            Simple("TITLE", "Targeted")))));

        var target = Assert.Single(metadata.OfType<MkvTagDirectory>());
        Assert.Equal(new[] { 0x8000000000000000UL, 0UL }, Assert.IsType<ulong[]>(target.GetObject(MkvTagDirectory.TagTrackUids)));
        Assert.Equal(new[] { 9UL, 9UL }, Assert.IsType<ulong[]>(target.GetObject(MkvTagDirectory.TagEditionUids)));
        Assert.Equal(new[] { ulong.MaxValue, 7UL }, Assert.IsType<ulong[]>(target.GetObject(MkvTagDirectory.TagChapterUids)));
        Assert.Equal(new[] { 8UL, 8UL }, Assert.IsType<ulong[]>(target.GetObject(MkvTagDirectory.TagAttachmentUids)));
        Assert.Same(target, Assert.Single(metadata.OfType<MkvSimpleTagDirectory>()).GetTargets());
        NoErrors(metadata);
    }

    [Theory]
    [CombinatorialData]
    public void Bcp47OverridesLegacyLanguageRegardlessOfFieldOrder(bool bcp47First)
    {
        var legacy = Text(Id.TagLanguage, "deu");
        var bcp47 = Text(Id.TagLanguageBcp47, "de-DE");
        var metadata = Extract(Document(Element(Id.Tags, Element(Id.Tag,
            Simple("TITLE", "English", Text(Id.TagLanguage, "eng")),
            Simple("TITLE", "Deutsch", bcp47First ? bcp47 : legacy,
                bcp47First ? legacy : bcp47, Unsigned(Id.TagDefault, 0)),
            Simple("TITLE", "Undetermined")))));

        var tags = metadata.OfType<MkvSimpleTagDirectory>().ToArray();
        Assert.Equal(new[] { "eng", "de-DE", "und" }, tags.Select(tag => tag.GetLanguage()));
        Assert.Equal("deu", tags[1].GetString(MkvSimpleTagDirectory.TagLanguage));
        Assert.Equal("de-DE", tags[1].GetString(MkvSimpleTagDirectory.TagLanguageBcp47));
        Assert.Equal("No", tags[1].GetDescription(MkvSimpleTagDirectory.TagDefault));
        Assert.Equal("Yes", tags[2].GetDescription(MkvSimpleTagDirectory.TagDefault));
        Assert.Equal(0UL, UInt(tags[1], MkvSimpleTagDirectory.TagDefault));
        Assert.Equal(1UL, UInt(tags[2], MkvSimpleTagDirectory.TagDefault));
        NoErrors(metadata);
    }

    [Fact]
    public void RetainsUtf8AndBinaryValuesWithoutConflatingTypes()
    {
        byte[] binary = [0, 1, 0xFF, 0];
        var metadata = Extract(Document(Element(Id.Tags, Element(Id.Tag,
            Simple("TITLE", "Caf\u00e9\0\0"),
            Element(Id.SimpleTag, Text(Id.TagName, "BINARY"), Element(Id.TagBinary, binary))))));

        var tags = metadata.OfType<MkvSimpleTagDirectory>().ToArray();
        Assert.Equal(2, tags.Length);
        Assert.Equal("Caf\u00e9", tags[0].GetString(MkvSimpleTagDirectory.TagString));
        Assert.False(tags[0].ContainsTag(MkvSimpleTagDirectory.TagBinary));
        Assert.Equal(binary, tags[1].GetByteArray(MkvSimpleTagDirectory.TagBinary));
        Assert.False(tags[1].ContainsTag(MkvSimpleTagDirectory.TagString));
        NoErrors(metadata);
    }

    [Fact]
    public void DistinguishesSegmentTargetsFromExplicitAllTracks()
    {
        var metadata = Extract(Document(Element(Id.Tags,
            Element(Id.Tag, Element(Id.Targets), Simple("TITLE", "Segment")),
            Element(Id.Tag, Element(Id.Targets, Element(Id.TagTrackUid)), Simple("TITLE", "All Tracks")))));

        var targets = metadata.OfType<MkvTagDirectory>().ToArray();
        Assert.Equal(2, targets.Length);
        Assert.Equal(50UL, UInt(targets[0], MkvTagDirectory.TagTargetTypeValue));
        Assert.Null(targets[0].GetObject(MkvTagDirectory.TagTrackUids));
        Assert.Equal(new[] { 0UL }, Assert.IsType<ulong[]>(targets[1].GetObject(MkvTagDirectory.TagTrackUids)));
        Assert.Equal("All Tracks", targets[1].GetDescription(MkvTagDirectory.TagTrackUids));
        NoErrors(metadata);
    }

    [Fact]
    public void KeepsLiveTagUpdatesInSeparateOrderedCollections()
    {
        var metadata = Extract(Concat(Header("webm"), Unknown(Id.Segment, 8,
            Element(Id.Tags, Element(Id.Tag, Simple("TITLE", "First"))),
            Unknown(Id.Cluster, 1, Unsigned(0xE7, 0)),
            Element(Id.Tags, Element(Id.Tag, Simple("TITLE", "Second"))))));

        var updates = metadata.OfType<MkvTagsDirectory>().ToArray();
        var values = metadata.OfType<MkvSimpleTagDirectory>().ToArray();
        Assert.Equal(2, updates.Length);
        Assert.Equal(new[] { 1, 2 }, updates.Select(update => Assert.IsType<int>(update.GetObject(MkvTagsDirectory.TagUpdateNumber))));
        Assert.Equal(new[] { "First", "Second" }, values.Select(value => value.GetString(MkvSimpleTagDirectory.TagString)));
        Assert.Same(updates[0], values[0].GetTargets()!.Parent);
        Assert.Same(updates[1], values[1].GetTargets()!.Parent);
        Assert.Same(updates[0].Parent, updates[1].Parent);
        NoErrors(metadata);
    }

    [Fact]
    public void RestartsUpdateNumbersForEachSegment()
    {
        var metadata = Extract(Concat(Header(),
            Element(Id.Segment, Element(Id.Tags, Element(Id.Tag, Simple("TITLE", "First segment")))),
            Element(Id.Segment, Element(Id.Tags, Element(Id.Tag, Simple("TITLE", "Second segment"))))));

        var updates = metadata.OfType<MkvTagsDirectory>().ToArray();
        var segments = metadata.OfType<MkvSegmentDirectory>().ToArray();
        var values = metadata.OfType<MkvSimpleTagDirectory>().ToArray();
        Assert.Equal(2, updates.Length);
        Assert.Equal(new[] { 1, 1 }, updates.Select(update => Assert.IsType<int>(update.GetObject(MkvTagsDirectory.TagUpdateNumber))));
        Assert.Same(segments[0], updates[0].Parent);
        Assert.Same(segments[1], updates[1].Parent);
        Assert.Equal(new[] { "First segment", "Second segment" }, values.Select(tag => tag.GetString(MkvSimpleTagDirectory.TagString)));
        NoErrors(metadata);
    }

    [Fact]
    public void ReportsMissingNamesAndConflictingValueTypesWithoutDiscardingValues()
    {
        var metadata = Extract(Document(Element(Id.Tags, Element(Id.Tag,
            Element(Id.SimpleTag, Text(Id.TagString, "Missing name")),
            Element(Id.SimpleTag, Text(Id.TagName, "CONFLICT"),
                Text(Id.TagString, "String"), Element(Id.TagBinary, [1]))))));

        var tags = metadata.OfType<MkvSimpleTagDirectory>().ToArray();
        Assert.Equal(2, tags.Length);
        Assert.All(tags, tag => Assert.True(tag.HasError));
        Assert.Equal("Missing name", tags[0].GetString(MkvSimpleTagDirectory.TagString));
        Assert.Equal("String", tags[1].GetString(MkvSimpleTagDirectory.TagString));
        Assert.Equal(new byte[] { 1 }, tags[1].GetByteArray(MkvSimpleTagDirectory.TagBinary));
    }

    [Fact]
    public void ReadsDeepNestingAndResolvesTargetsWithoutLosingAncestors()
    {
        var nested = Simple("LEAF", "Value");
        for (int i = 1; i < 512; i++)
            nested = Element(Id.SimpleTag, Text(Id.TagName, "PARENT"), nested);

        var metadata = Extract(Document(Element(Id.Tags, Element(Id.Tag, nested))));

        var tags = metadata.OfType<MkvSimpleTagDirectory>().ToArray();
        Assert.Equal(512, tags.Length);
        Assert.Equal("LEAF", tags[511].GetString(MkvSimpleTagDirectory.TagName));
        Assert.Equal("Value", tags[511].GetString(MkvSimpleTagDirectory.TagString));
        Directory? parent = tags[511];
        int depth = 0;
        while (parent is MkvSimpleTagDirectory)
        {
            depth++;
            parent = parent.Parent;
        }
        Assert.Equal(512, depth);
        Assert.Same(Assert.Single(metadata.OfType<MkvTagDirectory>()), parent);
        Assert.Same(parent, tags[511].GetTargets());
        NoErrors(metadata);
    }
}
