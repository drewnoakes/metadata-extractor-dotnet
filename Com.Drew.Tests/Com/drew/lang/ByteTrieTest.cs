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

using NUnit.Framework;
using Sharpen;

namespace Com.Drew.Lang
{
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public class ByteTrieTest
    {
        [Test]
        public virtual void TestBasics()
        {
            ByteTrie<string> trie = new ByteTrie<string>();
            string[] strings = new string[] { "HELLO", "HELLO WORLD", "HERBERT" };
            foreach (string s in strings)
            {
                trie.AddPath(s, Runtime.GetBytesForString(s));
            }
            foreach (string s_1 in strings)
            {
                Assert.AreSame(s_1, trie.Find(Runtime.GetBytesForString(s_1)));
            }
            Assert.IsNull(trie.Find(Runtime.GetBytesForString("Not Included")));
            Assert.IsNull(trie.Find(Runtime.GetBytesForString("HELL")));
            Assert.AreEqual("HELLO", trie.Find(Runtime.GetBytesForString("HELLO MUM")));
            Assert.AreEqual("HELLO WORLD".Length, trie.GetMaxDepth());
            trie.SetDefaultValue("DEFAULT");
            Assert.AreEqual("DEFAULT", trie.Find(Runtime.GetBytesForString("Also Not Included")));
        }
    }
}
