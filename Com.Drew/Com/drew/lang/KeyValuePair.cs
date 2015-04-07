using JetBrains.Annotations;
using Sharpen;

namespace Com.Drew.Lang
{
	/// <summary>
	/// Models a key/value pair, where both are non-null
	/// <see cref="string"/>
	/// objects.
	/// </summary>
	/// <author>Drew Noakes https://drewnoakes.com</author>
	public class KeyValuePair
	{
		private readonly string _key;

		private readonly string _value;

		public KeyValuePair([NotNull] string key, [NotNull] string value)
		{
			_key = key;
			_value = value;
		}

		[NotNull]
		public virtual string GetKey()
		{
			return _key;
		}

		[NotNull]
		public virtual string GetValue()
		{
			return _value;
		}
	}
}
