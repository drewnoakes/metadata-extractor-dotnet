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
    public class XmpPathSegment
    {
        /// <summary>name of the path segment</summary>
        private string _name;

        /// <summary>kind of the path segment</summary>
        private int _kind;

        /// <summary>flag if segment is an alias</summary>
        private bool _alias;

        /// <summary>alias form if applicable</summary>
        private int _aliasForm;

        /// <summary>Constructor with initial values.</summary>
        /// <param name="name">the name of the segment</param>
        public XmpPathSegment(string name)
        {
            _name = name;
        }

        /// <summary>Constructor with initial values.</summary>
        /// <param name="name">the name of the segment</param>
        /// <param name="kind">the kind of the segment</param>
        public XmpPathSegment(string name, int kind)
        {
            _name = name;
            _kind = kind;
        }

        /// <returns>Returns the kind.</returns>
        public virtual int GetKind()
        {
            return _kind;
        }

        /// <param name="kind">The kind to set.</param>
        public virtual void SetKind(int kind)
        {
            _kind = kind;
        }

        /// <returns>Returns the name.</returns>
        public virtual string GetName()
        {
            return _name;
        }

        /// <param name="name">The name to set.</param>
        public virtual void SetName(string name)
        {
            _name = name;
        }

        /// <param name="alias">the flag to set</param>
        public virtual void SetAlias(bool alias)
        {
            _alias = alias;
        }

        /// <returns>Returns the alias.</returns>
        public virtual bool IsAlias()
        {
            return _alias;
        }

        /// <returns>Returns the aliasForm if this segment has been created by an alias.</returns>
        public virtual int GetAliasForm()
        {
            return _aliasForm;
        }

        /// <param name="aliasForm">the aliasForm to set</param>
        public virtual void SetAliasForm(int aliasForm)
        {
            _aliasForm = aliasForm;
        }

        /// <seealso cref="object.ToString()"/>
        public override string ToString()
        {
            switch (_kind)
            {
                case XmpPath.StructFieldStep:
                case XmpPath.ArrayIndexStep:
                case XmpPath.QualifierStep:
                case XmpPath.ArrayLastStep:
                {
                    return _name;
                }

                case XmpPath.QualSelectorStep:
                case XmpPath.FieldSelectorStep:
                {
                    return _name;
                }

                default:
                {
                    // no defined step
                    return _name;
                }
            }
        }
    }
}
