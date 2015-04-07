using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Reflection;
using System.Text;
using System.Threading;
using ProcessStartInfo = System.Diagnostics.ProcessStartInfo;

namespace Sharpen
{
	public class Runtime
	{
		private static Runtime instance;
		private List<ShutdownHook> shutdownHooks = new List<ShutdownHook> ();

		public void AddShutdownHook (Runnable r)
		{
			ShutdownHook item = new ShutdownHook ();
			item.Runnable = r;
			this.shutdownHooks.Add (item);
		}

		public int AvailableProcessors ()
		{
			return Environment.ProcessorCount;
		}

		public static long CurrentTimeMillis ()
		{
			return DateTime.UtcNow.ToMillisecondsSinceEpoch ();
		}

		public SystemProcess Exec (string[] cmd, string[] envp, FilePath dir)
		{
			try {
				ProcessStartInfo psi = new ProcessStartInfo ();
				psi.FileName = cmd[0];
				psi.Arguments = string.Join (" ", cmd, 1, cmd.Length - 1);
				if (dir != null) {
					psi.WorkingDirectory = dir.GetPath ();
				}
				psi.UseShellExecute = false;
				psi.RedirectStandardInput = true;
				psi.RedirectStandardError = true;
				psi.RedirectStandardOutput = true;
				psi.CreateNoWindow = true;
				if (envp != null) {
					foreach (string str in envp) {
						int index = str.IndexOf ('=');
						psi.EnvironmentVariables[str.Substring (0, index)] = str.Substring (index + 1);
					}
				}
				return SystemProcess.Start (psi);
			} catch (System.ComponentModel.Win32Exception ex) {
				throw new IOException (ex.Message);
			}
		}

		public static string Getenv (string var)
		{
			return Environment.GetEnvironmentVariable (var);
		}

		public static IDictionary<string, string> GetEnv ()
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string> ();
			foreach (DictionaryEntry v in Environment.GetEnvironmentVariables ()) {
				dictionary[(string)v.Key] = (string)v.Value;
			}
			return dictionary;
		}

		public static IPAddress GetLocalHost ()
		{
			try {
				return Dns.GetHostEntry (Dns.GetHostName ()).AddressList[0];
			} catch (System.Net.Sockets.SocketException ex) {
				throw new UnknownHostException (ex);
			}
		}
		
		static Hashtable properties;
		
		public static Hashtable GetProperties ()
		{
			if (properties == null) {
				properties = new Hashtable ();
				properties ["jgit.fs.debug"] = "false";
				var home = Environment.GetFolderPath (Environment.SpecialFolder.UserProfile).Trim ();
				if (string.IsNullOrEmpty (home))
					home = Environment.GetFolderPath (Environment.SpecialFolder.Personal).Trim ();
				properties ["user.home"] = home;
				properties ["java.library.path"] = Environment.GetEnvironmentVariable ("PATH");
				if (Path.DirectorySeparatorChar != '\\')
					properties ["os.name"] = "Unix";
				else
					properties ["os.name"] = "Windows";
			    properties["file.encoding"] = Encoding.UTF8.BodyName;
			}
			return properties;
		}

		public static string GetProperty (string key)
		{
			return ((string) GetProperties()[key]);
		}
		
		public static void SetProperty (string key, string value)
		{
			GetProperties () [key] = value;
		}

		public static Runtime GetRuntime ()
		{
			if (instance == null) {
				instance = new Runtime ();
			}
			return instance;
		}

		public static int IdentityHashCode (object ob)
		{
			return RuntimeHelpers.GetHashCode (ob);
		}

		public long MaxMemory ()
		{
			return int.MaxValue;
		}

		private class ShutdownHook
		{
			public Sharpen.Runnable Runnable;

			~ShutdownHook ()
			{
				this.Runnable.Run ();
			}
		}
		
		public static void DeleteCharAt (StringBuilder sb, int index)
		{
			sb.Remove (index, 1);
		}
		
		public static sbyte[] GetBytesForString (string str)
		{
			return Extensions.ConvertToByteArray(Encoding.UTF8.GetBytes (str));
		}

	    public static sbyte[] GetBytesForString (string str, string encoding)
		{
            return Extensions.ConvertToByteArray(Encoding.GetEncoding(encoding).GetBytes(str));
		}

		public static FieldInfo[] GetDeclaredFields (Type t)
		{
			return t.GetFields (BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
		}

		public static void NotifyAll (object ob)
		{
			Monitor.PulseAll (ob);
		}

		public static void PrintStackTrace (Exception ex)
		{
			Console.WriteLine (ex);
		}

		public static void PrintStackTrace (Exception ex, TextWriter tw)
		{
			tw.WriteLine (ex);
		}

		public static string Substring (string str, int index)
		{
			return str.Substring (index);
		}

        public static string Substring(string str, int index, int endIndex)
		{
			return str.Substring (index, endIndex - index);
		}

		public static void Wait (object ob)
		{
			Monitor.Wait (ob);
		}

		public static bool Wait (object ob, long milis)
		{
			return Monitor.Wait (ob, (int)milis);
		}
		
		public static Type GetType (string name)
		{
			foreach (Assembly a in AppDomain.CurrentDomain.GetAssemblies ()) {
				Type t = a.GetType (name);
				if (t != null)
					return t;
			}
			throw new InvalidOperationException ("Type not found: " + name);
		}
		
		public static void SetCharAt (StringBuilder sb, int index, char c)
		{
			sb [index] = c;
		}
		
		public static bool EqualsIgnoreCase (string s1, string s2)
		{
			return s1.Equals (s2, StringComparison.CurrentCultureIgnoreCase);
		}
		
		public static long NanoTime ()
		{
			return Environment.TickCount * 1000 * 1000;
		}
		
		public static int CompareOrdinal (string s1, string s2)
		{
			return string.CompareOrdinal (s1, s2);
		}

        public static string GetStringForBytes(sbyte[] sbytes, int start, int len)
        {
            return GetStringForBytes(Extensions.ConvertToByteArray(sbytes), start, len);
        }

        public static string GetStringForBytes(sbyte[] sbytes, string encoding)
        {
            return GetStringForBytes(Extensions.ConvertToByteArray(sbytes), encoding);
        }

        public static string GetStringForBytes(sbyte[] sbytes, int start, int len, string encoding)
        {
            return GetStringForBytes(Extensions.ConvertToByteArray(sbytes), start, len, encoding);
        }

		public static string GetStringForBytes (sbyte[] chars)
		{
            return Encoding.UTF8.GetString(Extensions.ConvertToByteArray(chars));
		}

	    public static string GetStringForBytes (byte[] chars, string encoding)
		{
			return GetEncoding (encoding).GetString (chars);
		}

		public static string GetStringForBytes (byte[] chars, int start, int len)
		{
			return Encoding.UTF8.GetString (chars, start, len);
		}

		public static string GetStringForBytes (byte[] chars, int start, int len, string encoding)
		{
			return GetEncoding (encoding).Decode (chars, start, len);
		}
		
		public static Encoding GetEncoding (string name)
		{
		    try
		    {
//			Encoding e = Encoding.GetEncoding (name, EncoderFallback.ExceptionFallback, DecoderFallback.ExceptionFallback);
		        Encoding e = Encoding.GetEncoding(name.Replace('_', '-'));
		        if (e is UTF8Encoding)
		            return new UTF8Encoding(false, true);
		        return e;
		    }
		    catch (ArgumentException ex)
		    {
		        if (ex.ParamName.Equals("name"))
		        {
                    throw new UnsupportedEncodingException();
		        }

		        throw;
		    }
		}

        public static int GetArrayLength(object array)
	    {
            if (array.GetType().IsArray)
            {
                if (array is ICollection)
                {
                    return ((ICollection)array).Count;
                }
            }

            throw new NotSupportedException();
	    }

        private static T GetArrayValue<T>(object array, int index)
        {
            if (array.GetType().IsArray)
            {
                if (array is ICollection)
                {
                    return ((ICollection)array).Cast<T>().ElementAt(index);
                }
                
                if (array is IEnumerable)
                {
                    return ((IEnumerable)array).Cast<T>().ElementAt(index);
                }
            }

            throw new NotSupportedException();
        }
        
        public static object GetArrayValue(object array, int index)
        {
            return GetArrayValue<object>(array, index);
        }

        public static int GetInt(object array, int index)
	    {
            return GetArrayValue<int>(array, index);
	    }

        public static short GetShort(object array, int index)
	    {
            return GetArrayValue<short>(array, index);
	    }

        public static long GetLong(object array, int index)
	    {
            return GetArrayValue<long>(array, index);
	    }

        public static float GetFloat(object array, int index)
	    {
            return GetArrayValue<float>(array, index);
	    }

        public static double GetDouble(object array, int index)
	    {
            return GetArrayValue<double>(array, index);
	    }

        public static byte GetByte(object array, int index)
	    {
            return GetArrayValue<byte>(array, index);
	    }
	}
}
