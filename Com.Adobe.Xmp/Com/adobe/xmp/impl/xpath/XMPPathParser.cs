// =================================================================================================
// ADOBE SYSTEMS INCORPORATED
// Copyright 2006 Adobe Systems Incorporated
// All Rights Reserved
//
// NOTICE:  Adobe permits you to use, modify, and distribute this file in accordance with the terms
// of the Adobe license agreement accompanying it.
// =================================================================================================
using Com.Adobe.Xmp;
using Com.Adobe.Xmp.Impl;
using Com.Adobe.Xmp.Properties;
using Sharpen;

namespace Com.Adobe.Xmp.Impl.Xpath
{
	/// <summary>Parser for XMP XPaths.</summary>
	/// <since>01.03.2006</since>
	public sealed class XMPPathParser
	{
		/// <summary>Private constructor</summary>
		private XMPPathParser()
		{
		}

		// empty
		/// <summary>
		/// Split an XMPPath expression apart at the conceptual steps, adding the
		/// root namespace prefix to the first property component.
		/// </summary>
		/// <remarks>
		/// Split an XMPPath expression apart at the conceptual steps, adding the
		/// root namespace prefix to the first property component. The schema URI is
		/// put in the first (0th) slot in the expanded XMPPath. Check if the top
		/// level component is an alias, but don't resolve it.
		/// <p>
		/// In the most verbose case steps are separated by '/', and each step can be
		/// of these forms:
		/// <dl>
		/// <dt>prefix:name
		/// <dd> A top level property or struct field.
		/// <dt>[index]
		/// <dd> An element of an array.
		/// <dt>[last()]
		/// <dd> The last element of an array.
		/// <dt>[fieldName=&quot;value&quot;]
		/// <dd> An element in an array of structs, chosen by a field value.
		/// <dt>[@xml:lang=&quot;value&quot;]
		/// <dd> An element in an alt-text array, chosen by the xml:lang qualifier.
		/// <dt>[?qualName=&quot;value&quot;]
		/// <dd> An element in an array, chosen by a qualifier value.
		/// <dt>@xml:lang
		/// <dd> An xml:lang qualifier.
		/// <dt>?qualName
		/// <dd> A general qualifier.
		/// </dl>
		/// <p>
		/// The logic is complicated though by shorthand for arrays, the separating
		/// '/' and leading '*' are optional. These are all equivalent: array/*[2]
		/// array/[2] array*[2] array[2] All of these are broken into the 2 steps
		/// "array" and "[2]".
		/// <p>
		/// The value portion in the array selector forms is a string quoted by '''
		/// or '"'. The value may contain any character including a doubled quoting
		/// character. The value may be empty.
		/// <p>
		/// The syntax isn't checked, but an XML name begins with a letter or '_',
		/// and contains letters, digits, '.', '-', '_', and a bunch of special
		/// non-ASCII Unicode characters. An XML qualified name is a pair of names
		/// separated by a colon.
		/// </remarks>
		/// <param name="schemaNS">schema namespace</param>
		/// <param name="path">property name</param>
		/// <returns>Returns the expandet XMPPath.</returns>
		/// <exception cref="Com.Adobe.Xmp.XMPException">Thrown if the format is not correct somehow.</exception>
		public static XMPPath ExpandXPath(string schemaNS, string path)
		{
			if (schemaNS == null || path == null)
			{
				throw new XMPException("Parameter must not be null", XMPErrorConstants.Badparam);
			}
			XMPPath expandedXPath = new XMPPath();
			PathPosition pos = new PathPosition();
			pos.path = path;
			// Pull out the first component and do some special processing on it: add the schema
			// namespace prefix and and see if it is an alias. The start must be a "qualName".
			ParseRootNode(schemaNS, pos, expandedXPath);
			// Now continue to process the rest of the XMPPath string.
			while (pos.stepEnd < path.Length)
			{
				pos.stepBegin = pos.stepEnd;
				SkipPathDelimiter(path, pos);
				pos.stepEnd = pos.stepBegin;
				XMPPathSegment segment;
				if (path[pos.stepBegin] != '[')
				{
					// A struct field or qualifier.
					segment = ParseStructSegment(pos);
				}
				else
				{
					// One of the array forms.
					segment = ParseIndexSegment(pos);
				}
				if (segment.GetKind() == XMPPath.StructFieldStep)
				{
					if (segment.GetName()[0] == '@')
					{
						segment.SetName("?" + Sharpen.Runtime.Substring(segment.GetName(), 1));
						if (!"?xml:lang".Equals(segment.GetName()))
						{
							throw new XMPException("Only xml:lang allowed with '@'", XMPErrorConstants.Badxpath);
						}
					}
					if (segment.GetName()[0] == '?')
					{
						pos.nameStart++;
						segment.SetKind(XMPPath.QualifierStep);
					}
					VerifyQualName(Sharpen.Runtime.Substring(pos.path, pos.nameStart, pos.nameEnd));
				}
				else
				{
					if (segment.GetKind() == XMPPath.FieldSelectorStep)
					{
						if (segment.GetName()[1] == '@')
						{
							segment.SetName("[?" + Sharpen.Runtime.Substring(segment.GetName(), 2));
							if (!segment.GetName().StartsWith("[?xml:lang="))
							{
								throw new XMPException("Only xml:lang allowed with '@'", XMPErrorConstants.Badxpath);
							}
						}
						if (segment.GetName()[1] == '?')
						{
							pos.nameStart++;
							segment.SetKind(XMPPath.QualSelectorStep);
							VerifyQualName(Sharpen.Runtime.Substring(pos.path, pos.nameStart, pos.nameEnd));
						}
					}
				}
				expandedXPath.Add(segment);
			}
			return expandedXPath;
		}

		/// <param name="path"/>
		/// <param name="pos"/>
		/// <exception cref="Com.Adobe.Xmp.XMPException"/>
		private static void SkipPathDelimiter(string path, PathPosition pos)
		{
			if (path[pos.stepBegin] == '/')
			{
				// skip slash
				pos.stepBegin++;
				// added for Java
				if (pos.stepBegin >= path.Length)
				{
					throw new XMPException("Empty XMPPath segment", XMPErrorConstants.Badxpath);
				}
			}
			if (path[pos.stepBegin] == '*')
			{
				// skip asterisk
				pos.stepBegin++;
				if (pos.stepBegin >= path.Length || path[pos.stepBegin] != '[')
				{
					throw new XMPException("Missing '[' after '*'", XMPErrorConstants.Badxpath);
				}
			}
		}

		/// <summary>Parses a struct segment</summary>
		/// <param name="pos">the current position in the path</param>
		/// <returns>Retusn the segment or an errror</returns>
		/// <exception cref="Com.Adobe.Xmp.XMPException">If the sement is empty</exception>
		private static XMPPathSegment ParseStructSegment(PathPosition pos)
		{
			pos.nameStart = pos.stepBegin;
			while (pos.stepEnd < pos.path.Length && "/[*".IndexOf(pos.path[pos.stepEnd]) < 0)
			{
				pos.stepEnd++;
			}
			pos.nameEnd = pos.stepEnd;
			if (pos.stepEnd == pos.stepBegin)
			{
				throw new XMPException("Empty XMPPath segment", XMPErrorConstants.Badxpath);
			}
			// ! Touch up later, also changing '@' to '?'.
			XMPPathSegment segment = new XMPPathSegment(Sharpen.Runtime.Substring(pos.path, pos.stepBegin, pos.stepEnd), XMPPath.StructFieldStep);
			return segment;
		}

		/// <summary>Parses an array index segment.</summary>
		/// <param name="pos">the xmp path</param>
		/// <returns>Returns the segment or an error</returns>
		/// <exception cref="Com.Adobe.Xmp.XMPException">thrown on xmp path errors</exception>
		private static XMPPathSegment ParseIndexSegment(PathPosition pos)
		{
			XMPPathSegment segment;
			pos.stepEnd++;
			// Look at the character after the leading '['.
			if ('0' <= pos.path[pos.stepEnd] && pos.path[pos.stepEnd] <= '9')
			{
				// A numeric (decimal integer) array index.
				while (pos.stepEnd < pos.path.Length && '0' <= pos.path[pos.stepEnd] && pos.path[pos.stepEnd] <= '9')
				{
					pos.stepEnd++;
				}
				segment = new XMPPathSegment(null, XMPPath.ArrayIndexStep);
			}
			else
			{
				// Could be "[last()]" or one of the selector forms. Find the ']' or '='.
				while (pos.stepEnd < pos.path.Length && pos.path[pos.stepEnd] != ']' && pos.path[pos.stepEnd] != '=')
				{
					pos.stepEnd++;
				}
				if (pos.stepEnd >= pos.path.Length)
				{
					throw new XMPException("Missing ']' or '=' for array index", XMPErrorConstants.Badxpath);
				}
				if (pos.path[pos.stepEnd] == ']')
				{
					if (!"[last()".Equals(Sharpen.Runtime.Substring(pos.path, pos.stepBegin, pos.stepEnd)))
					{
						throw new XMPException("Invalid non-numeric array index", XMPErrorConstants.Badxpath);
					}
					segment = new XMPPathSegment(null, XMPPath.ArrayLastStep);
				}
				else
				{
					pos.nameStart = pos.stepBegin + 1;
					pos.nameEnd = pos.stepEnd;
					pos.stepEnd++;
					// Absorb the '=', remember the quote.
					char quote = pos.path[pos.stepEnd];
					if (quote != '\'' && quote != '"')
					{
						throw new XMPException("Invalid quote in array selector", XMPErrorConstants.Badxpath);
					}
					pos.stepEnd++;
					// Absorb the leading quote.
					while (pos.stepEnd < pos.path.Length)
					{
						if (pos.path[pos.stepEnd] == quote)
						{
							// check for escaped quote
							if (pos.stepEnd + 1 >= pos.path.Length || pos.path[pos.stepEnd + 1] != quote)
							{
								break;
							}
							pos.stepEnd++;
						}
						pos.stepEnd++;
					}
					if (pos.stepEnd >= pos.path.Length)
					{
						throw new XMPException("No terminating quote for array selector", XMPErrorConstants.Badxpath);
					}
					pos.stepEnd++;
					// Absorb the trailing quote.
					// ! Touch up later, also changing '@' to '?'.
					segment = new XMPPathSegment(null, XMPPath.FieldSelectorStep);
				}
			}
			if (pos.stepEnd >= pos.path.Length || pos.path[pos.stepEnd] != ']')
			{
				throw new XMPException("Missing ']' for array index", XMPErrorConstants.Badxpath);
			}
			pos.stepEnd++;
			segment.SetName(Sharpen.Runtime.Substring(pos.path, pos.stepBegin, pos.stepEnd));
			return segment;
		}

		/// <summary>
		/// Parses the root node of an XMP Path, checks if namespace and prefix fit together
		/// and resolve the property to the base property if it is an alias.
		/// </summary>
		/// <param name="schemaNS">the root namespace</param>
		/// <param name="pos">the parsing position helper</param>
		/// <param name="expandedXPath">the path to contribute to</param>
		/// <exception cref="Com.Adobe.Xmp.XMPException">If the path is not valid.</exception>
		private static void ParseRootNode(string schemaNS, PathPosition pos, XMPPath expandedXPath)
		{
			while (pos.stepEnd < pos.path.Length && "/[*".IndexOf(pos.path[pos.stepEnd]) < 0)
			{
				pos.stepEnd++;
			}
			if (pos.stepEnd == pos.stepBegin)
			{
				throw new XMPException("Empty initial XMPPath step", XMPErrorConstants.Badxpath);
			}
			string rootProp = VerifyXPathRoot(schemaNS, Sharpen.Runtime.Substring(pos.path, pos.stepBegin, pos.stepEnd));
			XMPAliasInfo aliasInfo = XMPMetaFactory.GetSchemaRegistry().FindAlias(rootProp);
			if (aliasInfo == null)
			{
				// add schema xpath step
				expandedXPath.Add(new XMPPathSegment(schemaNS, XMPPath.SchemaNode));
				XMPPathSegment rootStep = new XMPPathSegment(rootProp, XMPPath.StructFieldStep);
				expandedXPath.Add(rootStep);
			}
			else
			{
				// add schema xpath step and base step of alias
				expandedXPath.Add(new XMPPathSegment(aliasInfo.GetNamespace(), XMPPath.SchemaNode));
				XMPPathSegment rootStep = new XMPPathSegment(VerifyXPathRoot(aliasInfo.GetNamespace(), aliasInfo.GetPropName()), XMPPath.StructFieldStep);
				rootStep.SetAlias(true);
				rootStep.SetAliasForm(aliasInfo.GetAliasForm().GetOptions());
				expandedXPath.Add(rootStep);
				if (aliasInfo.GetAliasForm().IsArrayAltText())
				{
					XMPPathSegment qualSelectorStep = new XMPPathSegment("[?xml:lang='x-default']", XMPPath.QualSelectorStep);
					qualSelectorStep.SetAlias(true);
					qualSelectorStep.SetAliasForm(aliasInfo.GetAliasForm().GetOptions());
					expandedXPath.Add(qualSelectorStep);
				}
				else
				{
					if (aliasInfo.GetAliasForm().IsArray())
					{
						XMPPathSegment indexStep = new XMPPathSegment("[1]", XMPPath.ArrayIndexStep);
						indexStep.SetAlias(true);
						indexStep.SetAliasForm(aliasInfo.GetAliasForm().GetOptions());
						expandedXPath.Add(indexStep);
					}
				}
			}
		}

		/// <summary>
		/// Verifies whether the qualifier name is not XML conformant or the
		/// namespace prefix has not been registered.
		/// </summary>
		/// <param name="qualName">a qualifier name</param>
		/// <exception cref="Com.Adobe.Xmp.XMPException">If the name is not conformant</exception>
		private static void VerifyQualName(string qualName)
		{
			int colonPos = qualName.IndexOf(':');
			if (colonPos > 0)
			{
				string prefix = Sharpen.Runtime.Substring(qualName, 0, colonPos);
				if (Utils.IsXMLNameNS(prefix))
				{
					string regURI = XMPMetaFactory.GetSchemaRegistry().GetNamespaceURI(prefix);
					if (regURI != null)
					{
						return;
					}
					throw new XMPException("Unknown namespace prefix for qualified name", XMPErrorConstants.Badxpath);
				}
			}
			throw new XMPException("Ill-formed qualified name", XMPErrorConstants.Badxpath);
		}

		/// <summary>Verify if an XML name is conformant.</summary>
		/// <param name="name">an XML name</param>
		/// <exception cref="Com.Adobe.Xmp.XMPException">When the name is not XML conformant</exception>
		private static void VerifySimpleXMLName(string name)
		{
			if (!Utils.IsXMLName(name))
			{
				throw new XMPException("Bad XML name", XMPErrorConstants.Badxpath);
			}
		}

		/// <summary>Set up the first 2 components of the expanded XMPPath.</summary>
		/// <remarks>
		/// Set up the first 2 components of the expanded XMPPath. Normalizes the various cases of using
		/// the full schema URI and/or a qualified root property name. Returns true for normal
		/// processing. If allowUnknownSchemaNS is true and the schema namespace is not registered, false
		/// is returned. If allowUnknownSchemaNS is false and the schema namespace is not registered, an
		/// exception is thrown
		/// <P>
		/// (Should someday check the full syntax:)
		/// </remarks>
		/// <param name="schemaNS">schema namespace</param>
		/// <param name="rootProp">the root xpath segment</param>
		/// <returns>Returns root QName.</returns>
		/// <exception cref="Com.Adobe.Xmp.XMPException">Thrown if the format is not correct somehow.</exception>
		private static string VerifyXPathRoot(string schemaNS, string rootProp)
		{
			// Do some basic checks on the URI and name. Try to lookup the URI. See if the name is
			// qualified.
			if (schemaNS == null || schemaNS.Length == 0)
			{
				throw new XMPException("Schema namespace URI is required", XMPErrorConstants.Badschema);
			}
			if ((rootProp[0] == '?') || (rootProp[0] == '@'))
			{
				throw new XMPException("Top level name must not be a qualifier", XMPErrorConstants.Badxpath);
			}
			if (rootProp.IndexOf('/') >= 0 || rootProp.IndexOf('[') >= 0)
			{
				throw new XMPException("Top level name must be simple", XMPErrorConstants.Badxpath);
			}
			string prefix = XMPMetaFactory.GetSchemaRegistry().GetNamespacePrefix(schemaNS);
			if (prefix == null)
			{
				throw new XMPException("Unregistered schema namespace URI", XMPErrorConstants.Badschema);
			}
			// Verify the various URI and prefix combinations. Initialize the
			// expanded XMPPath.
			int colonPos = rootProp.IndexOf(':');
			if (colonPos < 0)
			{
				// The propName is unqualified, use the schemaURI and associated
				// prefix.
				VerifySimpleXMLName(rootProp);
				// Verify the part before any colon
				return prefix + rootProp;
			}
			else
			{
				// The propName is qualified. Make sure the prefix is legit. Use the associated URI and
				// qualified name.
				// Verify the part before any colon
				VerifySimpleXMLName(Sharpen.Runtime.Substring(rootProp, 0, colonPos));
				VerifySimpleXMLName(Sharpen.Runtime.Substring(rootProp, colonPos));
				prefix = Sharpen.Runtime.Substring(rootProp, 0, colonPos + 1);
				string regPrefix = XMPMetaFactory.GetSchemaRegistry().GetNamespacePrefix(schemaNS);
				if (regPrefix == null)
				{
					throw new XMPException("Unknown schema namespace prefix", XMPErrorConstants.Badschema);
				}
				if (!prefix.Equals(regPrefix))
				{
					throw new XMPException("Schema namespace URI and prefix mismatch", XMPErrorConstants.Badschema);
				}
				return rootProp;
			}
		}
	}

	/// <summary>This objects contains all needed char positions to parse.</summary>
	internal class PathPosition
	{
		/// <summary>the complete path</summary>
		public string path = null;

		/// <summary>the start of a segment name</summary>
		internal int nameStart = 0;

		/// <summary>the end of a segment name</summary>
		internal int nameEnd = 0;

		/// <summary>the begin of a step</summary>
		internal int stepBegin = 0;

		/// <summary>the end of a step</summary>
		internal int stepEnd = 0;
	}
}
