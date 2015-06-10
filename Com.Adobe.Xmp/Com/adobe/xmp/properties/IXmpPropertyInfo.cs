// =================================================================================================
// ADOBE SYSTEMS INCORPORATED
// Copyright 2006 Adobe Systems Incorporated
// All Rights Reserved
//
// NOTICE:  Adobe permits you to use, modify, and distribute this file in accordance with the terms
// of the Adobe license agreement accompanying it.
// =================================================================================================

namespace Com.Adobe.Xmp.Properties
{
    /// <summary>This interface is used to return a property together with its path and namespace.</summary>
    /// <remarks>
    /// This interface is used to return a property together with its path and namespace.
    /// It is returned when properties are iterated with the <c>XMPIterator</c>.
    /// </remarks>
    /// <since>06.07.2006</since>
    public interface IXmpPropertyInfo : IXmpProperty
    {
        /// <value>Returns the namespace of the property</value>
        string Namespace { get; }

        /// <value>Returns the path of the property, but only if returned by the iterator.</value>
        string Path { get; }
    }
}
