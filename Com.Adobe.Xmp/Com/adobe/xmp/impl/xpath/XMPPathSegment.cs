// =================================================================================================
// ADOBE SYSTEMS INCORPORATED
// Copyright 2006 Adobe Systems Incorporated
// All Rights Reserved
//
// NOTICE:  Adobe permits you to use, modify, and distribute this file in accordance with the terms
// of the Adobe license agreement accompanying it.
// =================================================================================================

namespace Com.Adobe.Xmp.Impl.Xpath
{
    /// <summary>A segment of a parsed <code>XMPPath</code>.</summary>
    /// <since>23.06.2006</since>
    public class XMPPathSegment
    {
        /// <summary>name of the path segment</summary>
        private string name;

        /// <summary>kind of the path segment</summary>
        private int kind;

        /// <summary>flag if segment is an alias</summary>
        private bool alias;

        /// <summary>alias form if applicable</summary>
        private int aliasForm;

        /// <summary>Constructor with initial values.</summary>
        /// <param name="name">the name of the segment</param>
        public XMPPathSegment(string name)
        {
            this.name = name;
        }

        /// <summary>Constructor with initial values.</summary>
        /// <param name="name">the name of the segment</param>
        /// <param name="kind">the kind of the segment</param>
        public XMPPathSegment(string name, int kind)
        {
            this.name = name;
            this.kind = kind;
        }

        /// <returns>Returns the kind.</returns>
        public virtual int GetKind()
        {
            return kind;
        }

        /// <param name="kind">The kind to set.</param>
        public virtual void SetKind(int kind)
        {
            this.kind = kind;
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

        /// <param name="alias">the flag to set</param>
        public virtual void SetAlias(bool alias)
        {
            this.alias = alias;
        }

        /// <returns>Returns the alias.</returns>
        public virtual bool IsAlias()
        {
            return alias;
        }

        /// <returns>Returns the aliasForm if this segment has been created by an alias.</returns>
        public virtual int GetAliasForm()
        {
            return aliasForm;
        }

        /// <param name="aliasForm">the aliasForm to set</param>
        public virtual void SetAliasForm(int aliasForm)
        {
            this.aliasForm = aliasForm;
        }

        /// <seealso cref="object.ToString()"/>
        public override string ToString()
        {
            switch (kind)
            {
                case XMPPath.StructFieldStep:
                case XMPPath.ArrayIndexStep:
                case XMPPath.QualifierStep:
                case XMPPath.ArrayLastStep:
                {
                    return name;
                }

                case XMPPath.QualSelectorStep:
                case XMPPath.FieldSelectorStep:
                {
                    return name;
                }

                default:
                {
                    // no defined step
                    return name;
                }
            }
        }
    }
}
