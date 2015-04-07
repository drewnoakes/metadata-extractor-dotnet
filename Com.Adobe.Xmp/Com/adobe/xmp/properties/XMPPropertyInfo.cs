// =================================================================================================
// ADOBE SYSTEMS INCORPORATED
// Copyright 2006 Adobe Systems Incorporated
// All Rights Reserved
//
// NOTICE:  Adobe permits you to use, modify, and distribute this file in accordance with the terms
// of the Adobe license agreement accompanying it.
// =================================================================================================
using Com.Adobe.Xmp.Options;
using Sharpen;

namespace Com.Adobe.Xmp.Properties
{
	/// <summary>This interface is used to return a property together with its path and namespace.</summary>
	/// <remarks>
	/// This interface is used to return a property together with its path and namespace.
	/// It is returned when properties are iterated with the <code>XMPIterator</code>.
	/// </remarks>
	/// <since>06.07.2006</since>
	public interface XMPPropertyInfo : XMPProperty
	{
		/// <returns>Returns the namespace of the property</returns>
		string GetNamespace();

		/// <returns>Returns the path of the property, but only if returned by the iterator.</returns>
		string GetPath();

		/// <returns>Returns the value of the property.</returns>
		string GetValue();

		/// <returns>Returns the options of the property.</returns>
		PropertyOptions GetOptions();
	}
}
