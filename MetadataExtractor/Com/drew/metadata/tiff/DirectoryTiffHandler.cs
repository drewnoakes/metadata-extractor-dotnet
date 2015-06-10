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
    /// <see cref="ITiffHandler"/>
    /// interface and the
    /// <see cref="Com.Drew.Metadata.Metadata"/>
    /// /
    /// <see cref="Com.Drew.Metadata.Directory"/>
    /// object model.
    /// </summary>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public abstract class DirectoryTiffHandler : ITiffHandler
    {
        private readonly Stack<Directory> _directoryStack = new Stack<Directory>();

        protected Directory CurrentDirectory;

        protected readonly Metadata Metadata;

        protected DirectoryTiffHandler(Metadata metadata, Type initialDirectoryClass)
        {
            Metadata = metadata;
            try
            {
                CurrentDirectory = (Directory)Activator.CreateInstance(initialDirectoryClass);
            }
            catch (InstantiationException e)
            {
                throw new RuntimeException(e);
            }
            catch (MemberAccessException e)
            {
                throw new RuntimeException(e);
            }
            Metadata.AddDirectory(CurrentDirectory);
        }

        public virtual void EndingIfd()
        {
            CurrentDirectory = _directoryStack.Count == 0 ? null : _directoryStack.Pop();
        }

        protected virtual void PushDirectory([NotNull] Type directoryClass)
        {
            _directoryStack.Push(CurrentDirectory);
            try
            {
                CurrentDirectory = (Directory)Activator.CreateInstance(directoryClass);
            }
            catch (InstantiationException e)
            {
                throw new RuntimeException(e);
            }
            catch (MemberAccessException e)
            {
                throw new RuntimeException(e);
            }
            Metadata.AddDirectory(CurrentDirectory);
        }

        public virtual void Warn(string message)
        {
            CurrentDirectory.AddError(message);
        }

        public virtual void Error(string message)
        {
            CurrentDirectory.AddError(message);
        }

        public virtual void SetByteArray(int tagId, byte[] bytes)
        {
            CurrentDirectory.SetByteArray(tagId, bytes);
        }

        public virtual void SetString(int tagId, string @string)
        {
            CurrentDirectory.SetString(tagId, @string);
        }

        public virtual void SetRational(int tagId, Rational rational)
        {
            CurrentDirectory.SetRational(tagId, rational);
        }

        public virtual void SetRationalArray(int tagId, Rational[] array)
        {
            CurrentDirectory.SetRationalArray(tagId, array);
        }

        public virtual void SetFloat(int tagId, float float32)
        {
            CurrentDirectory.SetFloat(tagId, float32);
        }

        public virtual void SetFloatArray(int tagId, float[] array)
        {
            CurrentDirectory.SetFloatArray(tagId, array);
        }

        public virtual void SetDouble(int tagId, double double64)
        {
            CurrentDirectory.SetDouble(tagId, double64);
        }

        public virtual void SetDoubleArray(int tagId, double[] array)
        {
            CurrentDirectory.SetDoubleArray(tagId, array);
        }

        public virtual void SetInt8S(int tagId, sbyte int8S)
        {
            // NOTE Directory stores all integral types as int32s, except for int32u and long
            CurrentDirectory.SetInt(tagId, int8S);
        }

        public virtual void SetInt8SArray(int tagId, sbyte[] array)
        {
            // NOTE Directory stores all integral types as int32s, except for int32u and long
            CurrentDirectory.SetSByteArray(tagId, array);
        }

        public virtual void SetInt8U(int tagId, byte int8U)
        {
            // NOTE Directory stores all integral types as int32s, except for int32u and long
            CurrentDirectory.SetInt(tagId, int8U);
        }

        public virtual void SetInt8UArray(int tagId, byte[] array)
        {
            // TODO create and use a proper setter for short[]
            CurrentDirectory.SetObjectArray(tagId, array);
        }

        public virtual void SetInt16S(int tagId, short int16S)
        {
            // TODO create and use a proper setter for int16u?
            CurrentDirectory.SetInt(tagId, int16S);
        }

        public virtual void SetInt16SArray(int tagId, short[] array)
        {
            // TODO create and use a proper setter for short[]
            CurrentDirectory.SetObjectArray(tagId, array);
        }

        public virtual void SetInt16U(int tagId, ushort int16U)
        {
            // TODO create and use a proper setter for
            CurrentDirectory.SetInt(tagId, int16U);
        }

        public virtual void SetInt16UArray(int tagId, ushort[] array)
        {
            // TODO create and use a proper setter for short[]
            CurrentDirectory.SetObjectArray(tagId, array);
        }

        public virtual void SetInt32S(int tagId, int int32S)
        {
            CurrentDirectory.SetInt(tagId, int32S);
        }

        public virtual void SetInt32SArray(int tagId, int[] array)
        {
            CurrentDirectory.SetIntArray(tagId, array);
        }

        public virtual void SetInt32U(int tagId, uint int32U)
        {
            CurrentDirectory.SetLong(tagId, int32U);
        }

        public virtual void SetInt32UArray(int tagId, uint[] array)
        {
            // TODO create and use a proper setter for short[]
            CurrentDirectory.SetObjectArray(tagId, array);
        }

        public abstract void Completed(IndexedReader arg1, int arg2);

        public abstract bool CustomProcessTag(int arg1, ICollection<int?> arg2, int arg3, IndexedReader arg4, int arg5, int arg6);

        public abstract bool HasFollowerIfd();

        public abstract bool IsTagIfdPointer(int arg1);

        public abstract void SetTiffMarker(int arg1);
    }
}
