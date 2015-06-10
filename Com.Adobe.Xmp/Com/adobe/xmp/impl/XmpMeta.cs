// =================================================================================================
// ADOBE SYSTEMS INCORPORATED
// Copyright 2006 Adobe Systems Incorporated
// All Rights Reserved
//
// NOTICE:  Adobe permits you to use, modify, and distribute this file in accordance with the terms
// of the Adobe license agreement accompanying it.
// =================================================================================================

using System.Diagnostics;
using Com.Adobe.Xmp.Impl.Xpath;
using Com.Adobe.Xmp.Options;
using Com.Adobe.Xmp.Properties;
using Sharpen;

namespace Com.Adobe.Xmp.Impl
{
    /// <summary>
    /// Implementation of <see cref="IXmpMeta"/>.
    /// </summary>
    /// <since>17.02.2006</since>
    public sealed class XmpMeta : IXmpMeta
    {
        public enum ValueType
        {
            /// <summary>Property values are Strings by default</summary>
            String = 0,
            Boolean = 1,
            Integer = 2,
            Long = 3,
            Double = 4,
            Date = 5,
            Calendar = 6,
            Base64 = 7
        }
        /// <summary>root of the metadata tree</summary>
        private readonly XmpNode _tree;

        /// <summary>the xpacket processing instructions content</summary>
        private string _packetHeader;

        /// <summary>Constructor for an empty metadata object.</summary>
        public XmpMeta()
        {
            // create root node
            _tree = new XmpNode(null, null, null);
        }

        /// <summary>Constructor for a cloned metadata tree.</summary>
        /// <param name="tree">
        /// an prefilled metadata tree which fulfills all
        /// <c>XMPNode</c> contracts.
        /// </param>
        public XmpMeta(XmpNode tree)
        {
            _tree = tree;
        }

        /// <exception cref="XmpException"/>
        public void AppendArrayItem(string schemaNs, string arrayName, PropertyOptions arrayOptions, string itemValue, PropertyOptions itemOptions)
        {
            ParameterAsserts.AssertSchemaNs(schemaNs);
            ParameterAsserts.AssertArrayName(arrayName);
            if (arrayOptions == null)
            {
                arrayOptions = new PropertyOptions();
            }
            if (!arrayOptions.IsOnlyArrayOptions)
            {
                throw new XmpException("Only array form flags allowed for arrayOptions", XmpErrorCode.BadOptions);
            }
            // Check if array options are set correctly.
            arrayOptions = XmpNodeUtils.VerifySetOptions(arrayOptions, null);
            // Locate or create the array. If it already exists, make sure the array
            // form from the options
            // parameter is compatible with the current state.
            var arrayPath = XmpPathParser.ExpandXPath(schemaNs, arrayName);
            // Just lookup, don't try to create.
            var arrayNode = XmpNodeUtils.FindNode(_tree, arrayPath, false, null);
            if (arrayNode != null)
            {
                // The array exists, make sure the form is compatible. Zero
                // arrayForm means take what exists.
                if (!arrayNode.Options.IsArray)
                {
                    throw new XmpException("The named property is not an array", XmpErrorCode.BadXPath);
                }
            }
            else
            {
                // if (arrayOptions != null && !arrayOptions.equalArrayTypes(arrayNode.getOptions()))
                // {
                // throw new XMPException("Mismatch of existing and specified array form", BADOPTIONS);
                // }
                // The array does not exist, try to create it.
                if (arrayOptions.IsArray)
                {
                    arrayNode = XmpNodeUtils.FindNode(_tree, arrayPath, true, arrayOptions);
                    if (arrayNode == null)
                    {
                        throw new XmpException("Failure creating array node", XmpErrorCode.BadXPath);
                    }
                }
                else
                {
                    // array options missing
                    throw new XmpException("Explicit arrayOptions required to create new array", XmpErrorCode.BadOptions);
                }
            }
            DoSetArrayItem(arrayNode, XmpConstConstants.ArrayLastItem, itemValue, itemOptions, true);
        }

        /// <exception cref="XmpException"/>
        public void AppendArrayItem(string schemaNs, string arrayName, string itemValue)
        {
            AppendArrayItem(schemaNs, arrayName, null, itemValue, null);
        }

        /// <exception cref="XmpException"/>
        public int CountArrayItems(string schemaNs, string arrayName)
        {
            ParameterAsserts.AssertSchemaNs(schemaNs);
            ParameterAsserts.AssertArrayName(arrayName);
            var arrayPath = XmpPathParser.ExpandXPath(schemaNs, arrayName);
            var arrayNode = XmpNodeUtils.FindNode(_tree, arrayPath, false, null);
            if (arrayNode == null)
            {
                return 0;
            }
            if (arrayNode.Options.IsArray)
            {
                return arrayNode.GetChildrenLength();
            }
            throw new XmpException("The named property is not an array", XmpErrorCode.BadXPath);
        }

        public void DeleteArrayItem(string schemaNs, string arrayName, int itemIndex)
        {
            try
            {
                ParameterAsserts.AssertSchemaNs(schemaNs);
                ParameterAsserts.AssertArrayName(arrayName);
                var itemPath = XmpPathFactory.ComposeArrayItemPath(arrayName, itemIndex);
                DeleteProperty(schemaNs, itemPath);
            }
            catch (XmpException)
            {
            }
        }

        public void DeleteProperty(string schemaNs, string propName)
        {
            try
            {
                ParameterAsserts.AssertSchemaNs(schemaNs);
                ParameterAsserts.AssertPropName(propName);
                var expPath = XmpPathParser.ExpandXPath(schemaNs, propName);
                var propNode = XmpNodeUtils.FindNode(_tree, expPath, false, null);
                if (propNode != null)
                {
                    XmpNodeUtils.DeleteNode(propNode);
                }
            }
            catch (XmpException)
            {
            }
        }

        public void DeleteQualifier(string schemaNs, string propName, string qualNs, string qualName)
        {
            try
            {
                // Note: qualNS and qualName are checked inside composeQualfierPath
                ParameterAsserts.AssertSchemaNs(schemaNs);
                ParameterAsserts.AssertPropName(propName);
                var qualPath = propName + XmpPathFactory.ComposeQualifierPath(qualNs, qualName);
                DeleteProperty(schemaNs, qualPath);
            }
            catch (XmpException)
            {
            }
        }

        public void DeleteStructField(string schemaNs, string structName, string fieldNs, string fieldName)
        {
            try
            {
                // fieldNS and fieldName are checked inside composeStructFieldPath
                ParameterAsserts.AssertSchemaNs(schemaNs);
                ParameterAsserts.AssertStructName(structName);
                var fieldPath = structName + XmpPathFactory.ComposeStructFieldPath(fieldNs, fieldName);
                DeleteProperty(schemaNs, fieldPath);
            }
            catch (XmpException)
            {
            }
        }

        public bool DoesPropertyExist(string schemaNs, string propName)
        {
            try
            {
                ParameterAsserts.AssertSchemaNs(schemaNs);
                ParameterAsserts.AssertPropName(propName);
                var expPath = XmpPathParser.ExpandXPath(schemaNs, propName);
                var propNode = XmpNodeUtils.FindNode(_tree, expPath, false, null);
                return propNode != null;
            }
            catch (XmpException)
            {
                return false;
            }
        }

        public bool DoesArrayItemExist(string schemaNs, string arrayName, int itemIndex)
        {
            try
            {
                ParameterAsserts.AssertSchemaNs(schemaNs);
                ParameterAsserts.AssertArrayName(arrayName);
                var path = XmpPathFactory.ComposeArrayItemPath(arrayName, itemIndex);
                return DoesPropertyExist(schemaNs, path);
            }
            catch (XmpException)
            {
                return false;
            }
        }

        public bool DoesStructFieldExist(string schemaNs, string structName, string fieldNs, string fieldName)
        {
            try
            {
                // fieldNS and fieldName are checked inside composeStructFieldPath()
                ParameterAsserts.AssertSchemaNs(schemaNs);
                ParameterAsserts.AssertStructName(structName);
                var path = XmpPathFactory.ComposeStructFieldPath(fieldNs, fieldName);
                return DoesPropertyExist(schemaNs, structName + path);
            }
            catch (XmpException)
            {
                return false;
            }
        }

        public bool DoesQualifierExist(string schemaNs, string propName, string qualNs, string qualName)
        {
            try
            {
                // qualNS and qualName are checked inside composeQualifierPath()
                ParameterAsserts.AssertSchemaNs(schemaNs);
                ParameterAsserts.AssertPropName(propName);
                var path = XmpPathFactory.ComposeQualifierPath(qualNs, qualName);
                return DoesPropertyExist(schemaNs, propName + path);
            }
            catch (XmpException)
            {
                return false;
            }
        }

        /// <exception cref="XmpException"/>
        public IXmpProperty GetArrayItem(string schemaNs, string arrayName, int itemIndex)
        {
            ParameterAsserts.AssertSchemaNs(schemaNs);
            ParameterAsserts.AssertArrayName(arrayName);
            var itemPath = XmpPathFactory.ComposeArrayItemPath(arrayName, itemIndex);
            return GetProperty(schemaNs, itemPath);
        }

        /// <exception cref="XmpException"/>
        public IXmpProperty GetLocalizedText(string schemaNs, string altTextName, string genericLang, string specificLang)
        {
            ParameterAsserts.AssertSchemaNs(schemaNs);
            ParameterAsserts.AssertArrayName(altTextName);
            ParameterAsserts.AssertSpecificLang(specificLang);
            genericLang = genericLang != null ? Utils.NormalizeLangValue(genericLang) : null;
            specificLang = Utils.NormalizeLangValue(specificLang);
            var arrayPath = XmpPathParser.ExpandXPath(schemaNs, altTextName);
            var arrayNode = XmpNodeUtils.FindNode(_tree, arrayPath, false, null);
            if (arrayNode == null)
            {
                return null;
            }
            var result = XmpNodeUtils.ChooseLocalizedText(arrayNode, genericLang, specificLang);
            var match = (int)result[0];
            var itemNode = (XmpNode)result[1];
            if (match != XmpNodeUtils.CltNoValues)
            {
                return new XmpProperty407(itemNode);
            }
            return null;
        }

        private sealed class XmpProperty407 : IXmpProperty
        {
            public XmpProperty407(XmpNode itemNode)
            {
                _itemNode = itemNode;
            }

            public string GetValue()
            {
                return _itemNode.Value;
            }

            public PropertyOptions GetOptions()
            {
                return _itemNode.Options;
            }

            public string GetLanguage()
            {
                return _itemNode.GetQualifier(1).Value;
            }

            public override string ToString()
            {
                return _itemNode.Value;
            }

            private readonly XmpNode _itemNode;
        }

        /// <exception cref="XmpException"/>
        public void SetLocalizedText(string schemaNs, string altTextName, string genericLang, string specificLang, string itemValue, PropertyOptions options)
        {
            ParameterAsserts.AssertSchemaNs(schemaNs);
            ParameterAsserts.AssertArrayName(altTextName);
            ParameterAsserts.AssertSpecificLang(specificLang);
            genericLang = genericLang != null ? Utils.NormalizeLangValue(genericLang) : null;
            specificLang = Utils.NormalizeLangValue(specificLang);
            var arrayPath = XmpPathParser.ExpandXPath(schemaNs, altTextName);
            // Find the array node and set the options if it was just created.
            var arrayNode = XmpNodeUtils.FindNode(_tree, arrayPath, true, new PropertyOptions(PropertyOptions.ArrayFlag | PropertyOptions.ArrayOrderedFlag | PropertyOptions.ArrayAlternateFlag | PropertyOptions.ArrayAltTextFlag));
            if (arrayNode == null)
            {
                throw new XmpException("Failed to find or create array node", XmpErrorCode.BadXPath);
            }
            if (!arrayNode.Options.IsArrayAltText)
            {
                if (!arrayNode.HasChildren && arrayNode.Options.IsArrayAlternate)
                {
                    arrayNode.Options.IsArrayAltText = true;
                }
                else
                {
                    throw new XmpException("Specified property is no alt-text array", XmpErrorCode.BadXPath);
                }
            }
            // Make sure the x-default item, if any, is first.
            var haveXDefault = false;
            XmpNode xdItem = null;
            for (var it = arrayNode.IterateChildren(); it.HasNext(); )
            {
                var currItem = (XmpNode)it.Next();
                if (!currItem.HasQualifier || !XmpConstConstants.XmlLang.Equals(currItem.GetQualifier(1).Name))
                {
                    throw new XmpException("Language qualifier must be first", XmpErrorCode.BadXPath);
                }
                if (XmpConstConstants.XDefault.Equals(currItem.GetQualifier(1).Value))
                {
                    xdItem = currItem;
                    haveXDefault = true;
                    break;
                }
            }
            // Moves x-default to the beginning of the array
            if (xdItem != null && arrayNode.GetChildrenLength() > 1)
            {
                arrayNode.RemoveChild(xdItem);
                arrayNode.AddChild(1, xdItem);
            }
            // Find the appropriate item.
            // chooseLocalizedText will make sure the array is a language
            // alternative.
            var result = XmpNodeUtils.ChooseLocalizedText(arrayNode, genericLang, specificLang);
            var match = (int)result[0];
            var itemNode = (XmpNode)result[1];
            var specificXDefault = XmpConstConstants.XDefault.Equals(specificLang);
            switch (match)
            {
                case XmpNodeUtils.CltNoValues:
                {
                    // Create the array items for the specificLang and x-default, with
                    // x-default first.
                    XmpNodeUtils.AppendLangItem(arrayNode, XmpConstConstants.XDefault, itemValue);
                    haveXDefault = true;
                    if (!specificXDefault)
                    {
                        XmpNodeUtils.AppendLangItem(arrayNode, specificLang, itemValue);
                    }
                    break;
                }

                case XmpNodeUtils.CltSpecificMatch:
                {
                    if (!specificXDefault)
                    {
                        // Update the specific item, update x-default if it matches the
                        // old value.
                        if (haveXDefault && xdItem != itemNode && xdItem != null && xdItem.Value.Equals(itemNode.Value))
                        {
                            xdItem.Value = itemValue;
                        }
                        // ! Do this after the x-default check!
                        itemNode.Value = itemValue;
                    }
                    else
                    {
                        // Update all items whose values match the old x-default value.
                        Debug.Assert(haveXDefault && xdItem == itemNode);
                        for (var it1 = arrayNode.IterateChildren(); it1.HasNext(); )
                        {
                            var currItem = (XmpNode)it1.Next();
                            if (currItem == xdItem || !currItem.Value.Equals(xdItem != null ? xdItem.Value : null))
                            {
                                continue;
                            }
                            currItem.Value = itemValue;
                        }
                        // And finally do the x-default item.
                        if (xdItem != null)
                        {
                            xdItem.Value = itemValue;
                        }
                    }
                    break;
                }

                case XmpNodeUtils.CltSingleGeneric:
                {
                    // Update the generic item, update x-default if it matches the old
                    // value.
                    if (haveXDefault && xdItem != itemNode && xdItem != null && xdItem.Value.Equals(itemNode.Value))
                    {
                        xdItem.Value = itemValue;
                    }
                    itemNode.Value = itemValue;
                    // ! Do this after
                    // the x-default
                    // check!
                    break;
                }

                case XmpNodeUtils.CltMultipleGeneric:
                {
                    // Create the specific language, ignore x-default.
                    XmpNodeUtils.AppendLangItem(arrayNode, specificLang, itemValue);
                    if (specificXDefault)
                    {
                        haveXDefault = true;
                    }
                    break;
                }

                case XmpNodeUtils.CltXdefault:
                {
                    // Create the specific language, update x-default if it was the only
                    // item.
                    if (xdItem != null && arrayNode.GetChildrenLength() == 1)
                    {
                        xdItem.Value = itemValue;
                    }
                    XmpNodeUtils.AppendLangItem(arrayNode, specificLang, itemValue);
                    break;
                }

                case XmpNodeUtils.CltFirstItem:
                {
                    // Create the specific language, don't add an x-default item.
                    XmpNodeUtils.AppendLangItem(arrayNode, specificLang, itemValue);
                    if (specificXDefault)
                    {
                        haveXDefault = true;
                    }
                    break;
                }

                default:
                {
                    // does not happen under normal circumstances
                    throw new XmpException("Unexpected result from ChooseLocalizedText", XmpErrorCode.InternalFailure);
                }
            }
            // Add an x-default at the front if needed.
            if (!haveXDefault && arrayNode.GetChildrenLength() == 1)
            {
                XmpNodeUtils.AppendLangItem(arrayNode, XmpConstConstants.XDefault, itemValue);
            }
        }

        /// <exception cref="XmpException"/>
        public void SetLocalizedText(string schemaNs, string altTextName, string genericLang, string specificLang, string itemValue)
        {
            SetLocalizedText(schemaNs, altTextName, genericLang, specificLang, itemValue, null);
        }

        /// <exception cref="XmpException"/>
        public IXmpProperty GetProperty(string schemaNs, string propName)
        {
            return GetProperty(schemaNs, propName, ValueType.String);
        }

        /// <summary>Returns a property, but the result value can be requested.</summary>
        /// <remarks>
        /// Returns a property, but the result value can be requested.
        /// </remarks>
        /// <param name="schemaNs">a schema namespace</param>
        /// <param name="propName">a property name or path</param>
        /// <param name="valueType">the type of the value, see VALUE_...</param>
        /// <returns>Returns an <c>XMPProperty</c></returns>
        /// <exception cref="XmpException">Collects any exception that occurs.</exception>
        private IXmpProperty GetProperty(string schemaNs, string propName, ValueType valueType)
        {
            ParameterAsserts.AssertSchemaNs(schemaNs);
            ParameterAsserts.AssertPropName(propName);
            var expPath = XmpPathParser.ExpandXPath(schemaNs, propName);
            var propNode = XmpNodeUtils.FindNode(_tree, expPath, false, null);
            if (propNode != null)
            {
                if (valueType != ValueType.String && propNode.Options.IsCompositeProperty)
                {
                    throw new XmpException("Property must be simple when a value type is requested", XmpErrorCode.BadXPath);
                }
                var value = EvaluateNodeValue(valueType, propNode);
                return new XmpProperty682(value, propNode);
            }
            return null;
        }

        private sealed class XmpProperty682 : IXmpProperty
        {
            public XmpProperty682(object value, XmpNode propNode)
            {
                _value = value;
                _propNode = propNode;
            }

            public string GetValue()
            {
                return _value != null ? _value.ToString() : null;
            }

            public PropertyOptions GetOptions()
            {
                return _propNode.Options;
            }

            public string GetLanguage()
            {
                return null;
            }

            public override string ToString()
            {
                return _value.ToString();
            }

            private readonly object _value;

            private readonly XmpNode _propNode;
        }

        /// <summary>Returns a property, but the result value can be requested.</summary>
        /// <param name="schemaNs">a schema namespace</param>
        /// <param name="propName">a property name or path</param>
        /// <param name="valueType">the type of the value, see VALUE_...</param>
        /// <returns>
        /// Returns the node value as an object according to the
        /// <c>valueType</c>.
        /// </returns>
        /// <exception cref="XmpException">Collects any exception that occurs.</exception>
        private object GetPropertyObject(string schemaNs, string propName, ValueType valueType)
        {
            ParameterAsserts.AssertSchemaNs(schemaNs);
            ParameterAsserts.AssertPropName(propName);
            var expPath = XmpPathParser.ExpandXPath(schemaNs, propName);
            var propNode = XmpNodeUtils.FindNode(_tree, expPath, false, null);
            if (propNode != null)
            {
                if (valueType != ValueType.String && propNode.Options.IsCompositeProperty)
                {
                    throw new XmpException("Property must be simple when a value type is requested", XmpErrorCode.BadXPath);
                }
                return EvaluateNodeValue(valueType, propNode);
            }
            return null;
        }

        /// <exception cref="XmpException"/>
        public bool GetPropertyBoolean(string schemaNs, string propName)
        {
            return (bool)GetPropertyObject(schemaNs, propName, ValueType.Boolean);
        }

        /// <exception cref="XmpException"/>
        public void SetPropertyBoolean(string schemaNs, string propName, bool propValue, PropertyOptions options)
        {
            SetProperty(schemaNs, propName, propValue ? XmpConstConstants.TrueString : XmpConstConstants.FalseString, options);
        }

        /// <exception cref="XmpException"/>
        public void SetPropertyBoolean(string schemaNs, string propName, bool propValue)
        {
            SetProperty(schemaNs, propName, propValue ? XmpConstConstants.TrueString : XmpConstConstants.FalseString, null);
        }

        /// <exception cref="XmpException"/>
        public int GetPropertyInteger(string schemaNs, string propName)
        {
            return (int)GetPropertyObject(schemaNs, propName, ValueType.Integer);
        }

        /// <exception cref="XmpException"/>
        public void SetPropertyInteger(string schemaNs, string propName, int propValue, PropertyOptions options)
        {
            SetProperty(schemaNs, propName, propValue, options);
        }

        /// <exception cref="XmpException"/>
        public void SetPropertyInteger(string schemaNs, string propName, int propValue)
        {
            SetProperty(schemaNs, propName, propValue, null);
        }

        /// <exception cref="XmpException"/>
        public long GetPropertyLong(string schemaNs, string propName)
        {
            return (long)GetPropertyObject(schemaNs, propName, ValueType.Long);
        }

        /// <exception cref="XmpException"/>
        public void SetPropertyLong(string schemaNs, string propName, long propValue, PropertyOptions options)
        {
            SetProperty(schemaNs, propName, propValue, options);
        }

        /// <exception cref="XmpException"/>
        public void SetPropertyLong(string schemaNs, string propName, long propValue)
        {
            SetProperty(schemaNs, propName, propValue, null);
        }

        /// <exception cref="XmpException"/>
        public double GetPropertyDouble(string schemaNs, string propName)
        {
            return (double)GetPropertyObject(schemaNs, propName, ValueType.Double);
        }

        /// <exception cref="XmpException"/>
        public void SetPropertyDouble(string schemaNs, string propName, double propValue, PropertyOptions options)
        {
            SetProperty(schemaNs, propName, propValue, options);
        }

        /// <exception cref="XmpException"/>
        public void SetPropertyDouble(string schemaNs, string propName, double propValue)
        {
            SetProperty(schemaNs, propName, propValue, null);
        }

        /// <exception cref="XmpException"/>
        public IXmpDateTime GetPropertyDate(string schemaNs, string propName)
        {
            return (IXmpDateTime)GetPropertyObject(schemaNs, propName, ValueType.Date);
        }

        /// <exception cref="XmpException"/>
        public void SetPropertyDate(string schemaNs, string propName, IXmpDateTime propValue, PropertyOptions options)
        {
            SetProperty(schemaNs, propName, propValue, options);
        }

        /// <exception cref="XmpException"/>
        public void SetPropertyDate(string schemaNs, string propName, IXmpDateTime propValue)
        {
            SetProperty(schemaNs, propName, propValue, null);
        }

        /// <exception cref="XmpException"/>
        public Calendar GetPropertyCalendar(string schemaNs, string propName)
        {
            return (Calendar)GetPropertyObject(schemaNs, propName, ValueType.Calendar);
        }

        /// <exception cref="XmpException"/>
        public void SetPropertyCalendar(string schemaNs, string propName, Calendar propValue, PropertyOptions options)
        {
            SetProperty(schemaNs, propName, propValue, options);
        }

        /// <exception cref="XmpException"/>
        public void SetPropertyCalendar(string schemaNs, string propName, Calendar propValue)
        {
            SetProperty(schemaNs, propName, propValue, null);
        }

        /// <exception cref="XmpException"/>
        public byte[] GetPropertyBase64(string schemaNs, string propName)
        {
            return (byte[])GetPropertyObject(schemaNs, propName, ValueType.Base64);
        }

        /// <exception cref="XmpException"/>
        public string GetPropertyString(string schemaNs, string propName)
        {
            return (string)GetPropertyObject(schemaNs, propName, ValueType.String);
        }

        /// <exception cref="XmpException"/>
        public void SetPropertyBase64(string schemaNs, string propName, byte[] propValue, PropertyOptions options)
        {
            SetProperty(schemaNs, propName, propValue, options);
        }

        /// <exception cref="XmpException"/>
        public void SetPropertyBase64(string schemaNs, string propName, byte[] propValue)
        {
            SetProperty(schemaNs, propName, propValue, null);
        }

        /// <exception cref="XmpException"/>
        public IXmpProperty GetQualifier(string schemaNs, string propName, string qualNs, string qualName)
        {
            // qualNS and qualName are checked inside composeQualfierPath
            ParameterAsserts.AssertSchemaNs(schemaNs);
            ParameterAsserts.AssertPropName(propName);
            var qualPath = propName + XmpPathFactory.ComposeQualifierPath(qualNs, qualName);
            return GetProperty(schemaNs, qualPath);
        }

        /// <exception cref="XmpException"/>
        public IXmpProperty GetStructField(string schemaNs, string structName, string fieldNs, string fieldName)
        {
            // fieldNS and fieldName are checked inside composeStructFieldPath
            ParameterAsserts.AssertSchemaNs(schemaNs);
            ParameterAsserts.AssertStructName(structName);
            var fieldPath = structName + XmpPathFactory.ComposeStructFieldPath(fieldNs, fieldName);
            return GetProperty(schemaNs, fieldPath);
        }

        /// <exception cref="XmpException"/>
        public IXmpIterator Iterator()
        {
            return Iterator(null, null, null);
        }

        /// <exception cref="XmpException"/>
        public IXmpIterator Iterator(IteratorOptions options)
        {
            return Iterator(null, null, options);
        }

        /// <exception cref="XmpException"/>
        public IXmpIterator Iterator(string schemaNs, string propName, IteratorOptions options)
        {
            return new XmpIterator(this, schemaNs, propName, options);
        }

        /// <exception cref="XmpException"/>
        public void SetArrayItem(string schemaNs, string arrayName, int itemIndex, string itemValue, PropertyOptions options)
        {
            ParameterAsserts.AssertSchemaNs(schemaNs);
            ParameterAsserts.AssertArrayName(arrayName);
            // Just lookup, don't try to create.
            var arrayPath = XmpPathParser.ExpandXPath(schemaNs, arrayName);
            var arrayNode = XmpNodeUtils.FindNode(_tree, arrayPath, false, null);
            if (arrayNode != null)
            {
                DoSetArrayItem(arrayNode, itemIndex, itemValue, options, false);
            }
            else
            {
                throw new XmpException("Specified array does not exist", XmpErrorCode.BadXPath);
            }
        }

        /// <exception cref="XmpException"/>
        public void SetArrayItem(string schemaNs, string arrayName, int itemIndex, string itemValue)
        {
            SetArrayItem(schemaNs, arrayName, itemIndex, itemValue, null);
        }

        /// <exception cref="XmpException"/>
        public void InsertArrayItem(string schemaNs, string arrayName, int itemIndex, string itemValue, PropertyOptions options)
        {
            ParameterAsserts.AssertSchemaNs(schemaNs);
            ParameterAsserts.AssertArrayName(arrayName);
            // Just lookup, don't try to create.
            var arrayPath = XmpPathParser.ExpandXPath(schemaNs, arrayName);
            var arrayNode = XmpNodeUtils.FindNode(_tree, arrayPath, false, null);
            if (arrayNode != null)
            {
                DoSetArrayItem(arrayNode, itemIndex, itemValue, options, true);
            }
            else
            {
                throw new XmpException("Specified array does not exist", XmpErrorCode.BadXPath);
            }
        }

        /// <exception cref="XmpException"/>
        public void InsertArrayItem(string schemaNs, string arrayName, int itemIndex, string itemValue)
        {
            InsertArrayItem(schemaNs, arrayName, itemIndex, itemValue, null);
        }

        /// <exception cref="XmpException"/>
        public void SetProperty(string schemaNs, string propName, object propValue, PropertyOptions options)
        {
            ParameterAsserts.AssertSchemaNs(schemaNs);
            ParameterAsserts.AssertPropName(propName);
            options = XmpNodeUtils.VerifySetOptions(options, propValue);
            var expPath = XmpPathParser.ExpandXPath(schemaNs, propName);
            var propNode = XmpNodeUtils.FindNode(_tree, expPath, true, options);
            if (propNode != null)
            {
                SetNode(propNode, propValue, options, false);
            }
            else
            {
                throw new XmpException("Specified property does not exist", XmpErrorCode.BadXPath);
            }
        }

        /// <exception cref="XmpException"/>
        public void SetProperty(string schemaNs, string propName, object propValue)
        {
            SetProperty(schemaNs, propName, propValue, null);
        }

        /// <exception cref="XmpException"/>
        public void SetQualifier(string schemaNs, string propName, string qualNs, string qualName, string qualValue, PropertyOptions options)
        {
            ParameterAsserts.AssertSchemaNs(schemaNs);
            ParameterAsserts.AssertPropName(propName);
            if (!DoesPropertyExist(schemaNs, propName))
            {
                throw new XmpException("Specified property does not exist!", XmpErrorCode.BadXPath);
            }
            var qualPath = propName + XmpPathFactory.ComposeQualifierPath(qualNs, qualName);
            SetProperty(schemaNs, qualPath, qualValue, options);
        }

        /// <exception cref="XmpException"/>
        public void SetQualifier(string schemaNs, string propName, string qualNs, string qualName, string qualValue)
        {
            SetQualifier(schemaNs, propName, qualNs, qualName, qualValue, null);
        }

        /// <exception cref="XmpException"/>
        public void SetStructField(string schemaNs, string structName, string fieldNs, string fieldName, string fieldValue, PropertyOptions options)
        {
            ParameterAsserts.AssertSchemaNs(schemaNs);
            ParameterAsserts.AssertStructName(structName);
            var fieldPath = structName + XmpPathFactory.ComposeStructFieldPath(fieldNs, fieldName);
            SetProperty(schemaNs, fieldPath, fieldValue, options);
        }

        /// <exception cref="XmpException"/>
        public void SetStructField(string schemaNs, string structName, string fieldNs, string fieldName, string fieldValue)
        {
            SetStructField(schemaNs, structName, fieldNs, fieldName, fieldValue, null);
        }

        public string GetObjectName()
        {
            return _tree.Name ?? string.Empty;
        }

        public void SetObjectName(string name)
        {
            _tree.Name = name;
        }

        public string GetPacketHeader()
        {
            return _packetHeader;
        }

        /// <summary>Sets the packetHeader attributes, only used by the parser.</summary>
        /// <param name="packetHeader">the processing instruction content</param>
        public void SetPacketHeader(string packetHeader)
        {
            _packetHeader = packetHeader;
        }

        /// <summary>Performs a deep clone of the XMPMeta-object</summary>
        public object Clone()
        {
            var clonedTree = (XmpNode)_tree.Clone();
            return new XmpMeta(clonedTree);
        }

        public string DumpObject()
        {
            // renders tree recursively
            return GetRoot().DumpNode(true);
        }

        public void Sort()
        {
            _tree.Sort();
        }

        /// <exception cref="XmpException"/>
        public void Normalize(ParseOptions options)
        {
            if (options == null)
            {
                options = new ParseOptions();
            }
            XmpNormalizer.Process(this, options);
        }

        /// <returns>Returns the root node of the XMP tree.</returns>
        public XmpNode GetRoot()
        {
            return _tree;
        }

        /// <summary>Locate or create the item node and set the value.</summary>
        /// <remarks>
        /// Locate or create the item node and set the value. Note the index
        /// parameter is one-based! The index can be in the range [1..size + 1] or
        /// "last()", normalize it and check the insert flags. The order of the
        /// normalization checks is important. If the array is empty we end up with
        /// an index and location to set item size + 1.
        /// </remarks>
        /// <param name="arrayNode">an array node</param>
        /// <param name="itemIndex">the index where to insert the item</param>
        /// <param name="itemValue">the item value</param>
        /// <param name="itemOptions">the options for the new item</param>
        /// <param name="insert">insert oder overwrite at index position?</param>
        /// <exception cref="XmpException"/>
        private void DoSetArrayItem(XmpNode arrayNode, int itemIndex, string itemValue, PropertyOptions itemOptions, bool insert)
        {
            var itemNode = new XmpNode(XmpConstConstants.ArrayItemName, null);
            itemOptions = XmpNodeUtils.VerifySetOptions(itemOptions, itemValue);
            // in insert mode the index after the last is allowed,
            // even ARRAY_LAST_ITEM points to the index *after* the last.
            var maxIndex = insert ? arrayNode.GetChildrenLength() + 1 : arrayNode.GetChildrenLength();
            if (itemIndex == XmpConstConstants.ArrayLastItem)
            {
                itemIndex = maxIndex;
            }
            if (1 <= itemIndex && itemIndex <= maxIndex)
            {
                if (!insert)
                {
                    arrayNode.RemoveChild(itemIndex);
                }
                arrayNode.AddChild(itemIndex, itemNode);
                SetNode(itemNode, itemValue, itemOptions, false);
            }
            else
            {
                throw new XmpException("Array index out of bounds", XmpErrorCode.BadIndex);
            }
        }

        /// <summary>
        /// The internals for setProperty() and related calls, used after the node is
        /// found or created.
        /// </summary>
        /// <param name="node">the newly created node</param>
        /// <param name="value">the node value, can be <c>null</c></param>
        /// <param name="newOptions">options for the new node, must not be <c>null</c>.</param>
        /// <param name="deleteExisting">flag if the existing value is to be overwritten</param>
        /// <exception cref="XmpException">thrown if options and value do not correspond</exception>
        internal void SetNode(XmpNode node, object value, PropertyOptions newOptions, bool deleteExisting)
        {
            if (deleteExisting)
            {
                node.Clear();
            }
            // its checked by setOptions(), if the merged result is a valid options set
            node.Options.MergeWith(newOptions);
            if (!node.Options.IsCompositeProperty)
            {
                // This is setting the value of a leaf node.
                XmpNodeUtils.SetNodeValue(node, value);
            }
            else
            {
                if (value != null && value.ToString().Length > 0)
                {
                    throw new XmpException("Composite nodes can't have values", XmpErrorCode.BadXPath);
                }
                node.RemoveChildren();
            }
        }

        /// <summary>
        /// Evaluates a raw node value to the given value type, apply special
        /// conversions for defined types in XMP.
        /// </summary>
        /// <param name="valueType">an int indicating the value type</param>
        /// <param name="propNode">the node containing the value</param>
        /// <returns>Returns a literal value for the node.</returns>
        /// <exception cref="XmpException"/>
        private static object EvaluateNodeValue(ValueType valueType, XmpNode propNode)
        {
            object value;
            var rawValue = propNode.Value;
            switch (valueType)
            {
                case ValueType.Boolean:
                {
                    value = Xmp.XmpUtils.ConvertToBoolean(rawValue);
                    break;
                }

                case ValueType.Integer:
                {
                    value = Xmp.XmpUtils.ConvertToInteger(rawValue);
                    break;
                }

                case ValueType.Long:
                {
                    value = Xmp.XmpUtils.ConvertToLong(rawValue);
                    break;
                }

                case ValueType.Double:
                {
                    value = Xmp.XmpUtils.ConvertToDouble(rawValue);
                    break;
                }

                case ValueType.Date:
                {
                    value = Xmp.XmpUtils.ConvertToDate(rawValue);
                    break;
                }

                case ValueType.Calendar:
                {
                    var dt = Xmp.XmpUtils.ConvertToDate(rawValue);
                    value = dt.GetCalendar();
                    break;
                }

                case ValueType.Base64:
                {
                    value = Xmp.XmpUtils.DecodeBase64(rawValue);
                    break;
                }

                case ValueType.String:
                default:
                {
                    // leaf values return empty string instead of null
                    // for the other cases the converter methods provides a "null"
                    // value.
                    // a default value can only occur if this method is made public.
                    value = rawValue != null || propNode.Options.IsCompositeProperty ? rawValue : string.Empty;
                    break;
                }
            }
            return value;
        }
    }
}
