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
    /// Implementation for <see cref="IXmpMeta"/>.
    /// </summary>
    /// <since>17.02.2006</since>
    public class XmpMeta : IXmpMeta
    {
        /// <summary>Property values are Strings by default</summary>
        private const int ValueString = 0;

        private const int ValueBoolean = 1;

        private const int ValueInteger = 2;

        private const int ValueLong = 3;

        private const int ValueDouble = 4;

        private const int ValueDate = 5;

        private const int ValueCalendar = 6;

        private const int ValueBase64 = 7;

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
        /// <code>XMPNode</code> contracts.
        /// </param>
        public XmpMeta(XmpNode tree)
        {
            _tree = tree;
        }

        /// <seealso cref="IXmpMeta.AppendArrayItem(string, string, Com.Adobe.Xmp.Options.PropertyOptions, string, Com.Adobe.Xmp.Options.PropertyOptions)"/>
        /// <exception cref="XmpException"/>
        public virtual void AppendArrayItem(string schemaNs, string arrayName, PropertyOptions arrayOptions, string itemValue, PropertyOptions itemOptions)
        {
            ParameterAsserts.AssertSchemaNs(schemaNs);
            ParameterAsserts.AssertArrayName(arrayName);
            if (arrayOptions == null)
            {
                arrayOptions = new PropertyOptions();
            }
            if (!arrayOptions.IsOnlyArrayOptions())
            {
                throw new XmpException("Only array form flags allowed for arrayOptions", XmpErrorCode.Badoptions);
            }
            // Check if array options are set correctly.
            arrayOptions = XmpNodeUtils.VerifySetOptions(arrayOptions, null);
            // Locate or create the array. If it already exists, make sure the array
            // form from the options
            // parameter is compatible with the current state.
            XmpPath arrayPath = XmpPathParser.ExpandXPath(schemaNs, arrayName);
            // Just lookup, don't try to create.
            XmpNode arrayNode = XmpNodeUtils.FindNode(_tree, arrayPath, false, null);
            if (arrayNode != null)
            {
                // The array exists, make sure the form is compatible. Zero
                // arrayForm means take what exists.
                if (!arrayNode.GetOptions().IsArray())
                {
                    throw new XmpException("The named property is not an array", XmpErrorCode.Badxpath);
                }
            }
            else
            {
                // if (arrayOptions != null && !arrayOptions.equalArrayTypes(arrayNode.getOptions()))
                // {
                // throw new XMPException("Mismatch of existing and specified array form", BADOPTIONS);
                // }
                // The array does not exist, try to create it.
                if (arrayOptions.IsArray())
                {
                    arrayNode = XmpNodeUtils.FindNode(_tree, arrayPath, true, arrayOptions);
                    if (arrayNode == null)
                    {
                        throw new XmpException("Failure creating array node", XmpErrorCode.Badxpath);
                    }
                }
                else
                {
                    // array options missing
                    throw new XmpException("Explicit arrayOptions required to create new array", XmpErrorCode.Badoptions);
                }
            }
            DoSetArrayItem(arrayNode, XmpConstConstants.ArrayLastItem, itemValue, itemOptions, true);
        }

        /// <seealso cref="IXmpMeta.AppendArrayItem(string, string, string)"/>
        /// <exception cref="XmpException"/>
        public virtual void AppendArrayItem(string schemaNs, string arrayName, string itemValue)
        {
            AppendArrayItem(schemaNs, arrayName, null, itemValue, null);
        }

        /// <exception cref="XmpException"/>
        /// <seealso cref="IXmpMeta.CountArrayItems(string, string)"/>
        public virtual int CountArrayItems(string schemaNs, string arrayName)
        {
            ParameterAsserts.AssertSchemaNs(schemaNs);
            ParameterAsserts.AssertArrayName(arrayName);
            XmpPath arrayPath = XmpPathParser.ExpandXPath(schemaNs, arrayName);
            XmpNode arrayNode = XmpNodeUtils.FindNode(_tree, arrayPath, false, null);
            if (arrayNode == null)
            {
                return 0;
            }
            if (arrayNode.GetOptions().IsArray())
            {
                return arrayNode.GetChildrenLength();
            }
            throw new XmpException("The named property is not an array", XmpErrorCode.Badxpath);
        }

        /// <seealso cref="IXmpMeta.DeleteArrayItem(string, string, int)"/>
        public virtual void DeleteArrayItem(string schemaNs, string arrayName, int itemIndex)
        {
            try
            {
                ParameterAsserts.AssertSchemaNs(schemaNs);
                ParameterAsserts.AssertArrayName(arrayName);
                string itemPath = XmpPathFactory.ComposeArrayItemPath(arrayName, itemIndex);
                DeleteProperty(schemaNs, itemPath);
            }
            catch (XmpException)
            {
            }
        }

        // EMPTY, exceptions are ignored within delete
        /// <seealso cref="IXmpMeta.DeleteProperty(string, string)"/>
        public virtual void DeleteProperty(string schemaNs, string propName)
        {
            try
            {
                ParameterAsserts.AssertSchemaNs(schemaNs);
                ParameterAsserts.AssertPropName(propName);
                XmpPath expPath = XmpPathParser.ExpandXPath(schemaNs, propName);
                XmpNode propNode = XmpNodeUtils.FindNode(_tree, expPath, false, null);
                if (propNode != null)
                {
                    XmpNodeUtils.DeleteNode(propNode);
                }
            }
            catch (XmpException)
            {
            }
        }

        // EMPTY, exceptions are ignored within delete
        /// <seealso cref="IXmpMeta.DeleteQualifier(string, string, string, string)"/>
        public virtual void DeleteQualifier(string schemaNs, string propName, string qualNs, string qualName)
        {
            try
            {
                // Note: qualNS and qualName are checked inside composeQualfierPath
                ParameterAsserts.AssertSchemaNs(schemaNs);
                ParameterAsserts.AssertPropName(propName);
                string qualPath = propName + XmpPathFactory.ComposeQualifierPath(qualNs, qualName);
                DeleteProperty(schemaNs, qualPath);
            }
            catch (XmpException)
            {
            }
        }

        // EMPTY, exceptions within delete are ignored
        /// <seealso cref="IXmpMeta.DeleteStructField(string, string, string, string)"/>
        public virtual void DeleteStructField(string schemaNs, string structName, string fieldNs, string fieldName)
        {
            try
            {
                // fieldNS and fieldName are checked inside composeStructFieldPath
                ParameterAsserts.AssertSchemaNs(schemaNs);
                ParameterAsserts.AssertStructName(structName);
                string fieldPath = structName + XmpPathFactory.ComposeStructFieldPath(fieldNs, fieldName);
                DeleteProperty(schemaNs, fieldPath);
            }
            catch (XmpException)
            {
            }
        }

        // EMPTY, exceptions within delete are ignored
        /// <seealso cref="IXmpMeta.DoesPropertyExist(string, string)"/>
        public virtual bool DoesPropertyExist(string schemaNs, string propName)
        {
            try
            {
                ParameterAsserts.AssertSchemaNs(schemaNs);
                ParameterAsserts.AssertPropName(propName);
                XmpPath expPath = XmpPathParser.ExpandXPath(schemaNs, propName);
                XmpNode propNode = XmpNodeUtils.FindNode(_tree, expPath, false, null);
                return propNode != null;
            }
            catch (XmpException)
            {
                return false;
            }
        }

        /// <seealso cref="IXmpMeta.DoesArrayItemExist(string, string, int)"/>
        public virtual bool DoesArrayItemExist(string schemaNs, string arrayName, int itemIndex)
        {
            try
            {
                ParameterAsserts.AssertSchemaNs(schemaNs);
                ParameterAsserts.AssertArrayName(arrayName);
                string path = XmpPathFactory.ComposeArrayItemPath(arrayName, itemIndex);
                return DoesPropertyExist(schemaNs, path);
            }
            catch (XmpException)
            {
                return false;
            }
        }

        /// <seealso cref="IXmpMeta.DoesStructFieldExist(string, string, string, string)"/>
        public virtual bool DoesStructFieldExist(string schemaNs, string structName, string fieldNs, string fieldName)
        {
            try
            {
                // fieldNS and fieldName are checked inside composeStructFieldPath()
                ParameterAsserts.AssertSchemaNs(schemaNs);
                ParameterAsserts.AssertStructName(structName);
                string path = XmpPathFactory.ComposeStructFieldPath(fieldNs, fieldName);
                return DoesPropertyExist(schemaNs, structName + path);
            }
            catch (XmpException)
            {
                return false;
            }
        }

        /// <seealso cref="IXmpMeta.DoesQualifierExist(string, string, string, string)"/>
        public virtual bool DoesQualifierExist(string schemaNs, string propName, string qualNs, string qualName)
        {
            try
            {
                // qualNS and qualName are checked inside composeQualifierPath()
                ParameterAsserts.AssertSchemaNs(schemaNs);
                ParameterAsserts.AssertPropName(propName);
                string path = XmpPathFactory.ComposeQualifierPath(qualNs, qualName);
                return DoesPropertyExist(schemaNs, propName + path);
            }
            catch (XmpException)
            {
                return false;
            }
        }

        /// <seealso cref="IXmpMeta.GetArrayItem(string, string, int)"/>
        /// <exception cref="XmpException"/>
        public virtual IXmpProperty GetArrayItem(string schemaNs, string arrayName, int itemIndex)
        {
            ParameterAsserts.AssertSchemaNs(schemaNs);
            ParameterAsserts.AssertArrayName(arrayName);
            string itemPath = XmpPathFactory.ComposeArrayItemPath(arrayName, itemIndex);
            return GetProperty(schemaNs, itemPath);
        }

        /// <exception cref="XmpException"/>
        /// <seealso cref="IXmpMeta.GetLocalizedText(string, string, string, string)"/>
        public virtual IXmpProperty GetLocalizedText(string schemaNs, string altTextName, string genericLang, string specificLang)
        {
            ParameterAsserts.AssertSchemaNs(schemaNs);
            ParameterAsserts.AssertArrayName(altTextName);
            ParameterAsserts.AssertSpecificLang(specificLang);
            genericLang = genericLang != null ? Utils.NormalizeLangValue(genericLang) : null;
            specificLang = Utils.NormalizeLangValue(specificLang);
            XmpPath arrayPath = XmpPathParser.ExpandXPath(schemaNs, altTextName);
            XmpNode arrayNode = XmpNodeUtils.FindNode(_tree, arrayPath, false, null);
            if (arrayNode == null)
            {
                return null;
            }
            object[] result = XmpNodeUtils.ChooseLocalizedText(arrayNode, genericLang, specificLang);
            int match = ((int)result[0]).IntValue();
            XmpNode itemNode = (XmpNode)result[1];
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
                return _itemNode.GetValue();
            }

            public PropertyOptions GetOptions()
            {
                return _itemNode.GetOptions();
            }

            public string GetLanguage()
            {
                return _itemNode.GetQualifier(1).GetValue();
            }

            public override string ToString()
            {
                return _itemNode.GetValue().ToString();
            }

            private readonly XmpNode _itemNode;
        }

        /// <seealso cref="IXmpMeta.SetLocalizedText(string, string, string, string, string, Com.Adobe.Xmp.Options.PropertyOptions)"/>
        /// <exception cref="XmpException"/>
        public virtual void SetLocalizedText(string schemaNs, string altTextName, string genericLang, string specificLang, string itemValue, PropertyOptions options)
        {
            ParameterAsserts.AssertSchemaNs(schemaNs);
            ParameterAsserts.AssertArrayName(altTextName);
            ParameterAsserts.AssertSpecificLang(specificLang);
            genericLang = genericLang != null ? Utils.NormalizeLangValue(genericLang) : null;
            specificLang = Utils.NormalizeLangValue(specificLang);
            XmpPath arrayPath = XmpPathParser.ExpandXPath(schemaNs, altTextName);
            // Find the array node and set the options if it was just created.
            XmpNode arrayNode = XmpNodeUtils.FindNode(_tree, arrayPath, true, new PropertyOptions(PropertyOptions.Array | PropertyOptions.ArrayOrdered | PropertyOptions.ArrayAlternate | PropertyOptions.ArrayAltText));
            if (arrayNode == null)
            {
                throw new XmpException("Failed to find or create array node", XmpErrorCode.Badxpath);
            }
            if (!arrayNode.GetOptions().IsArrayAltText())
            {
                if (!arrayNode.HasChildren() && arrayNode.GetOptions().IsArrayAlternate())
                {
                    arrayNode.GetOptions().SetArrayAltText(true);
                }
                else
                {
                    throw new XmpException("Specified property is no alt-text array", XmpErrorCode.Badxpath);
                }
            }
            // Make sure the x-default item, if any, is first.
            bool haveXDefault = false;
            XmpNode xdItem = null;
            for (IIterator it = arrayNode.IterateChildren(); it.HasNext(); )
            {
                XmpNode currItem = (XmpNode)it.Next();
                if (!currItem.HasQualifier() || !XmpConstConstants.XmlLang.Equals(currItem.GetQualifier(1).GetName()))
                {
                    throw new XmpException("Language qualifier must be first", XmpErrorCode.Badxpath);
                }
                if (XmpConstConstants.XDefault.Equals(currItem.GetQualifier(1).GetValue()))
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
            object[] result = XmpNodeUtils.ChooseLocalizedText(arrayNode, genericLang, specificLang);
            int match = ((int)result[0]).IntValue();
            XmpNode itemNode = (XmpNode)result[1];
            bool specificXDefault = XmpConstConstants.XDefault.Equals(specificLang);
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
                        if (haveXDefault && xdItem != itemNode && xdItem != null && xdItem.GetValue().Equals(itemNode.GetValue()))
                        {
                            xdItem.SetValue(itemValue);
                        }
                        // ! Do this after the x-default check!
                        itemNode.SetValue(itemValue);
                    }
                    else
                    {
                        // Update all items whose values match the old x-default value.
                        Debug.Assert(haveXDefault && xdItem == itemNode);
                        for (IIterator it1 = arrayNode.IterateChildren(); it1.HasNext(); )
                        {
                            XmpNode currItem = (XmpNode)it1.Next();
                            if (currItem == xdItem || !currItem.GetValue().Equals(xdItem != null ? xdItem.GetValue() : null))
                            {
                                continue;
                            }
                            currItem.SetValue(itemValue);
                        }
                        // And finally do the x-default item.
                        if (xdItem != null)
                        {
                            xdItem.SetValue(itemValue);
                        }
                    }
                    break;
                }

                case XmpNodeUtils.CltSingleGeneric:
                {
                    // Update the generic item, update x-default if it matches the old
                    // value.
                    if (haveXDefault && xdItem != itemNode && xdItem != null && xdItem.GetValue().Equals(itemNode.GetValue()))
                    {
                        xdItem.SetValue(itemValue);
                    }
                    itemNode.SetValue(itemValue);
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
                        xdItem.SetValue(itemValue);
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
                    throw new XmpException("Unexpected result from ChooseLocalizedText", XmpErrorCode.Internalfailure);
                }
            }
            // Add an x-default at the front if needed.
            if (!haveXDefault && arrayNode.GetChildrenLength() == 1)
            {
                XmpNodeUtils.AppendLangItem(arrayNode, XmpConstConstants.XDefault, itemValue);
            }
        }

        /// <seealso cref="IXmpMeta.SetLocalizedText(string, string, string, string, string)"/>
        /// <exception cref="XmpException"/>
        public virtual void SetLocalizedText(string schemaNs, string altTextName, string genericLang, string specificLang, string itemValue)
        {
            SetLocalizedText(schemaNs, altTextName, genericLang, specificLang, itemValue, null);
        }

        /// <exception cref="XmpException"/>
        /// <seealso cref="IXmpMeta.GetProperty(string, string)"/>
        public virtual IXmpProperty GetProperty(string schemaNs, string propName)
        {
            return GetProperty(schemaNs, propName, ValueString);
        }

        /// <summary>Returns a property, but the result value can be requested.</summary>
        /// <remarks>
        /// Returns a property, but the result value can be requested. It can be one of
        /// <see cref="ValueString"/>, <see cref="ValueBoolean"/>, <see cref="ValueInteger"/>,
        /// <see cref="ValueLong"/>, <see cref="ValueDouble"/>, <see cref="ValueDate"/>,
        /// <see cref="ValueCalendar"/>, <see cref="ValueBase64"/>.
        /// </remarks>
        /// <seealso cref="IXmpMeta.GetProperty(string, string)"/>
        /// <param name="schemaNs">a schema namespace</param>
        /// <param name="propName">a property name or path</param>
        /// <param name="valueType">the type of the value, see VALUE_...</param>
        /// <returns>Returns an <code>XMPProperty</code></returns>
        /// <exception cref="XmpException">Collects any exception that occurs.</exception>
        protected internal virtual IXmpProperty GetProperty(string schemaNs, string propName, int valueType)
        {
            ParameterAsserts.AssertSchemaNs(schemaNs);
            ParameterAsserts.AssertPropName(propName);
            XmpPath expPath = XmpPathParser.ExpandXPath(schemaNs, propName);
            XmpNode propNode = XmpNodeUtils.FindNode(_tree, expPath, false, null);
            if (propNode != null)
            {
                if (valueType != ValueString && propNode.GetOptions().IsCompositeProperty())
                {
                    throw new XmpException("Property must be simple when a value type is requested", XmpErrorCode.Badxpath);
                }
                object value = EvaluateNodeValue(valueType, propNode);
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
                return _propNode.GetOptions();
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
        /// <seealso cref="IXmpMeta.GetProperty(string, string)"/>
        /// <param name="schemaNs">a schema namespace</param>
        /// <param name="propName">a property name or path</param>
        /// <param name="valueType">the type of the value, see VALUE_...</param>
        /// <returns>
        /// Returns the node value as an object according to the
        /// <code>valueType</code>.
        /// </returns>
        /// <exception cref="XmpException">Collects any exception that occurs.</exception>
        protected internal virtual object GetPropertyObject(string schemaNs, string propName, int valueType)
        {
            ParameterAsserts.AssertSchemaNs(schemaNs);
            ParameterAsserts.AssertPropName(propName);
            XmpPath expPath = XmpPathParser.ExpandXPath(schemaNs, propName);
            XmpNode propNode = XmpNodeUtils.FindNode(_tree, expPath, false, null);
            if (propNode != null)
            {
                if (valueType != ValueString && propNode.GetOptions().IsCompositeProperty())
                {
                    throw new XmpException("Property must be simple when a value type is requested", XmpErrorCode.Badxpath);
                }
                return EvaluateNodeValue(valueType, propNode);
            }
            return null;
        }

        /// <seealso cref="IXmpMeta.GetPropertyBoolean(string, string)"/>
        /// <exception cref="XmpException"/>
        public virtual bool GetPropertyBoolean(string schemaNs, string propName)
        {
            return (bool)GetPropertyObject(schemaNs, propName, ValueBoolean);
        }

        /// <exception cref="XmpException"/>
        /// <seealso cref="IXmpMeta.SetPropertyBoolean(string, string, bool, Com.Adobe.Xmp.Options.PropertyOptions)"/>
        public virtual void SetPropertyBoolean(string schemaNs, string propName, bool propValue, PropertyOptions options)
        {
            SetProperty(schemaNs, propName, propValue ? XmpConstConstants.Truestr : XmpConstConstants.Falsestr, options);
        }

        /// <seealso cref="IXmpMeta.SetPropertyBoolean(string, string, bool)"/>
        /// <exception cref="XmpException"/>
        public virtual void SetPropertyBoolean(string schemaNs, string propName, bool propValue)
        {
            SetProperty(schemaNs, propName, propValue ? XmpConstConstants.Truestr : XmpConstConstants.Falsestr, null);
        }

        /// <seealso cref="IXmpMeta.GetPropertyInteger(string, string)"/>
        /// <exception cref="XmpException"/>
        public virtual int GetPropertyInteger(string schemaNs, string propName)
        {
            return (int)GetPropertyObject(schemaNs, propName, ValueInteger);
        }

        /// <seealso cref="IXmpMeta.SetPropertyInteger(string, string, int, Com.Adobe.Xmp.Options.PropertyOptions)"/>
        /// <exception cref="XmpException"/>
        public virtual void SetPropertyInteger(string schemaNs, string propName, int propValue, PropertyOptions options)
        {
            SetProperty(schemaNs, propName, propValue, options);
        }

        /// <seealso cref="IXmpMeta.SetPropertyInteger(string, string, int)"/>
        /// <exception cref="XmpException"/>
        public virtual void SetPropertyInteger(string schemaNs, string propName, int propValue)
        {
            SetProperty(schemaNs, propName, propValue, null);
        }

        /// <seealso cref="IXmpMeta.GetPropertyLong(string, string)"/>
        /// <exception cref="XmpException"/>
        public virtual long GetPropertyLong(string schemaNs, string propName)
        {
            return (long)GetPropertyObject(schemaNs, propName, ValueLong);
        }

        /// <seealso cref="IXmpMeta.SetPropertyLong(string, string, long, Com.Adobe.Xmp.Options.PropertyOptions)"/>
        /// <exception cref="XmpException"/>
        public virtual void SetPropertyLong(string schemaNs, string propName, long propValue, PropertyOptions options)
        {
            SetProperty(schemaNs, propName, propValue, options);
        }

        /// <seealso cref="IXmpMeta.SetPropertyLong(string, string, long)"/>
        /// <exception cref="XmpException"/>
        public virtual void SetPropertyLong(string schemaNs, string propName, long propValue)
        {
            SetProperty(schemaNs, propName, propValue, null);
        }

        /// <seealso cref="IXmpMeta.GetPropertyDouble(string, string)"/>
        /// <exception cref="XmpException"/>
        public virtual double GetPropertyDouble(string schemaNs, string propName)
        {
            return (double)GetPropertyObject(schemaNs, propName, ValueDouble);
        }

        /// <seealso cref="IXmpMeta.SetPropertyDouble(string, string, double, Com.Adobe.Xmp.Options.PropertyOptions)"/>
        /// <exception cref="XmpException"/>
        public virtual void SetPropertyDouble(string schemaNs, string propName, double propValue, PropertyOptions options)
        {
            SetProperty(schemaNs, propName, propValue, options);
        }

        /// <seealso cref="IXmpMeta.SetPropertyDouble(string, string, double)"/>
        /// <exception cref="XmpException"/>
        public virtual void SetPropertyDouble(string schemaNs, string propName, double propValue)
        {
            SetProperty(schemaNs, propName, propValue, null);
        }

        /// <seealso cref="IXmpMeta.GetPropertyDate(string, string)"/>
        /// <exception cref="XmpException"/>
        public virtual IXmpDateTime GetPropertyDate(string schemaNs, string propName)
        {
            return (IXmpDateTime)GetPropertyObject(schemaNs, propName, ValueDate);
        }

        /// <seealso cref="IXmpMeta.SetPropertyDate(string, string, IXmpDateTime, Com.Adobe.Xmp.Options.PropertyOptions)"/>
        /// <exception cref="XmpException"/>
        public virtual void SetPropertyDate(string schemaNs, string propName, IXmpDateTime propValue, PropertyOptions options)
        {
            SetProperty(schemaNs, propName, propValue, options);
        }

        /// <seealso cref="IXmpMeta.SetPropertyDate(string, string, IXmpDateTime)"/>
        /// <exception cref="XmpException"/>
        public virtual void SetPropertyDate(string schemaNs, string propName, IXmpDateTime propValue)
        {
            SetProperty(schemaNs, propName, propValue, null);
        }

        /// <seealso cref="IXmpMeta.GetPropertyCalendar(string, string)"/>
        /// <exception cref="XmpException"/>
        public virtual Calendar GetPropertyCalendar(string schemaNs, string propName)
        {
            return (Calendar)GetPropertyObject(schemaNs, propName, ValueCalendar);
        }

        /// <seealso cref="IXmpMeta.SetPropertyCalendar(string, string, Sharpen.Calendar, Com.Adobe.Xmp.Options.PropertyOptions)"/>
        /// <exception cref="XmpException"/>
        public virtual void SetPropertyCalendar(string schemaNs, string propName, Calendar propValue, PropertyOptions options)
        {
            SetProperty(schemaNs, propName, propValue, options);
        }

        /// <seealso cref="IXmpMeta.SetPropertyCalendar(string, string, Sharpen.Calendar)"/>
        /// <exception cref="XmpException"/>
        public virtual void SetPropertyCalendar(string schemaNs, string propName, Calendar propValue)
        {
            SetProperty(schemaNs, propName, propValue, null);
        }

        /// <seealso cref="IXmpMeta.GetPropertyBase64(string, string)"/>
        /// <exception cref="XmpException"/>
        public virtual sbyte[] GetPropertyBase64(string schemaNs, string propName)
        {
            return (sbyte[])GetPropertyObject(schemaNs, propName, ValueBase64);
        }

        /// <seealso cref="IXmpMeta.GetPropertyString(string, string)"/>
        /// <exception cref="XmpException"/>
        public virtual string GetPropertyString(string schemaNs, string propName)
        {
            return (string)GetPropertyObject(schemaNs, propName, ValueString);
        }

        /// <seealso cref="IXmpMeta.SetPropertyBase64(string, string, sbyte[], Com.Adobe.Xmp.Options.PropertyOptions)"/>
        /// <exception cref="XmpException"/>
        public virtual void SetPropertyBase64(string schemaNs, string propName, sbyte[] propValue, PropertyOptions options)
        {
            SetProperty(schemaNs, propName, propValue, options);
        }

        /// <seealso cref="IXmpMeta.SetPropertyBase64(string, string, sbyte[])"/>
        /// <exception cref="XmpException"/>
        public virtual void SetPropertyBase64(string schemaNs, string propName, sbyte[] propValue)
        {
            SetProperty(schemaNs, propName, propValue, null);
        }

        /// <exception cref="XmpException"/>
        /// <seealso cref="IXmpMeta.GetQualifier(string, string, string, string)"/>
        public virtual IXmpProperty GetQualifier(string schemaNs, string propName, string qualNs, string qualName)
        {
            // qualNS and qualName are checked inside composeQualfierPath
            ParameterAsserts.AssertSchemaNs(schemaNs);
            ParameterAsserts.AssertPropName(propName);
            string qualPath = propName + XmpPathFactory.ComposeQualifierPath(qualNs, qualName);
            return GetProperty(schemaNs, qualPath);
        }

        /// <seealso cref="IXmpMeta.GetStructField(string, string, string, string)"/>
        /// <exception cref="XmpException"/>
        public virtual IXmpProperty GetStructField(string schemaNs, string structName, string fieldNs, string fieldName)
        {
            // fieldNS and fieldName are checked inside composeStructFieldPath
            ParameterAsserts.AssertSchemaNs(schemaNs);
            ParameterAsserts.AssertStructName(structName);
            string fieldPath = structName + XmpPathFactory.ComposeStructFieldPath(fieldNs, fieldName);
            return GetProperty(schemaNs, fieldPath);
        }

        /// <exception cref="XmpException"/>
        /// <seealso cref="IXmpMeta.Iterator()"/>
        public virtual IXmpIterator Iterator()
        {
            return Iterator(null, null, null);
        }

        /// <seealso cref="IXmpMeta.Iterator(Com.Adobe.Xmp.Options.IteratorOptions)"/>
        /// <exception cref="XmpException"/>
        public virtual IXmpIterator Iterator(IteratorOptions options)
        {
            return Iterator(null, null, options);
        }

        /// <seealso cref="IXmpMeta.Iterator(string, string, Com.Adobe.Xmp.Options.IteratorOptions)"/>
        /// <exception cref="XmpException"/>
        public virtual IXmpIterator Iterator(string schemaNs, string propName, IteratorOptions options)
        {
            return new XmpIterator(this, schemaNs, propName, options);
        }

        /// <exception cref="XmpException"/>
        /// <seealso cref="IXmpMeta.SetArrayItem(string, string, int, string, Com.Adobe.Xmp.Options.PropertyOptions)"/>
        public virtual void SetArrayItem(string schemaNs, string arrayName, int itemIndex, string itemValue, PropertyOptions options)
        {
            ParameterAsserts.AssertSchemaNs(schemaNs);
            ParameterAsserts.AssertArrayName(arrayName);
            // Just lookup, don't try to create.
            XmpPath arrayPath = XmpPathParser.ExpandXPath(schemaNs, arrayName);
            XmpNode arrayNode = XmpNodeUtils.FindNode(_tree, arrayPath, false, null);
            if (arrayNode != null)
            {
                DoSetArrayItem(arrayNode, itemIndex, itemValue, options, false);
            }
            else
            {
                throw new XmpException("Specified array does not exist", XmpErrorCode.Badxpath);
            }
        }

        /// <seealso cref="IXmpMeta.SetArrayItem(string, string, int, string)"/>
        /// <exception cref="XmpException"/>
        public virtual void SetArrayItem(string schemaNs, string arrayName, int itemIndex, string itemValue)
        {
            SetArrayItem(schemaNs, arrayName, itemIndex, itemValue, null);
        }

        /// <exception cref="XmpException"/>
        /// <seealso cref="IXmpMeta.InsertArrayItem(string, string, int, string, Com.Adobe.Xmp.Options.PropertyOptions)"/>
        public virtual void InsertArrayItem(string schemaNs, string arrayName, int itemIndex, string itemValue, PropertyOptions options)
        {
            ParameterAsserts.AssertSchemaNs(schemaNs);
            ParameterAsserts.AssertArrayName(arrayName);
            // Just lookup, don't try to create.
            XmpPath arrayPath = XmpPathParser.ExpandXPath(schemaNs, arrayName);
            XmpNode arrayNode = XmpNodeUtils.FindNode(_tree, arrayPath, false, null);
            if (arrayNode != null)
            {
                DoSetArrayItem(arrayNode, itemIndex, itemValue, options, true);
            }
            else
            {
                throw new XmpException("Specified array does not exist", XmpErrorCode.Badxpath);
            }
        }

        /// <seealso cref="IXmpMeta.InsertArrayItem(string, string, int, string)"/>
        /// <exception cref="XmpException"/>
        public virtual void InsertArrayItem(string schemaNs, string arrayName, int itemIndex, string itemValue)
        {
            InsertArrayItem(schemaNs, arrayName, itemIndex, itemValue, null);
        }

        /// <exception cref="XmpException"/>
        /// <seealso cref="IXmpMeta.SetProperty(string, string, object, Com.Adobe.Xmp.Options.PropertyOptions)"/>
        public virtual void SetProperty(string schemaNs, string propName, object propValue, PropertyOptions options)
        {
            ParameterAsserts.AssertSchemaNs(schemaNs);
            ParameterAsserts.AssertPropName(propName);
            options = XmpNodeUtils.VerifySetOptions(options, propValue);
            XmpPath expPath = XmpPathParser.ExpandXPath(schemaNs, propName);
            XmpNode propNode = XmpNodeUtils.FindNode(_tree, expPath, true, options);
            if (propNode != null)
            {
                SetNode(propNode, propValue, options, false);
            }
            else
            {
                throw new XmpException("Specified property does not exist", XmpErrorCode.Badxpath);
            }
        }

        /// <seealso cref="IXmpMeta.SetProperty(string, string, object)"/>
        /// <exception cref="XmpException"/>
        public virtual void SetProperty(string schemaNs, string propName, object propValue)
        {
            SetProperty(schemaNs, propName, propValue, null);
        }

        /// <exception cref="XmpException"/>
        /// <seealso cref="IXmpMeta.SetQualifier(string, string, string, string, string, Com.Adobe.Xmp.Options.PropertyOptions)"/>
        public virtual void SetQualifier(string schemaNs, string propName, string qualNs, string qualName, string qualValue, PropertyOptions options)
        {
            ParameterAsserts.AssertSchemaNs(schemaNs);
            ParameterAsserts.AssertPropName(propName);
            if (!DoesPropertyExist(schemaNs, propName))
            {
                throw new XmpException("Specified property does not exist!", XmpErrorCode.Badxpath);
            }
            string qualPath = propName + XmpPathFactory.ComposeQualifierPath(qualNs, qualName);
            SetProperty(schemaNs, qualPath, qualValue, options);
        }

        /// <seealso cref="IXmpMeta.SetQualifier(string, string, string, string, string)"/>
        /// <exception cref="XmpException"/>
        public virtual void SetQualifier(string schemaNs, string propName, string qualNs, string qualName, string qualValue)
        {
            SetQualifier(schemaNs, propName, qualNs, qualName, qualValue, null);
        }

        /// <seealso cref="IXmpMeta.SetStructField(string, string, string, string, string, Com.Adobe.Xmp.Options.PropertyOptions)"/>
        /// <exception cref="XmpException"/>
        public virtual void SetStructField(string schemaNs, string structName, string fieldNs, string fieldName, string fieldValue, PropertyOptions options)
        {
            ParameterAsserts.AssertSchemaNs(schemaNs);
            ParameterAsserts.AssertStructName(structName);
            string fieldPath = structName + XmpPathFactory.ComposeStructFieldPath(fieldNs, fieldName);
            SetProperty(schemaNs, fieldPath, fieldValue, options);
        }

        /// <seealso cref="IXmpMeta.SetStructField(string, string, string, string, string)"/>
        /// <exception cref="XmpException"/>
        public virtual void SetStructField(string schemaNs, string structName, string fieldNs, string fieldName, string fieldValue)
        {
            SetStructField(schemaNs, structName, fieldNs, fieldName, fieldValue, null);
        }

        /// <seealso cref="IXmpMeta.GetObjectName()"/>
        public virtual string GetObjectName()
        {
            return _tree.GetName() != null ? _tree.GetName() : string.Empty;
        }

        /// <seealso cref="IXmpMeta.SetObjectName(string)"/>
        public virtual void SetObjectName(string name)
        {
            _tree.SetName(name);
        }

        /// <seealso cref="IXmpMeta.GetPacketHeader()"/>
        public virtual string GetPacketHeader()
        {
            return _packetHeader;
        }

        /// <summary>Sets the packetHeader attributes, only used by the parser.</summary>
        /// <param name="packetHeader">the processing instruction content</param>
        public virtual void SetPacketHeader(string packetHeader)
        {
            _packetHeader = packetHeader;
        }

        /// <summary>Performs a deep clone of the XMPMeta-object</summary>
        public virtual object Clone()
        {
            XmpNode clonedTree = (XmpNode)_tree.Clone();
            return new XmpMeta(clonedTree);
        }

        /// <seealso cref="IXmpMeta.DumpObject()"/>
        public virtual string DumpObject()
        {
            // renders tree recursively
            return GetRoot().DumpNode(true);
        }

        /// <seealso cref="IXmpMeta.Sort()"/>
        public virtual void Sort()
        {
            _tree.Sort();
        }

        /// <seealso cref="IXmpMeta.Normalize(Com.Adobe.Xmp.Options.ParseOptions)"/>
        /// <exception cref="XmpException"/>
        public virtual void Normalize(ParseOptions options)
        {
            if (options == null)
            {
                options = new ParseOptions();
            }
            XmpNormalizer.Process(this, options);
        }

        /// <returns>Returns the root node of the XMP tree.</returns>
        public virtual XmpNode GetRoot()
        {
            return _tree;
        }

        // -------------------------------------------------------------------------------------
        // private
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
            XmpNode itemNode = new XmpNode(XmpConstConstants.ArrayItemName, null);
            itemOptions = XmpNodeUtils.VerifySetOptions(itemOptions, itemValue);
            // in insert mode the index after the last is allowed,
            // even ARRAY_LAST_ITEM points to the index *after* the last.
            int maxIndex = insert ? arrayNode.GetChildrenLength() + 1 : arrayNode.GetChildrenLength();
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
                throw new XmpException("Array index out of bounds", XmpErrorCode.Badindex);
            }
        }

        /// <summary>
        /// The internals for setProperty() and related calls, used after the node is
        /// found or created.
        /// </summary>
        /// <param name="node">the newly created node</param>
        /// <param name="value">the node value, can be <code>null</code></param>
        /// <param name="newOptions">options for the new node, must not be <code>null</code>.</param>
        /// <param name="deleteExisting">flag if the existing value is to be overwritten</param>
        /// <exception cref="XmpException">thrown if options and value do not correspond</exception>
        internal virtual void SetNode(XmpNode node, object value, PropertyOptions newOptions, bool deleteExisting)
        {
            if (deleteExisting)
            {
                node.Clear();
            }
            // its checked by setOptions(), if the merged result is a valid options set
            node.GetOptions().MergeWith(newOptions);
            if (!node.GetOptions().IsCompositeProperty())
            {
                // This is setting the value of a leaf node.
                XmpNodeUtils.SetNodeValue(node, value);
            }
            else
            {
                if (value != null && value.ToString().Length > 0)
                {
                    throw new XmpException("Composite nodes can't have values", XmpErrorCode.Badxpath);
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
        private static object EvaluateNodeValue(int valueType, XmpNode propNode)
        {
            object value;
            string rawValue = propNode.GetValue();
            switch (valueType)
            {
                case ValueBoolean:
                {
                    value = Xmp.XmpUtils.ConvertToBoolean(rawValue);
                    break;
                }

                case ValueInteger:
                {
                    value = Xmp.XmpUtils.ConvertToInteger(rawValue);
                    break;
                }

                case ValueLong:
                {
                    value = Xmp.XmpUtils.ConvertToLong(rawValue);
                    break;
                }

                case ValueDouble:
                {
                    value = Xmp.XmpUtils.ConvertToDouble(rawValue);
                    break;
                }

                case ValueDate:
                {
                    value = Xmp.XmpUtils.ConvertToDate(rawValue);
                    break;
                }

                case ValueCalendar:
                {
                    IXmpDateTime dt = Xmp.XmpUtils.ConvertToDate(rawValue);
                    value = dt.GetCalendar();
                    break;
                }

                case ValueBase64:
                {
                    value = Xmp.XmpUtils.DecodeBase64(rawValue);
                    break;
                }

                case ValueString:
                default:
                {
                    // leaf values return empty string instead of null
                    // for the other cases the converter methods provides a "null"
                    // value.
                    // a default value can only occur if this method is made public.
                    value = rawValue != null || propNode.GetOptions().IsCompositeProperty() ? rawValue : string.Empty;
                    break;
                }
            }
            return value;
        }
    }
}
