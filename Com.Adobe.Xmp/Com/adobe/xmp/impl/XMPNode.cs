//=================================================================================================
// ADOBE SYSTEMS INCORPORATED
// Copyright 2006 Adobe Systems Incorporated
// All Rights Reserved
//
// NOTICE:  Adobe permits you to use, modify, and distribute this file in accordance with the terms
// of the Adobe license agreement accompanying it.
// =================================================================================================

using System;
using System.Collections;
using System.Diagnostics;
using System.Text;
using Com.Adobe.Xmp.Options;
using Sharpen;

namespace Com.Adobe.Xmp.Impl
{
    /// <summary>
    /// A node in the internally XMP tree, which can be a schema node, a property node, an array node,
    /// an array item, a struct node or a qualifier node (without '?').
    /// </summary>
    /// <remarks>
    /// A node in the internally XMP tree, which can be a schema node, a property node, an array node,
    /// an array item, a struct node or a qualifier node (without '?').
    /// Possible improvements:
    /// 1. The kind Node of node might be better represented by a class-hierarchy of different nodes.
    /// 2. The array type should be an enum
    /// 3. isImplicitNode should be removed completely and replaced by return values of fi.
    /// 4. hasLanguage, hasType should be automatically maintained by XMPNode
    /// </remarks>
    /// <since>21.02.2006</since>
    public sealed class XmpNode : IComparable
    {
        /// <summary>list of child nodes, lazy initialized</summary>
        private IList _children;

        /// <summary>list of qualifier of the node, lazy initialized</summary>
        private IList _qualifier;

        /// <summary>options describing the kind of the node</summary>
        private PropertyOptions _options;

        /// <summary>Creates an <c>XMPNode</c> with initial values.</summary>
        /// <param name="name">the name of the node</param>
        /// <param name="value">the value of the node</param>
        /// <param name="options">the options of the node</param>
        public XmpNode(string name, string value, PropertyOptions options)
        {
            // internal processing options
            Name = name;
            Value = value;
            _options = options;
        }

        /// <summary>Constructor for the node without value.</summary>
        /// <param name="name">the name of the node</param>
        /// <param name="options">the options of the node</param>
        public XmpNode(string name, PropertyOptions options)
            : this(name, null, options)
        {
        }

        /// <summary>Resets the node.</summary>
        public void Clear()
        {
            _options = null;
            Name = null;
            Value = null;
            _children = null;
            _qualifier = null;
        }

        /// <summary>
        /// Get the parent node.
        /// </summary>
        public XmpNode Parent { get; private set; }

        /// <param name="index">an index [1..size]</param>
        /// <returns>Returns the child with the requested index.</returns>
        public XmpNode GetChild(int index)
        {
            return (XmpNode)GetChildren()[index - 1];
        }

        /// <summary>Adds a node as child to this node.</summary>
        /// <param name="node">an XMPNode</param>
        /// <exception cref="XmpException"></exception>
        public void AddChild(XmpNode node)
        {
            // check for duplicate properties
            AssertChildNotExisting(node.Name);
            node.Parent = this;
            GetChildren().Add(node);
        }

        /// <summary>Adds a node as child to this node.</summary>
        /// <param name="index">
        /// the index of the node <em>before</em> which the new one is inserted.
        /// <em>Note:</em> The node children are indexed from [1..size]!
        /// An index of size + 1 appends a node.
        /// </param>
        /// <param name="node">an XMPNode</param>
        /// <exception cref="XmpException"></exception>
        public void AddChild(int index, XmpNode node)
        {
            AssertChildNotExisting(node.Name);
            node.Parent = this;
            GetChildren().Insert(index - 1, node);
        }

        /// <summary>Replaces a node with another one.</summary>
        /// <param name="index">
        /// the index of the node that will be replaced.
        /// <em>Note:</em> The node children are indexed from [1..size]!
        /// </param>
        /// <param name="node">the replacement XMPNode</param>
        public void ReplaceChild(int index, XmpNode node)
        {
            node.Parent = this;
            GetChildren()[index - 1] = node;
        }

        /// <summary>Removes a child at the requested index.</summary>
        /// <param name="itemIndex">the index to remove [1..size]</param>
        public void RemoveChild(int itemIndex)
        {
            GetChildren().Remove(itemIndex - 1);
            CleanupChildren();
        }

        /// <summary>Removes a child node.</summary>
        /// <remarks>
        /// Removes a child node.
        /// If its a schema node and doesn't have any children anymore, its deleted.
        /// </remarks>
        /// <param name="node">the child node to delete.</param>
        public void RemoveChild(XmpNode node)
        {
            GetChildren().Remove(node);
            CleanupChildren();
        }

        /// <summary>
        /// Removes the children list if this node has no children anymore;
        /// checks if the provided node is a schema node and doesn't have any children anymore,
        /// its deleted.
        /// </summary>
        private void CleanupChildren()
        {
            if (_children.Count == 0)
            {
                _children = null;
            }
        }

        /// <summary>Removes all children from the node.</summary>
        public void RemoveChildren()
        {
            _children = null;
        }

        /// <returns>Returns the number of children without neccessarily creating a list.</returns>
        public int GetChildrenLength()
        {
            return _children != null ? _children.Count : 0;
        }

        /// <param name="expr">child node name to look for</param>
        /// <returns>Returns an <c>XMPNode</c> if node has been found, <c>null</c> otherwise.</returns>
        public XmpNode FindChildByName(string expr)
        {
            return Find(GetChildren(), expr);
        }

        /// <param name="index">an index [1..size]</param>
        /// <returns>Returns the qualifier with the requested index.</returns>
        public XmpNode GetQualifier(int index)
        {
            return (XmpNode)GetQualifier()[index - 1];
        }

        /// <returns>Returns the number of qualifier without neccessarily creating a list.</returns>
        public int GetQualifierLength()
        {
            return _qualifier != null ? _qualifier.Count : 0;
        }

        /// <summary>Appends a qualifier to the qualifier list and sets respective options.</summary>
        /// <param name="qualNode">a qualifier node.</param>
        /// <exception cref="XmpException"></exception>
        public void AddQualifier(XmpNode qualNode)
        {
            AssertQualifierNotExisting(qualNode.Name);
            qualNode.Parent = this;
            qualNode.Options.IsQualifier = true;
            Options.HasQualifiers = true;
            // contraints
            if (qualNode.IsLanguageNode)
            {
                // "xml:lang" is always first and the option "hasLanguage" is set
                _options.HasLanguage = true;
                GetQualifier().Insert(0, qualNode);
            }
            else
            {
                if (qualNode.IsTypeNode)
                {
                    // "rdf:type" must be first or second after "xml:lang" and the option "hasType" is set
                    _options.HasType = true;
                    GetQualifier().Insert(!_options.HasLanguage ? 0 : 1, qualNode);
                }
                else
                {
                    // other qualifiers are appended
                    GetQualifier().Add(qualNode);
                }
            }
        }

        /// <summary>Removes one qualifier node and fixes the options.</summary>
        /// <param name="qualNode">qualifier to remove</param>
        public void RemoveQualifier(XmpNode qualNode)
        {
            PropertyOptions opts = Options;
            if (qualNode.IsLanguageNode)
            {
                // if "xml:lang" is removed, remove hasLanguage-flag too
                opts.HasLanguage = false;
            }
            else
            {
                if (qualNode.IsTypeNode)
                {
                    // if "rdf:type" is removed, remove hasType-flag too
                    opts.HasType = false;
                }
            }
            GetQualifier().Remove(qualNode);
            if (_qualifier.Count == 0)
            {
                opts.HasQualifiers = false;
                _qualifier = null;
            }
        }

        /// <summary>Removes all qualifiers from the node and sets the options appropriate.</summary>
        public void RemoveQualifiers()
        {
            PropertyOptions opts = Options;
            // clear qualifier related options
            opts.HasQualifiers = false;
            opts.HasLanguage = false;
            opts.HasType = false;
            _qualifier = null;
        }

        /// <param name="expr">qualifier node name to look for</param>
        /// <returns>
        /// Returns a qualifier <c>XMPNode</c> if node has been found,
        /// <c>null</c> otherwise.
        /// </returns>
        public XmpNode FindQualifierByName(string expr)
        {
            return Find(_qualifier, expr);
        }

        /// <summary>
        /// Get whether the node has children.
        /// </summary>
        public bool HasChildren
        {
            get { return _children != null && _children.Count > 0; }
        }

        /// <returns>
        /// Returns an iterator for the children.
        /// <em>Note:</em> take care to use it.remove(), as the flag are not adjusted in that case.
        /// </returns>
        public IIterator IterateChildren()
        {
            if (_children != null)
            {
                return GetChildren().Iterator();
            }
            return Collections.EmptyList().ListIterator();
        }

        /// <summary>
        /// Returns whether the node has qualifier attached.
        /// </summary>
        public bool HasQualifier
        {
            get { return _qualifier != null && _qualifier.Count > 0; }
        }

        /// <returns>
        /// Returns an iterator for the qualifier.
        /// <em>Note:</em> take care to use it.remove(), as the flag are not adjusted in that case.
        /// </returns>
        public IIterator IterateQualifier()
        {
            if (_qualifier != null)
            {
                IIterator it = GetQualifier().Iterator();
                return new Iterator391(it);
            }
            return Collections.EmptyList().Iterator();
        }

        /// <summary>
        /// Iterator that disallows removal.
        /// </summary>
        private sealed class Iterator391 : IIterator
        {
            public Iterator391(IIterator it)
            {
                _it = it;
            }

            public bool HasNext()
            {
                return _it.HasNext();
            }

            public object Next()
            {
                return _it.Next();
            }

            public void Remove()
            {
                throw new NotSupportedException("remove() is not allowed due to the internal contraints");
            }

            private readonly IIterator _it;
        }

        /// <summary>Performs a <b>deep clone</b> of the node and the complete subtree.</summary>
        public object Clone()
        {
            PropertyOptions newOptions;
            try
            {
                newOptions = new PropertyOptions(Options.GetOptions());
            }
            catch (XmpException)
            {
                // cannot happen
                newOptions = new PropertyOptions();
            }
            XmpNode newNode = new XmpNode(Name, Value, newOptions);
            CloneSubtree(newNode);
            return newNode;
        }

        /// <summary>
        /// Performs a <b>deep clone</b> of the complete subtree (children and
        /// qualifier )into and add it to the destination node.
        /// </summary>
        /// <param name="destination">the node to add the cloned subtree</param>
        public void CloneSubtree(XmpNode destination)
        {
            try
            {
                for (IIterator it = IterateChildren(); it.HasNext(); )
                {
                    XmpNode child = (XmpNode)it.Next();
                    destination.AddChild((XmpNode)child.Clone());
                }
                for (IIterator it1 = IterateQualifier(); it1.HasNext(); )
                {
                    XmpNode qualifier = (XmpNode)it1.Next();
                    destination.AddQualifier((XmpNode)qualifier.Clone());
                }
            }
            catch (XmpException)
            {
                // cannot happen (duplicate childs/quals do not exist in this node)
                Debug.Assert(false);
            }
        }

        /// <summary>Renders this node and the tree unter this node in a human readable form.</summary>
        /// <param name="recursive">Flag is qualifier and child nodes shall be rendered too</param>
        /// <returns>Returns a multiline string containing the dump.</returns>
        public string DumpNode(bool recursive)
        {
            StringBuilder result = new StringBuilder(512);
            DumpNode(result, recursive, 0, 0);
            return result.ToString();
        }

        /// <seealso cref="System.IComparable{T}.CompareTo(object)"></seealso>
        public int CompareTo(object xmpNode)
        {
            if (Options.IsSchemaNode)
            {
                return string.CompareOrdinal(Value, ((XmpNode)xmpNode).Value);
            }
            return string.CompareOrdinal(Name, ((XmpNode)xmpNode).Name);
        }

        public string Name { set; get; }

        public string Value { get; set; }

        public PropertyOptions Options
        {
            get { return _options ?? (_options = new PropertyOptions()); }
            set { _options = value; }
        }

        /// <summary>
        /// Get and set the implicit node flag.
        /// </summary>
        public bool IsImplicit { get; set; }

        /// <summary>
        /// Get and set whether the node contains aliases (applies only to schema nodes).
        /// </summary>
        public bool HasAliases { get; set; }

        /// <summary>
        /// Get and set whether this node is an alias (applies only to schema nodes).
        /// </summary>
        public bool IsAlias { get; set; }

        /// <summary>
        /// Get and set whether this node has an <c>rdf:value</c> child node.
        /// </summary>
        public bool HasValueChild { get; set; }

        /// <summary>
        /// Sorts the complete datamodel according to the following rules:
        /// <list type="bullet">
        /// <item>Nodes at one level are sorted by name, that is prefix + local name
        /// <item>Starting at the root node the children and qualifier are sorted recursively,
        /// which the following exceptions.
        /// </summary>
        /// <remarks>
        /// Sorts the complete datamodel according to the following rules:
        /// <list type="bullet">
        /// <item>Nodes at one level are sorted by name, that is prefix + local name
        /// <item>Starting at the root node the children and qualifier are sorted recursively,
        /// which the following exceptions.
        /// <item>Sorting will not be used for arrays.
        /// <item>Within qualifier "xml:lang" and/or "rdf:type" stay at the top in that order,
        /// all others are sorted.
        /// </list>
        /// </remarks>
        public void Sort()
        {
            // sort qualifier
            if (HasQualifier)
            {
                XmpNode[] quals = (XmpNode[])Collections.ToArray(GetQualifier(), new XmpNode[GetQualifierLength()]);
                int sortFrom = 0;
                while (quals.Length > sortFrom && (XmpConstConstants.XmlLang.Equals(quals[sortFrom].Name) || "rdf:type".Equals(quals[sortFrom].Name)))
                {
                    quals[sortFrom].Sort();
                    sortFrom++;
                }
                Array.Sort (quals, sortFrom, quals.Length);
                ListIterator it = _qualifier.ListIterator();
                for (int j = 0; j < quals.Length; j++)
                {
                    it.Next();
                    it.Set(quals[j]);
                    quals[j].Sort();
                }
            }
            // sort children
            if (HasChildren)
            {
                if (!Options.IsArray)
                {
                    _children.Sort();
                }
                for (IIterator it = IterateChildren(); it.HasNext(); )
                {
                    ((XmpNode)it.Next()).Sort();
                }
            }
        }

        //------------------------------------------------------------------------------ private methods
        /// <summary>Dumps this node and its qualifier and children recursively.</summary>
        /// <remarks>
        /// Dumps this node and its qualifier and children recursively.
        /// <em>Note:</em> It creats empty options on every node.
        /// </remarks>
        /// <param name="result">the buffer to append the dump.</param>
        /// <param name="recursive">Flag is qualifier and child nodes shall be rendered too</param>
        /// <param name="indent">the current indent level.</param>
        /// <param name="index">the index within the parent node (important for arrays)</param>
        private void DumpNode(StringBuilder result, bool recursive, int indent, int index)
        {
            // write indent
            for (int i = 0; i < indent; i++)
            {
                result.Append('\t');
            }
            // render Node
            if (Parent != null)
            {
                if (Options.IsQualifier)
                {
                    result.Append('?');
                    result.Append(Name);
                }
                else
                {
                    if (Parent.Options.IsArray)
                    {
                        result.Append('[');
                        result.Append(index);
                        result.Append(']');
                    }
                    else
                    {
                        result.Append(Name);
                    }
                }
            }
            else
            {
                // applies only to the root node
                result.Append("ROOT NODE");
                if (!string.IsNullOrEmpty(Name))
                {
                    // the "about" attribute
                    result.Append(" (");
                    result.Append(Name);
                    result.Append(')');
                }
            }
            if (!string.IsNullOrEmpty(Value))
            {
                result.Append(" = \"");
                result.Append(Value);
                result.Append('"');
            }
            // render options if at least one is set
            if (Options.ContainsOneOf(unchecked((int)(0xffffffff))))
            {
                result.Append("\t(");
                result.Append(Options);
                result.Append(" : ");
                result.Append(Options.GetOptionsString());
                result.Append(')');
            }
            result.Append('\n');
            // render qualifier
            if (recursive && HasQualifier)
            {
                XmpNode[] quals = (XmpNode[])Collections.ToArray(GetQualifier(), new XmpNode[GetQualifierLength()]);
                int i1 = 0;
                while (quals.Length > i1 && (XmpConstConstants.XmlLang.Equals(quals[i1].Name) || "rdf:type".Equals(quals[i1].Name)))
                {
                    i1++;
                }
                Array.Sort (quals, i1, quals.Length);
                for (i1 = 0; i1 < quals.Length; i1++)
                {
                    XmpNode qualifier = quals[i1];
                    qualifier.DumpNode(result, recursive, indent + 2, i1 + 1);
                }
            }
            // render children
            if (recursive && HasChildren)
            {
                XmpNode[] children = (XmpNode[])Collections.ToArray(GetChildren(), new XmpNode[GetChildrenLength()]);
                if (!Options.IsArray)
                {
                    Array.Sort(children);
                }
                for (int i1 = 0; i1 < children.Length; i1++)
                {
                    XmpNode child = children[i1];
                    child.DumpNode(result, recursive, indent + 1, i1 + 1);
                }
            }
        }

        /// <summary>
        /// Get whether this node is a language qualifier.
        /// </summary>
        private bool IsLanguageNode
        {
            get { return XmpConstConstants.XmlLang.Equals(Name); }
        }

        /// <summary>
        /// Get whether this node is a type qualifier.
        /// </summary>
        private bool IsTypeNode
        {
            get { return "rdf:type".Equals(Name); }
        }

        /// <summary>
        /// <em>Note:</em> This method should always be called when accessing 'children' to be sure
        /// that its initialized.
        /// </summary>
        /// <returns>Returns list of children that is lazy initialized.</returns>
        private IList GetChildren()
        {
            if (_children == null)
            {
                _children = new ArrayList(0);
            }
            return _children;
        }

        /// <returns>Returns a read-only copy of child nodes list.</returns>
        public IList GetUnmodifiableChildren()
        {
            return Collections.UnmodifiableList(new ArrayList(GetChildren()));
        }

        /// <returns>Returns list of qualifier that is lazy initialized.</returns>
        private IList GetQualifier()
        {
            if (_qualifier == null)
            {
                _qualifier = new ArrayList(0);
            }
            return _qualifier;
        }

        /// <summary>Internal find.</summary>
        /// <param name="list">the list to search in</param>
        /// <param name="expr">the search expression</param>
        /// <returns>Returns the found node or <c>nulls</c>.</returns>
        private static XmpNode Find(IList list, string expr)
        {
            if (list != null)
            {
                for (IIterator it = list.Iterator(); it.HasNext(); )
                {
                    XmpNode child = (XmpNode)it.Next();
                    if (child.Name.Equals(expr))
                    {
                        return child;
                    }
                }
            }
            return null;
        }

        /// <summary>Checks that a node name is not existing on the same level, except for array items.</summary>
        /// <param name="childName">the node name to check</param>
        /// <exception cref="XmpException">Thrown if a node with the same name is existing.</exception>
        private void AssertChildNotExisting(string childName)
        {
            if (!XmpConstConstants.ArrayItemName.Equals(childName) && FindChildByName(childName) != null)
            {
                throw new XmpException("Duplicate property or field node '" + childName + "'", XmpErrorCode.BadXmp);
            }
        }

        /// <summary>Checks that a qualifier name is not existing on the same level.</summary>
        /// <param name="qualifierName">the new qualifier name</param>
        /// <exception cref="XmpException">Thrown if a node with the same name is existing.</exception>
        private void AssertQualifierNotExisting(string qualifierName)
        {
            if (!XmpConstConstants.ArrayItemName.Equals(qualifierName) && FindQualifierByName(qualifierName) != null)
            {
                throw new XmpException("Duplicate '" + qualifierName + "' qualifier", XmpErrorCode.BadXmp);
            }
        }
    }
}
