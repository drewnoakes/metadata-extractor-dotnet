// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace MetadataExtractor.Formats.GeoTiff;

public sealed class GeoTiffDirectory : Directory
{
#pragma warning disable format

    public const int TagModelType                    = 0x0400;
    public const int TagRasterType                   = 0x0401;
    public const int TagCitation                     = 0x0402;

    public const int TagGeographicType               = 0x0800;
    public const int TagGeographicCitation           = 0x0801;
    public const int TagGeodeticDatum                = 0x0802;
    public const int TagGeographicPrimeMeridian      = 0x0803;
    public const int TagGeographicLinearUnits        = 0x0804;
    public const int TagGeographicLinearUnitSize     = 0x0805;
    public const int TagGeographicAngularUnits       = 0x0806;
    public const int TagGeographicAngularUnitSize    = 0x0807;
    public const int TagGeographicEllipsoid          = 0x0808;
    public const int TagGeographicSemiMajorAxis      = 0x0809;
    public const int TagGeographicSemiMinorAxis      = 0x080a;
    public const int TagGeographicInvFlattening      = 0x080b;
    public const int TagGeographicAzimuthUnits       = 0x080c;
    public const int TagGeographicPrimeMeridianLong  = 0x080d;
    public const int TagGeographicToWgs84            = 0x080e;

    public const int TagProjectedCSType              = 0x0c00;
    public const int TagProjectedCSCitation          = 0x0c01;
    public const int TagProjection                   = 0x0c02;
    public const int TagProjectedCoordinateTransform = 0x0c03;
    public const int TagProjLinearUnits              = 0x0c04;
    public const int TagProjLinearUnitSize           = 0x0c05;
    public const int TagProjStdParallel1             = 0x0c06;
    public const int TagProjStdParallel2             = 0x0c07;
    public const int TagProjNatOriginLong            = 0x0c08;
    public const int TagProjNatOriginLat             = 0x0c09;
    public const int TagProjFalseEasting             = 0x0c0a;
    public const int TagProjFalseNorthing            = 0x0c0b;
    public const int TagProjFalseOriginLong          = 0x0c0c;
    public const int TagProjFalseOriginLat           = 0x0c0d;
    public const int TagProjFalseOriginEasting       = 0x0c0e;
    public const int TagProjFalseOriginNorthing      = 0x0c0f;
    public const int TagProjCenterLong               = 0x0c10;
    public const int TagProjCenterLat                = 0x0c11;
    public const int TagProjCenterEasting            = 0x0c12;
    public const int TagProjCenterNorthing           = 0x0c13;
    public const int TagProjScaleAtNatOrigin         = 0x0c14;
    public const int TagProjScaleAtCenter            = 0x0c15;
    public const int TagProjAzimuthAngle             = 0x0c16;
    public const int TagProjStraightVertPoleLong     = 0x0c17;
    public const int TagProjRectifiedGridAngle       = 0x0c18;

    public const int TagVerticalCSType               = 0x1000;
    public const int TagVerticalCitation             = 0x1001;
    public const int TagVerticalDatum                = 0x1002;
    public const int TagVerticalUnits                = 0x1003;

    public const int TagChartFormat                  = 0xb799;
    public const int TagChartSource                  = 0xb79a;
    public const int TagChartSourceEdition           = 0xb79b;
    public const int TagChartSourceDate              = 0xb79c;
    public const int TagChartCorrDate                = 0xb79d;
    public const int TagChartCountryOrigin           = 0xb79e;
    public const int TagChartRasterEdition           = 0xb79f;
    public const int TagChartSoundingDatum           = 0xb7a0;
    public const int TagChartDepthUnits              = 0xb7a1;
    public const int TagChartMagVar                  = 0xb7a2;
    public const int TagChartMagVarYear              = 0xb7a3;
    public const int TagChartMagVarAnnChange         = 0xb7a4;
    public const int TagChartWGSNSShift              = 0xb7a5;
    public const int TagInsetNWPixelX                = 0xb7a7;
    public const int TagInsetNWPixelY                = 0xb7a8;
    public const int TagChartContourInterval         = 0xb7a9;

    private static readonly Dictionary<int, string> _tagNameMap = new()
    {
        { TagModelType, "Model Type" },
        { TagRasterType, "Raster Type" },
        { TagCitation, "Citation" },
        { TagGeographicType, "Geographic Type" },
        { TagGeographicCitation, "Geographic Citation" },
        { TagGeodeticDatum, "Geodetic Datum" },
        { TagGeographicPrimeMeridian, "Prime Meridian" },
        { TagGeographicLinearUnits, "Geographic Linear Units" },
        { TagGeographicLinearUnitSize, "Geographic Linear Unit Size" },
        { TagGeographicAngularUnits, "Geographic Angular Units" },
        { TagGeographicAngularUnitSize, "Geographic Angular Unit Size" },
        { TagGeographicEllipsoid, "Geographic Ellipsoid" },
        { TagGeographicSemiMajorAxis, "Semi-major axis" },
        { TagGeographicSemiMinorAxis, "Semi-minor axis" },
        { TagGeographicInvFlattening, "Inv. Flattening" },
        { TagGeographicAzimuthUnits, "Azimuth Units" },
        { TagGeographicPrimeMeridianLong, "To WGS84" },
        { TagGeographicToWgs84, "To WGS84" },
        { TagProjectedCSType, "Projected Coordinate System Type" },
        { TagProjectedCSCitation, "Projected Coordinate System Citation" },
        { TagProjection, "Projection" },
        { TagProjectedCoordinateTransform, "Projected Coordinate Transform" },
        { TagProjLinearUnits, "Projection Linear Units" },
        { TagProjLinearUnitSize, "Projection Linear Unit Size" },
        { TagProjStdParallel1, "Projection Standard Parallel 1" },
        { TagProjStdParallel2, "Projection Standard Parallel 2" },
        { TagProjNatOriginLong, "Projection Natural Origin Longitude" },
        { TagProjNatOriginLat, "Projection Natural Origin Latitude" },
        { TagProjFalseEasting, "Projection False Easting" },
        { TagProjFalseNorthing, "Projection False Northing" },
        { TagProjFalseOriginLong, "Projection False Origin Longitude" },
        { TagProjFalseOriginLat, "Projection False Origin Latitude" },
        { TagProjFalseOriginEasting, "Projection False Origin Easting" },
        { TagProjFalseOriginNorthing, "Projection False Origin Northing" },
        { TagProjCenterLong, "Projection Center Longitude" },
        { TagProjCenterLat, "Projection Center Latitude" },
        { TagProjCenterEasting, "Projection Center Easting" },
        { TagProjCenterNorthing, "Projection Center Northing" },
        { TagProjScaleAtNatOrigin, "Projection Scale at Natural Origin" },
        { TagProjScaleAtCenter, "Projection Scale at Center" },
        { TagProjAzimuthAngle, "Projection Azimuth Angle" },
        { TagProjStraightVertPoleLong, "Projection Straight Vertical Pole Longitude" },
        { TagProjRectifiedGridAngle, "Projection Straight Vertical Pole Latitude" },
        { TagVerticalCSType, "Vertical Coordinate System Type" },
        { TagVerticalCitation, "Vertical Citation" },
        { TagVerticalDatum, "Vertical Datum" },
        { TagVerticalUnits, "Vertical Units" },
        { TagChartFormat, "Chart Format" },
        { TagChartSource, "Chart Source" },
        { TagChartSourceEdition, "Chart Source Edition" },
        { TagChartSourceDate, "Chart Source Date" },
        { TagChartCorrDate, "Chart Corr Date" },
        { TagChartCountryOrigin, "Chart Country Origin" },
        { TagChartRasterEdition, "Chart Raster Edition" },
        { TagChartSoundingDatum, "Chart Sounding Datum" },
        { TagChartDepthUnits, "Chart Depth Units" },
        { TagChartMagVar, "Chart Mag Var" },
        { TagChartMagVarYear, "Chart Mag Var Year" },
        { TagChartMagVarAnnChange, "Chart Mag Var Annual Change" },
        { TagChartWGSNSShift, "Chart WGSNS Shift" },
        { TagInsetNWPixelX, "Inset NW Pixel X" },
        { TagInsetNWPixelY, "Inset NW Pixel Y" },
        { TagChartContourInterval, "Chart Contour Interval" }
    };

    public GeoTiffDirectory() : base(_tagNameMap)
    {
        SetDescriptor(new GeoTiffDescriptor(this));
    }

    public override string Name => "GeoTIFF";
}
