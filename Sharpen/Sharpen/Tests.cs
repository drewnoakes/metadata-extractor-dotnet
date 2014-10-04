using System;

namespace Sharpen
{
    public static class Tests
    {
        public static void IsFalse(bool condition)
        {
            NUnit.Framework.Assert.IsFalse(condition);
        }

        public static void IsFalse(string message, bool condition)
        {
            NUnit.Framework.Assert.IsFalse(condition, message);
        }

        public static void AreEqual(int expected, int actual)
        {
            NUnit.Framework.Assert.AreEqual(expected, actual);
        }
        
        public static void AreEqual(object expected, object actual)
        {
            NUnit.Framework.Assert.AreEqual(expected, actual);
        }

        public static void AreEqual(DateTime expected, DateTime actual)
        {
            NUnit.Framework.Assert.AreEqual(expected, actual);
        }

        public static void AreEqual(double expected, double actual, double delta)
        {
            NUnit.Framework.Assert.AreEqual(expected, actual, delta);
        }

        public static void AreEqual(string message, object expected, object actual)
        {
            NUnit.Framework.Assert.AreEqual(expected, actual, message);
        }

        public static void IsTrue(bool condition)
        {
            NUnit.Framework.Assert.IsTrue(condition);
        }

        public static void IsTrue(string message, bool condition)
        {
            NUnit.Framework.Assert.IsTrue(condition, message);
        }
    }
}
