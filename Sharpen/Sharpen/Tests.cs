using System;
using NUnit.Framework;

namespace Sharpen
{
    public static class Tests
    {
        public static void IsFalse(bool condition)
        {
            Assert.IsFalse(condition);
        }

        public static void IsFalse(string message, bool condition)
        {
            Assert.IsFalse(condition, message);
        }

        public static void AreEqual(int expected, int actual)
        {
            Assert.AreEqual(expected, actual);
        }
        
        public static void AreEqual(object expected, object actual)
        {
            Assert.AreEqual(expected, actual);
        }

        public static void AreEqual(DateTime expected, DateTime actual)
        {
            Assert.AreEqual(expected, actual);
        }

        public static void AreEqual(double expected, double actual, double delta)
        {
            Assert.AreEqual(expected, actual, delta);
        }

        public static void AreEqual(string message, object expected, object actual)
        {
            Assert.AreEqual(expected, actual, message);
        }

        public static void IsTrue(bool condition)
        {
            Assert.IsTrue(condition);
        }

        public static void IsTrue(string message, bool condition)
        {
            Assert.IsTrue(condition, message);
        }
    }
}
