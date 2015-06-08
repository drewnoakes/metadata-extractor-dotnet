// =================================================================================================
// ADOBE SYSTEMS INCORPORATED
// Copyright 2006 Adobe Systems Incorporated
// All Rights Reserved
//
// NOTICE:  Adobe permits you to use, modify, and distribute this file in accordance with the terms
// of the Adobe license agreement accompanying it.
// =================================================================================================

using System;
using System.Diagnostics;
using Com.Adobe.Xmp.Impl.Xpath;
using Com.Adobe.Xmp.Options;
using Sharpen;

namespace Com.Adobe.Xmp.Impl
{
    /// <summary>Utilities for <code>XMPNode</code>.</summary>
    /// <since>Aug 28, 2006</since>
    public static class XmpNodeUtils
    {
        internal const int CltNoValues = 0;

        internal const int CltSpecificMatch = 1;

        internal const int CltSingleGeneric = 2;

        internal const int CltMultipleGeneric = 3;

        internal const int CltXdefault = 4;

        internal const int CltFirstItem = 5;

        // EMPTY
        /// <summary>Find or create a schema node if <code>createNodes</code> is false and</summary>
        /// <param name="tree">the root of the xmp tree.</param>
        /// <param name="namespaceUri">a namespace</param>
        /// <param name="createNodes">
        /// a flag indicating if the node shall be created if not found.
        /// <em>Note:</em> The namespace must be registered prior to this call.
        /// </param>
        /// <returns>
        /// Returns the schema node if found, <code>null</code> otherwise.
        /// Note: If <code>createNodes</code> is <code>true</code>, it is <b>always</b>
        /// returned a valid node.
        /// </returns>
        /// <exception cref="XmpException">
        /// An exception is only thrown if an error occurred, not if a
        /// node was not found.
        /// </exception>
        internal static XmpNode FindSchemaNode(XmpNode tree, string namespaceUri, bool createNodes)
        {
            return FindSchemaNode(tree, namespaceUri, null, createNodes);
        }

        /// <summary>Find or create a schema node if <code>createNodes</code> is true.</summary>
        /// <param name="tree">the root of the xmp tree.</param>
        /// <param name="namespaceUri">a namespace</param>
        /// <param name="suggestedPrefix">If a prefix is suggested, the namespace is allowed to be registered.</param>
        /// <param name="createNodes">
        /// a flag indicating if the node shall be created if not found.
        /// <em>Note:</em> The namespace must be registered prior to this call.
        /// </param>
        /// <returns>
        /// Returns the schema node if found, <code>null</code> otherwise.
        /// Note: If <code>createNodes</code> is <code>true</code>, it is <b>always</b>
        /// returned a valid node.
        /// </returns>
        /// <exception cref="XmpException">
        /// An exception is only thrown if an error occurred, not if a
        /// node was not found.
        /// </exception>
        internal static XmpNode FindSchemaNode(XmpNode tree, string namespaceUri, string suggestedPrefix, bool createNodes)
        {
            Debug.Assert(tree.GetParent() == null);
            // make sure that its the root
            XmpNode schemaNode = tree.FindChildByName(namespaceUri);
            if (schemaNode == null && createNodes)
            {
                schemaNode = new XmpNode(namespaceUri, new PropertyOptions().SetSchemaNode(true));
                schemaNode.SetImplicit(true);
                // only previously registered schema namespaces are allowed in the XMP tree.
                string prefix = XmpMetaFactory.GetSchemaRegistry().GetNamespacePrefix(namespaceUri);
                if (prefix == null)
                {
                    if (suggestedPrefix != null && suggestedPrefix.Length != 0)
                    {
                        prefix = XmpMetaFactory.GetSchemaRegistry().RegisterNamespace(namespaceUri, suggestedPrefix);
                    }
                    else
                    {
                        throw new XmpException("Unregistered schema namespace URI", XmpErrorConstants.Badschema);
                    }
                }
                schemaNode.SetValue(prefix);
                tree.AddChild(schemaNode);
            }
            return schemaNode;
        }

        /// <summary>Find or create a child node under a given parent node.</summary>
        /// <remarks>
        /// Find or create a child node under a given parent node. If the parent node is no
        /// Returns the found or created child node.
        /// </remarks>
        /// <param name="parent">the parent node</param>
        /// <param name="childName">the node name to find</param>
        /// <param name="createNodes">flag, if new nodes shall be created.</param>
        /// <returns>Returns the found or created node or <code>null</code>.</returns>
        /// <exception cref="XmpException">Thrown if</exception>
        internal static XmpNode FindChildNode(XmpNode parent, string childName, bool createNodes)
        {
            if (!parent.GetOptions().IsSchemaNode() && !parent.GetOptions().IsStruct())
            {
                if (!parent.IsImplicit())
                {
                    throw new XmpException("Named children only allowed for schemas and structs", XmpErrorConstants.Badxpath);
                }
                else
                {
                    if (parent.GetOptions().IsArray())
                    {
                        throw new XmpException("Named children not allowed for arrays", XmpErrorConstants.Badxpath);
                    }
                    else
                    {
                        if (createNodes)
                        {
                            parent.GetOptions().SetStruct(true);
                        }
                    }
                }
            }
            XmpNode childNode = parent.FindChildByName(childName);
            if (childNode == null && createNodes)
            {
                PropertyOptions options = new PropertyOptions();
                childNode = new XmpNode(childName, options);
                childNode.SetImplicit(true);
                parent.AddChild(childNode);
            }
            Debug.Assert(childNode != null || !createNodes);
            return childNode;
        }

        /// <summary>Follow an expanded path expression to find or create a node.</summary>
        /// <param name="xmpTree">the node to begin the search.</param>
        /// <param name="xpath">the complete xpath</param>
        /// <param name="createNodes">
        /// flag if nodes shall be created
        /// (when called by <code>setProperty()</code>)
        /// </param>
        /// <param name="leafOptions">
        /// the options for the created leaf nodes (only when
        /// <code>createNodes == true</code>).
        /// </param>
        /// <returns>Returns the node if found or created or <code>null</code>.</returns>
        /// <exception cref="XmpException">
        /// An exception is only thrown if an error occurred,
        /// not if a node was not found.
        /// </exception>
        internal static XmpNode FindNode(XmpNode xmpTree, XmpPath xpath, bool createNodes, PropertyOptions leafOptions)
        {
            // check if xpath is set.
            if (xpath == null || xpath.Size() == 0)
            {
                throw new XmpException("Empty XMPPath", XmpErrorConstants.Badxpath);
            }
            // Root of implicitly created subtree to possible delete it later.
            // Valid only if leaf is new.
            XmpNode rootImplicitNode = null;
            XmpNode currNode = null;
            // resolve schema step
            currNode = FindSchemaNode(xmpTree, xpath.GetSegment(XmpPath.StepSchema).GetName(), createNodes);
            if (currNode == null)
            {
                return null;
            }
            else
            {
                if (currNode.IsImplicit())
                {
                    currNode.SetImplicit(false);
                    // Clear the implicit node bit.
                    rootImplicitNode = currNode;
                }
            }
            // Save the top most implicit node.
            // Now follow the remaining steps of the original XMPPath.
            try
            {
                for (int i = 1; i < xpath.Size(); i++)
                {
                    currNode = FollowXPathStep(currNode, xpath.GetSegment(i), createNodes);
                    if (currNode == null)
                    {
                        if (createNodes)
                        {
                            // delete implicitly created nodes
                            DeleteNode(rootImplicitNode);
                        }
                        return null;
                    }
                    else
                    {
                        if (currNode.IsImplicit())
                        {
                            // clear the implicit node flag
                            currNode.SetImplicit(false);
                            // if node is an ALIAS (can be only in root step, auto-create array
                            // when the path has been resolved from a not simple alias type
                            if (i == 1 && xpath.GetSegment(i).IsAlias() && xpath.GetSegment(i).GetAliasForm() != 0)
                            {
                                currNode.GetOptions().SetOption(xpath.GetSegment(i).GetAliasForm(), true);
                            }
                            else
                            {
                                // "CheckImplicitStruct" in C++
                                if (i < xpath.Size() - 1 && xpath.GetSegment(i).GetKind() == XmpPath.StructFieldStep && !currNode.GetOptions().IsCompositeProperty())
                                {
                                    currNode.GetOptions().SetStruct(true);
                                }
                            }
                            if (rootImplicitNode == null)
                            {
                                rootImplicitNode = currNode;
                            }
                        }
                    }
                }
            }
            catch (XmpException e)
            {
                // Save the top most implicit node.
                // if new notes have been created prior to the error, delete them
                if (rootImplicitNode != null)
                {
                    DeleteNode(rootImplicitNode);
                }
                throw;
            }
            if (rootImplicitNode != null)
            {
                // set options only if a node has been successful created
                currNode.GetOptions().MergeWith(leafOptions);
                currNode.SetOptions(currNode.GetOptions());
            }
            return currNode;
        }

        /// <summary>Deletes the the given node and its children from its parent.</summary>
        /// <remarks>
        /// Deletes the the given node and its children from its parent.
        /// Takes care about adjusting the flags.
        /// </remarks>
        /// <param name="node">the top-most node to delete.</param>
        internal static void DeleteNode(XmpNode node)
        {
            XmpNode parent = node.GetParent();
            if (node.GetOptions().IsQualifier())
            {
                // root is qualifier
                parent.RemoveQualifier(node);
            }
            else
            {
                // root is NO qualifier
                parent.RemoveChild(node);
            }
            // delete empty Schema nodes
            if (!parent.HasChildren() && parent.GetOptions().IsSchemaNode())
            {
                parent.GetParent().RemoveChild(parent);
            }
        }

        /// <summary>This is setting the value of a leaf node.</summary>
        /// <param name="node">an XMPNode</param>
        /// <param name="value">a value</param>
        internal static void SetNodeValue(XmpNode node, object value)
        {
            string strValue = SerializeNodeValue(value);
            if (!(node.GetOptions().IsQualifier() && XmpConstConstants.XmlLang.Equals(node.GetName())))
            {
                node.SetValue(strValue);
            }
            else
            {
                node.SetValue(Utils.NormalizeLangValue(strValue));
            }
        }

        /// <summary>Verifies the PropertyOptions for consistancy and updates them as needed.</summary>
        /// <remarks>
        /// Verifies the PropertyOptions for consistancy and updates them as needed.
        /// If options are <code>null</code> they are created with default values.
        /// </remarks>
        /// <param name="options">the <code>PropertyOptions</code></param>
        /// <param name="itemValue">the node value to set</param>
        /// <returns>Returns the updated options.</returns>
        /// <exception cref="XmpException">If the options are not consistant.</exception>
        internal static PropertyOptions VerifySetOptions(PropertyOptions options, object itemValue)
        {
            // create empty and fix existing options
            if (options == null)
            {
                // set default options
                options = new PropertyOptions();
            }
            if (options.IsArrayAltText())
            {
                options.SetArrayAlternate(true);
            }
            if (options.IsArrayAlternate())
            {
                options.SetArrayOrdered(true);
            }
            if (options.IsArrayOrdered())
            {
                options.SetArray(true);
            }
            if (options.IsCompositeProperty() && itemValue != null && itemValue.ToString().Length > 0)
            {
                throw new XmpException("Structs and arrays can't have values", XmpErrorConstants.Badoptions);
            }
            options.AssertConsistency(options.GetOptions());
            return options;
        }

        /// <summary>
        /// Converts the node value to String, apply special conversions for defined
        /// types in XMP.
        /// </summary>
        /// <param name="value">the node value to set</param>
        /// <returns>Returns the String representation of the node value.</returns>
        internal static string SerializeNodeValue(object value)
        {
            string strValue;
            if (value == null)
            {
                strValue = null;
            }
            else
            {
                if (value is bool)
                {
                    strValue = Xmp.XmpUtils.ConvertFromBoolean(((bool)value));
                }
                else
                {
                    if (value is int)
                    {
                        strValue = Xmp.XmpUtils.ConvertFromInteger(((int)value).IntValue());
                    }
                    else
                    {
                        if (value is long)
                        {
                            strValue = Xmp.XmpUtils.ConvertFromLong(((long)value).LongValue());
                        }
                        else
                        {
                            if (value is double)
                            {
                                strValue = Xmp.XmpUtils.ConvertFromDouble(((double)value).DoubleValue());
                            }
                            else
                            {
                                if (value is IXmpDateTime)
                                {
                                    strValue = Xmp.XmpUtils.ConvertFromDate((IXmpDateTime)value);
                                }
                                else
                                {
                                    if (value is GregorianCalendar)
                                    {
                                        IXmpDateTime dt = XmpDateTimeFactory.CreateFromCalendar((GregorianCalendar)value);
                                        strValue = Xmp.XmpUtils.ConvertFromDate(dt);
                                    }
                                    else
                                    {
                                        if (value is sbyte[])
                                        {
                                            strValue = Xmp.XmpUtils.EncodeBase64((sbyte[])value);
                                        }
                                        else
                                        {
                                            strValue = value.ToString();
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return strValue != null ? Utils.RemoveControlChars(strValue) : null;
        }

        /// <summary>
        /// After processing by ExpandXPath, a step can be of these forms:
        /// <ul>
        /// <li>qualName - A top level property or struct field.
        /// </summary>
        /// <remarks>
        /// After processing by ExpandXPath, a step can be of these forms:
        /// <ul>
        /// <li>qualName - A top level property or struct field.
        /// <li>[index] - An element of an array.
        /// <li>[last()] - The last element of an array.
        /// <li>[qualName="value"] - An element in an array of structs, chosen by a field value.
        /// <li>[?qualName="value"] - An element in an array, chosen by a qualifier value.
        /// <li>?qualName - A general qualifier.
        /// </ul>
        /// Find the appropriate child node, resolving aliases, and optionally creating nodes.
        /// </remarks>
        /// <param name="parentNode">the node to start to start from</param>
        /// <param name="nextStep">the xpath segment</param>
        /// <param name="createNodes"></param>
        /// <returns>returns the found or created XMPPath node</returns>
        /// <exception cref="XmpException"></exception>
        private static XmpNode FollowXPathStep(XmpNode parentNode, XmpPathSegment nextStep, bool createNodes)
        {
            XmpNode nextNode = null;
            int index = 0;
            int stepKind = nextStep.GetKind();
            if (stepKind == XmpPath.StructFieldStep)
            {
                nextNode = FindChildNode(parentNode, nextStep.GetName(), createNodes);
            }
            else
            {
                if (stepKind == XmpPath.QualifierStep)
                {
                    nextNode = FindQualifierNode(parentNode, Runtime.Substring(nextStep.GetName(), 1), createNodes);
                }
                else
                {
                    // This is an array indexing step. First get the index, then get the node.
                    if (!parentNode.GetOptions().IsArray())
                    {
                        throw new XmpException("Indexing applied to non-array", XmpErrorConstants.Badxpath);
                    }
                    if (stepKind == XmpPath.ArrayIndexStep)
                    {
                        index = FindIndexedItem(parentNode, nextStep.GetName(), createNodes);
                    }
                    else
                    {
                        if (stepKind == XmpPath.ArrayLastStep)
                        {
                            index = parentNode.GetChildrenLength();
                        }
                        else
                        {
                            if (stepKind == XmpPath.FieldSelectorStep)
                            {
                                string[] result = Utils.SplitNameAndValue(nextStep.GetName());
                                string fieldName = result[0];
                                string fieldValue = result[1];
                                index = LookupFieldSelector(parentNode, fieldName, fieldValue);
                            }
                            else
                            {
                                if (stepKind == XmpPath.QualSelectorStep)
                                {
                                    string[] result = Utils.SplitNameAndValue(nextStep.GetName());
                                    string qualName = result[0];
                                    string qualValue = result[1];
                                    index = LookupQualSelector(parentNode, qualName, qualValue, nextStep.GetAliasForm());
                                }
                                else
                                {
                                    throw new XmpException("Unknown array indexing step in FollowXPathStep", XmpErrorConstants.Internalfailure);
                                }
                            }
                        }
                    }
                    if (1 <= index && index <= parentNode.GetChildrenLength())
                    {
                        nextNode = parentNode.GetChild(index);
                    }
                }
            }
            return nextNode;
        }

        /// <summary>Find or create a qualifier node under a given parent node.</summary>
        /// <remarks>
        /// Find or create a qualifier node under a given parent node. Returns a pointer to the
        /// qualifier node, and optionally an iterator for the node's position in
        /// the parent's vector of qualifiers. The iterator is unchanged if no qualifier node (null)
        /// is returned.
        /// <em>Note:</em> On entry, the qualName parameter must not have the leading '?' from the
        /// XMPPath step.
        /// </remarks>
        /// <param name="parent">the parent XMPNode</param>
        /// <param name="qualName">the qualifier name</param>
        /// <param name="createNodes">flag if nodes shall be created</param>
        /// <returns>Returns the qualifier node if found or created, <code>null</code> otherwise.</returns>
        /// <exception cref="XmpException"></exception>
        private static XmpNode FindQualifierNode(XmpNode parent, string qualName, bool createNodes)
        {
            Debug.Assert(!qualName.StartsWith("?"));
            XmpNode qualNode = parent.FindQualifierByName(qualName);
            if (qualNode == null && createNodes)
            {
                qualNode = new XmpNode(qualName, null);
                qualNode.SetImplicit(true);
                parent.AddQualifier(qualNode);
            }
            return qualNode;
        }

        /// <param name="arrayNode">an array node</param>
        /// <param name="segment">the segment containing the array index</param>
        /// <param name="createNodes">flag if new nodes are allowed to be created.</param>
        /// <returns>Returns the index or index = -1 if not found</returns>
        /// <exception cref="XmpException">Throws Exceptions</exception>
        private static int FindIndexedItem(XmpNode arrayNode, string segment, bool createNodes)
        {
            int index = 0;
            try
            {
                segment = Runtime.Substring(segment, 1, segment.Length - 1);
                index = Convert.ToInt32(segment);
                if (index < 1)
                {
                    throw new XmpException("Array index must be larger than zero", XmpErrorConstants.Badxpath);
                }
            }
            catch (FormatException)
            {
                throw new XmpException("Array index not digits.", XmpErrorConstants.Badxpath);
            }
            if (createNodes && index == arrayNode.GetChildrenLength() + 1)
            {
                // Append a new last + 1 node.
                XmpNode newItem = new XmpNode(XmpConstConstants.ArrayItemName, null);
                newItem.SetImplicit(true);
                arrayNode.AddChild(newItem);
            }
            return index;
        }

        /// <summary>
        /// Searches for a field selector in a node:
        /// [fieldName="value] - an element in an array of structs, chosen by a field value.
        /// </summary>
        /// <remarks>
        /// Searches for a field selector in a node:
        /// [fieldName="value] - an element in an array of structs, chosen by a field value.
        /// No implicit nodes are created by field selectors.
        /// </remarks>
        /// <param name="arrayNode"/>
        /// <param name="fieldName"/>
        /// <param name="fieldValue"/>
        /// <returns>Returns the index of the field if found, otherwise -1.</returns>
        /// <exception cref="XmpException"></exception>
        private static int LookupFieldSelector(XmpNode arrayNode, string fieldName, string fieldValue)
        {
            int result = -1;
            for (int index = 1; index <= arrayNode.GetChildrenLength() && result < 0; index++)
            {
                XmpNode currItem = arrayNode.GetChild(index);
                if (!currItem.GetOptions().IsStruct())
                {
                    throw new XmpException("Field selector must be used on array of struct", XmpErrorConstants.Badxpath);
                }
                for (int f = 1; f <= currItem.GetChildrenLength(); f++)
                {
                    XmpNode currField = currItem.GetChild(f);
                    if (!fieldName.Equals(currField.GetName()))
                    {
                        continue;
                    }
                    if (fieldValue.Equals(currField.GetValue()))
                    {
                        result = index;
                        break;
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Searches for a qualifier selector in a node:
        /// [?qualName="value"] - an element in an array, chosen by a qualifier value.
        /// </summary>
        /// <remarks>
        /// Searches for a qualifier selector in a node:
        /// [?qualName="value"] - an element in an array, chosen by a qualifier value.
        /// No implicit nodes are created for qualifier selectors,
        /// except for an alias to an x-default item.
        /// </remarks>
        /// <param name="arrayNode">an array node</param>
        /// <param name="qualName">the qualifier name</param>
        /// <param name="qualValue">the qualifier value</param>
        /// <param name="aliasForm">
        /// in case the qual selector results from an alias,
        /// an x-default node is created if there has not been one.
        /// </param>
        /// <returns>Returns the index of th</returns>
        /// <exception cref="XmpException"></exception>
        private static int LookupQualSelector(XmpNode arrayNode, string qualName, string qualValue, int aliasForm)
        {
            if (XmpConstConstants.XmlLang.Equals(qualName))
            {
                qualValue = Utils.NormalizeLangValue(qualValue);
                int index = LookupLanguageItem(arrayNode, qualValue);
                if (index < 0 && (aliasForm & AliasOptions.PropArrayAltText) > 0)
                {
                    XmpNode langNode = new XmpNode(XmpConstConstants.ArrayItemName, null);
                    XmpNode xdefault = new XmpNode(XmpConstConstants.XmlLang, XmpConstConstants.XDefault, null);
                    langNode.AddQualifier(xdefault);
                    arrayNode.AddChild(1, langNode);
                    return 1;
                }
                else
                {
                    return index;
                }
            }
            else
            {
                for (int index = 1; index < arrayNode.GetChildrenLength(); index++)
                {
                    XmpNode currItem = arrayNode.GetChild(index);
                    for (IIterator it = currItem.IterateQualifier(); it.HasNext(); )
                    {
                        XmpNode qualifier = (XmpNode)it.Next();
                        if (qualName.Equals(qualifier.GetName()) && qualValue.Equals(qualifier.GetValue()))
                        {
                            return index;
                        }
                    }
                }
                return -1;
            }
        }

        /// <summary>Make sure the x-default item is first.</summary>
        /// <remarks>
        /// Make sure the x-default item is first. Touch up &quot;single value&quot;
        /// arrays that have a default plus one real language. This case should have
        /// the same value for both items. Older Adobe apps were hardwired to only
        /// use the &quot;x-default&quot; item, so we copy that value to the other
        /// item.
        /// </remarks>
        /// <param name="arrayNode">an alt text array node</param>
        internal static void NormalizeLangArray(XmpNode arrayNode)
        {
            if (!arrayNode.GetOptions().IsArrayAltText())
            {
                return;
            }
            // check if node with x-default qual is first place
            for (int i = 2; i <= arrayNode.GetChildrenLength(); i++)
            {
                XmpNode child = arrayNode.GetChild(i);
                if (child.HasQualifier() && XmpConstConstants.XDefault.Equals(child.GetQualifier(1).GetValue()))
                {
                    // move node to first place
                    try
                    {
                        arrayNode.RemoveChild(i);
                        arrayNode.AddChild(1, child);
                    }
                    catch (XmpException)
                    {
                        // cannot occur, because same child is removed before
                        Debug.Assert(false);
                    }
                    if (i == 2)
                    {
                        arrayNode.GetChild(2).SetValue(child.GetValue());
                    }
                    break;
                }
            }
        }

        /// <summary>See if an array is an alt-text array.</summary>
        /// <remarks>
        /// See if an array is an alt-text array. If so, make sure the x-default item
        /// is first.
        /// </remarks>
        /// <param name="arrayNode">the array node to check if its an alt-text array</param>
        internal static void DetectAltText(XmpNode arrayNode)
        {
            if (arrayNode.GetOptions().IsArrayAlternate() && arrayNode.HasChildren())
            {
                bool isAltText = false;
                for (IIterator it = arrayNode.IterateChildren(); it.HasNext(); )
                {
                    XmpNode child = (XmpNode)it.Next();
                    if (child.GetOptions().GetHasLanguage())
                    {
                        isAltText = true;
                        break;
                    }
                }
                if (isAltText)
                {
                    arrayNode.GetOptions().SetArrayAltText(true);
                    NormalizeLangArray(arrayNode);
                }
            }
        }

        /// <summary>Appends a language item to an alt text array.</summary>
        /// <param name="arrayNode">the language array</param>
        /// <param name="itemLang">the language of the item</param>
        /// <param name="itemValue">the content of the item</param>
        /// <exception cref="XmpException">Thrown if a duplicate property is added</exception>
        internal static void AppendLangItem(XmpNode arrayNode, string itemLang, string itemValue)
        {
            XmpNode newItem = new XmpNode(XmpConstConstants.ArrayItemName, itemValue, null);
            XmpNode langQual = new XmpNode(XmpConstConstants.XmlLang, itemLang, null);
            newItem.AddQualifier(langQual);
            if (!XmpConstConstants.XDefault.Equals(langQual.GetValue()))
            {
                arrayNode.AddChild(newItem);
            }
            else
            {
                arrayNode.AddChild(1, newItem);
            }
        }

        /// <summary>
        /// <ol>
        /// <li>Look for an exact match with the specific language.
        /// </summary>
        /// <remarks>
        /// <ol>
        /// <li>Look for an exact match with the specific language.
        /// <li>If a generic language is given, look for partial matches.
        /// <li>Look for an "x-default"-item.
        /// <li>Choose the first item.
        /// </ol>
        /// </remarks>
        /// <param name="arrayNode">the alt text array node</param>
        /// <param name="genericLang">the generic language</param>
        /// <param name="specificLang">the specific language</param>
        /// <returns>
        /// Returns the kind of match as an Integer and the found node in an
        /// array.
        /// </returns>
        /// <exception cref="XmpException"/>
        internal static object[] ChooseLocalizedText(XmpNode arrayNode, string genericLang, string specificLang)
        {
            // See if the array has the right form. Allow empty alt arrays,
            // that is what parsing returns.
            if (!arrayNode.GetOptions().IsArrayAltText())
            {
                throw new XmpException("Localized text array is not alt-text", XmpErrorConstants.Badxpath);
            }
            else
            {
                if (!arrayNode.HasChildren())
                {
                    return new object[] { CltNoValues, null };
                }
            }
            int foundGenericMatches = 0;
            XmpNode resultNode = null;
            XmpNode xDefault = null;
            // Look for the first partial match with the generic language.
            for (IIterator it = arrayNode.IterateChildren(); it.HasNext(); )
            {
                XmpNode currItem = (XmpNode)it.Next();
                // perform some checks on the current item
                if (currItem.GetOptions().IsCompositeProperty())
                {
                    throw new XmpException("Alt-text array item is not simple", XmpErrorConstants.Badxpath);
                }
                else
                {
                    if (!currItem.HasQualifier() || !XmpConstConstants.XmlLang.Equals(currItem.GetQualifier(1).GetName()))
                    {
                        throw new XmpException("Alt-text array item has no language qualifier", XmpErrorConstants.Badxpath);
                    }
                }
                string currLang = currItem.GetQualifier(1).GetValue();
                // Look for an exact match with the specific language.
                if (specificLang.Equals(currLang))
                {
                    return new object[] { CltSpecificMatch, currItem };
                }
                else
                {
                    if (genericLang != null && currLang.StartsWith(genericLang))
                    {
                        if (resultNode == null)
                        {
                            resultNode = currItem;
                        }
                        // ! Don't return/break, need to look for other matches.
                        foundGenericMatches++;
                    }
                    else
                    {
                        if (XmpConstConstants.XDefault.Equals(currLang))
                        {
                            xDefault = currItem;
                        }
                    }
                }
            }
            // evaluate loop
            if (foundGenericMatches == 1)
            {
                return new object[] { CltSingleGeneric, resultNode };
            }
            else
            {
                if (foundGenericMatches > 1)
                {
                    return new object[] { CltMultipleGeneric, resultNode };
                }
                else
                {
                    if (xDefault != null)
                    {
                        return new object[] { CltXdefault, xDefault };
                    }
                    else
                    {
                        // Everything failed, choose the first item.
                        return new object[] { CltFirstItem, arrayNode.GetChild(1) };
                    }
                }
            }
        }

        /// <summary>Looks for the appropriate language item in a text alternative array.item</summary>
        /// <param name="arrayNode">an array node</param>
        /// <param name="language">the requested language</param>
        /// <returns>Returns the index if the language has been found, -1 otherwise.</returns>
        /// <exception cref="XmpException"/>
        internal static int LookupLanguageItem(XmpNode arrayNode, string language)
        {
            if (!arrayNode.GetOptions().IsArray())
            {
                throw new XmpException("Language item must be used on array", XmpErrorConstants.Badxpath);
            }
            for (int index = 1; index <= arrayNode.GetChildrenLength(); index++)
            {
                XmpNode child = arrayNode.GetChild(index);
                if (!child.HasQualifier() || !XmpConstConstants.XmlLang.Equals(child.GetQualifier(1).GetName()))
                {
                    continue;
                }
                else
                {
                    if (language.Equals(child.GetQualifier(1).GetValue()))
                    {
                        return index;
                    }
                }
            }
            return -1;
        }
    }
}
