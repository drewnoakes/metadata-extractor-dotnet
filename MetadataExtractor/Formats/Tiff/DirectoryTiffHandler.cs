#region License
//
// Copyright 2002-2015 Drew Noakes
// Ported from Java to C# by Yakov Danilov for Imazen LLC in 2014
//
//    Licensed under the Apache License, Version 2.0 (the "License");
//    you may not use this file except in compliance with the License.
//    You may obtain a copy of the License at
//
//        http://www.apache.org/licenses/LICENSE-2.0
//
//    Unless required by applicable law or agreed to in writing, software
//    distributed under the License is distributed on an "AS IS" BASIS,
//    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//    See the License for the specific language governing permissions and
//    limitations under the License.
//
// More information about this project is available at:
//
//    https://github.com/drewnoakes/metadata-extractor-dotnet
//    https://drewnoakes.com/code/exif/
//
#endregion

using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using MetadataExtractor.IO;

namespace MetadataExtractor.Formats.Tiff
{
    /// <summary>
    /// An implementation of <see cref="ITiffHandler"/> that stores tag values in <see cref="Directory"/> objects.
    /// </summary>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public abstract class DirectoryTiffHandler : ITiffHandler
    {
        private readonly Stack<Directory> _directoryStack = new Stack<Directory>();

        protected Directory CurrentDirectory;

        protected readonly List<Directory> Directories;

        protected DirectoryTiffHandler(List<Directory> directories, Type initialDirectoryClass)
        {
            Directories = directories;
            CurrentDirectory = (Directory)Activator.CreateInstance(initialDirectoryClass);
            Directories.Add(CurrentDirectory);
        }

        public virtual void EndingIfd()
        {
            CurrentDirectory = _directoryStack.Count == 0 ? null : _directoryStack.Pop();
        }

        protected void PushDirectory([NotNull] Type directoryClass)
        {
            _directoryStack.Push(CurrentDirectory);
            CurrentDirectory = (Directory)Activator.CreateInstance(directoryClass);
            Directories.Add(CurrentDirectory);
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
            CurrentDirectory.Set(tagId, bytes);
        }

        public virtual void SetString(int tagId, string @string)
        {
            CurrentDirectory.Set(tagId, @string);
        }

        public virtual void SetRational(int tagId, Rational rational)
        {
            CurrentDirectory.Set(tagId, rational);
        }

        public virtual void SetRationalArray(int tagId, Rational[] array)
        {
            CurrentDirectory.Set(tagId, array);
        }

        public virtual void SetFloat(int tagId, float float32)
        {
            CurrentDirectory.Set(tagId, float32);
        }

        public virtual void SetFloatArray(int tagId, float[] array)
        {
            CurrentDirectory.Set(tagId, array);
        }

        public virtual void SetDouble(int tagId, double double64)
        {
            CurrentDirectory.Set(tagId, double64);
        }

        public virtual void SetDoubleArray(int tagId, double[] array)
        {
            CurrentDirectory.Set(tagId, array);
        }

        public virtual void SetInt8S(int tagId, sbyte int8S)
        {
            CurrentDirectory.Set(tagId, int8S);
        }

        public virtual void SetInt8SArray(int tagId, sbyte[] array)
        {
            CurrentDirectory.Set(tagId, array);
        }

        public virtual void SetInt8U(int tagId, byte int8U)
        {
            CurrentDirectory.Set(tagId, int8U);
        }

        public virtual void SetInt8UArray(int tagId, byte[] array)
        {
            CurrentDirectory.Set(tagId, array);
        }

        public virtual void SetInt16S(int tagId, short int16S)
        {
            CurrentDirectory.Set(tagId, int16S);
        }

        public virtual void SetInt16SArray(int tagId, short[] array)
        {
            CurrentDirectory.Set(tagId, array);
        }

        public virtual void SetInt16U(int tagId, ushort int16U)
        {
            CurrentDirectory.Set(tagId, int16U);
        }

        public virtual void SetInt16UArray(int tagId, ushort[] array)
        {
            CurrentDirectory.Set(tagId, array);
        }

        public virtual void SetInt32S(int tagId, int int32S)
        {
            CurrentDirectory.Set(tagId, int32S);
        }

        public virtual void SetInt32SArray(int tagId, int[] array)
        {
            CurrentDirectory.Set(tagId, array);
        }

        public virtual void SetInt32U(int tagId, uint int32U)
        {
            CurrentDirectory.Set(tagId, int32U);
        }

        public virtual void SetInt32UArray(int tagId, uint[] array)
        {
            CurrentDirectory.Set(tagId, array);
        }

        public abstract void Completed(IndexedReader reader, int tiffHeaderOffset);

        public abstract bool CustomProcessTag(int tagOffset, ICollection<int> processedIfdOffsets, int tiffHeaderOffset, IndexedReader reader, int tagId, int byteCount);

        public abstract bool HasFollowerIfd();

        public abstract bool IsTagIfdPointer(int tagType);

        public abstract void SetTiffMarker(int marker);
    }
}
