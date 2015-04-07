// =================================================================================================
// ADOBE SYSTEMS INCORPORATED
// Copyright 2006 Adobe Systems Incorporated
// All Rights Reserved
//
// NOTICE:  Adobe permits you to use, modify, and distribute this file in accordance with the terms
// of the Adobe license agreement accompanying it.
// =================================================================================================
using Sharpen;

namespace Com.Adobe.Xmp
{
	/// <since>21.09.2006</since>
	public interface XMPError
	{
	}

	public static class XMPErrorConstants
	{
		public const int Unknown = 0;

		public const int Badparam = 4;

		public const int Badvalue = 5;

		public const int Internalfailure = 9;

		public const int Badschema = 101;

		public const int Badxpath = 102;

		public const int Badoptions = 103;

		public const int Badindex = 104;

		public const int Badserialize = 107;

		public const int Badxml = 201;

		public const int Badrdf = 202;

		public const int Badxmp = 203;

		/// <summary><em>Note:</em> This is an error code introduced by Java.</summary>
		public const int Badstream = 204;
	}
}
