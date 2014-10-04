/*
 * Copyright 2002-2013 Drew Noakes
 *
 *    Licensed under the Apache License, Version 2.0 (the "License");
 *    you may not use this file except in compliance with the License.
 *    You may obtain a copy of the License at
 *
 *        http://www.apache.org/licenses/LICENSE-2.0
 *
 *    Unless required by applicable law or agreed to in writing, software
 *    distributed under the License is distributed on an "AS IS" BASIS,
 *    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 *    See the License for the specific language governing permissions and
 *    limitations under the License.
 *
 * More information about this project is available at:
 *
 *    http://drewnoakes.com/code/exif/
 *    http://code.google.com/p/metadata-extractor/
 */
using System.Collections.Generic;
using Com.Adobe.Xmp;
using Com.Drew.Metadata.Xmp;
using JetBrains.Annotations;
using Sharpen;

namespace Com.Drew.Metadata.Xmp
{
	/// <author>Torsten Skadell, Drew Noakes http://drewnoakes.com</author>
	public class XmpDirectory : Com.Drew.Metadata.Directory
	{
		public const int TagMake = unchecked((int)(0x0001));

		public const int TagModel = unchecked((int)(0x0002));

		public const int TagExposureTime = unchecked((int)(0x0003));

		public const int TagShutterSpeed = unchecked((int)(0x0004));

		public const int TagFNumber = unchecked((int)(0x0005));

		public const int TagLensInfo = unchecked((int)(0x0006));

		public const int TagLens = unchecked((int)(0x0007));

		public const int TagCameraSerialNumber = unchecked((int)(0x0008));

		public const int TagFirmware = unchecked((int)(0x0009));

		public const int TagFocalLength = unchecked((int)(0x000a));

		public const int TagApertureValue = unchecked((int)(0x000b));

		public const int TagExposureProgram = unchecked((int)(0x000c));

		public const int TagDatetimeOriginal = unchecked((int)(0x000d));

		public const int TagDatetimeDigitized = unchecked((int)(0x000e));

		/// <summary>A value from 0 to 5, or -1 if the image is rejected.</summary>
		public const int TagRating = unchecked((int)(0x1001));

		[NotNull]
		protected internal static readonly Dictionary<int, string> _tagNameMap = new Dictionary<int, string>();

		[NotNull]
		private readonly IDictionary<string, string> _propertyValueByPath = new Dictionary<string, string>();

		static XmpDirectory()
		{
			// These are some Tags, belonging to xmp-data-tags
			// The numeration is more like enums. The real xmp-tags are strings,
			// so we do some kind of mapping here...
			/*
    // dublin core properties
    // this requires further research
    public static int TAG_TITLE = 0x100;
    public static int TAG_SUBJECT = 0x1001;
    public static int TAG_DATE = 0x1002;
    public static int TAG_TYPE = 0x1003;
    public static int TAG_DESCRIPTION = 0x1004;
    public static int TAG_RELATION = 0x1005;
    public static int TAG_COVERAGE = 0x1006;
    public static int TAG_CREATOR = 0x1007;
    public static int TAG_PUBLISHER = 0x1008;
    public static int TAG_CONTRIBUTOR = 0x1009;
    public static int TAG_RIGHTS = 0x100A;
    public static int TAG_FORMAT = 0x100B;
    public static int TAG_IDENTIFIER = 0x100C;
    public static int TAG_LANGUAGE = 0x100D;
    public static int TAG_AUDIENCE = 0x100E;
    public static int TAG_PROVENANCE = 0x100F;
    public static int TAG_RIGHTS_HOLDER = 0x1010;
    public static int TAG_INSTRUCTIONAL_METHOD = 0x1011;
    public static int TAG_ACCRUAL_METHOD = 0x1012;
    public static int TAG_ACCRUAL_PERIODICITY = 0x1013;
    public static int TAG_ACCRUAL_POLICY = 0x1014;
*/
			_tagNameMap.Put(TagMake, "Make");
			_tagNameMap.Put(TagModel, "Model");
			_tagNameMap.Put(TagExposureTime, "Exposure Time");
			_tagNameMap.Put(TagShutterSpeed, "Shutter Speed Value");
			_tagNameMap.Put(TagFNumber, "F-Number");
			_tagNameMap.Put(TagLensInfo, "Lens Information");
			_tagNameMap.Put(TagLens, "Lens");
			_tagNameMap.Put(TagCameraSerialNumber, "Serial Number");
			_tagNameMap.Put(TagFirmware, "Firmware");
			_tagNameMap.Put(TagFocalLength, "Focal Length");
			_tagNameMap.Put(TagApertureValue, "Aperture Value");
			_tagNameMap.Put(TagExposureProgram, "Exposure Program");
			_tagNameMap.Put(TagDatetimeOriginal, "Date/Time Original");
			_tagNameMap.Put(TagDatetimeDigitized, "Date/Time Digitized");
			_tagNameMap.Put(TagRating, "Rating");
		}

		[CanBeNull]
		private XMPMeta _xmpMeta;

		public XmpDirectory()
		{
			/*
        // this requires further research
        _tagNameMap.put(TAG_TITLE, "Title");
        _tagNameMap.put(TAG_SUBJECT, "Subject");
        _tagNameMap.put(TAG_DATE, "Date");
        _tagNameMap.put(TAG_TYPE, "Type");
        _tagNameMap.put(TAG_DESCRIPTION, "Description");
        _tagNameMap.put(TAG_RELATION, "Relation");
        _tagNameMap.put(TAG_COVERAGE, "Coverage");
        _tagNameMap.put(TAG_CREATOR, "Creator");
        _tagNameMap.put(TAG_PUBLISHER, "Publisher");
        _tagNameMap.put(TAG_CONTRIBUTOR, "Contributor");
        _tagNameMap.put(TAG_RIGHTS, "Rights");
        _tagNameMap.put(TAG_FORMAT, "Format");
        _tagNameMap.put(TAG_IDENTIFIER, "Identifier");
        _tagNameMap.put(TAG_LANGUAGE, "Language");
        _tagNameMap.put(TAG_AUDIENCE, "Audience");
        _tagNameMap.put(TAG_PROVENANCE, "Provenance");
        _tagNameMap.put(TAG_RIGHTS_HOLDER, "Rights Holder");
        _tagNameMap.put(TAG_INSTRUCTIONAL_METHOD, "Instructional Method");
        _tagNameMap.put(TAG_ACCRUAL_METHOD, "Accrual Method");
        _tagNameMap.put(TAG_ACCRUAL_PERIODICITY, "Accrual Periodicity");
        _tagNameMap.put(TAG_ACCRUAL_POLICY, "Accrual Policy");
*/
			this.SetDescriptor(new XmpDescriptor(this));
		}

		[NotNull]
		public override string GetName()
		{
			return "Xmp";
		}

		[NotNull]
		protected internal override Dictionary<int, string> GetTagNameMap()
		{
			return _tagNameMap;
		}

		internal virtual void AddProperty(string path, string value)
		{
			_propertyValueByPath.Put(path, value);
		}

		/// <summary>Gets a map of all XMP properties in this directory, not just the known ones.</summary>
		/// <remarks>
		/// Gets a map of all XMP properties in this directory, not just the known ones.
		/// <p/>
		/// This is required because XMP properties are represented as strings, whereas the rest of this library
		/// uses integers for keys.
		/// </remarks>
		[NotNull]
		public virtual IDictionary<string, string> GetXmpProperties()
		{
			return _propertyValueByPath;
		}

		public virtual void SetXMPMeta(XMPMeta xmpMeta)
		{
			_xmpMeta = xmpMeta;
		}

		/// <summary>Gets the XMPMeta object used to populate this directory.</summary>
		/// <remarks>Gets the XMPMeta object used to populate this directory.  It can be used for more XMP-oriented operations.</remarks>
		[CanBeNull]
		public virtual XMPMeta GetXMPMeta()
		{
			return _xmpMeta;
		}
	}
}
