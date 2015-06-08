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
using System.Text;
using Com.Adobe.Xmp;
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
    public class XMPNode : IComparable
    {
        /// <summary>name of the node, contains different information depending of the node kind</summary>
        private string name;

        /// <summary>value of the node, contains different information depending of the node kind</summary>
        private string value;

        /// <summary>link to the parent node</summary>
        private Com.Adobe.Xmp.Impl.XMPNode parent;

        /// <summary>list of child nodes, lazy initialized</summary>
        private IList children = null;

        /// <summary>list of qualifier of the node, lazy initialized</summary>
        private IList qualifier = null;

        /// <summary>options describing the kind of the node</summary>
        private PropertyOptions options = null;

        /// <summary>flag if the node is implicitly created</summary>
        private bool @implicit;

        /// <summary>flag if the node has aliases</summary>
        private bool hasAliases;

        /// <summary>flag if the node is an alias</summary>
        private bool alias;

        /// <summary>flag if the node has an "rdf:value" child node.</summary>
        private bool hasValueChild;

        /// <summary>Creates an <code>XMPNode</code> with initial values.</summary>
        /// <param name="name">the name of the node</param>
        /// <param name="value">the value of the node</param>
        /// <param name="options">the options of the node</param>
        public XMPNode(string name, string value, PropertyOptions options)
        {
            // internal processing options
            this.name = name;
            this.value = value;
            this.options = options;
        }

        /// <summary>Constructor for the node without value.</summary>
        /// <param name="name">the name of the node</param>
        /// <param name="options">the options of the node</param>
        public XMPNode(string name, PropertyOptions options)
            : this(name, null, options)
        {
        }

        /// <summary>Resets the node.</summary>
        public virtual void Clear()
        {
            options = null;
            name = null;
            value = null;
            children = null;
            qualifier = null;
        }

        /// <returns>Returns the parent node.</returns>
        public virtual Com.Adobe.Xmp.Impl.XMPNode GetParent()
        {
            return parent;
        }

        /// <param name="index">an index [1..size]</param>
        /// <returns>Returns the child with the requested index.</returns>
        public virtual Com.Adobe.Xmp.Impl.XMPNode GetChild(int index)
        {
            return (Com.Adobe.Xmp.Impl.XMPNode)GetChildren()[index - 1];
        }

        /// <summary>Adds a node as child to this node.</summary>
        /// <param name="node">an XMPNode</param>
        /// <exception cref="Com.Adobe.Xmp.XMPException"></exception>
        public virtual void AddChild(Com.Adobe.Xmp.Impl.XMPNode node)
        {
            // check for duplicate properties
            AssertChildNotExisting(node.GetName());
            node.SetParent(this);
            GetChildren().Add(node);
        }

        /// <summary>Adds a node as child to this node.</summary>
        /// <param name="index">
        /// the index of the node <em>before</em> which the new one is inserted.
        /// <em>Note:</em> The node children are indexed from [1..size]!
        /// An index of size + 1 appends a node.
        /// </param>
        /// <param name="node">an XMPNode</param>
        /// <exception cref="Com.Adobe.Xmp.XMPException"></exception>
        public virtual void AddChild(int index, Com.Adobe.Xmp.Impl.XMPNode node)
        {
            AssertChildNotExisting(node.GetName());
            node.SetParent(this);
            GetChildren().Add(index - 1, node);
        }

        /// <summary>Replaces a node with another one.</summary>
        /// <param name="index">
        /// the index of the node that will be replaced.
        /// <em>Note:</em> The node children are indexed from [1..size]!
        /// </param>
        /// <param name="node">the replacement XMPNode</param>
        public virtual void ReplaceChild(int index, Com.Adobe.Xmp.Impl.XMPNode node)
        {
            node.SetParent(this);
            GetChildren().Set(index - 1, node);
        }

        /// <summary>Removes a child at the requested index.</summary>
        /// <param name="itemIndex">the index to remove [1..size]</param>
        public virtual void RemoveChild(int itemIndex)
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
        public virtual void RemoveChild(Com.Adobe.Xmp.Impl.XMPNode node)
        {
            GetChildren().Remove(node);
            CleanupChildren();
        }

        /// <summary>
        /// Removes the children list if this node has no children anymore;
        /// checks if the provided node is a schema node and doesn't have any children anymore,
        /// its deleted.
        /// </summary>
        protected internal virtual void CleanupChildren()
        {
            if (children.IsEmpty())
            {
                children = null;
            }
        }

        /// <summary>Removes all children from the node.</summary>
        public virtual void RemoveChildren()
        {
            children = null;
        }

        /// <returns>Returns the number of children without neccessarily creating a list.</returns>
        public virtual int GetChildrenLength()
        {
            return children != null ? children.Count : 0;
        }

        /// <param name="expr">child node name to look for</param>
        /// <returns>Returns an <code>XMPNode</code> if node has been found, <code>null</code> otherwise.</returns>
        public virtual Com.Adobe.Xmp.Impl.XMPNode FindChildByName(string expr)
        {
            return Find(GetChildren(), expr);
        }

        /// <param name="index">an index [1..size]</param>
        /// <returns>Returns the qualifier with the requested index.</returns>
        public virtual Com.Adobe.Xmp.Impl.XMPNode GetQualifier(int index)
        {
            return (Com.Adobe.Xmp.Impl.XMPNode)GetQualifier()[index - 1];
        }

        /// <returns>Returns the number of qualifier without neccessarily creating a list.</returns>
        public virtual int GetQualifierLength()
        {
            return qualifier != null ? qualifier.Count : 0;
        }

        /// <summary>Appends a qualifier to the qualifier list and sets respective options.</summary>
        /// <param name="qualNode">a qualifier node.</param>
        /// <exception cref="Com.Adobe.Xmp.XMPException"></exception>
        public virtual void AddQualifier(Com.Adobe.Xmp.Impl.XMPNode qualNode)
        {
            AssertQualifierNotExisting(qualNode.GetName());
            qualNode.SetParent(this);
            qualNode.GetOptions().SetQualifier(true);
            GetOptions().SetHasQualifiers(true);
            // contraints
            if (qualNode.IsLanguageNode())
            {
                // "xml:lang" is always first and the option "hasLanguage" is set
                options.SetHasLanguage(true);
                GetQualifier().Add(0, qualNode);
            }
            else
            {
                if (qualNode.IsTypeNode())
                {
                    // "rdf:type" must be first or second after "xml:lang" and the option "hasType" is set
                    options.SetHasType(true);
                    GetQualifier().Add(!options.GetHasLanguage() ? 0 : 1, qualNode);
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
        public virtual void RemoveQualifier(Com.Adobe.Xmp.Impl.XMPNode qualNode)
        {
            PropertyOptions opts = GetOptions();
            if (qualNode.IsLanguageNode())
            {
                // if "xml:lang" is removed, remove hasLanguage-flag too
                opts.SetHasLanguage(false);
            }
            else
            {
                if (qualNode.IsTypeNode())
                {
                    // if "rdf:type" is removed, remove hasType-flag too
                    opts.SetHasType(false);
                }
            }
            GetQualifier().Remove(qualNode);
            if (qualifier.IsEmpty())
            {
                opts.SetHasQualifiers(false);
                qualifier = null;
            }
        }

        /// <summary>Removes all qualifiers from the node and sets the options appropriate.</summary>
        public virtual void RemoveQualifiers()
        {
            PropertyOptions opts = GetOptions();
            // clear qualifier related options
            opts.SetHasQualifiers(false);
            opts.SetHasLanguage(false);
            opts.SetHasType(false);
            qualifier = null;
        }

        /// <param name="expr">qualifier node name to look for</param>
        /// <returns>
        /// Returns a qualifier <code>XMPNode</code> if node has been found,
        /// <code>null</code> otherwise.
        /// </returns>
        public virtual Com.Adobe.Xmp.Impl.XMPNode FindQualifierByName(string expr)
        {
            return Find(qualifier, expr);
        }

        /// <returns>Returns whether the node has children.</returns>
        public virtual bool HasChildren()
        {
            return children != null && children.Count > 0;
        }

        /// <returns>
        /// Returns an iterator for the children.
        /// <em>Note:</em> take care to use it.remove(), as the flag are not adjusted in that case.
        /// </returns>
        public virtual Iterator IterateChildren()
        {
            if (children != null)
            {
                return GetChildren().Iterator();
            }
            else
            {
                return Sharpen.Collections.EmptyList().ListIterator();
            }
        }

        /// <returns>Returns whether the node has qualifier attached.</returns>
        public virtual bool HasQualifier()
        {
            return qualifier != null && qualifier.Count > 0;
        }

        /// <returns>
        /// Returns an iterator for the qualifier.
        /// <em>Note:</em> take care to use it.remove(), as the flag are not adjusted in that case.
        /// </returns>
        public virtual Iterator IterateQualifier()
        {
            if (qualifier != null)
            {
                Iterator it = GetQualifier().Iterator();
                return new _Iterator_391(it);
            }
            else
            {
                return Sharpen.Collections.EmptyList().Iterator();
            }
        }

        private sealed class _Iterator_391 : Iterator
        {
            public _Iterator_391(Iterator it)
            {
                this.it = it;
            }

            public bool HasNext()
            {
                return it.HasNext();
            }

            public object Next()
            {
                return it.Next();
            }

            public void Remove()
            {
                throw new NotSupportedException("remove() is not allowed due to the internal contraints");
            }

            private readonly Iterator it;
        }

        /// <summary>Performs a <b>deep clone</b> of the node and the complete subtree.</summary>
        /// <seealso cref="object.Clone()"/>
        public virtual object Clone()
        {
            PropertyOptions newOptions;
            try
            {
                newOptions = new PropertyOptions(GetOptions().GetOptions());
            }
            catch (XMPException)
            {
                // cannot happen
                newOptions = new PropertyOptions();
            }
            Com.Adobe.Xmp.Impl.XMPNode newNode = new Com.Adobe.Xmp.Impl.XMPNode(name, value, newOptions);
            CloneSubtree(newNode);
            return newNode;
        }

        /// <summary>
        /// Performs a <b>deep clone</b> of the complete subtree (children and
        /// qualifier )into and add it to the destination node.
        /// </summary>
        /// <param name="destination">the node to add the cloned subtree</param>
        public virtual void CloneSubtree(Com.Adobe.Xmp.Impl.XMPNode destination)
        {
            try
            {
                for (Iterator it = IterateChildren(); it.HasNext(); )
                {
                    Com.Adobe.Xmp.Impl.XMPNode child = (Com.Adobe.Xmp.Impl.XMPNode)it.Next();
                    destination.AddChild((Com.Adobe.Xmp.Impl.XMPNode)child.Clone());
                }
                for (Iterator it_1 = IterateQualifier(); it_1.HasNext(); )
                {
                    Com.Adobe.Xmp.Impl.XMPNode qualifier = (Com.Adobe.Xmp.Impl.XMPNode)it_1.Next();
                    destination.AddQualifier((Com.Adobe.Xmp.Impl.XMPNode)qualifier.Clone());
                }
            }
            catch (XMPException)
            {
                // cannot happen (duplicate childs/quals do not exist in this node)
                System.Diagnostics.Debug.Assert(false);
            }
        }

        /// <summary>Renders this node and the tree unter this node in a human readable form.</summary>
        /// <param name="recursive">Flag is qualifier and child nodes shall be rendered too</param>
        /// <returns>Returns a multiline string containing the dump.</returns>
        public virtual string DumpNode(bool recursive)
        {
            StringBuilder result = new StringBuilder(512);
            this.DumpNode(result, recursive, 0, 0);
            return result.ToString();
        }

        /// <seealso cref="System.IComparable{T}.CompareTo(object)"></seealso>
        public virtual int CompareTo(object xmpNode)
        {
            if (GetOptions().IsSchemaNode())
            {
                return string.CompareOrdinal(this.value, ((Com.Adobe.Xmp.Impl.XMPNode)xmpNode).GetValue());
            }
            else
            {
                return string.CompareOrdinal(this.name, ((Com.Adobe.Xmp.Impl.XMPNode)xmpNode).GetName());
            }
        }

        /// <returns>Returns the name.</returns>
        public virtual string GetName()
        {
            return name;
        }

        /// <param name="name">The name to set.</param>
        public virtual void SetName(string name)
        {
            this.name = name;
        }

        /// <returns>Returns the value.</returns>
        public virtual string GetValue()
        {
            return value;
        }

        /// <param name="value">The value to set.</param>
        public virtual void SetValue(string value)
        {
            this.value = value;
        }

        /// <returns>Returns the options.</returns>
        public virtual PropertyOptions GetOptions()
        {
            if (options == null)
            {
                options = new PropertyOptions();
            }
            return options;
        }

        /// <summary>Updates the options of the node.</summary>
        /// <param name="options">the options to set.</param>
        public virtual void SetOptions(PropertyOptions options)
        {
            this.options = options;
        }

        /// <returns>Returns the implicit flag</returns>
        public virtual bool IsImplicit()
        {
            return @implicit;
        }

        /// <param name="implicit">Sets the implicit node flag</param>
        public virtual void SetImplicit(bool @implicit)
        {
            this.@implicit = @implicit;
        }

        /// <returns>Returns if the node contains aliases (applies only to schema nodes)</returns>
        public virtual bool GetHasAliases()
        {
            return hasAliases;
        }

        /// <param name="hasAliases">sets the flag that the node contains aliases</param>
        public virtual void SetHasAliases(bool hasAliases)
        {
            this.hasAliases = hasAliases;
        }

        /// <returns>Returns if the node contains aliases (applies only to schema nodes)</returns>
        public virtual bool IsAlias()
        {
            return alias;
        }

        /// <param name="alias">sets the flag that the node is an alias</param>
        public virtual void SetAlias(bool alias)
        {
            this.alias = alias;
        }

        /// <returns>the hasValueChild</returns>
        public virtual bool GetHasValueChild()
        {
            return hasValueChild;
        }

        /// <param name="hasValueChild">the hasValueChild to set</param>
        public virtual void SetHasValueChild(bool hasValueChild)
        {
            this.hasValueChild = hasValueChild;
        }

        /// <summary>
        /// Sorts the complete datamodel according to the following rules:
        /// <ul>
        /// <li>Nodes at one level are sorted by name, that is prefix + local name
        /// <li>Starting at the root node the children and qualifier are sorted recursively,
        /// which the following exceptions.
        /// </summary>
        /// <remarks>
        /// Sorts the complete datamodel according to the following rules:
        /// <ul>
        /// <li>Nodes at one level are sorted by name, that is prefix + local name
        /// <li>Starting at the root node the children and qualifier are sorted recursively,
        /// which the following exceptions.
        /// <li>Sorting will not be used for arrays.
        /// <li>Within qualifier "xml:lang" and/or "rdf:type" stay at the top in that order,
        /// all others are sorted.
        /// </ul>
        /// </remarks>
        public virtual void Sort()
        {
            // sort qualifier
            if (HasQualifier())
            {
                Com.Adobe.Xmp.Impl.XMPNode[] quals = (Com.Adobe.Xmp.Impl.XMPNode[])Sharpen.Collections.ToArray(GetQualifier(), new Com.Adobe.Xmp.Impl.XMPNode[GetQualifierLength()]);
                int sortFrom = 0;
                while (quals.Length > sortFrom && (XMPConstConstants.XmlLang.Equals(quals[sortFrom].GetName()) || "rdf:type".Equals(quals[sortFrom].GetName())))
                {
                    quals[sortFrom].Sort();
                    sortFrom++;
                }
                Arrays.Sort(quals, sortFrom, quals.Length);
                ListIterator it = qualifier.ListIterator();
                for (int j = 0; j < quals.Length; j++)
                {
                    it.Next();
                    it.Set(quals[j]);
                    quals[j].Sort();
                }
            }
            // sort children
            if (HasChildren())
            {
                if (!GetOptions().IsArray())
                {
                    children.Sort();
                }
                for (Iterator it = IterateChildren(); it.HasNext(); )
                {
                    ((Com.Adobe.Xmp.Impl.XMPNode)it.Next()).Sort();
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
            if (parent != null)
            {
                if (GetOptions().IsQualifier())
                {
                    result.Append('?');
                    result.Append(name);
                }
                else
                {
                    if (GetParent().GetOptions().IsArray())
                    {
                        result.Append('[');
                        result.Append(index);
                        result.Append(']');
                    }
                    else
                    {
                        result.Append(name);
                    }
                }
            }
            else
            {
                // applies only to the root node
                result.Append("ROOT NODE");
                if (name != null && name.Length > 0)
                {
                    // the "about" attribute
                    result.Append(" (");
                    result.Append(name);
                    result.Append(')');
                }
            }
            if (value != null && value.Length > 0)
            {
                result.Append(" = \"");
                result.Append(value);
                result.Append('"');
            }
            // render options if at least one is set
            if (GetOptions().ContainsOneOf(unchecked((int)(0xffffffff))))
            {
                result.Append("\t(");
                result.Append(GetOptions().ToString());
                result.Append(" : ");
                result.Append(GetOptions().GetOptionsString());
                result.Append(')');
            }
            result.Append('\n');
            // render qualifier
            if (recursive && HasQualifier())
            {
                Com.Adobe.Xmp.Impl.XMPNode[] quals = (Com.Adobe.Xmp.Impl.XMPNode[])Sharpen.Collections.ToArray(GetQualifier(), new Com.Adobe.Xmp.Impl.XMPNode[GetQualifierLength()]);
                int i_1 = 0;
                while (quals.Length > i_1 && (XMPConstConstants.XmlLang.Equals(quals[i_1].GetName()) || "rdf:type".Equals(quals[i_1].GetName())))
                {
                    i_1++;
                }
                Arrays.Sort(quals, i_1, quals.Length);
                for (i_1 = 0; i_1 < quals.Length; i_1++)
                {
                    Com.Adobe.Xmp.Impl.XMPNode qualifier = quals[i_1];
                    qualifier.DumpNode(result, recursive, indent + 2, i_1 + 1);
                }
            }
            // render children
            if (recursive && HasChildren())
            {
                Com.Adobe.Xmp.Impl.XMPNode[] children = (Com.Adobe.Xmp.Impl.XMPNode[])Sharpen.Collections.ToArray(GetChildren(), new Com.Adobe.Xmp.Impl.XMPNode[GetChildrenLength()]);
                if (!GetOptions().IsArray())
                {
                    Arrays.Sort(children);
                }
                for (int i_1 = 0; i_1 < children.Length; i_1++)
                {
                    Com.Adobe.Xmp.Impl.XMPNode child = children[i_1];
                    child.DumpNode(result, recursive, indent + 1, i_1 + 1);
                }
            }
        }

        /// <returns>Returns whether this node is a language qualifier.</returns>
        private bool IsLanguageNode()
        {
            return XMPConstConstants.XmlLang.Equals(name);
        }

        /// <returns>Returns whether this node is a type qualifier.</returns>
        private bool IsTypeNode()
        {
            return "rdf:type".Equals(name);
        }

        /// <summary>
        /// <em>Note:</em> This method should always be called when accessing 'children' to be sure
        /// that its initialized.
        /// </summary>
        /// <returns>Returns list of children that is lazy initialized.</returns>
        private IList GetChildren()
        {
            if (children == null)
            {
                children = new ArrayList(0);
            }
            return children;
        }

        /// <returns>Returns a read-only copy of child nodes list.</returns>
        public virtual IList GetUnmodifiableChildren()
        {
            return Sharpen.Collections.UnmodifiableList(new ArrayList(GetChildren()));
        }

        /// <returns>Returns list of qualifier that is lazy initialized.</returns>
        private IList GetQualifier()
        {
            if (qualifier == null)
            {
                qualifier = new ArrayList(0);
            }
            return qualifier;
        }

        /// <summary>
        /// Sets the parent node, this is solely done by <code>addChild(...)</code>
        /// and <code>addQualifier()</code>.
        /// </summary>
        /// <param name="parent">Sets the parent node.</param>
        protected internal virtual void SetParent(Com.Adobe.Xmp.Impl.XMPNode parent)
        {
            this.parent = parent;
        }

        /// <summary>Internal find.</summary>
        /// <param name="list">the list to search in</param>
        /// <param name="expr">the search expression</param>
        /// <returns>Returns the found node or <code>nulls</code>.</returns>
        private Com.Adobe.Xmp.Impl.XMPNode Find(IList list, string expr)
        {
            if (list != null)
            {
                for (Iterator it = list.Iterator(); it.HasNext(); )
                {
                    Com.Adobe.Xmp.Impl.XMPNode child = (Com.Adobe.Xmp.Impl.XMPNode)it.Next();
                    if (child.GetName().Equals(expr))
                    {
                        return child;
                    }
                }
            }
            return null;
        }

        /// <summary>Checks that a node name is not existing on the same level, except for array items.</summary>
        /// <param name="childName">the node name to check</param>
        /// <exception cref="Com.Adobe.Xmp.XMPException">Thrown if a node with the same name is existing.</exception>
        private void AssertChildNotExisting(string childName)
        {
            if (!XMPConstConstants.ArrayItemName.Equals(childName) && FindChildByName(childName) != null)
            {
                throw new XMPException("Duplicate property or field node '" + childName + "'", XMPErrorConstants.Badxmp);
            }
        }

        /// <summary>Checks that a qualifier name is not existing on the same level.</summary>
        /// <param name="qualifierName">the new qualifier name</param>
        /// <exception cref="Com.Adobe.Xmp.XMPException">Thrown if a node with the same name is existing.</exception>
        private void AssertQualifierNotExisting(string qualifierName)
        {
            if (!XMPConstConstants.ArrayItemName.Equals(qualifierName) && FindQualifierByName(qualifierName) != null)
            {
                throw new XMPException("Duplicate '" + qualifierName + "' qualifier", XMPErrorConstants.Badxmp);
            }
        }
    }
}
