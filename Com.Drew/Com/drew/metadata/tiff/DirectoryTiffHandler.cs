/*
 * Copyright 2002-2015 Drew Noakes
 *
 *    Modified by Yakov Danilov <yakodani@gmail.com> for Imazen LLC (Ported from Java to C#)
 *    Licensed under the Apache License, Version 2.0 (the "License");
 *    you may not use this file except in compliance with the License.
 *    You may obtain a copy of the License at
 *
 *        http://www.apache.org/licenses/LICENSE-2.0
 *
 *    Unless required by applicable law or agreed to in writing, software
 *    distributed under the License is distributed on an "AS IS" BASIS,
 *    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 *    See the License for the specific language governing permissions and
 *    limitations under the License.
 *
 * More information about this project is available at:
 *
 *    https://drewnoakes.com/code/exif/
 *    https://github.com/drewnoakes/metadata-extractor
 */
using System;
using System.Collections.Generic;
using Com.Drew.Imaging.Tiff;
using Com.Drew.Lang;
using JetBrains.Annotations;
using Sharpen;

namespace Com.Drew.Metadata.Tiff
{
	/// <summary>
	/// Adapter between the
	/// <see cref="Com.Drew.Imaging.Tiff.TiffHandler"/>
	/// interface and the
	/// <see cref="Com.Drew.Metadata.Metadata"/>
	/// /
	/// <see cref="Com.Drew.Metadata.Directory"/>
	/// object model.
	/// </summary>
	/// <author>Drew Noakes https://drewnoakes.com</author>
	public abstract class DirectoryTiffHandler : TiffHandler
	{
		private readonly Stack<Com.Drew.Metadata.Directory> _directoryStack = new Stack<Com.Drew.Metadata.Directory>();

		protected internal Com.Drew.Metadata.Directory _currentDirectory;

		protected internal readonly Com.Drew.Metadata.Metadata _metadata;

		protected internal DirectoryTiffHandler(Com.Drew.Metadata.Metadata metadata, Type initialDirectoryClass)
		{
			_metadata = metadata;
			try
			{
				_currentDirectory = (Directory)System.Activator.CreateInstance(initialDirectoryClass);
			}
			catch (InstantiationException e)
			{
				throw new RuntimeException(e);
			}
			catch (MemberAccessException e)
			{
				throw new RuntimeException(e);
			}
			_metadata.AddDirectory(_currentDirectory);
		}

		public virtual void EndingIFD()
		{
			_currentDirectory = _directoryStack.IsEmpty() ? null : _directoryStack.Pop();
		}

		protected internal virtual void PushDirectory([NotNull] Type directoryClass)
		{
			_directoryStack.Push(_currentDirectory);
			try
			{
				_currentDirectory = (Directory)System.Activator.CreateInstance(directoryClass);
			}
			catch (InstantiationException e)
			{
				throw new RuntimeException(e);
			}
			catch (MemberAccessException e)
			{
				throw new RuntimeException(e);
			}
			_metadata.AddDirectory(_currentDirectory);
		}

		public virtual void Warn([NotNull] string message)
		{
			_currentDirectory.AddError(message);
		}

		public virtual void Error([NotNull] string message)
		{
			_currentDirectory.AddError(message);
		}

		public virtual void SetByteArray(int tagId, [NotNull] sbyte[] bytes)
		{
			_currentDirectory.SetByteArray(tagId, bytes);
		}

		public virtual void SetString(int tagId, [NotNull] string @string)
		{
			_currentDirectory.SetString(tagId, @string);
		}

		public virtual void SetRational(int tagId, [NotNull] Rational rational)
		{
			_currentDirectory.SetRational(tagId, rational);
		}

		public virtual void SetRationalArray(int tagId, [NotNull] Rational[] array)
		{
			_currentDirectory.SetRationalArray(tagId, array);
		}

		public virtual void SetFloat(int tagId, float float32)
		{
			_currentDirectory.SetFloat(tagId, float32);
		}

		public virtual void SetFloatArray(int tagId, [NotNull] float[] array)
		{
			_currentDirectory.SetFloatArray(tagId, array);
		}

		public virtual void SetDouble(int tagId, double double64)
		{
			_currentDirectory.SetDouble(tagId, double64);
		}

		public virtual void SetDoubleArray(int tagId, [NotNull] double[] array)
		{
			_currentDirectory.SetDoubleArray(tagId, array);
		}

		public virtual void SetInt8s(int tagId, sbyte int8s)
		{
			// NOTE Directory stores all integral types as int32s, except for int32u and long
			_currentDirectory.SetInt(tagId, int8s);
		}

		public virtual void SetInt8sArray(int tagId, [NotNull] sbyte[] array)
		{
			// NOTE Directory stores all integral types as int32s, except for int32u and long
			_currentDirectory.SetByteArray(tagId, array);
		}

		public virtual void SetInt8u(int tagId, short int8u)
		{
			// NOTE Directory stores all integral types as int32s, except for int32u and long
			_currentDirectory.SetInt(tagId, int8u);
		}

		public virtual void SetInt8uArray(int tagId, [NotNull] short[] array)
		{
			// TODO create and use a proper setter for short[]
			_currentDirectory.SetObjectArray(tagId, array);
		}

		public virtual void SetInt16s(int tagId, int int16s)
		{
			// TODO create and use a proper setter for int16u?
			_currentDirectory.SetInt(tagId, int16s);
		}

		public virtual void SetInt16sArray(int tagId, [NotNull] short[] array)
		{
			// TODO create and use a proper setter for short[]
			_currentDirectory.SetObjectArray(tagId, array);
		}

		public virtual void SetInt16u(int tagId, int int16u)
		{
			// TODO create and use a proper setter for
			_currentDirectory.SetInt(tagId, int16u);
		}

		public virtual void SetInt16uArray(int tagId, [NotNull] int[] array)
		{
			// TODO create and use a proper setter for short[]
			_currentDirectory.SetObjectArray(tagId, array);
		}

		public virtual void SetInt32s(int tagId, int int32s)
		{
			_currentDirectory.SetInt(tagId, int32s);
		}

		public virtual void SetInt32sArray(int tagId, [NotNull] int[] array)
		{
			_currentDirectory.SetIntArray(tagId, array);
		}

		public virtual void SetInt32u(int tagId, long int32u)
		{
			_currentDirectory.SetLong(tagId, int32u);
		}

		public virtual void SetInt32uArray(int tagId, [NotNull] long[] array)
		{
			// TODO create and use a proper setter for short[]
			_currentDirectory.SetObjectArray(tagId, array);
		}

		public abstract void Completed(RandomAccessReader arg1, int arg2);

		public abstract bool CustomProcessTag(int arg1, ICollection<int?> arg2, int arg3, RandomAccessReader arg4, int arg5, int arg6);

		public abstract bool HasFollowerIfd();

		public abstract bool IsTagIfdPointer(int arg1);

		public abstract void SetTiffMarker(int arg1);
	}
}
