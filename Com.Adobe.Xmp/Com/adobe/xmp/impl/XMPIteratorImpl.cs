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
    public class XMPIteratorImpl : XMPIterator
    {
        /// <summary>stores the iterator options</summary>
        private readonly IteratorOptions options;

        /// <summary>the base namespace of the property path, will be changed during the iteration</summary>
        private string baseNS = null;

        /// <summary>flag to indicate that skipSiblings() has been called.</summary>
        protected internal bool skipSiblings = false;

        /// <summary>flag to indicate that skipSiblings() has been called.</summary>
        protected internal bool skipSubtree = false;

        /// <summary>the node iterator doing the work</summary>
        private readonly Iterator nodeIterator = null;

        /// <summary>Constructor with optionsl initial values.</summary>
        /// <remarks>
        /// Constructor with optionsl initial values. If <code>propName</code> is provided,
        /// <code>schemaNS</code> has also be provided.
        /// </remarks>
        /// <param name="xmp">the iterated metadata object.</param>
        /// <param name="schemaNS">the iteration is reduced to this schema (optional)</param>
        /// <param name="propPath">the iteration is redurce to this property within the <code>schemaNS</code></param>
        /// <param name="options">
        /// advanced iteration options, see
        /// <see cref="Com.Adobe.Xmp.Options.IteratorOptions"/>
        /// </param>
        /// <exception cref="Com.Adobe.Xmp.XMPException">If the node defined by the paramters is not existing.</exception>
        public XMPIteratorImpl(XMPMetaImpl xmp, string schemaNS, string propPath, IteratorOptions options)
        {
            // make sure that options is defined at least with defaults
            this.options = options != null ? options : new IteratorOptions();
            // the start node of the iteration depending on the schema and property filter
            XMPNode startNode = null;
            string initialPath = null;
            bool baseSchema = schemaNS != null && schemaNS.Length > 0;
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
                    XMPPath path = XMPPathParser.ExpandXPath(schemaNS, propPath);
                    // base path is the prop path without the property leaf
                    XMPPath basePath = new XMPPath();
                    for (int i = 0; i < path.Size() - 1; i++)
                    {
                        basePath.Add(path.GetSegment(i));
                    }
                    startNode = XMPNodeUtils.FindNode(xmp.GetRoot(), path, false, null);
                    baseNS = schemaNS;
                    initialPath = basePath.ToString();
                }
                else
                {
                    if (baseSchema && !baseProperty)
                    {
                        // Only Schema provided
                        startNode = XMPNodeUtils.FindSchemaNode(xmp.GetRoot(), schemaNS, false);
                    }
                    else
                    {
                        // !baseSchema  &&  baseProperty
                        // No schema but property provided -> error
                        throw new XMPException("Schema namespace URI is required", XMPErrorConstants.Badschema);
                    }
                }
            }
            // create iterator
            if (startNode != null)
            {
                if (!this.options.IsJustChildren())
                {
                    nodeIterator = new NodeIterator(this, startNode, initialPath, 1);
                }
                else
                {
                    nodeIterator = new NodeIteratorChildren(this, startNode, initialPath);
                }
            }
            else
            {
                // create null iterator
                nodeIterator = Collections.EmptyList().Iterator();
            }
        }

        /// <seealso cref="Com.Adobe.Xmp.XMPIterator.SkipSubtree()"/>
        public virtual void SkipSubtree()
        {
            this.skipSubtree = true;
        }

        /// <seealso cref="Com.Adobe.Xmp.XMPIterator.SkipSiblings()"/>
        public virtual void SkipSiblings()
        {
            SkipSubtree();
            this.skipSiblings = true;
        }

        /// <seealso cref="Sharpen.Iterator{E}.HasNext()"/>
        public virtual bool HasNext()
        {
            return nodeIterator.HasNext();
        }

        /// <seealso cref="Sharpen.Iterator{E}.Next()"/>
        public virtual object Next()
        {
            return nodeIterator.Next();
        }

        /// <seealso cref="Sharpen.Iterator{E}.Remove()"/>
        public virtual void Remove()
        {
            throw new NotSupportedException("The XMPIterator does not support remove().");
        }

        /// <returns>Exposes the options for inner class.</returns>
        protected internal virtual IteratorOptions GetOptions()
        {
            return options;
        }

        /// <returns>Exposes the options for inner class.</returns>
        protected internal virtual string GetBaseNS()
        {
            return baseNS;
        }

        /// <param name="baseNS">sets the baseNS from the inner class.</param>
        protected internal virtual void SetBaseNS(string baseNS)
        {
            this.baseNS = baseNS;
        }

        /// <summary>The <code>XMPIterator</code> implementation.</summary>
        /// <remarks>
        /// The <code>XMPIterator</code> implementation.
        /// It first returns the node itself, then recursivly the children and qualifier of the node.
        /// </remarks>
        /// <since>29.06.2006</since>
        private class NodeIterator : Iterator
        {
            /// <summary>iteration state</summary>
            protected internal const int IterateNode = 0;

            /// <summary>iteration state</summary>
            protected internal const int IterateChildren = 1;

            /// <summary>iteration state</summary>
            protected internal const int IterateQualifier = 2;

            /// <summary>the state of the iteration</summary>
            private int state = IterateNode;

            /// <summary>the currently visited node</summary>
            private readonly XMPNode visitedNode;

            /// <summary>the recursively accumulated path</summary>
            private readonly string path;

            /// <summary>the iterator that goes through the children and qualifier list</summary>
            private Iterator childrenIterator = null;

            /// <summary>index of node with parent, only interesting for arrays</summary>
            private int index = 0;

            /// <summary>the iterator for each child</summary>
            private Iterator subIterator = Collections.EmptyList().Iterator();

            /// <summary>the cached <code>PropertyInfo</code> to return</summary>
            private XMPPropertyInfo returnProperty = null;

            /// <summary>Default constructor</summary>
            public NodeIterator(XMPIteratorImpl _enclosing)
            {
                this._enclosing = _enclosing;
            }

            /// <summary>Constructor for the node iterator.</summary>
            /// <param name="visitedNode">the currently visited node</param>
            /// <param name="parentPath">the accumulated path of the node</param>
            /// <param name="index">the index within the parent node (only for arrays)</param>
            public NodeIterator(XMPIteratorImpl _enclosing, XMPNode visitedNode, string parentPath, int index)
            {
                this._enclosing = _enclosing;
                // EMPTY
                this.visitedNode = visitedNode;
                this.state = IterateNode;
                if (visitedNode.GetOptions().IsSchemaNode())
                {
                    this._enclosing.SetBaseNS(visitedNode.GetName());
                }
                // for all but the root node and schema nodes
                this.path = this.AccumulatePath(visitedNode, parentPath, index);
            }

            /// <summary>Prepares the next node to return if not already done.</summary>
            /// <seealso cref="Sharpen.Iterator{E}.HasNext()"/>
            public virtual bool HasNext()
            {
                if (this.returnProperty != null)
                {
                    // hasNext has been called before
                    return true;
                }
                // find next node
                if (this.state == IterateNode)
                {
                    return this.ReportNode();
                }
                else
                {
                    if (this.state == IterateChildren)
                    {
                        if (this.childrenIterator == null)
                        {
                            this.childrenIterator = this.visitedNode.IterateChildren();
                        }
                        bool hasNext = this.IterateChildrenMethod(this.childrenIterator);
                        if (!hasNext && this.visitedNode.HasQualifier() && !this._enclosing.GetOptions().IsOmitQualifiers())
                        {
                            this.state = IterateQualifier;
                            this.childrenIterator = null;
                            hasNext = this.HasNext();
                        }
                        return hasNext;
                    }
                    else
                    {
                        if (this.childrenIterator == null)
                        {
                            this.childrenIterator = this.visitedNode.IterateQualifier();
                        }
                        return this.IterateChildrenMethod(this.childrenIterator);
                    }
                }
            }

            /// <summary>Sets the returnProperty as next item or recurses into <code>hasNext()</code>.</summary>
            /// <returns>Returns if there is a next item to return.</returns>
            protected internal virtual bool ReportNode()
            {
                this.state = IterateChildren;
                if (this.visitedNode.GetParent() != null && (!this._enclosing.GetOptions().IsJustLeafnodes() || !this.visitedNode.HasChildren()))
                {
                    this.returnProperty = this.CreatePropertyInfo(this.visitedNode, this._enclosing.GetBaseNS(), this.path);
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
            private bool IterateChildrenMethod(Iterator iterator)
            {
                if (this._enclosing.skipSiblings)
                {
                    // setSkipSiblings(false);
                    this._enclosing.skipSiblings = false;
                    this.subIterator = Collections.EmptyList().Iterator();
                }
                // create sub iterator for every child,
                // if its the first child visited or the former child is finished
                if ((!this.subIterator.HasNext()) && iterator.HasNext())
                {
                    XMPNode child = (XMPNode)iterator.Next();
                    this.index++;
                    this.subIterator = new NodeIterator(this._enclosing, child, this.path, this.index);
                }
                if (this.subIterator.HasNext())
                {
                    this.returnProperty = (XMPPropertyInfo)this.subIterator.Next();
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
                    XMPPropertyInfo result = this.returnProperty;
                    this.returnProperty = null;
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
            protected internal virtual string AccumulatePath(XMPNode currNode, string parentPath, int currentIndex)
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
            /// <param name="baseNS">the base namespace to report</param>
            /// <param name="path">the full property path</param>
            /// <returns>Returns a <code>XMPProperty</code>-object that serves representation of the node.</returns>
            protected internal virtual XMPPropertyInfo CreatePropertyInfo(XMPNode node, string baseNS, string path)
            {
                string value = node.GetOptions().IsSchemaNode() ? null : node.GetValue();
                return new _XMPPropertyInfo_450(node, baseNS, path, value);
            }

            private sealed class _XMPPropertyInfo_450 : XMPPropertyInfo
            {
                public _XMPPropertyInfo_450(XMPNode node, string baseNS, string path, string value)
                {
                    this.node = node;
                    this.baseNS = baseNS;
                    this.path = path;
                    this.value = value;
                }

                public string GetNamespace()
                {
                    if (!node.GetOptions().IsSchemaNode())
                    {
                        // determine namespace of leaf node
                        QName qname = new QName(node.GetName());
                        return XMPMetaFactory.GetSchemaRegistry().GetNamespaceURI(qname.GetPrefix());
                    }
                    else
                    {
                        return baseNS;
                    }
                }

                public string GetPath()
                {
                    return path;
                }

                public string GetValue()
                {
                    return value;
                }

                public PropertyOptions GetOptions()
                {
                    return node.GetOptions();
                }

                public string GetLanguage()
                {
                    // the language is not reported
                    return null;
                }

                private readonly XMPNode node;

                private readonly string baseNS;

                private readonly string path;

                private readonly string value;
            }

            /// <returns>the childrenIterator</returns>
            protected internal virtual Iterator GetChildrenIterator()
            {
                return this.childrenIterator;
            }

            /// <param name="childrenIterator">the childrenIterator to set</param>
            protected internal virtual void SetChildrenIterator(Iterator childrenIterator)
            {
                this.childrenIterator = childrenIterator;
            }

            /// <returns>Returns the returnProperty.</returns>
            protected internal virtual XMPPropertyInfo GetReturnProperty()
            {
                return this.returnProperty;
            }

            /// <param name="returnProperty">the returnProperty to set</param>
            protected internal virtual void SetReturnProperty(XMPPropertyInfo returnProperty)
            {
                this.returnProperty = returnProperty;
            }

            private readonly XMPIteratorImpl _enclosing;
        }

        /// <summary>
        /// This iterator is derived from the default <code>NodeIterator</code>,
        /// and is only used for the option
        /// <see cref="Com.Adobe.Xmp.Options.IteratorOptions.JustChildren"/>
        /// .
        /// </summary>
        /// <since>02.10.2006</since>
        private class NodeIteratorChildren : NodeIterator
        {
            private readonly string parentPath;

            private readonly Iterator childrenIterator;

            private int index = 0;

            /// <summary>Constructor</summary>
            /// <param name="parentNode">the node which children shall be iterated.</param>
            /// <param name="parentPath">the full path of the former node without the leaf node.</param>
            public NodeIteratorChildren(XMPIteratorImpl _enclosing, XMPNode parentNode, string parentPath)
                : base(_enclosing)
            {
                this._enclosing = _enclosing;
                if (parentNode.GetOptions().IsSchemaNode())
                {
                    this._enclosing.SetBaseNS(parentNode.GetName());
                }
                this.parentPath = this.AccumulatePath(parentNode, parentPath, 1);
                this.childrenIterator = parentNode.IterateChildren();
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
                        if (this.childrenIterator.HasNext())
                        {
                            XMPNode child = (XMPNode)this.childrenIterator.Next();
                            this.index++;
                            string path = null;
                            if (child.GetOptions().IsSchemaNode())
                            {
                                this._enclosing.SetBaseNS(child.GetName());
                            }
                            else
                            {
                                if (child.GetParent() != null)
                                {
                                    // for all but the root node and schema nodes
                                    path = this.AccumulatePath(child, this.parentPath, this.index);
                                }
                            }
                            // report next property, skip not-leaf nodes in case options is set
                            if (!this._enclosing.GetOptions().IsJustLeafnodes() || !child.HasChildren())
                            {
                                this.SetReturnProperty(this.CreatePropertyInfo(child, this._enclosing.GetBaseNS(), path));
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

            private readonly XMPIteratorImpl _enclosing;
        }
    }
}
