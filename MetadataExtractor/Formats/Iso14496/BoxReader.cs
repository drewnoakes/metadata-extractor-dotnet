// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using MetadataExtractor.Formats.Iso14496.Boxes;

namespace MetadataExtractor.Formats.Iso14496;

/// <summary>
/// Reads ISO base media boxes, allowing at most 100 levels of recursively parsed containers.
/// </summary>
internal sealed class BoxReader(Action<string> errorHandler)
{
    private const int MaxBoxDepth = 100;

    private int _depth;

    public List<Box> ReadBoxes(SequentialReader reader)
    {
        var boxes = new List<Box>();

        Box? item;
        while ((item = ReadBox(reader)) is not null)
        {
            boxes.Add(item);
        }

        return boxes;
    }

    public List<Box> ReadBoxes(SequentialReader sr, BoxLocation loc)
    {
        var ret = new List<Box>();
        while (sr.IsWithinBox(loc))
        {
            var box = ReadBox(sr, loc);
            if (box is null)
                break;
            ret.Add(box);
        }

        return ret;
    }

    private Box? ReadBox(SequentialReader sr, BoxLocation? parent = null)
    {
        return ReadBox(sr, ParseBox, parent);

        Box ParseBox(BoxLocation location, SequentialReader sr)
        {
            return location.Type switch
            {
                BoxTypes.FTypTag => new FileTypeBox(location, sr),
                BoxTypes.MetaTag => ReadContainer(location, sr, static (l, r, b) => new MetaBox(l, r, b)),
                BoxTypes.HdlrTag => new HandlerBox(location, sr),
                BoxTypes.DinfTag => ReadContainer(location, sr, static (l, r, b) => new DataInformationBox(l, r, b)),
                BoxTypes.DrefTag => ReadContainer(location, sr, static (l, r, b) => new DataReferenceBox(l, r, b)),
                BoxTypes.UrlTag => new DataEntryLocationBox(location, sr, hasName: false),
                BoxTypes.UrnTag => new DataEntryLocationBox(location, sr, hasName: true),
                BoxTypes.PitmTag => new PrimaryItemBox(location, sr),
                BoxTypes.Iinftag => ReadContainer(location, sr, static (l, r, b) => new ItemInformationBox(l, r, b)),
                BoxTypes.InfeTag => new ItemInfoEntryBox(location, sr),
                BoxTypes.IrefTag => new ItemReferenceBox(location, sr),
                BoxTypes.IprpTag => ReadContainer(location, sr, static (l, r, b) => new ItemPropertyBox(l, r, b)),
                BoxTypes.IpcoTag => ReadContainer(location, sr, static (l, r, b) => new ItemPropertyContainerBox(l, r, b)),
                BoxTypes.IspeTag => new ImageSpatialExtentsBox(location, sr),
                BoxTypes.HvcCTag => new DecoderConfigurationBox(location, sr),
                BoxTypes.ColrTag => new ColorInformationBox(location, sr),
                BoxTypes.IrotTag => new ImageRotationBox(location, sr),
                BoxTypes.PixiTag => new PixelInformationBox(location, sr),
                BoxTypes.IdatTag => new ItemDataBox(location, sr),
                BoxTypes.IlocTag => new ItemLocationBox(location, sr),
                BoxTypes.IpmaTag => new ItemPropertyAssociationBox(location, sr),
                _ => new Box(location)
            };
        }
    }

    private Box ReadContainer(BoxLocation location, SequentialReader reader, Func<BoxLocation, SequentialReader, BoxReader, Box> parseBox)
    {
        if (_depth >= MaxBoxDepth)
        {
            errorHandler("Box nesting too deep.");
            // ReadBox skips the payload using the absolute end, including extended-size headers.
            return new Box(location);
        }

        _depth++;
        try
        {
            return parseBox(location, reader, this);
        }
        finally
        {
            _depth--;
        }
    }

    internal static Box? ReadBox(SequentialReader sr, Func<BoxLocation, SequentialReader, Box> parseBox, BoxLocation? parent = null)
    {
        if (parent is not null && parent.NextPosition - (ulong)sr.Position < 8)
            throw new ImageProcessingException($"Incomplete box header within {parent.TypeString}.");

        if (sr.Available() < 8)
            return null;

        var location = new BoxLocation(sr);

        if (parent is not null && location.NextPosition > parent.NextPosition)
            throw new ImageProcessingException($"Box with type {location.TypeString} extends beyond its parent.");

        var available = sr is SequentialStreamReader streamReader ? streamReader.AvailableBytes : sr.Available();
        if ((long)location.NextPosition - sr.Position > available)
            throw new ImageProcessingException($"Box with type {location.TypeString} extends beyond the end of the stream.");

        var box = parseBox(location, sr);

        var unreadBytes = (long)box.NextPosition - sr.Position;

        if (unreadBytes < 0)
        {
            throw new ImageProcessingException($"Reader for box with type {location.TypeString} read beyond end of allocated space.");
        }

        if (unreadBytes > 0)
        {
            sr.Skip(unreadBytes);
        }

        return box;
    }
}
