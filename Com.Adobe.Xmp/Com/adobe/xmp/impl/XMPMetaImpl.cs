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
    /// Implementation for
    /// <see cref="Com.Adobe.Xmp.XMPMeta"/>
    /// .
    /// </summary>
    /// <since>17.02.2006</since>
    public class XMPMetaImpl : XMPMeta, XMPConst
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
        private readonly XMPNode tree;

        /// <summary>the xpacket processing instructions content</summary>
        private string packetHeader = null;

        /// <summary>Constructor for an empty metadata object.</summary>
        public XMPMetaImpl()
        {
            // create root node
            tree = new XMPNode(null, null, null);
        }

        /// <summary>Constructor for a cloned metadata tree.</summary>
        /// <param name="tree">
        /// an prefilled metadata tree which fulfills all
        /// <code>XMPNode</code> contracts.
        /// </param>
        public XMPMetaImpl(XMPNode tree)
        {
            this.tree = tree;
        }

        /// <seealso cref="Com.Adobe.Xmp.XMPMeta.AppendArrayItem(string, string, Com.Adobe.Xmp.Options.PropertyOptions, string, Com.Adobe.Xmp.Options.PropertyOptions)"/>
        /// <exception cref="Com.Adobe.Xmp.XMPException"/>
        public virtual void AppendArrayItem(string schemaNS, string arrayName, PropertyOptions arrayOptions, string itemValue, PropertyOptions itemOptions)
        {
            ParameterAsserts.AssertSchemaNS(schemaNS);
            ParameterAsserts.AssertArrayName(arrayName);
            if (arrayOptions == null)
            {
                arrayOptions = new PropertyOptions();
            }
            if (!arrayOptions.IsOnlyArrayOptions())
            {
                throw new XMPException("Only array form flags allowed for arrayOptions", XMPErrorConstants.Badoptions);
            }
            // Check if array options are set correctly.
            arrayOptions = XMPNodeUtils.VerifySetOptions(arrayOptions, null);
            // Locate or create the array. If it already exists, make sure the array
            // form from the options
            // parameter is compatible with the current state.
            XMPPath arrayPath = XMPPathParser.ExpandXPath(schemaNS, arrayName);
            // Just lookup, don't try to create.
            XMPNode arrayNode = XMPNodeUtils.FindNode(tree, arrayPath, false, null);
            if (arrayNode != null)
            {
                // The array exists, make sure the form is compatible. Zero
                // arrayForm means take what exists.
                if (!arrayNode.GetOptions().IsArray())
                {
                    throw new XMPException("The named property is not an array", XMPErrorConstants.Badxpath);
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
                    arrayNode = XMPNodeUtils.FindNode(tree, arrayPath, true, arrayOptions);
                    if (arrayNode == null)
                    {
                        throw new XMPException("Failure creating array node", XMPErrorConstants.Badxpath);
                    }
                }
                else
                {
                    // array options missing
                    throw new XMPException("Explicit arrayOptions required to create new array", XMPErrorConstants.Badoptions);
                }
            }
            DoSetArrayItem(arrayNode, XMPConstConstants.ArrayLastItem, itemValue, itemOptions, true);
        }

        /// <seealso cref="Com.Adobe.Xmp.XMPMeta.AppendArrayItem(string, string, string)"/>
        /// <exception cref="Com.Adobe.Xmp.XMPException"/>
        public virtual void AppendArrayItem(string schemaNS, string arrayName, string itemValue)
        {
            AppendArrayItem(schemaNS, arrayName, null, itemValue, null);
        }

        /// <exception cref="Com.Adobe.Xmp.XMPException"/>
        /// <seealso cref="Com.Adobe.Xmp.XMPMeta.CountArrayItems(string, string)"/>
        public virtual int CountArrayItems(string schemaNS, string arrayName)
        {
            ParameterAsserts.AssertSchemaNS(schemaNS);
            ParameterAsserts.AssertArrayName(arrayName);
            XMPPath arrayPath = XMPPathParser.ExpandXPath(schemaNS, arrayName);
            XMPNode arrayNode = XMPNodeUtils.FindNode(tree, arrayPath, false, null);
            if (arrayNode == null)
            {
                return 0;
            }
            if (arrayNode.GetOptions().IsArray())
            {
                return arrayNode.GetChildrenLength();
            }
            else
            {
                throw new XMPException("The named property is not an array", XMPErrorConstants.Badxpath);
            }
        }

        /// <seealso cref="Com.Adobe.Xmp.XMPMeta.DeleteArrayItem(string, string, int)"/>
        public virtual void DeleteArrayItem(string schemaNS, string arrayName, int itemIndex)
        {
            try
            {
                ParameterAsserts.AssertSchemaNS(schemaNS);
                ParameterAsserts.AssertArrayName(arrayName);
                string itemPath = XMPPathFactory.ComposeArrayItemPath(arrayName, itemIndex);
                DeleteProperty(schemaNS, itemPath);
            }
            catch (XMPException)
            {
            }
        }

        // EMPTY, exceptions are ignored within delete
        /// <seealso cref="Com.Adobe.Xmp.XMPMeta.DeleteProperty(string, string)"/>
        public virtual void DeleteProperty(string schemaNS, string propName)
        {
            try
            {
                ParameterAsserts.AssertSchemaNS(schemaNS);
                ParameterAsserts.AssertPropName(propName);
                XMPPath expPath = XMPPathParser.ExpandXPath(schemaNS, propName);
                XMPNode propNode = XMPNodeUtils.FindNode(tree, expPath, false, null);
                if (propNode != null)
                {
                    XMPNodeUtils.DeleteNode(propNode);
                }
            }
            catch (XMPException)
            {
            }
        }

        // EMPTY, exceptions are ignored within delete
        /// <seealso cref="Com.Adobe.Xmp.XMPMeta.DeleteQualifier(string, string, string, string)"/>
        public virtual void DeleteQualifier(string schemaNS, string propName, string qualNS, string qualName)
        {
            try
            {
                // Note: qualNS and qualName are checked inside composeQualfierPath
                ParameterAsserts.AssertSchemaNS(schemaNS);
                ParameterAsserts.AssertPropName(propName);
                string qualPath = propName + XMPPathFactory.ComposeQualifierPath(qualNS, qualName);
                DeleteProperty(schemaNS, qualPath);
            }
            catch (XMPException)
            {
            }
        }

        // EMPTY, exceptions within delete are ignored
        /// <seealso cref="Com.Adobe.Xmp.XMPMeta.DeleteStructField(string, string, string, string)"/>
        public virtual void DeleteStructField(string schemaNS, string structName, string fieldNS, string fieldName)
        {
            try
            {
                // fieldNS and fieldName are checked inside composeStructFieldPath
                ParameterAsserts.AssertSchemaNS(schemaNS);
                ParameterAsserts.AssertStructName(structName);
                string fieldPath = structName + XMPPathFactory.ComposeStructFieldPath(fieldNS, fieldName);
                DeleteProperty(schemaNS, fieldPath);
            }
            catch (XMPException)
            {
            }
        }

        // EMPTY, exceptions within delete are ignored
        /// <seealso cref="Com.Adobe.Xmp.XMPMeta.DoesPropertyExist(string, string)"/>
        public virtual bool DoesPropertyExist(string schemaNS, string propName)
        {
            try
            {
                ParameterAsserts.AssertSchemaNS(schemaNS);
                ParameterAsserts.AssertPropName(propName);
                XMPPath expPath = XMPPathParser.ExpandXPath(schemaNS, propName);
                XMPNode propNode = XMPNodeUtils.FindNode(tree, expPath, false, null);
                return propNode != null;
            }
            catch (XMPException)
            {
                return false;
            }
        }

        /// <seealso cref="Com.Adobe.Xmp.XMPMeta.DoesArrayItemExist(string, string, int)"/>
        public virtual bool DoesArrayItemExist(string schemaNS, string arrayName, int itemIndex)
        {
            try
            {
                ParameterAsserts.AssertSchemaNS(schemaNS);
                ParameterAsserts.AssertArrayName(arrayName);
                string path = XMPPathFactory.ComposeArrayItemPath(arrayName, itemIndex);
                return DoesPropertyExist(schemaNS, path);
            }
            catch (XMPException)
            {
                return false;
            }
        }

        /// <seealso cref="Com.Adobe.Xmp.XMPMeta.DoesStructFieldExist(string, string, string, string)"/>
        public virtual bool DoesStructFieldExist(string schemaNS, string structName, string fieldNS, string fieldName)
        {
            try
            {
                // fieldNS and fieldName are checked inside composeStructFieldPath()
                ParameterAsserts.AssertSchemaNS(schemaNS);
                ParameterAsserts.AssertStructName(structName);
                string path = XMPPathFactory.ComposeStructFieldPath(fieldNS, fieldName);
                return DoesPropertyExist(schemaNS, structName + path);
            }
            catch (XMPException)
            {
                return false;
            }
        }

        /// <seealso cref="Com.Adobe.Xmp.XMPMeta.DoesQualifierExist(string, string, string, string)"/>
        public virtual bool DoesQualifierExist(string schemaNS, string propName, string qualNS, string qualName)
        {
            try
            {
                // qualNS and qualName are checked inside composeQualifierPath()
                ParameterAsserts.AssertSchemaNS(schemaNS);
                ParameterAsserts.AssertPropName(propName);
                string path = XMPPathFactory.ComposeQualifierPath(qualNS, qualName);
                return DoesPropertyExist(schemaNS, propName + path);
            }
            catch (XMPException)
            {
                return false;
            }
        }

        /// <seealso cref="Com.Adobe.Xmp.XMPMeta.GetArrayItem(string, string, int)"/>
        /// <exception cref="Com.Adobe.Xmp.XMPException"/>
        public virtual XMPProperty GetArrayItem(string schemaNS, string arrayName, int itemIndex)
        {
            ParameterAsserts.AssertSchemaNS(schemaNS);
            ParameterAsserts.AssertArrayName(arrayName);
            string itemPath = XMPPathFactory.ComposeArrayItemPath(arrayName, itemIndex);
            return GetProperty(schemaNS, itemPath);
        }

        /// <exception cref="Com.Adobe.Xmp.XMPException"/>
        /// <seealso cref="Com.Adobe.Xmp.XMPMeta.GetLocalizedText(string, string, string, string)"/>
        public virtual XMPProperty GetLocalizedText(string schemaNS, string altTextName, string genericLang, string specificLang)
        {
            ParameterAsserts.AssertSchemaNS(schemaNS);
            ParameterAsserts.AssertArrayName(altTextName);
            ParameterAsserts.AssertSpecificLang(specificLang);
            genericLang = genericLang != null ? Utils.NormalizeLangValue(genericLang) : null;
            specificLang = Utils.NormalizeLangValue(specificLang);
            XMPPath arrayPath = XMPPathParser.ExpandXPath(schemaNS, altTextName);
            XMPNode arrayNode = XMPNodeUtils.FindNode(tree, arrayPath, false, null);
            if (arrayNode == null)
            {
                return null;
            }
            object[] result = XMPNodeUtils.ChooseLocalizedText(arrayNode, genericLang, specificLang);
            int match = ((int)result[0]).IntValue();
            XMPNode itemNode = (XMPNode)result[1];
            if (match != XMPNodeUtils.CltNoValues)
            {
                return new _XMPProperty_407(itemNode);
            }
            else
            {
                return null;
            }
        }

        private sealed class _XMPProperty_407 : XMPProperty
        {
            public _XMPProperty_407(XMPNode itemNode)
            {
                this.itemNode = itemNode;
            }

            public string GetValue()
            {
                return itemNode.GetValue();
            }

            public PropertyOptions GetOptions()
            {
                return itemNode.GetOptions();
            }

            public string GetLanguage()
            {
                return itemNode.GetQualifier(1).GetValue();
            }

            public override string ToString()
            {
                return itemNode.GetValue().ToString();
            }

            private readonly XMPNode itemNode;
        }

        /// <seealso cref="Com.Adobe.Xmp.XMPMeta.SetLocalizedText(string, string, string, string, string, Com.Adobe.Xmp.Options.PropertyOptions)"/>
        /// <exception cref="Com.Adobe.Xmp.XMPException"/>
        public virtual void SetLocalizedText(string schemaNS, string altTextName, string genericLang, string specificLang, string itemValue, PropertyOptions options)
        {
            ParameterAsserts.AssertSchemaNS(schemaNS);
            ParameterAsserts.AssertArrayName(altTextName);
            ParameterAsserts.AssertSpecificLang(specificLang);
            genericLang = genericLang != null ? Utils.NormalizeLangValue(genericLang) : null;
            specificLang = Utils.NormalizeLangValue(specificLang);
            XMPPath arrayPath = XMPPathParser.ExpandXPath(schemaNS, altTextName);
            // Find the array node and set the options if it was just created.
            XMPNode arrayNode = XMPNodeUtils.FindNode(tree, arrayPath, true, new PropertyOptions(PropertyOptions.Array | PropertyOptions.ArrayOrdered | PropertyOptions.ArrayAlternate | PropertyOptions.ArrayAltText));
            if (arrayNode == null)
            {
                throw new XMPException("Failed to find or create array node", XMPErrorConstants.Badxpath);
            }
            else
            {
                if (!arrayNode.GetOptions().IsArrayAltText())
                {
                    if (!arrayNode.HasChildren() && arrayNode.GetOptions().IsArrayAlternate())
                    {
                        arrayNode.GetOptions().SetArrayAltText(true);
                    }
                    else
                    {
                        throw new XMPException("Specified property is no alt-text array", XMPErrorConstants.Badxpath);
                    }
                }
            }
            // Make sure the x-default item, if any, is first.
            bool haveXDefault = false;
            XMPNode xdItem = null;
            for (Iterator it = arrayNode.IterateChildren(); it.HasNext(); )
            {
                XMPNode currItem = (XMPNode)it.Next();
                if (!currItem.HasQualifier() || !XMPConstConstants.XmlLang.Equals(currItem.GetQualifier(1).GetName()))
                {
                    throw new XMPException("Language qualifier must be first", XMPErrorConstants.Badxpath);
                }
                else
                {
                    if (XMPConstConstants.XDefault.Equals(currItem.GetQualifier(1).GetValue()))
                    {
                        xdItem = currItem;
                        haveXDefault = true;
                        break;
                    }
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
            object[] result = XMPNodeUtils.ChooseLocalizedText(arrayNode, genericLang, specificLang);
            int match = ((int)result[0]).IntValue();
            XMPNode itemNode = (XMPNode)result[1];
            bool specificXDefault = XMPConstConstants.XDefault.Equals(specificLang);
            switch (match)
            {
                case XMPNodeUtils.CltNoValues:
                {
                    // Create the array items for the specificLang and x-default, with
                    // x-default first.
                    XMPNodeUtils.AppendLangItem(arrayNode, XMPConstConstants.XDefault, itemValue);
                    haveXDefault = true;
                    if (!specificXDefault)
                    {
                        XMPNodeUtils.AppendLangItem(arrayNode, specificLang, itemValue);
                    }
                    break;
                }

                case XMPNodeUtils.CltSpecificMatch:
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
                        for (Iterator it_1 = arrayNode.IterateChildren(); it_1.HasNext(); )
                        {
                            XMPNode currItem = (XMPNode)it_1.Next();
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

                case XMPNodeUtils.CltSingleGeneric:
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

                case XMPNodeUtils.CltMultipleGeneric:
                {
                    // Create the specific language, ignore x-default.
                    XMPNodeUtils.AppendLangItem(arrayNode, specificLang, itemValue);
                    if (specificXDefault)
                    {
                        haveXDefault = true;
                    }
                    break;
                }

                case XMPNodeUtils.CltXdefault:
                {
                    // Create the specific language, update x-default if it was the only
                    // item.
                    if (xdItem != null && arrayNode.GetChildrenLength() == 1)
                    {
                        xdItem.SetValue(itemValue);
                    }
                    XMPNodeUtils.AppendLangItem(arrayNode, specificLang, itemValue);
                    break;
                }

                case XMPNodeUtils.CltFirstItem:
                {
                    // Create the specific language, don't add an x-default item.
                    XMPNodeUtils.AppendLangItem(arrayNode, specificLang, itemValue);
                    if (specificXDefault)
                    {
                        haveXDefault = true;
                    }
                    break;
                }

                default:
                {
                    // does not happen under normal circumstances
                    throw new XMPException("Unexpected result from ChooseLocalizedText", XMPErrorConstants.Internalfailure);
                }
            }
            // Add an x-default at the front if needed.
            if (!haveXDefault && arrayNode.GetChildrenLength() == 1)
            {
                XMPNodeUtils.AppendLangItem(arrayNode, XMPConstConstants.XDefault, itemValue);
            }
        }

        /// <seealso cref="Com.Adobe.Xmp.XMPMeta.SetLocalizedText(string, string, string, string, string)"/>
        /// <exception cref="Com.Adobe.Xmp.XMPException"/>
        public virtual void SetLocalizedText(string schemaNS, string altTextName, string genericLang, string specificLang, string itemValue)
        {
            SetLocalizedText(schemaNS, altTextName, genericLang, specificLang, itemValue, null);
        }

        /// <exception cref="Com.Adobe.Xmp.XMPException"/>
        /// <seealso cref="Com.Adobe.Xmp.XMPMeta.GetProperty(string, string)"/>
        public virtual XMPProperty GetProperty(string schemaNS, string propName)
        {
            return GetProperty(schemaNS, propName, ValueString);
        }

        /// <summary>Returns a property, but the result value can be requested.</summary>
        /// <remarks>
        /// Returns a property, but the result value can be requested. It can be one
        /// of
        /// <see cref="ValueString"/>
        /// ,
        /// <see cref="ValueBoolean"/>
        /// ,
        /// <see cref="ValueInteger"/>
        /// ,
        /// <see cref="ValueLong"/>
        /// ,
        /// <see cref="ValueDouble"/>
        /// ,
        /// <see cref="ValueDate"/>
        /// ,
        /// <see cref="ValueCalendar"/>
        /// ,
        /// <see cref="ValueBase64"/>
        /// .
        /// </remarks>
        /// <seealso cref="Com.Adobe.Xmp.XMPMeta.GetProperty(string, string)"/>
        /// <param name="schemaNS">a schema namespace</param>
        /// <param name="propName">a property name or path</param>
        /// <param name="valueType">the type of the value, see VALUE_...</param>
        /// <returns>Returns an <code>XMPProperty</code></returns>
        /// <exception cref="Com.Adobe.Xmp.XMPException">Collects any exception that occurs.</exception>
        protected internal virtual XMPProperty GetProperty(string schemaNS, string propName, int valueType)
        {
            ParameterAsserts.AssertSchemaNS(schemaNS);
            ParameterAsserts.AssertPropName(propName);
            XMPPath expPath = XMPPathParser.ExpandXPath(schemaNS, propName);
            XMPNode propNode = XMPNodeUtils.FindNode(tree, expPath, false, null);
            if (propNode != null)
            {
                if (valueType != ValueString && propNode.GetOptions().IsCompositeProperty())
                {
                    throw new XMPException("Property must be simple when a value type is requested", XMPErrorConstants.Badxpath);
                }
                object value = EvaluateNodeValue(valueType, propNode);
                return new _XMPProperty_682(value, propNode);
            }
            else
            {
                return null;
            }
        }

        private sealed class _XMPProperty_682 : XMPProperty
        {
            public _XMPProperty_682(object value, XMPNode propNode)
            {
                this.value = value;
                this.propNode = propNode;
            }

            public string GetValue()
            {
                return value != null ? value.ToString() : null;
            }

            public PropertyOptions GetOptions()
            {
                return propNode.GetOptions();
            }

            public string GetLanguage()
            {
                return null;
            }

            public override string ToString()
            {
                return value.ToString();
            }

            private readonly object value;

            private readonly XMPNode propNode;
        }

        /// <summary>Returns a property, but the result value can be requested.</summary>
        /// <seealso cref="Com.Adobe.Xmp.XMPMeta.GetProperty(string, string)"/>
        /// <param name="schemaNS">a schema namespace</param>
        /// <param name="propName">a property name or path</param>
        /// <param name="valueType">the type of the value, see VALUE_...</param>
        /// <returns>
        /// Returns the node value as an object according to the
        /// <code>valueType</code>.
        /// </returns>
        /// <exception cref="Com.Adobe.Xmp.XMPException">Collects any exception that occurs.</exception>
        protected internal virtual object GetPropertyObject(string schemaNS, string propName, int valueType)
        {
            ParameterAsserts.AssertSchemaNS(schemaNS);
            ParameterAsserts.AssertPropName(propName);
            XMPPath expPath = XMPPathParser.ExpandXPath(schemaNS, propName);
            XMPNode propNode = XMPNodeUtils.FindNode(tree, expPath, false, null);
            if (propNode != null)
            {
                if (valueType != ValueString && propNode.GetOptions().IsCompositeProperty())
                {
                    throw new XMPException("Property must be simple when a value type is requested", XMPErrorConstants.Badxpath);
                }
                return EvaluateNodeValue(valueType, propNode);
            }
            else
            {
                return null;
            }
        }

        /// <seealso cref="Com.Adobe.Xmp.XMPMeta.GetPropertyBoolean(string, string)"/>
        /// <exception cref="Com.Adobe.Xmp.XMPException"/>
        public virtual bool GetPropertyBoolean(string schemaNS, string propName)
        {
            return (bool)GetPropertyObject(schemaNS, propName, ValueBoolean);
        }

        /// <exception cref="Com.Adobe.Xmp.XMPException"/>
        /// <seealso cref="Com.Adobe.Xmp.XMPMeta.SetPropertyBoolean(string, string, bool, Com.Adobe.Xmp.Options.PropertyOptions)"/>
        public virtual void SetPropertyBoolean(string schemaNS, string propName, bool propValue, PropertyOptions options)
        {
            SetProperty(schemaNS, propName, propValue ? XMPConstConstants.Truestr : XMPConstConstants.Falsestr, options);
        }

        /// <seealso cref="Com.Adobe.Xmp.XMPMeta.SetPropertyBoolean(string, string, bool)"/>
        /// <exception cref="Com.Adobe.Xmp.XMPException"/>
        public virtual void SetPropertyBoolean(string schemaNS, string propName, bool propValue)
        {
            SetProperty(schemaNS, propName, propValue ? XMPConstConstants.Truestr : XMPConstConstants.Falsestr, null);
        }

        /// <seealso cref="Com.Adobe.Xmp.XMPMeta.GetPropertyInteger(string, string)"/>
        /// <exception cref="Com.Adobe.Xmp.XMPException"/>
        public virtual int GetPropertyInteger(string schemaNS, string propName)
        {
            return (int)GetPropertyObject(schemaNS, propName, ValueInteger);
        }

        /// <seealso cref="Com.Adobe.Xmp.XMPMeta.SetPropertyInteger(string, string, int, Com.Adobe.Xmp.Options.PropertyOptions)"/>
        /// <exception cref="Com.Adobe.Xmp.XMPException"/>
        public virtual void SetPropertyInteger(string schemaNS, string propName, int propValue, PropertyOptions options)
        {
            SetProperty(schemaNS, propName, propValue, options);
        }

        /// <seealso cref="Com.Adobe.Xmp.XMPMeta.SetPropertyInteger(string, string, int)"/>
        /// <exception cref="Com.Adobe.Xmp.XMPException"/>
        public virtual void SetPropertyInteger(string schemaNS, string propName, int propValue)
        {
            SetProperty(schemaNS, propName, propValue, null);
        }

        /// <seealso cref="Com.Adobe.Xmp.XMPMeta.GetPropertyLong(string, string)"/>
        /// <exception cref="Com.Adobe.Xmp.XMPException"/>
        public virtual long GetPropertyLong(string schemaNS, string propName)
        {
            return (long)GetPropertyObject(schemaNS, propName, ValueLong);
        }

        /// <seealso cref="Com.Adobe.Xmp.XMPMeta.SetPropertyLong(string, string, long, Com.Adobe.Xmp.Options.PropertyOptions)"/>
        /// <exception cref="Com.Adobe.Xmp.XMPException"/>
        public virtual void SetPropertyLong(string schemaNS, string propName, long propValue, PropertyOptions options)
        {
            SetProperty(schemaNS, propName, propValue, options);
        }

        /// <seealso cref="Com.Adobe.Xmp.XMPMeta.SetPropertyLong(string, string, long)"/>
        /// <exception cref="Com.Adobe.Xmp.XMPException"/>
        public virtual void SetPropertyLong(string schemaNS, string propName, long propValue)
        {
            SetProperty(schemaNS, propName, propValue, null);
        }

        /// <seealso cref="Com.Adobe.Xmp.XMPMeta.GetPropertyDouble(string, string)"/>
        /// <exception cref="Com.Adobe.Xmp.XMPException"/>
        public virtual double GetPropertyDouble(string schemaNS, string propName)
        {
            return (double)GetPropertyObject(schemaNS, propName, ValueDouble);
        }

        /// <seealso cref="Com.Adobe.Xmp.XMPMeta.SetPropertyDouble(string, string, double, Com.Adobe.Xmp.Options.PropertyOptions)"/>
        /// <exception cref="Com.Adobe.Xmp.XMPException"/>
        public virtual void SetPropertyDouble(string schemaNS, string propName, double propValue, PropertyOptions options)
        {
            SetProperty(schemaNS, propName, propValue, options);
        }

        /// <seealso cref="Com.Adobe.Xmp.XMPMeta.SetPropertyDouble(string, string, double)"/>
        /// <exception cref="Com.Adobe.Xmp.XMPException"/>
        public virtual void SetPropertyDouble(string schemaNS, string propName, double propValue)
        {
            SetProperty(schemaNS, propName, propValue, null);
        }

        /// <seealso cref="Com.Adobe.Xmp.XMPMeta.GetPropertyDate(string, string)"/>
        /// <exception cref="Com.Adobe.Xmp.XMPException"/>
        public virtual XMPDateTime GetPropertyDate(string schemaNS, string propName)
        {
            return (XMPDateTime)GetPropertyObject(schemaNS, propName, ValueDate);
        }

        /// <seealso cref="Com.Adobe.Xmp.XMPMeta.SetPropertyDate(string, string, Com.Adobe.Xmp.XMPDateTime, Com.Adobe.Xmp.Options.PropertyOptions)"/>
        /// <exception cref="Com.Adobe.Xmp.XMPException"/>
        public virtual void SetPropertyDate(string schemaNS, string propName, XMPDateTime propValue, PropertyOptions options)
        {
            SetProperty(schemaNS, propName, propValue, options);
        }

        /// <seealso cref="Com.Adobe.Xmp.XMPMeta.SetPropertyDate(string, string, Com.Adobe.Xmp.XMPDateTime)"/>
        /// <exception cref="Com.Adobe.Xmp.XMPException"/>
        public virtual void SetPropertyDate(string schemaNS, string propName, XMPDateTime propValue)
        {
            SetProperty(schemaNS, propName, propValue, null);
        }

        /// <seealso cref="Com.Adobe.Xmp.XMPMeta.GetPropertyCalendar(string, string)"/>
        /// <exception cref="Com.Adobe.Xmp.XMPException"/>
        public virtual Calendar GetPropertyCalendar(string schemaNS, string propName)
        {
            return (Calendar)GetPropertyObject(schemaNS, propName, ValueCalendar);
        }

        /// <seealso cref="Com.Adobe.Xmp.XMPMeta.SetPropertyCalendar(string, string, Sharpen.Calendar, Com.Adobe.Xmp.Options.PropertyOptions)"/>
        /// <exception cref="Com.Adobe.Xmp.XMPException"/>
        public virtual void SetPropertyCalendar(string schemaNS, string propName, Calendar propValue, PropertyOptions options)
        {
            SetProperty(schemaNS, propName, propValue, options);
        }

        /// <seealso cref="Com.Adobe.Xmp.XMPMeta.SetPropertyCalendar(string, string, Sharpen.Calendar)"/>
        /// <exception cref="Com.Adobe.Xmp.XMPException"/>
        public virtual void SetPropertyCalendar(string schemaNS, string propName, Calendar propValue)
        {
            SetProperty(schemaNS, propName, propValue, null);
        }

        /// <seealso cref="Com.Adobe.Xmp.XMPMeta.GetPropertyBase64(string, string)"/>
        /// <exception cref="Com.Adobe.Xmp.XMPException"/>
        public virtual sbyte[] GetPropertyBase64(string schemaNS, string propName)
        {
            return (sbyte[])GetPropertyObject(schemaNS, propName, ValueBase64);
        }

        /// <seealso cref="Com.Adobe.Xmp.XMPMeta.GetPropertyString(string, string)"/>
        /// <exception cref="Com.Adobe.Xmp.XMPException"/>
        public virtual string GetPropertyString(string schemaNS, string propName)
        {
            return (string)GetPropertyObject(schemaNS, propName, ValueString);
        }

        /// <seealso cref="Com.Adobe.Xmp.XMPMeta.SetPropertyBase64(string, string, sbyte[], Com.Adobe.Xmp.Options.PropertyOptions)"/>
        /// <exception cref="Com.Adobe.Xmp.XMPException"/>
        public virtual void SetPropertyBase64(string schemaNS, string propName, sbyte[] propValue, PropertyOptions options)
        {
            SetProperty(schemaNS, propName, propValue, options);
        }

        /// <seealso cref="Com.Adobe.Xmp.XMPMeta.SetPropertyBase64(string, string, sbyte[])"/>
        /// <exception cref="Com.Adobe.Xmp.XMPException"/>
        public virtual void SetPropertyBase64(string schemaNS, string propName, sbyte[] propValue)
        {
            SetProperty(schemaNS, propName, propValue, null);
        }

        /// <exception cref="Com.Adobe.Xmp.XMPException"/>
        /// <seealso cref="Com.Adobe.Xmp.XMPMeta.GetQualifier(string, string, string, string)"/>
        public virtual XMPProperty GetQualifier(string schemaNS, string propName, string qualNS, string qualName)
        {
            // qualNS and qualName are checked inside composeQualfierPath
            ParameterAsserts.AssertSchemaNS(schemaNS);
            ParameterAsserts.AssertPropName(propName);
            string qualPath = propName + XMPPathFactory.ComposeQualifierPath(qualNS, qualName);
            return GetProperty(schemaNS, qualPath);
        }

        /// <seealso cref="Com.Adobe.Xmp.XMPMeta.GetStructField(string, string, string, string)"/>
        /// <exception cref="Com.Adobe.Xmp.XMPException"/>
        public virtual XMPProperty GetStructField(string schemaNS, string structName, string fieldNS, string fieldName)
        {
            // fieldNS and fieldName are checked inside composeStructFieldPath
            ParameterAsserts.AssertSchemaNS(schemaNS);
            ParameterAsserts.AssertStructName(structName);
            string fieldPath = structName + XMPPathFactory.ComposeStructFieldPath(fieldNS, fieldName);
            return GetProperty(schemaNS, fieldPath);
        }

        /// <exception cref="Com.Adobe.Xmp.XMPException"/>
        /// <seealso cref="Com.Adobe.Xmp.XMPMeta.Iterator()"/>
        public virtual XMPIterator Iterator()
        {
            return Iterator(null, null, null);
        }

        /// <seealso cref="Com.Adobe.Xmp.XMPMeta.Iterator(Com.Adobe.Xmp.Options.IteratorOptions)"/>
        /// <exception cref="Com.Adobe.Xmp.XMPException"/>
        public virtual XMPIterator Iterator(IteratorOptions options)
        {
            return Iterator(null, null, options);
        }

        /// <seealso cref="Com.Adobe.Xmp.XMPMeta.Iterator(string, string, Com.Adobe.Xmp.Options.IteratorOptions)"/>
        /// <exception cref="Com.Adobe.Xmp.XMPException"/>
        public virtual XMPIterator Iterator(string schemaNS, string propName, IteratorOptions options)
        {
            return new XMPIteratorImpl(this, schemaNS, propName, options);
        }

        /// <exception cref="Com.Adobe.Xmp.XMPException"/>
        /// <seealso cref="Com.Adobe.Xmp.XMPMeta.SetArrayItem(string, string, int, string, Com.Adobe.Xmp.Options.PropertyOptions)"/>
        public virtual void SetArrayItem(string schemaNS, string arrayName, int itemIndex, string itemValue, PropertyOptions options)
        {
            ParameterAsserts.AssertSchemaNS(schemaNS);
            ParameterAsserts.AssertArrayName(arrayName);
            // Just lookup, don't try to create.
            XMPPath arrayPath = XMPPathParser.ExpandXPath(schemaNS, arrayName);
            XMPNode arrayNode = XMPNodeUtils.FindNode(tree, arrayPath, false, null);
            if (arrayNode != null)
            {
                DoSetArrayItem(arrayNode, itemIndex, itemValue, options, false);
            }
            else
            {
                throw new XMPException("Specified array does not exist", XMPErrorConstants.Badxpath);
            }
        }

        /// <seealso cref="Com.Adobe.Xmp.XMPMeta.SetArrayItem(string, string, int, string)"/>
        /// <exception cref="Com.Adobe.Xmp.XMPException"/>
        public virtual void SetArrayItem(string schemaNS, string arrayName, int itemIndex, string itemValue)
        {
            SetArrayItem(schemaNS, arrayName, itemIndex, itemValue, null);
        }

        /// <exception cref="Com.Adobe.Xmp.XMPException"/>
        /// <seealso cref="Com.Adobe.Xmp.XMPMeta.InsertArrayItem(string, string, int, string, Com.Adobe.Xmp.Options.PropertyOptions)"/>
        public virtual void InsertArrayItem(string schemaNS, string arrayName, int itemIndex, string itemValue, PropertyOptions options)
        {
            ParameterAsserts.AssertSchemaNS(schemaNS);
            ParameterAsserts.AssertArrayName(arrayName);
            // Just lookup, don't try to create.
            XMPPath arrayPath = XMPPathParser.ExpandXPath(schemaNS, arrayName);
            XMPNode arrayNode = XMPNodeUtils.FindNode(tree, arrayPath, false, null);
            if (arrayNode != null)
            {
                DoSetArrayItem(arrayNode, itemIndex, itemValue, options, true);
            }
            else
            {
                throw new XMPException("Specified array does not exist", XMPErrorConstants.Badxpath);
            }
        }

        /// <seealso cref="Com.Adobe.Xmp.XMPMeta.InsertArrayItem(string, string, int, string)"/>
        /// <exception cref="Com.Adobe.Xmp.XMPException"/>
        public virtual void InsertArrayItem(string schemaNS, string arrayName, int itemIndex, string itemValue)
        {
            InsertArrayItem(schemaNS, arrayName, itemIndex, itemValue, null);
        }

        /// <exception cref="Com.Adobe.Xmp.XMPException"/>
        /// <seealso cref="Com.Adobe.Xmp.XMPMeta.SetProperty(string, string, object, Com.Adobe.Xmp.Options.PropertyOptions)"/>
        public virtual void SetProperty(string schemaNS, string propName, object propValue, PropertyOptions options)
        {
            ParameterAsserts.AssertSchemaNS(schemaNS);
            ParameterAsserts.AssertPropName(propName);
            options = XMPNodeUtils.VerifySetOptions(options, propValue);
            XMPPath expPath = XMPPathParser.ExpandXPath(schemaNS, propName);
            XMPNode propNode = XMPNodeUtils.FindNode(tree, expPath, true, options);
            if (propNode != null)
            {
                SetNode(propNode, propValue, options, false);
            }
            else
            {
                throw new XMPException("Specified property does not exist", XMPErrorConstants.Badxpath);
            }
        }

        /// <seealso cref="Com.Adobe.Xmp.XMPMeta.SetProperty(string, string, object)"/>
        /// <exception cref="Com.Adobe.Xmp.XMPException"/>
        public virtual void SetProperty(string schemaNS, string propName, object propValue)
        {
            SetProperty(schemaNS, propName, propValue, null);
        }

        /// <exception cref="Com.Adobe.Xmp.XMPException"/>
        /// <seealso cref="Com.Adobe.Xmp.XMPMeta.SetQualifier(string, string, string, string, string, Com.Adobe.Xmp.Options.PropertyOptions)"/>
        public virtual void SetQualifier(string schemaNS, string propName, string qualNS, string qualName, string qualValue, PropertyOptions options)
        {
            ParameterAsserts.AssertSchemaNS(schemaNS);
            ParameterAsserts.AssertPropName(propName);
            if (!DoesPropertyExist(schemaNS, propName))
            {
                throw new XMPException("Specified property does not exist!", XMPErrorConstants.Badxpath);
            }
            string qualPath = propName + XMPPathFactory.ComposeQualifierPath(qualNS, qualName);
            SetProperty(schemaNS, qualPath, qualValue, options);
        }

        /// <seealso cref="Com.Adobe.Xmp.XMPMeta.SetQualifier(string, string, string, string, string)"/>
        /// <exception cref="Com.Adobe.Xmp.XMPException"/>
        public virtual void SetQualifier(string schemaNS, string propName, string qualNS, string qualName, string qualValue)
        {
            SetQualifier(schemaNS, propName, qualNS, qualName, qualValue, null);
        }

        /// <seealso cref="Com.Adobe.Xmp.XMPMeta.SetStructField(string, string, string, string, string, Com.Adobe.Xmp.Options.PropertyOptions)"/>
        /// <exception cref="Com.Adobe.Xmp.XMPException"/>
        public virtual void SetStructField(string schemaNS, string structName, string fieldNS, string fieldName, string fieldValue, PropertyOptions options)
        {
            ParameterAsserts.AssertSchemaNS(schemaNS);
            ParameterAsserts.AssertStructName(structName);
            string fieldPath = structName + XMPPathFactory.ComposeStructFieldPath(fieldNS, fieldName);
            SetProperty(schemaNS, fieldPath, fieldValue, options);
        }

        /// <seealso cref="Com.Adobe.Xmp.XMPMeta.SetStructField(string, string, string, string, string)"/>
        /// <exception cref="Com.Adobe.Xmp.XMPException"/>
        public virtual void SetStructField(string schemaNS, string structName, string fieldNS, string fieldName, string fieldValue)
        {
            SetStructField(schemaNS, structName, fieldNS, fieldName, fieldValue, null);
        }

        /// <seealso cref="Com.Adobe.Xmp.XMPMeta.GetObjectName()"/>
        public virtual string GetObjectName()
        {
            return tree.GetName() != null ? tree.GetName() : string.Empty;
        }

        /// <seealso cref="Com.Adobe.Xmp.XMPMeta.SetObjectName(string)"/>
        public virtual void SetObjectName(string name)
        {
            tree.SetName(name);
        }

        /// <seealso cref="Com.Adobe.Xmp.XMPMeta.GetPacketHeader()"/>
        public virtual string GetPacketHeader()
        {
            return packetHeader;
        }

        /// <summary>Sets the packetHeader attributes, only used by the parser.</summary>
        /// <param name="packetHeader">the processing instruction content</param>
        public virtual void SetPacketHeader(string packetHeader)
        {
            this.packetHeader = packetHeader;
        }

        /// <summary>Performs a deep clone of the XMPMeta-object</summary>
        /// <seealso cref="object.Clone()"/>
        public virtual object Clone()
        {
            XMPNode clonedTree = (XMPNode)tree.Clone();
            return new XMPMetaImpl(clonedTree);
        }

        /// <seealso cref="Com.Adobe.Xmp.XMPMeta.DumpObject()"/>
        public virtual string DumpObject()
        {
            // renders tree recursively
            return GetRoot().DumpNode(true);
        }

        /// <seealso cref="Com.Adobe.Xmp.XMPMeta.Sort()"/>
        public virtual void Sort()
        {
            this.tree.Sort();
        }

        /// <seealso cref="Com.Adobe.Xmp.XMPMeta.Normalize(Com.Adobe.Xmp.Options.ParseOptions)"/>
        /// <exception cref="Com.Adobe.Xmp.XMPException"/>
        public virtual void Normalize(ParseOptions options)
        {
            if (options == null)
            {
                options = new ParseOptions();
            }
            XMPNormalizer.Process(this, options);
        }

        /// <returns>Returns the root node of the XMP tree.</returns>
        public virtual XMPNode GetRoot()
        {
            return tree;
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
        /// <exception cref="Com.Adobe.Xmp.XMPException"/>
        private void DoSetArrayItem(XMPNode arrayNode, int itemIndex, string itemValue, PropertyOptions itemOptions, bool insert)
        {
            XMPNode itemNode = new XMPNode(XMPConstConstants.ArrayItemName, null);
            itemOptions = XMPNodeUtils.VerifySetOptions(itemOptions, itemValue);
            // in insert mode the index after the last is allowed,
            // even ARRAY_LAST_ITEM points to the index *after* the last.
            int maxIndex = insert ? arrayNode.GetChildrenLength() + 1 : arrayNode.GetChildrenLength();
            if (itemIndex == XMPConstConstants.ArrayLastItem)
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
                throw new XMPException("Array index out of bounds", XMPErrorConstants.Badindex);
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
        /// <exception cref="Com.Adobe.Xmp.XMPException">thrown if options and value do not correspond</exception>
        internal virtual void SetNode(XMPNode node, object value, PropertyOptions newOptions, bool deleteExisting)
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
                XMPNodeUtils.SetNodeValue(node, value);
            }
            else
            {
                if (value != null && value.ToString().Length > 0)
                {
                    throw new XMPException("Composite nodes can't have values", XMPErrorConstants.Badxpath);
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
        /// <exception cref="Com.Adobe.Xmp.XMPException"/>
        private object EvaluateNodeValue(int valueType, XMPNode propNode)
        {
            object value;
            string rawValue = propNode.GetValue();
            switch (valueType)
            {
                case ValueBoolean:
                {
                    value = XMPUtils.ConvertToBoolean(rawValue);
                    break;
                }

                case ValueInteger:
                {
                    value = XMPUtils.ConvertToInteger(rawValue);
                    break;
                }

                case ValueLong:
                {
                    value = XMPUtils.ConvertToLong(rawValue);
                    break;
                }

                case ValueDouble:
                {
                    value = XMPUtils.ConvertToDouble(rawValue);
                    break;
                }

                case ValueDate:
                {
                    value = XMPUtils.ConvertToDate(rawValue);
                    break;
                }

                case ValueCalendar:
                {
                    XMPDateTime dt = XMPUtils.ConvertToDate(rawValue);
                    value = dt.GetCalendar();
                    break;
                }

                case ValueBase64:
                {
                    value = XMPUtils.DecodeBase64(rawValue);
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
