using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CircularBuffer;

namespace CircularBuffer.Tests
{
    [TestClass]
    public class CircularBufferTests
    {
        private static Random rand;

        public TestContext TestContext
        {
            get;
            set;
        }

        [ClassInitialize]
        public static void Initialize(TestContext context)
        {
        }

        [TestMethod]
        public void FillBuffer()
        {
            CircularBuffer<int> buf = new CircularBuffer<int>(15);
            Assert.IsTrue(buf.Capacity == 15);
        }

        [TestMethod]
        public void IterateBuffer()
        {
            Assert.IsTrue(false, "Not implemented");
        }

        [TestMethod]
        public void CleanBuffer()
        {
            Assert.IsTrue(false, "Not implemented");
        }

        [TestMethod]
        public void OverFlowBuffer()
        {
            Assert.IsTrue(false, "Not implemented");
        }

        [TestMethod]
        public void LoadTest()
        {
            Assert.IsTrue(false, "Not implemented");
        }
    }
}
