// =================================================================================================
// ADOBE SYSTEMS INCORPORATED
// Copyright 2006 Adobe Systems Incorporated
// All Rights Reserved
//
// NOTICE:  Adobe permits you to use, modify, and distribute this file in accordance with the terms
// of the Adobe license agreement accompanying it.
// =================================================================================================

using System.Collections;
using System.Collections.Generic;
using Com.Adobe.Xmp.Properties;

namespace Com.Adobe.Xmp
{
    /// <summary>
    /// The schema registry keeps track of all namespaces and aliases used in the XMP
    /// metadata.
    /// </summary>
    /// <remarks>
    /// The schema registry keeps track of all namespaces and aliases used in the XMP
    /// metadata. At initialisation time, the default namespaces and default aliases
    /// are automatically registered. <b>Namespaces</b> must be registered before
    /// used in namespace URI parameters or path expressions. Within the XMP Toolkit
    /// the registered namespace URIs and prefixes must be unique. Additional
    /// namespaces encountered when parsing RDF are automatically registered. The
    /// namespace URI should always end in an XML name separator such as '/' or '#'.
    /// This is because some forms of RDF shorthand catenate a namespace URI with an
    /// element name to form a new URI.
    /// <para />
    /// <b>Aliases</b> in XMP serve the same purpose as Windows file shortcuts,
    /// Macintosh file aliases, or UNIX file symbolic links. The aliases are simply
    /// multiple names for the same property. One distinction of XMP aliases is that
    /// they are ordered, there is an alias name pointing to an actual name. The
    /// primary significance of the actual name is that it is the preferred name for
    /// output, generally the most widely recognized name.
    /// <para />
    /// The names that can be aliased in XMP are restricted. The alias must be a top
    /// level property name, not a field within a structure or an element within an
    /// array. The actual may be a top level property name, the first element within
    /// a top level array, or the default element in an alt-text array. This does not
    /// mean the alias can only be a simple property. It is OK to alias a top level
    /// structure or array to an identical top level structure or array, or to the
    /// first item of an array of structures.
    /// </remarks>
    /// <since>27.01.2006</since>
    public interface IXmpSchemaRegistry
    {
        #region Namespaces

        /// <summary>Register a namespace URI with a suggested prefix.</summary>
        /// <remarks>
        /// Register a namespace URI with a suggested prefix. It is not an error if
        /// the URI is already registered, no matter what the prefix is. If the URI
        /// is not registered but the suggested prefix is in use, a unique prefix is
        /// created from the suggested one. The actual registeed prefix is always
        /// returned. The function result tells if the registered prefix is the
        /// suggested one.
        /// <para />
        /// Note: No checking is presently done on either the URI or the prefix.
        /// </remarks>
        /// <param name="namespaceUri">The URI for the namespace. Must be a valid XML URI.</param>
        /// <param name="suggestedPrefix">
        /// The suggested prefix to be used if the URI is not yet
        /// registered. Must be a valid XML name.
        /// </param>
        /// <returns>
        /// Returns the registered prefix for this URI, is equal to the
        /// suggestedPrefix if the namespace hasn't been registered before,
        /// otherwise the existing prefix.
        /// </returns>
        /// <exception cref="XmpException">If the parameters are not accordingly set</exception>
        /// <exception cref="XmpException"/>
        string RegisterNamespace(string namespaceUri, string suggestedPrefix);

        /// <summary>Obtain the prefix for a registered namespace URI.</summary>
        /// <remarks>
        /// Obtain the prefix for a registered namespace URI.
        /// <para />
        /// It is not an error if the namespace URI is not registered.
        /// </remarks>
        /// <param name="namespaceUri">
        /// The URI for the namespace. Must not be null or the empty
        /// string.
        /// </param>
        /// <returns>Returns the prefix registered for this namespace URI or null.</returns>
        string GetNamespacePrefix(string namespaceUri);

        /// <summary>Obtain the URI for a registered namespace prefix.</summary>
        /// <remarks>
        /// Obtain the URI for a registered namespace prefix.
        /// <para />
        /// It is not an error if the namespace prefix is not registered.
        /// </remarks>
        /// <param name="namespacePrefix">
        /// The prefix for the namespace. Must not be null or the empty
        /// string.
        /// </param>
        /// <returns>Returns the URI registered for this prefix or null.</returns>
        string GetNamespaceUri(string namespacePrefix);

        /// <returns>
        /// Returns the registered prefix/namespace-pairs as map, where the keys are the
        /// namespaces and the values are the prefixes.
        /// </returns>
        IDictionary GetNamespaces();

        /// <returns>
        /// Returns the registered namespace/prefix-pairs as map, where the keys are the
        /// prefixes and the values are the namespaces.
        /// </returns>
        IDictionary GetPrefixes();

        /// <summary>Deletes a namespace from the registry.</summary>
        /// <remarks>
        /// Deletes a namespace from the registry.
        /// <para />
        /// Does nothing if the URI is not registered, or if the namespaceURI
        /// parameter is null or the empty string.
        /// <para />
        /// Note: Not yet implemented.
        /// </remarks>
        /// <param name="namespaceUri">The URI for the namespace.</param>
        void DeleteNamespace(string namespaceUri);

        #endregion

        #region Aliases

        /// <summary>Determines if a name is an alias, and what it is aliased to.</summary>
        /// <param name="aliasNs">
        /// The namespace URI of the alias. Must not be <c>null</c> or the empty
        /// string.
        /// </param>
        /// <param name="aliasProp">
        /// The name of the alias. May be an arbitrary path expression
        /// path, must not be <c>null</c> or the empty string.
        /// </param>
        /// <returns>
        /// Returns the <c>XMPAliasInfo</c> for the given alias namespace and property or
        /// <c>null</c> if there is no such alias.
        /// </returns>
        IXmpAliasInfo ResolveAlias(string aliasNs, string aliasProp);

        /// <summary>Collects all aliases that are contained in the provided namespace.</summary>
        /// <remarks>
        /// Collects all aliases that are contained in the provided namespace.
        /// If nothing is found, an empty array is returned.
        /// </remarks>
        /// <param name="aliasNs">a schema namespace URI</param>
        /// <returns>Returns all alias infos from aliases that are contained in the provided namespace.</returns>
        IReadOnlyList<IXmpAliasInfo> FindAliases(string aliasNs);

        /// <summary>Searches for registered aliases.</summary>
        /// <param name="qname">an XML conform qname</param>
        /// <returns>
        /// Returns if an alias definition for the given qname to another
        /// schema and property is registered.
        /// </returns>
        IXmpAliasInfo FindAlias(string qname);

        /// <returns>
        /// Returns the registered aliases as map, where the key is the "qname" (prefix and name)
        /// and the value an <c>XMPAliasInfo</c>-object.
        /// </returns>
        IDictionary GetAliases();

        #endregion

    }
}
