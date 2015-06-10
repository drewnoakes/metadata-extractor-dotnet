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

using System.Text;
using NUnit.Framework;

namespace Com.Drew.Lang
{
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public sealed class ByteTrieTest
    {
        [Test]
        public void TestBasics()
        {
            var trie = new ByteTrie<string>();
            var strings = new[] { "HELLO", "HELLO WORLD", "HERBERT" };
            foreach (var s in strings)
            {
                trie.AddPath(s, Encoding.UTF8.GetBytes(s));
            }
            foreach (var s1 in strings)
            {
                Assert.AreSame(s1, trie.Find(Encoding.UTF8.GetBytes(s1)));
            }
            Assert.IsNull(trie.Find(Encoding.UTF8.GetBytes("Not Included")));
            Assert.IsNull(trie.Find(Encoding.UTF8.GetBytes("HELL")));
            Assert.AreEqual("HELLO", trie.Find(Encoding.UTF8.GetBytes("HELLO MUM")));
            Assert.AreEqual("HELLO WORLD".Length, trie.MaxDepth);
            trie.SetDefaultValue("DEFAULT");
            Assert.AreEqual("DEFAULT", trie.Find(Encoding.UTF8.GetBytes("Also Not Included")));
        }
    }
}
