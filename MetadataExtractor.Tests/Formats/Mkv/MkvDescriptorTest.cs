// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using static MetadataExtractor.Formats.Mkv.EbmlTestUtil;

namespace MetadataExtractor.Formats.Mkv;

public sealed class MkvDescriptorTest
{
    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void DerivesDurationWithNonDefaultScaleRegardlessOfFieldOrder(bool scaleFirst)
    {
        var scale = Unsigned(Id.TimestampScale, 2000000);
        var duration = Floating(Id.Duration, 5250);
        var metadata = Extract(Document(Element(Id.Info, scaleFirst ? scale : duration, scaleFirst ? duration : scale)));

        var segment = Assert.Single(metadata.OfType<MkvSegmentDirectory>());
        var descriptor = new MkvSegmentDescriptor(segment);
        Assert.Equal(10.5, segment.GetDouble(MkvSegmentDirectory.TagDurationSeconds));
        Assert.Equal(5250d, segment.GetDouble(MkvSegmentDirectory.TagDuration));
        Assert.Equal("10.5 seconds", segment.GetDescription(MkvSegmentDirectory.TagDurationSeconds));
        Assert.Equal("10.5 seconds", descriptor.GetDurationSecondsDescription());
        Assert.Equal("5250 segment ticks", descriptor.GetDurationDescription());
        Assert.Equal("2000000 ns", descriptor.GetTimestampScaleDescription());
        NoErrors(metadata);
    }

    [Fact]
    public void DerivesNominalFrameRateWithoutChangingNanosecondDuration()
    {
        var metadata = Extract(Document(Element(Id.Tracks, Element(Id.TrackEntry,
            Unsigned(Id.TrackType, 1), Unsigned(Id.DefaultDuration, 40000000), Element(Id.Video)))));

        var video = Assert.Single(metadata.OfType<MkvVideoDirectory>());
        var descriptor = new MkvVideoDescriptor(video);
        Assert.Equal(25d, video.GetDouble(MkvVideoDirectory.TagFrameRate));
        Assert.Equal(40000000UL, UInt(video, MkvTrackDirectory.TagDefaultDuration));
        Assert.Equal("25 fps", video.GetDescription(MkvVideoDirectory.TagFrameRate));
        Assert.Equal("25 fps", descriptor.GetFrameRateDescription());
        Assert.Equal("40000000 ns", descriptor.GetDefaultDurationDescription());
        NoErrors(metadata);
    }

    [Fact]
    public void ZeroDefaultDurationDoesNotProduceInfiniteFrameRate()
    {
        var metadata = Extract(Document(Element(Id.Tracks, Element(Id.TrackEntry,
            Unsigned(Id.TrackType, 1), Unsigned(Id.DefaultDuration, 0), Element(Id.Video)))));

        var video = Assert.Single(metadata.OfType<MkvVideoDirectory>());
        Assert.Equal(0UL, UInt(video, MkvTrackDirectory.TagDefaultDuration));
        Assert.False(video.ContainsTag(MkvVideoDirectory.TagFrameRate));
        NoErrors(metadata);
    }

    [Theory]
    [InlineData(MkvVideoDirectory.TagFlagInterlaced, 0, "Undetermined")]
    [InlineData(MkvVideoDirectory.TagFlagInterlaced, 1, "Interlaced")]
    [InlineData(MkvVideoDirectory.TagFlagInterlaced, 2, "Progressive")]
    [InlineData(MkvVideoDirectory.TagFlagInterlaced, 3, "Unknown (3)")]
    [InlineData(MkvVideoDirectory.TagDisplayUnit, 0, "Pixels")]
    [InlineData(MkvVideoDirectory.TagDisplayUnit, 3, "Display Aspect Ratio")]
    [InlineData(MkvVideoDirectory.TagDisplayUnit, 4, "Unknown")]
    [InlineData(MkvVideoDirectory.TagRange, 0, "Unspecified")]
    [InlineData(MkvVideoDirectory.TagRange, 1, "Broadcast Range")]
    [InlineData(MkvVideoDirectory.TagRange, 2, "Full Range")]
    [InlineData(MkvVideoDirectory.TagChromaSitingHorz, 1, "Left")]
    [InlineData(MkvVideoDirectory.TagChromaSitingHorz, 2, "Half")]
    [InlineData(MkvVideoDirectory.TagChromaSitingVert, 1, "Top")]
    [InlineData(MkvVideoDirectory.TagChromaSitingVert, 2, "Half")]
    [InlineData(MkvVideoDirectory.TagMatrixCoefficients, 0, "Identity")]
    [InlineData(MkvVideoDirectory.TagMatrixCoefficients, 1, "BT.709")]
    [InlineData(MkvVideoDirectory.TagMatrixCoefficients, 3, "Unknown (3)")]
    [InlineData(MkvVideoDirectory.TagMatrixCoefficients, 14, "ICtCp")]
    [InlineData(MkvVideoDirectory.TagTransferCharacteristics, 0, "Unknown (0)")]
    [InlineData(MkvVideoDirectory.TagTransferCharacteristics, 1, "BT.709")]
    [InlineData(MkvVideoDirectory.TagTransferCharacteristics, 3, "Unknown (3)")]
    [InlineData(MkvVideoDirectory.TagTransferCharacteristics, 16, "SMPTE 2084 (PQ)")]
    [InlineData(MkvVideoDirectory.TagTransferCharacteristics, 18, "ARIB STD-B67 (HLG)")]
    [InlineData(MkvVideoDirectory.TagPrimaries, 1, "BT.709")]
    [InlineData(MkvVideoDirectory.TagPrimaries, 3, "Unknown (3)")]
    [InlineData(MkvVideoDirectory.TagPrimaries, 9, "BT.2020")]
    [InlineData(MkvVideoDirectory.TagPrimaries, 22, "EBU Tech. 3213-E")]
    [InlineData(MkvVideoDirectory.TagPrimaries, 255, "Unknown (255)")]
    public void DescribesVideoEnumsThroughDispatcherAndNamedMethod(int tag, int value, string expected)
    {
        var directory = new MkvVideoDirectory();
        directory.Set(tag, (ulong)value);
        var descriptor = new MkvVideoDescriptor(directory);

        Assert.Equal(expected, directory.GetDescription(tag));
        Assert.Equal(expected, tag switch
        {
            MkvVideoDirectory.TagFlagInterlaced => descriptor.GetFlagInterlacedDescription(),
            MkvVideoDirectory.TagDisplayUnit => descriptor.GetDisplayUnitDescription(),
            MkvVideoDirectory.TagRange => descriptor.GetRangeDescription(),
            MkvVideoDirectory.TagChromaSitingHorz => descriptor.GetChromaSitingHorzDescription(),
            MkvVideoDirectory.TagChromaSitingVert => descriptor.GetChromaSitingVertDescription(),
            MkvVideoDirectory.TagMatrixCoefficients => descriptor.GetMatrixCoefficientsDescription(),
            MkvVideoDirectory.TagTransferCharacteristics => descriptor.GetTransferCharacteristicsDescription(),
            MkvVideoDirectory.TagPrimaries => descriptor.GetPrimariesDescription(),
            _ => throw new InvalidOperationException()
        });
    }

    [Theory]
    [InlineData(1, "Video")]
    [InlineData(2, "Audio")]
    [InlineData(3, "Complex")]
    [InlineData(16, "Logo")]
    [InlineData(17, "Subtitle")]
    [InlineData(18, "Buttons")]
    [InlineData(32, "Control")]
    [InlineData(33, "Metadata")]
    [InlineData(255, "Unknown (255)")]
    public void DescribesTrackTypes(int value, string expected)
    {
        var directory = new MkvTrackDirectory();
        directory.Set(MkvTrackDirectory.TagTrackType, (ulong)value);
        var descriptor = new MkvTrackDescriptor<MkvTrackDirectory>(directory);

        Assert.Equal(expected, descriptor.GetTrackTypeDescription());
        Assert.Equal(expected, directory.GetDescription(MkvTrackDirectory.TagTrackType));
    }

    [Fact]
    public void DescribesAudioUnitsAndInheritedTrackFields()
    {
        var audio = new MkvAudioDirectory();
        audio.Set(MkvAudioDirectory.TagSamplingFrequency, 48000d);
        audio.Set(MkvAudioDirectory.TagBitDepth, 24UL);
        audio.Set(MkvTrackDirectory.TagCodecDelay, 6500000UL);
        audio.Set(MkvTrackDirectory.TagSeekPreRoll, 80000000UL);
        audio.Set(MkvTrackDirectory.TagFlagLacing, 0UL);
        var descriptor = new MkvAudioDescriptor(audio);

        Assert.Equal("48000 Hz", descriptor.GetSamplingFrequencyDescription());
        Assert.Equal("48000 Hz", audio.GetDescription(MkvAudioDirectory.TagSamplingFrequency));
        Assert.Equal("24 bits", descriptor.GetBitDepthDescription());
        Assert.Equal("24 bits", audio.GetDescription(MkvAudioDirectory.TagBitDepth));
        Assert.Equal("6500000 ns", descriptor.GetCodecDelayDescription());
        Assert.Equal("6500000 ns", audio.GetDescription(MkvTrackDirectory.TagCodecDelay));
        Assert.Equal("80000000 ns", descriptor.GetSeekPreRollDescription());
        Assert.Equal("80000000 ns", audio.GetDescription(MkvTrackDirectory.TagSeekPreRoll));
        Assert.Equal("No", descriptor.GetFlagLacingDescription());
        audio.Set(MkvTrackDirectory.TagFlagLacing, 1UL);
        Assert.Equal("Yes", descriptor.GetFlagLacingDescription());
    }

    [Theory]
    [InlineData(false, "00010203-0405-0607-0809-0a0b0c0d0e0f")]
    [InlineData(true, "80010203-8405-8607-8809-0a0b0c0d0e0f")]
    public void DescribesUuidInNetworkOrderWithoutRewritingItsBits(bool highBits, string expected)
    {
        var uuid = Enumerable.Range(0, 16).Select(i => (byte)i).ToArray();
        if (highBits)
        {
            uuid[0] |= 0x80;
            uuid[4] |= 0x80;
            uuid[6] |= 0x80;
            uuid[8] |= 0x80;
        }
        var metadata = Extract(Document(Element(Id.Info, Element(Id.SegmentUuid, uuid))));
        var segment = Assert.Single(metadata.OfType<MkvSegmentDirectory>());

        Assert.Equal(expected, segment.GetDescription(MkvSegmentDirectory.TagSegmentUuid));
        Assert.Equal(expected, new MkvSegmentDescriptor(segment).GetSegmentUuidDescription());
        Assert.Equal(uuid, segment.GetByteArray(MkvSegmentDirectory.TagSegmentUuid));
        NoErrors(metadata);
    }

    [Theory]
    [InlineData(10, "Shot")]
    [InlineData(20, "Subtrack / Movement / Scene")]
    [InlineData(30, "Track / Song / Chapter")]
    [InlineData(40, "Part / Session")]
    [InlineData(50, "Album / Opera / Concert / Movie / Episode")]
    [InlineData(60, "Edition / Issue / Volume / Opus / Season / Sequel")]
    [InlineData(70, "Collection")]
    [InlineData(80, "80")]
    public void DescribesTargetTypes(int value, string expected)
    {
        var directory = new MkvTagDirectory();
        directory.Set(MkvTagDirectory.TagTargetTypeValue, (ulong)value);

        Assert.Equal(expected, directory.GetDescription(MkvTagDirectory.TagTargetTypeValue));
        Assert.Equal(expected, new MkvTagDescriptor(directory).GetTargetTypeValueDescription());
    }

    [Fact]
    public void DescribesTargetListsWithoutLosingUnsignedPrecisionOrOrder()
    {
        var target = new MkvTagDirectory();
        target.Set(MkvTagDirectory.TagTrackUids, new[] { 1UL, 0UL, ulong.MaxValue, 1UL });
        target.Set(MkvTagDirectory.TagEditionUids, new[] { 0UL, 9UL });
        target.Set(MkvTagDirectory.TagChapterUids, new[] { 7UL, 0UL });
        target.Set(MkvTagDirectory.TagAttachmentUids, new[] { 0UL, 8UL });
        var descriptor = new MkvTagDescriptor(target);

        Assert.Equal("1, All Tracks, 18446744073709551615, 1", descriptor.GetTrackUidsDescription());
        Assert.Equal("1, All Tracks, 18446744073709551615, 1", target.GetDescription(MkvTagDirectory.TagTrackUids));
        Assert.Equal("All Editions, 9", descriptor.GetEditionUidsDescription());
        Assert.Equal("7, All Chapters", descriptor.GetChapterUidsDescription());
        Assert.Equal("All Attachments, 8", descriptor.GetAttachmentUidsDescription());
    }

    [Fact]
    public void MissingDescriptorValuesReturnNull()
    {
        var directory = new MkvVideoDirectory();
        var video = new MkvVideoDescriptor(directory);
        var segment = new MkvSegmentDescriptor(new MkvSegmentDirectory());
        var audio = new MkvAudioDescriptor(new MkvAudioDirectory());
        var target = new MkvTagDescriptor(new MkvTagDirectory());

        Assert.Null(video.GetPrimariesDescription());
        Assert.Null(video.GetTrackTypeDescription());
        Assert.Null(video.GetFrameRateDescription());
        Assert.Null(segment.GetSegmentUuidDescription());
        Assert.Null(segment.GetDurationSecondsDescription());
        Assert.Null(audio.GetSamplingFrequencyDescription());
        Assert.Null(target.GetTargetTypeValueDescription());
        Assert.Null(target.GetTrackUidsDescription());

        directory.Set(MkvVideoDirectory.TagPrimaries, 22UL);
        Assert.Equal("EBU Tech. 3213-E", video.GetPrimariesDescription());
        directory.Set(MkvTrackDirectory.TagTrackType, 1UL);
        Assert.Equal("Video", video.GetTrackTypeDescription());
    }
}
