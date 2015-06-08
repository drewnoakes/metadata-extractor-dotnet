using System;
using System.Collections.Generic;
using System.Threading;

namespace Sharpen
{
    public class Thread : Runnable
    {
        private static ThreadGroup defaultGroup = new ThreadGroup ();
        private bool interrupted;
        private Runnable runnable;
        private ThreadGroup tgroup;
        private System.Threading.Thread thread;

        [ThreadStatic]
        private static Thread wrapperThread;

        public Thread () : this(null, null, null)
        {
        }

        public Thread (string name) : this (null, null, name)
        {
        }

        public Thread (ThreadGroup grp, string name) : this (null, grp, name)
        {
        }

        public Thread (Runnable runnable): this (runnable, null, null)
        {
        }

        Thread (Runnable runnable, ThreadGroup grp, string name)
        {
            thread = new System.Threading.Thread (new ThreadStart (InternalRun));

            this.runnable = runnable ?? this;
            tgroup = grp ?? defaultGroup;
            tgroup.Add (this);
            if (name != null)
                thread.Name = name;
        }

        private Thread (System.Threading.Thread t)
        {
            thread = t;
            tgroup = defaultGroup;
            tgroup.Add (this);
        }

        public static Thread CurrentThread ()
        {
            if (wrapperThread == null) {
                wrapperThread = new Thread (System.Threading.Thread.CurrentThread);
            }
            return wrapperThread;
        }

        public string GetName ()
        {
            return thread.Name;
        }

        public ThreadGroup GetThreadGroup ()
        {
            return tgroup;
        }

        private void InternalRun ()
        {
            wrapperThread = this;
            try {
                runnable.Run ();
            } catch (Exception exception) {
                Console.WriteLine (exception);
            } finally {
                tgroup.Remove (this);
            }
        }

        public static void Yield ()
        {
        }

        public void Interrupt ()
        {
            lock (thread) {
                interrupted = true;
                thread.Interrupt ();
            }
        }

        public static bool Interrupted ()
        {
            if (Thread.wrapperThread == null) {
                return false;
            }
            Thread wrapperThread = Thread.wrapperThread;
            lock (wrapperThread) {
                bool interrupted = Thread.wrapperThread.interrupted;
                Thread.wrapperThread.interrupted = false;
                return interrupted;
            }
        }

        public bool IsAlive ()
        {
            return thread.IsAlive;
        }

        public void Join ()
        {
            thread.Join ();
        }

        public void Join (long timeout)
        {
            thread.Join ((int)timeout);
        }

        public virtual void Run ()
        {
        }

        public void SetDaemon (bool daemon)
        {
            thread.IsBackground = daemon;
        }

        public void SetName (string name)
        {
            thread.Name = name;
        }

        public static void Sleep (long milis)
        {
            System.Threading.Thread.Sleep ((int)milis);
        }

        public void Start ()
        {
            thread.Start ();
        }

        public void Abort ()
        {
            thread.Abort ();
        }

    }

    public class ThreadGroup
    {
        private List<Thread> threads = new List<Thread> ();

        public ThreadGroup()
        {
        }

        public ThreadGroup (string name)
        {
        }

        public void Add (Thread t)
        {
            lock (threads) {
                threads.Add (t);
            }
        }

        public void Remove (Thread t)
        {
            lock (threads) {
                threads.Remove (t);
            }
        }

        public int Enumerate (Thread[] array)
        {
            lock (threads) {
                int count = Math.Min (array.Length, threads.Count);
                threads.CopyTo (0, array, 0, count);
                return count;
            }
        }
    }
}
