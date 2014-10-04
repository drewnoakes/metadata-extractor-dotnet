/*
 * Copyright 2002-2013 Drew Noakes
 *
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
 *    http://drewnoakes.com/code/exif/
 *    http://code.google.com/p/metadata-extractor/
 */
using System;
using System.Collections.Generic;
using System.IO;
using Com.Drew.Imaging;
using Com.Drew.Lang;
using Com.Drew.Metadata;
using Com.Drew.Metadata.Exif;
using Com.Drew.Tools;
using JetBrains.Annotations;
using Sharpen;

namespace Com.Drew.Tools
{
	/// <author>Drew Noakes http://drewnoakes.com</author>
	public class ProcessAllImagesInFolderUtility
	{
		/// <exception cref="System.IO.IOException"/>
		/// <exception cref="Com.Drew.Imaging.Jpeg.JpegProcessingException"/>
		public static void Main(string[] args)
		{
			if (args.Length == 0)
			{
				System.Console.Error.Println("Expects one or more directories as arguments.");
				System.Environment.Exit(1);
			}
			IList<string> directories = new AList<string>();
			ProcessAllImagesInFolderUtility.FileHandler handler = null;
			foreach (string arg in args)
			{
				if (Sharpen.Runtime.EqualsIgnoreCase(arg, "-text"))
				{
					// If "-test" is specified, write the discovered metadata into a sub-folder relative to the image
					handler = new ProcessAllImagesInFolderUtility.TextFileOutputHandler();
				}
				else
				{
					if (Sharpen.Runtime.EqualsIgnoreCase(arg, "-wiki"))
					{
						handler = new ProcessAllImagesInFolderUtility.WikiTableOutputHandler();
					}
					else
					{
						directories.Add(arg);
					}
				}
			}
			if (handler == null)
			{
				handler = new ProcessAllImagesInFolderUtility.BasicFileHandler();
			}
			long start = Runtime.NanoTime();
			// Order alphabetically so that output is stable across invocations
			directories.Sort();
			foreach (string directory in directories)
			{
				ProcessDirectory(new FilePath(directory), handler);
			}
			handler.OnCompleted();
			System.Console.Out.Println(Sharpen.Extensions.StringFormat("Completed in %d ms", (Runtime.NanoTime() - start) / 1000000));
		}

		private static void ProcessDirectory(FilePath path, ProcessAllImagesInFolderUtility.FileHandler handler)
		{
			string[] pathItems = path.List();
			if (pathItems == null)
			{
				return;
			}
			// Order alphabetically so that output is stable across invocations
			Arrays.Sort(pathItems);
			foreach (string pathItem in pathItems)
			{
				FilePath file = new FilePath(path, pathItem);
				if (file.IsDirectory())
				{
					ProcessDirectory(file, handler);
				}
				else
				{
					if (handler.ShouldProcess(file))
					{
						handler.OnProcessingStarting(file);
						// Read metadata
						Com.Drew.Metadata.Metadata metadata;
						try
						{
							metadata = ImageMetadataReader.ReadMetadata(file);
						}
						catch (Exception t)
						{
							handler.OnException(file, t);
							continue;
						}
						handler.OnExtracted(file, metadata);
					}
				}
			}
		}

		internal interface FileHandler
		{
			bool ShouldProcess(FilePath file);

			void OnException(FilePath file, Exception throwable);

			void OnExtracted(FilePath file, Com.Drew.Metadata.Metadata metadata);

			void OnCompleted();

			void OnProcessingStarting(FilePath file);
		}

		internal abstract class FileHandlerBase : ProcessAllImagesInFolderUtility.FileHandler
		{
			private readonly ICollection<string> _supportedExtensions = new HashSet<string>(Arrays.AsList("jpg", "jpeg", "nef", "crw", "cr2", "orf", "tif", "tiff", "png", "gif", "bmp"));

			private int _processedFileCount = 0;

			private int _exceptionCount = 0;

			private int _errorCount = 0;

			private long _processedByteCount = 0;

			public virtual bool ShouldProcess(FilePath file)
			{
				string extension = GetExtension(file);
				return extension != null && _supportedExtensions.Contains(extension.ToLower());
			}

			public virtual void OnProcessingStarting(FilePath file)
			{
				_processedFileCount++;
				_processedByteCount += file.Length();
			}

			public virtual void OnException(FilePath file, Exception throwable)
			{
				_exceptionCount++;
				if (throwable is ImageProcessingException)
				{
					// this is an error in the Jpeg segment structure.  we're looking for bad handling of
					// metadata segments.  in this case, we didn't even get a segment.
					System.Console.Error.Printf("%s: %s [Error Extracting Metadata]\n\t%s%n", throwable.GetType().FullName, file, throwable.Message);
				}
				else
				{
					// general, uncaught exception during processing of jpeg segments
					System.Console.Error.Printf("%s: %s [Error Extracting Metadata]%n", throwable.GetType().FullName, file);
					Sharpen.Runtime.PrintStackTrace(throwable, System.Console.Error);
				}
			}

			public virtual void OnExtracted(FilePath file, Com.Drew.Metadata.Metadata metadata)
			{
				if (metadata.HasErrors())
				{
					System.Console.Error.Println(file);
					foreach (Com.Drew.Metadata.Directory directory in metadata.GetDirectories())
					{
						if (!directory.HasErrors())
						{
							continue;
						}
						foreach (string error in directory.GetErrors())
						{
							System.Console.Error.Printf("\t[%s] %s%n", directory.GetName(), error);
							_errorCount++;
						}
					}
				}
			}

			public virtual void OnCompleted()
			{
				if (_processedFileCount > 0)
				{
					System.Console.Out.Println(Sharpen.Extensions.StringFormat("Processed %,d files (%,d bytes) with %,d exceptions and %,d file errors", _processedFileCount, _processedByteCount, _exceptionCount, _errorCount));
				}
			}

			[CanBeNull]
			protected internal virtual string GetExtension(FilePath file)
			{
				string fileName = file.GetName();
				int i = fileName.LastIndexOf('.');
				if (i == -1)
				{
					return null;
				}
				if (i == fileName.Length - 1)
				{
					return null;
				}
				return Sharpen.Runtime.Substring(fileName, i + 1);
			}
		}

		/// <summary>Writes a text file containing the extracted metadata for each input file.</summary>
		internal class TextFileOutputHandler : ProcessAllImagesInFolderUtility.FileHandlerBase
		{
			public override void OnExtracted(FilePath file, Com.Drew.Metadata.Metadata metadata)
			{
				base.OnExtracted(file, metadata);
				try
				{
					WriteOutputFile(file, metadata);
				}
				catch (IOException e)
				{
					Sharpen.Runtime.PrintStackTrace(e);
				}
			}

			/// <exception cref="System.IO.IOException"/>
			private void WriteOutputFile(FilePath file, Com.Drew.Metadata.Metadata metadata)
			{
				FileWriter writer = null;
				try
				{
					string outputPath = Sharpen.Extensions.StringFormat("%s/metadata/%s.txt", file.GetParent(), file.GetName()).ToLower();
					writer = new FileWriter(outputPath, false);
					writer.Write("FILE: " + file.GetName() + "\n");
					writer.Write("\n");
					if (metadata.HasErrors())
					{
						foreach (Com.Drew.Metadata.Directory directory in metadata.GetDirectories())
						{
							if (!directory.HasErrors())
							{
								continue;
							}
							foreach (string error in directory.GetErrors())
							{
								writer.Write(Sharpen.Extensions.StringFormat("[ERROR: %s] %s\n", directory.GetName(), error));
							}
						}
						writer.Write("\n");
					}
					// Iterate through all values
					foreach (Com.Drew.Metadata.Directory directory_1 in metadata.GetDirectories())
					{
						string directoryName = directory_1.GetName();
						foreach (Tag tag in directory_1.GetTags())
						{
							string tagName = tag.GetTagName();
							string description = tag.GetDescription();
							writer.Write(Sharpen.Extensions.StringFormat("[%s - %s] %s = %s%n", directoryName, tag.GetTagTypeHex(), tagName, description));
						}
						if (directory_1.GetTagCount() != 0)
						{
							writer.Write("\n");
						}
					}
				}
				finally
				{
					if (writer != null)
					{
						writer.Write("Generated using metadata-extractor\n");
						writer.Write("http://drewnoakes.com/code/exif/\n");
						writer.Flush();
						writer.Close();
					}
				}
			}
		}

		/// <summary>Creates a table describing sample images using Wiki markdown.</summary>
		internal class WikiTableOutputHandler : ProcessAllImagesInFolderUtility.FileHandlerBase
		{
			private readonly IDictionary<string, string> _extensionEquivalence = new Dictionary<string, string>();

			private readonly IDictionary<string, IList<ProcessAllImagesInFolderUtility.WikiTableOutputHandler.Row>> _rowListByExtension = new Dictionary<string, IList<ProcessAllImagesInFolderUtility.WikiTableOutputHandler.Row>>();

			internal class Row
			{
				internal FilePath file;

				internal Com.Drew.Metadata.Metadata metadata;

				[CanBeNull] internal string manufacturer;

				[CanBeNull] internal string model;

				[CanBeNull] internal string exifVersion;

				[CanBeNull] internal string thumbnail;

				[CanBeNull] internal string makernote;

				internal Row(WikiTableOutputHandler _enclosing, FilePath file, Com.Drew.Metadata.Metadata metadata)
				{
					this._enclosing = _enclosing;
					this.file = file;
					this.metadata = metadata;
					ExifIFD0Directory ifd0Dir = metadata.GetDirectory<ExifIFD0Directory>();
					ExifSubIFDDirectory subIfdDir = metadata.GetDirectory<ExifSubIFDDirectory>();
					ExifThumbnailDirectory thumbDir = metadata.GetDirectory<ExifThumbnailDirectory>();
					if (ifd0Dir != null)
					{
						this.manufacturer = ifd0Dir.GetDescription(ExifIFD0Directory.TagMake);
						this.model = ifd0Dir.GetDescription(ExifIFD0Directory.TagModel);
					}
					bool hasMakernoteData = false;
					if (subIfdDir != null)
					{
						this.exifVersion = subIfdDir.GetDescription(ExifSubIFDDirectory.TagExifVersion);
						hasMakernoteData = subIfdDir.ContainsTag(ExifSubIFDDirectory.TagMakernote);
					}
					if (thumbDir != null)
					{
						int? width = thumbDir.GetInteger(ExifThumbnailDirectory.TagThumbnailImageWidth);
						int? height = thumbDir.GetInteger(ExifThumbnailDirectory.TagThumbnailImageHeight);
						this.thumbnail = width != null && height != null ? Sharpen.Extensions.StringFormat("Yes (%s x %s)", width, height) : "Yes";
					}
					foreach (Com.Drew.Metadata.Directory directory in metadata.GetDirectories())
					{
						if (directory.GetType().FullName.Contains("Makernote"))
						{
							this.makernote = Sharpen.Extensions.Trim(directory.GetName().Replace("Makernote", string.Empty));
						}
					}
					if (this.makernote == null)
					{
						this.makernote = hasMakernoteData ? "(Unknown)" : "N/A";
					}
				}

				private readonly WikiTableOutputHandler _enclosing;
			}

			public WikiTableOutputHandler()
			{
				_extensionEquivalence.Put("jpeg", "jpg");
			}

			public override void OnExtracted(FilePath file, Com.Drew.Metadata.Metadata metadata)
			{
				base.OnExtracted(file, metadata);
				string extension = GetExtension(file);
				if (extension == null)
				{
					return;
				}
				// Sanitise the extension
				extension = extension.ToLower();
				if (_extensionEquivalence.ContainsKey(extension))
				{
					extension = _extensionEquivalence.Get(extension);
				}
				IList<ProcessAllImagesInFolderUtility.WikiTableOutputHandler.Row> list = _rowListByExtension.Get(extension);
				if (list == null)
				{
					list = new AList<ProcessAllImagesInFolderUtility.WikiTableOutputHandler.Row>();
					_rowListByExtension.Put(extension, list);
				}
				list.Add(new ProcessAllImagesInFolderUtility.WikiTableOutputHandler.Row(this, file, metadata));
			}

			public override void OnCompleted()
			{
				base.OnCompleted();
				OutputStream outputStream = null;
				PrintStream stream = null;
				try
				{
					outputStream = new FileOutputStream("../wiki/ImageDatabaseSummary.wiki", false);
					stream = new PrintStream(outputStream, false);
					WriteOutput(stream);
					stream.Flush();
				}
				catch (IOException e)
				{
					Sharpen.Runtime.PrintStackTrace(e);
				}
				finally
				{
					if (stream != null)
					{
						stream.Close();
					}
					if (outputStream != null)
					{
						try
						{
							outputStream.Close();
						}
						catch (IOException e)
						{
							Sharpen.Runtime.PrintStackTrace(e);
						}
					}
				}
			}

			/// <exception cref="System.IO.IOException"/>
			private void WriteOutput(PrintStream stream)
			{
				TextWriter writer = new OutputStreamWriter(stream);
				writer.Write("#summary Tabular summary of metadata found in the image database\n\n");
				writer.Write("= Image Database Summary =\n\n");
				foreach (string extension in _rowListByExtension.Keys)
				{
					writer.Write("== " + extension.ToUpper() + " Files ==\n\n");
					writer.Write("|| *File* || *Manufacturer* || *Model* || *Dir Count* || *Exif?* || *Makernote*  || *Thumbnail* || *All Data* ||\n");
					IList<ProcessAllImagesInFolderUtility.WikiTableOutputHandler.Row> rows = _rowListByExtension.Get(extension);
					// Order by manufacturer, then model
					rows.Sort(new _IComparer_386());
					foreach (ProcessAllImagesInFolderUtility.WikiTableOutputHandler.Row row in rows)
					{
						writer.Write(Sharpen.Extensions.StringFormat("|| [http://sample-images.metadata-extractor.googlecode.com/git/%s %s] || %s || %s || %d || %s ||  %s || %s || [http://sample-images.metadata-extractor.googlecode.com/git/metadata/%s.txt metadata] ||%n"
							, StringUtil.UrlEncode(row.file.GetName()), row.file.GetName(), row.manufacturer == null ? string.Empty : StringUtil.EscapeForWiki(row.manufacturer), row.model == null ? string.Empty : StringUtil.EscapeForWiki(row.model), row.metadata.GetDirectoryCount
							(), row.exifVersion == null ? string.Empty : row.exifVersion, row.makernote == null ? string.Empty : row.makernote, row.thumbnail == null ? string.Empty : row.thumbnail, StringUtil.UrlEncode(row.file.GetName()).ToLower()));
					}
					writer.Write('\n');
				}
				writer.Flush();
			}

			private sealed class _IComparer_386 : IComparer<ProcessAllImagesInFolderUtility.WikiTableOutputHandler.Row>
			{
				public _IComparer_386()
				{
				}

				public int Compare(ProcessAllImagesInFolderUtility.WikiTableOutputHandler.Row o1, ProcessAllImagesInFolderUtility.WikiTableOutputHandler.Row o2)
				{
					int c1 = StringUtil.Compare(o1.manufacturer, o2.manufacturer);
					return c1 != 0 ? c1 : StringUtil.Compare(o1.model, o2.model);
				}
			}
		}

		/// <summary>Does nothing with the output except enumerate it in memory and format descriptions.</summary>
		/// <remarks>
		/// Does nothing with the output except enumerate it in memory and format descriptions. This is useful in order to
		/// flush out any potential exceptions raised during the formatting of extracted value descriptions.
		/// </remarks>
		internal class BasicFileHandler : ProcessAllImagesInFolderUtility.FileHandlerBase
		{
			public override void OnExtracted(FilePath file, Com.Drew.Metadata.Metadata metadata)
			{
				base.OnExtracted(file, metadata);
				// Iterate through all values, calling toString to flush out any formatting exceptions
				foreach (Com.Drew.Metadata.Directory directory in metadata.GetDirectories())
				{
					directory.GetName();
					foreach (Tag tag in directory.GetTags())
					{
						tag.GetTagName();
						tag.GetDescription();
					}
				}
			}
		}
	}
}
