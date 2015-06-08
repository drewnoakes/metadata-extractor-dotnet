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
    /// <summary>The <code>XMPIterator</code> implementation.</summary>
    /// <remarks>
    /// The <code>XMPIterator</code> implementation.
    /// Iterates the XMP Tree according to a set of options.
    /// During the iteration the XMPMeta-object must not be changed.
    /// Calls to <code>skipSubtree()</code> / <code>skipSiblings()</code> will affect the iteration.
    /// </remarks>
    /// <since>29.06.2006</since>
    public class XmpIterator : IXmpIterator
    {
        /// <summary>stores the iterator options</summary>
        private readonly IteratorOptions _options;

        /// <summary>the base namespace of the property path, will be changed during the iteration</summary>
        private string _baseNs;

        /// <summary>flag to indicate that skipSiblings() has been called.</summary>
        protected internal bool skipSiblings;

        /// <summary>flag to indicate that skipSiblings() has been called.</summary>
        protected internal bool skipSubtree;

        /// <summary>the node iterator doing the work</summary>
        private readonly IIterator _nodeIterator;

        /// <summary>Constructor with optionsl initial values.</summary>
        /// <remarks>
        /// Constructor with optionsl initial values. If <code>propName</code> is provided,
        /// <code>schemaNS</code> has also be provided.
        /// </remarks>
        /// <param name="xmp">the iterated metadata object.</param>
        /// <param name="schemaNs">the iteration is reduced to this schema (optional)</param>
        /// <param name="propPath">the iteration is redurce to this property within the <code>schemaNS</code></param>
        /// <param name="options">
        /// advanced iteration options, see
        /// <see cref="Com.Adobe.Xmp.Options.IteratorOptions"/>
        /// </param>
        /// <exception cref="XmpException">If the node defined by the paramters is not existing.</exception>
        public XmpIterator(XmpMeta xmp, string schemaNs, string propPath, IteratorOptions options)
        {
            // make sure that options is defined at least with defaults
            this._options = options != null ? options : new IteratorOptions();
            // the start node of the iteration depending on the schema and property filter
            XmpNode startNode = null;
            string initialPath = null;
            bool baseSchema = schemaNs != null && schemaNs.Length > 0;
            bool baseProperty = propPath != null && propPath.Length > 0;
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
                        throw new XmpException("Schema namespace URI is required", XmpErrorConstants.Badschema);
                    }
                }
            }
            // create iterator
            if (startNode != null)
            {
                if (!this._options.IsJustChildren())
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

        /// <seealso cref="IXmpIterator.SkipSubtree()"/>
        public virtual void SkipSubtree()
        {
            this.skipSubtree = true;
        }

        /// <seealso cref="IXmpIterator.SkipSiblings()"/>
        public virtual void SkipSiblings()
        {
            SkipSubtree();
            this.skipSiblings = true;
        }

        /// <seealso cref="Sharpen.Iterator{E}.HasNext()"/>
        public virtual bool HasNext()
        {
            return _nodeIterator.HasNext();
        }

        /// <seealso cref="Sharpen.Iterator{E}.Next()"/>
        public virtual object Next()
        {
            return _nodeIterator.Next();
        }

        /// <seealso cref="Sharpen.Iterator{E}.Remove()"/>
        public virtual void Remove()
        {
            throw new NotSupportedException("The XMPIterator does not support remove().");
        }

        /// <returns>Exposes the options for inner class.</returns>
        protected internal virtual IteratorOptions GetOptions()
        {
            return _options;
        }

        /// <returns>Exposes the options for inner class.</returns>
        protected internal virtual string GetBaseNs()
        {
            return _baseNs;
        }

        /// <param name="baseNs">sets the baseNS from the inner class.</param>
        protected internal virtual void SetBaseNs(string baseNs)
        {
            this._baseNs = baseNs;
        }

        /// <summary>The <code>XMPIterator</code> implementation.</summary>
        /// <remarks>
        /// The <code>XMPIterator</code> implementation.
        /// It first returns the node itself, then recursivly the children and qualifier of the node.
        /// </remarks>
        /// <since>29.06.2006</since>
        private class NodeIterator : IIterator
        {
            /// <summary>iteration state</summary>
            protected internal const int IterateNode = 0;

            /// <summary>iteration state</summary>
            protected internal const int IterateChildren = 1;

            /// <summary>iteration state</summary>
            protected internal const int IterateQualifier = 2;

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

            /// <summary>the cached <code>PropertyInfo</code> to return</summary>
            private IXmpPropertyInfo _returnProperty;

            /// <summary>Default constructor</summary>
            public NodeIterator(XmpIterator enclosing)
            {
                this._enclosing = enclosing;
            }

            /// <summary>Constructor for the node iterator.</summary>
            /// <param name="visitedNode">the currently visited node</param>
            /// <param name="parentPath">the accumulated path of the node</param>
            /// <param name="index">the index within the parent node (only for arrays)</param>
            public NodeIterator(XmpIterator enclosing, XmpNode visitedNode, string parentPath, int index)
            {
                this._enclosing = enclosing;
                // EMPTY
                this._visitedNode = visitedNode;
                this._state = IterateNode;
                if (visitedNode.GetOptions().IsSchemaNode())
                {
                    this._enclosing.SetBaseNs(visitedNode.GetName());
                }
                // for all but the root node and schema nodes
                this._path = this.AccumulatePath(visitedNode, parentPath, index);
            }

            /// <summary>Prepares the next node to return if not already done.</summary>
            /// <seealso cref="Sharpen.Iterator{E}.HasNext()"/>
            public virtual bool HasNext()
            {
                if (this._returnProperty != null)
                {
                    // hasNext has been called before
                    return true;
                }
                // find next node
                if (this._state == IterateNode)
                {
                    return this.ReportNode();
                }
                else
                {
                    if (this._state == IterateChildren)
                    {
                        if (this._childrenIterator == null)
                        {
                            this._childrenIterator = this._visitedNode.IterateChildren();
                        }
                        bool hasNext = this.IterateChildrenMethod(this._childrenIterator);
                        if (!hasNext && this._visitedNode.HasQualifier() && !this._enclosing.GetOptions().IsOmitQualifiers())
                        {
                            this._state = IterateQualifier;
                            this._childrenIterator = null;
                            hasNext = this.HasNext();
                        }
                        return hasNext;
                    }
                    else
                    {
                        if (this._childrenIterator == null)
                        {
                            this._childrenIterator = this._visitedNode.IterateQualifier();
                        }
                        return this.IterateChildrenMethod(this._childrenIterator);
                    }
                }
            }

            /// <summary>Sets the returnProperty as next item or recurses into <code>hasNext()</code>.</summary>
            /// <returns>Returns if there is a next item to return.</returns>
            protected internal virtual bool ReportNode()
            {
                this._state = IterateChildren;
                if (this._visitedNode.GetParent() != null && (!this._enclosing.GetOptions().IsJustLeafnodes() || !this._visitedNode.HasChildren()))
                {
                    this._returnProperty = this.CreatePropertyInfo(this._visitedNode, this._enclosing.GetBaseNs(), this._path);
                    return true;
                }
                else
                {
                    return this.HasNext();
                }
            }

            /// <summary>Handles the iteration of the children or qualfier</summary>
            /// <param name="iterator">an iterator</param>
            /// <returns>Returns if there are more elements available.</returns>
            private bool IterateChildrenMethod(IIterator iterator)
            {
                if (this._enclosing.skipSiblings)
                {
                    // setSkipSiblings(false);
                    this._enclosing.skipSiblings = false;
                    this._subIterator = Collections.EmptyList().Iterator();
                }
                // create sub iterator for every child,
                // if its the first child visited or the former child is finished
                if ((!this._subIterator.HasNext()) && iterator.HasNext())
                {
                    XmpNode child = (XmpNode)iterator.Next();
                    this._index++;
                    this._subIterator = new NodeIterator(this._enclosing, child, this._path, this._index);
                }
                if (this._subIterator.HasNext())
                {
                    this._returnProperty = (IXmpPropertyInfo)this._subIterator.Next();
                    return true;
                }
                else
                {
                    return false;
                }
            }

            /// <summary>Calls hasNext() and returnes the prepared node.</summary>
            /// <remarks>
            /// Calls hasNext() and returnes the prepared node. Afterwards its set to null.
            /// The existance of returnProperty indicates if there is a next node, otherwise
            /// an exceptio is thrown.
            /// </remarks>
            /// <seealso cref="Sharpen.Iterator{E}.Next()"/>
            public virtual object Next()
            {
                if (this.HasNext())
                {
                    IXmpPropertyInfo result = this._returnProperty;
                    this._returnProperty = null;
                    return result;
                }
                else
                {
                    throw new NoSuchElementException("There are no more nodes to return");
                }
            }

            /// <summary>Not supported.</summary>
            /// <seealso cref="Sharpen.Iterator{E}.Remove()"/>
            public virtual void Remove()
            {
                throw new NotSupportedException();
            }

            /// <param name="currNode">the node that will be added to the path.</param>
            /// <param name="parentPath">the path up to this node.</param>
            /// <param name="currentIndex">the current array index if an arrey is traversed</param>
            /// <returns>Returns the updated path.</returns>
            protected internal virtual string AccumulatePath(XmpNode currNode, string parentPath, int currentIndex)
            {
                string separator;
                string segmentName;
                if (currNode.GetParent() == null || currNode.GetOptions().IsSchemaNode())
                {
                    return null;
                }
                else
                {
                    if (currNode.GetParent().GetOptions().IsArray())
                    {
                        separator = string.Empty;
                        segmentName = "[" + currentIndex.ToString() + "]";
                    }
                    else
                    {
                        separator = "/";
                        segmentName = currNode.GetName();
                    }
                }
                if (parentPath == null || parentPath.Length == 0)
                {
                    return segmentName;
                }
                else
                {
                    if (this._enclosing.GetOptions().IsJustLeafname())
                    {
                        return !segmentName.StartsWith("?") ? segmentName : Runtime.Substring(segmentName, 1);
                    }
                    else
                    {
                        // qualifier
                        return parentPath + separator + segmentName;
                    }
                }
            }

            /// <summary>Creates a property info object from an <code>XMPNode</code>.</summary>
            /// <param name="node">an <code>XMPNode</code></param>
            /// <param name="baseNs">the base namespace to report</param>
            /// <param name="path">the full property path</param>
            /// <returns>Returns a <code>XMPProperty</code>-object that serves representation of the node.</returns>
            protected internal virtual IXmpPropertyInfo CreatePropertyInfo(XmpNode node, string baseNs, string path)
            {
                string value = node.GetOptions().IsSchemaNode() ? null : node.GetValue();
                return new XmpPropertyInfo450(node, baseNs, path, value);
            }

            private sealed class XmpPropertyInfo450 : IXmpPropertyInfo
            {
                public XmpPropertyInfo450(XmpNode node, string baseNs, string path, string value)
                {
                    this._node = node;
                    this._baseNs = baseNs;
                    this._path = path;
                    this._value = value;
                }

                public string GetNamespace()
                {
                    if (!_node.GetOptions().IsSchemaNode())
                    {
                        // determine namespace of leaf node
                        QName qname = new QName(_node.GetName());
                        return XmpMetaFactory.GetSchemaRegistry().GetNamespaceUri(qname.GetPrefix());
                    }
                    else
                    {
                        return _baseNs;
                    }
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
                    return _node.GetOptions();
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
            protected internal virtual IIterator GetChildrenIterator()
            {
                return this._childrenIterator;
            }

            /// <param name="childrenIterator">the childrenIterator to set</param>
            protected internal virtual void SetChildrenIterator(IIterator childrenIterator)
            {
                this._childrenIterator = childrenIterator;
            }

            /// <returns>Returns the returnProperty.</returns>
            protected internal virtual IXmpPropertyInfo GetReturnProperty()
            {
                return this._returnProperty;
            }

            /// <param name="returnProperty">the returnProperty to set</param>
            protected internal virtual void SetReturnProperty(IXmpPropertyInfo returnProperty)
            {
                this._returnProperty = returnProperty;
            }

            private readonly XmpIterator _enclosing;
        }

        /// <summary>
        /// This iterator is derived from the default <code>NodeIterator</code>,
        /// and is only used for the option <see cref="Com.Adobe.Xmp.Options.IteratorOptions.JustChildren"/>.
        /// </summary>
        /// <since>02.10.2006</since>
        private class NodeIteratorChildren : NodeIterator
        {
            private readonly string _parentPath;

            private readonly IIterator _childrenIterator;

            private int _index;

            /// <summary>Constructor</summary>
            /// <param name="parentNode">the node which children shall be iterated.</param>
            /// <param name="parentPath">the full path of the former node without the leaf node.</param>
            public NodeIteratorChildren(XmpIterator enclosing, XmpNode parentNode, string parentPath)
                : base(enclosing)
            {
                this._enclosing = enclosing;
                if (parentNode.GetOptions().IsSchemaNode())
                {
                    this._enclosing.SetBaseNs(parentNode.GetName());
                }
                this._parentPath = this.AccumulatePath(parentNode, parentPath, 1);
                this._childrenIterator = parentNode.IterateChildren();
            }

            /// <summary>Prepares the next node to return if not already done.</summary>
            /// <seealso cref="Sharpen.Iterator{E}.HasNext()"/>
            public override bool HasNext()
            {
                if (this.GetReturnProperty() != null)
                {
                    // hasNext has been called before
                    return true;
                }
                else
                {
                    if (this._enclosing.skipSiblings)
                    {
                        return false;
                    }
                    else
                    {
                        if (this._childrenIterator.HasNext())
                        {
                            XmpNode child = (XmpNode)this._childrenIterator.Next();
                            this._index++;
                            string path = null;
                            if (child.GetOptions().IsSchemaNode())
                            {
                                this._enclosing.SetBaseNs(child.GetName());
                            }
                            else
                            {
                                if (child.GetParent() != null)
                                {
                                    // for all but the root node and schema nodes
                                    path = this.AccumulatePath(child, this._parentPath, this._index);
                                }
                            }
                            // report next property, skip not-leaf nodes in case options is set
                            if (!this._enclosing.GetOptions().IsJustLeafnodes() || !child.HasChildren())
                            {
                                this.SetReturnProperty(this.CreatePropertyInfo(child, this._enclosing.GetBaseNs(), path));
                                return true;
                            }
                            else
                            {
                                return this.HasNext();
                            }
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
            }

            private readonly XmpIterator _enclosing;
        }
    }
}
