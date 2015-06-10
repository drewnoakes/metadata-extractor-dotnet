// =================================================================================================
// ADOBE SYSTEMS INCORPORATED
// Copyright 2006 Adobe Systems Incorporated
// All Rights Reserved
//
// NOTICE:  Adobe permits you to use, modify, and distribute this file in accordance with the terms
// of the Adobe license agreement accompanying it.
// =================================================================================================

using System;
using Com.Adobe.Xmp.Impl.Xpath;
using Com.Adobe.Xmp.Options;
using Com.Adobe.Xmp.Properties;
using Sharpen;

namespace Com.Adobe.Xmp.Impl
{
    /// <summary>The <c>XMPIterator</c> implementation.</summary>
    /// <remarks>
    /// The <c>XMPIterator</c> implementation.
    /// Iterates the XMP Tree according to a set of options.
    /// During the iteration the XMPMeta-object must not be changed.
    /// Calls to <c>skipSubtree()</c> / <c>skipSiblings()</c> will affect the iteration.
    /// </remarks>
    /// <since>29.06.2006</since>
    public sealed class XmpIterator : IXmpIterator
    {
        /// <summary>stores the iterator options</summary>
        private readonly IteratorOptions _options;

        /// <summary>the base namespace of the property path, will be changed during the iteration</summary>
        private string _baseNs;

        /// <summary>flag to indicate that skipSiblings() has been called.</summary>
        private bool _skipSiblings;

        /// <summary>flag to indicate that skipSubtree() has been called.</summary>
        private bool _skipSubtree;

        /// <summary>the node iterator doing the work</summary>
        private readonly IIterator _nodeIterator;

        /// <summary>Constructor with optionsl initial values.</summary>
        /// <remarks>
        /// Constructor with optionsl initial values. If <c>propName</c> is provided,
        /// <c>schemaNS</c> has also be provided.
        /// </remarks>
        /// <param name="xmp">the iterated metadata object.</param>
        /// <param name="schemaNs">the iteration is reduced to this schema (optional)</param>
        /// <param name="propPath">the iteration is redurce to this property within the <c>schemaNS</c></param>
        /// <param name="options">
        /// advanced iteration options, see
        /// <see cref="Com.Adobe.Xmp.Options.IteratorOptions"/>
        /// </param>
        /// <exception cref="XmpException">If the node defined by the paramters is not existing.</exception>
        public XmpIterator(XmpMeta xmp, string schemaNs, string propPath, IteratorOptions options)
        {
            // make sure that options is defined at least with defaults
            _options = options ?? new IteratorOptions();
            // the start node of the iteration depending on the schema and property filter
            XmpNode startNode = null;
            string initialPath = null;
            bool baseSchema = !string.IsNullOrEmpty(schemaNs);
            bool baseProperty = !string.IsNullOrEmpty(propPath);
            if (!baseSchema && !baseProperty)
            {
                // complete tree will be iterated
                startNode = xmp.GetRoot();
            }
            else
            {
                if (baseSchema && baseProperty)
                {
                    // Schema and property node provided
                    XmpPath path = XmpPathParser.ExpandXPath(schemaNs, propPath);
                    // base path is the prop path without the property leaf
                    XmpPath basePath = new XmpPath();
                    for (int i = 0; i < path.Size() - 1; i++)
                    {
                        basePath.Add(path.GetSegment(i));
                    }
                    startNode = XmpNodeUtils.FindNode(xmp.GetRoot(), path, false, null);
                    _baseNs = schemaNs;
                    initialPath = basePath.ToString();
                }
                else
                {
                    if (baseSchema && !baseProperty)
                    {
                        // Only Schema provided
                        startNode = XmpNodeUtils.FindSchemaNode(xmp.GetRoot(), schemaNs, false);
                    }
                    else
                    {
                        // !baseSchema  &&  baseProperty
                        // No schema but property provided -> error
                        throw new XmpException("Schema namespace URI is required", XmpErrorCode.BadSchema);
                    }
                }
            }
            // create iterator
            if (startNode != null)
            {
                if (!_options.IsJustChildren)
                {
                    _nodeIterator = new NodeIterator(this, startNode, initialPath, 1);
                }
                else
                {
                    _nodeIterator = new NodeIteratorChildren(this, startNode, initialPath);
                }
            }
            else
            {
                // create null iterator
                _nodeIterator = Collections.EmptyList().Iterator();
            }
        }

        public void SkipSubtree()
        {
            _skipSubtree = true;
        }

        public void SkipSiblings()
        {
            SkipSubtree();
            _skipSiblings = true;
        }

        public bool HasNext()
        {
            return _nodeIterator.HasNext();
        }

        public object Next()
        {
            return _nodeIterator.Next();
        }

        public void Remove()
        {
            throw new NotSupportedException("The XMPIterator does not support remove().");
        }

        /// <returns>Exposes the options for inner class.</returns>
        private IteratorOptions GetOptions()
        {
            return _options;
        }

        /// <returns>Exposes the options for inner class.</returns>
        private string GetBaseNs()
        {
            return _baseNs;
        }

        /// <param name="baseNs">sets the baseNS from the inner class.</param>
        private void SetBaseNs(string baseNs)
        {
            _baseNs = baseNs;
        }

        /// <summary>The <c>XMPIterator</c> implementation.</summary>
        /// <remarks>
        /// The <c>XMPIterator</c> implementation.
        /// It first returns the node itself, then recursivly the children and qualifier of the node.
        /// </remarks>
        /// <since>29.06.2006</since>
        private class NodeIterator : IIterator
        {
            /// <summary>iteration state</summary>
            protected const int IterateNode = 0;

            /// <summary>iteration state</summary>
            protected const int IterateChildren = 1;

            /// <summary>iteration state</summary>
            protected const int IterateQualifier = 2;

            /// <summary>the state of the iteration</summary>
            private int _state = IterateNode;

            /// <summary>the currently visited node</summary>
            private readonly XmpNode _visitedNode;

            /// <summary>the recursively accumulated path</summary>
            private readonly string _path;

            /// <summary>the iterator that goes through the children and qualifier list</summary>
            private IIterator _childrenIterator;

            /// <summary>index of node with parent, only interesting for arrays</summary>
            private int _index;

            /// <summary>the iterator for each child</summary>
            private IIterator _subIterator = Collections.EmptyList().Iterator();

            /// <summary>the cached <c>PropertyInfo</c> to return</summary>
            private IXmpPropertyInfo _returnProperty;

            /// <summary>Default constructor</summary>
            public NodeIterator(XmpIterator enclosing)
            {
                _enclosing = enclosing;
            }

            /// <summary>Constructor for the node iterator.</summary>
            /// <param name="enclosing"></param>
            /// <param name="visitedNode">the currently visited node</param>
            /// <param name="parentPath">the accumulated path of the node</param>
            /// <param name="index">the index within the parent node (only for arrays)</param>
            public NodeIterator(XmpIterator enclosing, XmpNode visitedNode, string parentPath, int index)
            {
                _enclosing = enclosing;
                _visitedNode = visitedNode;
                _state = IterateNode;
                if (visitedNode.Options.IsSchemaNode)
                {
                    _enclosing.SetBaseNs(visitedNode.Name);
                }
                // for all but the root node and schema nodes
                _path = AccumulatePath(visitedNode, parentPath, index);
            }

            /// <summary>Prepares the next node to return if not already done.</summary>
            public virtual bool HasNext()
            {
                if (_returnProperty != null)
                {
                    // hasNext has been called before
                    return true;
                }
                // find next node
                if (_state == IterateNode)
                {
                    return ReportNode();
                }
                if (_state == IterateChildren)
                {
                    if (_childrenIterator == null)
                    {
                        _childrenIterator = _visitedNode.IterateChildren();
                    }
                    bool hasNext = IterateChildrenMethod(_childrenIterator);
                    if (!hasNext && _visitedNode.HasQualifier && !_enclosing.GetOptions().IsOmitQualifiers)
                    {
                        _state = IterateQualifier;
                        _childrenIterator = null;
                        hasNext = HasNext();
                    }
                    return hasNext;
                }
                if (_childrenIterator == null)
                {
                    _childrenIterator = _visitedNode.IterateQualifier();
                }
                return IterateChildrenMethod(_childrenIterator);
            }

            /// <summary>Sets the returnProperty as next item or recurses into <c>hasNext()</c>.</summary>
            /// <returns>Returns if there is a next item to return.</returns>
            protected virtual bool ReportNode()
            {
                _state = IterateChildren;
                if (_visitedNode.Parent != null && (!_enclosing.GetOptions().IsJustLeafnodes || !_visitedNode.HasChildren))
                {
                    _returnProperty = CreatePropertyInfo(_visitedNode, _enclosing.GetBaseNs(), _path);
                    return true;
                }
                return HasNext();
            }

            /// <summary>Handles the iteration of the children or qualfier</summary>
            /// <param name="iterator">an iterator</param>
            /// <returns>Returns if there are more elements available.</returns>
            private bool IterateChildrenMethod(IIterator iterator)
            {
                if (_enclosing._skipSiblings)
                {
                    // setSkipSiblings(false);
                    _enclosing._skipSiblings = false;
                    _subIterator = Collections.EmptyList().Iterator();
                }
                // create sub iterator for every child,
                // if its the first child visited or the former child is finished
                if ((!_subIterator.HasNext()) && iterator.HasNext())
                {
                    XmpNode child = (XmpNode)iterator.Next();
                    _index++;
                    _subIterator = new NodeIterator(_enclosing, child, _path, _index);
                }
                if (_subIterator.HasNext())
                {
                    _returnProperty = (IXmpPropertyInfo)_subIterator.Next();
                    return true;
                }
                return false;
            }

            /// <summary>Calls hasNext() and returnes the prepared node.</summary>
            /// <remarks>
            /// Calls hasNext() and returnes the prepared node. Afterwards its set to null.
            /// The existance of returnProperty indicates if there is a next node, otherwise
            /// an exceptio is thrown.
            /// </remarks>
            public virtual object Next()
            {
                if (HasNext())
                {
                    IXmpPropertyInfo result = _returnProperty;
                    _returnProperty = null;
                    return result;
                }
                throw new InvalidOperationException("There are no more nodes to return");
            }

            /// <summary>Not supported.</summary>
            public virtual void Remove()
            {
                throw new NotSupportedException();
            }

            /// <param name="currNode">the node that will be added to the path.</param>
            /// <param name="parentPath">the path up to this node.</param>
            /// <param name="currentIndex">the current array index if an arrey is traversed</param>
            /// <returns>Returns the updated path.</returns>
            protected virtual string AccumulatePath(XmpNode currNode, string parentPath, int currentIndex)
            {
                string separator;
                string segmentName;
                if (currNode.Parent == null || currNode.Options.IsSchemaNode)
                {
                    return null;
                }
                if (currNode.Parent.Options.IsArray)
                {
                    separator = string.Empty;
                    segmentName = "[" + currentIndex + "]";
                }
                else
                {
                    separator = "/";
                    segmentName = currNode.Name;
                }
                if (string.IsNullOrEmpty(parentPath))
                {
                    return segmentName;
                }
                if (_enclosing.GetOptions().IsJustLeafname)
                {
                    return !segmentName.StartsWith("?") ? segmentName : segmentName.Substring (1);
                }
                // qualifier
                return parentPath + separator + segmentName;
            }

            /// <summary>Creates a property info object from an <c>XMPNode</c>.</summary>
            /// <param name="node">an <c>XMPNode</c></param>
            /// <param name="baseNs">the base namespace to report</param>
            /// <param name="path">the full property path</param>
            /// <returns>Returns a <c>XMPProperty</c>-object that serves representation of the node.</returns>
            protected virtual IXmpPropertyInfo CreatePropertyInfo(XmpNode node, string baseNs, string path)
            {
                string value = node.Options.IsSchemaNode ? null : node.Value;
                return new XmpPropertyInfo450(node, baseNs, path, value);
            }

            private sealed class XmpPropertyInfo450 : IXmpPropertyInfo
            {
                public XmpPropertyInfo450(XmpNode node, string baseNs, string path, string value)
                {
                    _node = node;
                    _baseNs = baseNs;
                    _path = path;
                    _value = value;
                }

                public string GetNamespace()
                {
                    if (!_node.Options.IsSchemaNode)
                    {
                        // determine namespace of leaf node
                        QName qname = new QName(_node.Name);
                        return XmpMetaFactory.GetSchemaRegistry().GetNamespaceUri(qname.GetPrefix());
                    }
                    return _baseNs;
                }

                public string GetPath()
                {
                    return _path;
                }

                public string GetValue()
                {
                    return _value;
                }

                public PropertyOptions GetOptions()
                {
                    return _node.Options;
                }

                public string GetLanguage()
                {
                    // the language is not reported
                    return null;
                }

                private readonly XmpNode _node;

                private readonly string _baseNs;

                private readonly string _path;

                private readonly string _value;
            }

            /// <returns>the childrenIterator</returns>
            protected virtual IIterator GetChildrenIterator()
            {
                return _childrenIterator;
            }

            /// <param name="childrenIterator">the childrenIterator to set</param>
            protected virtual void SetChildrenIterator(IIterator childrenIterator)
            {
                _childrenIterator = childrenIterator;
            }

            /// <returns>Returns the returnProperty.</returns>
            protected virtual IXmpPropertyInfo GetReturnProperty()
            {
                return _returnProperty;
            }

            /// <param name="returnProperty">the returnProperty to set</param>
            protected virtual void SetReturnProperty(IXmpPropertyInfo returnProperty)
            {
                _returnProperty = returnProperty;
            }

            private readonly XmpIterator _enclosing;
        }

        /// <summary>
        /// This iterator is derived from the default <c>NodeIterator</c>,
        /// and is only used for the option <see cref="Com.Adobe.Xmp.Options.IteratorOptions.JustChildren"/>.
        /// </summary>
        /// <since>02.10.2006</since>
        private class NodeIteratorChildren : NodeIterator
        {
            private readonly string _parentPath;

            private readonly IIterator _childrenIterator;

            private int _index;

            /// <summary>Constructor</summary>
            /// <param name="enclosing"></param>
            /// <param name="parentNode">the node which children shall be iterated.</param>
            /// <param name="parentPath">the full path of the former node without the leaf node.</param>
            public NodeIteratorChildren(XmpIterator enclosing, XmpNode parentNode, string parentPath)
                : base(enclosing)
            {
                _enclosing = enclosing;
                if (parentNode.Options.IsSchemaNode)
                {
                    _enclosing.SetBaseNs(parentNode.Name);
                }
                _parentPath = AccumulatePath(parentNode, parentPath, 1);
                _childrenIterator = parentNode.IterateChildren();
            }

            /// <summary>Prepares the next node to return if not already done.</summary>
            public override bool HasNext()
            {
                if (GetReturnProperty() != null)
                {
                    // hasNext has been called before
                    return true;
                }
                if (_enclosing._skipSiblings)
                {
                    return false;
                }
                if (_childrenIterator.HasNext())
                {
                    XmpNode child = (XmpNode)_childrenIterator.Next();
                    _index++;
                    string path = null;
                    if (child.Options.IsSchemaNode)
                    {
                        _enclosing.SetBaseNs(child.Name);
                    }
                    else
                    {
                        if (child.Parent != null)
                        {
                            // for all but the root node and schema nodes
                            path = AccumulatePath(child, _parentPath, _index);
                        }
                    }
                    // report next property, skip not-leaf nodes in case options is set
                    if (!_enclosing.GetOptions().IsJustLeafnodes || !child.HasChildren)
                    {
                        SetReturnProperty(CreatePropertyInfo(child, _enclosing.GetBaseNs(), path));
                        return true;
                    }
                    return HasNext();
                }
                return false;
            }

            private readonly XmpIterator _enclosing;
        }
    }
}
