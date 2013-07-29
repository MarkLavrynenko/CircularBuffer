using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
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
			rand = new Random();
        }

	    protected byte[] GenerateRandomBytes(int length)
	    {
			var bytes = new byte[length];
		    rand.NextBytes(bytes);
		    return bytes;
	    }

        [TestMethod]
        public void FillBuffer()
        {
            var buf = new CircularBuffer<int>(15);
            Assert.IsTrue(buf.Capacity == 15);
        }

        [TestMethod]
        public void IterateBuffer()
        {
	        var buff = new CircularBuffer<byte>(50);
			buff.Put(new byte[] { 10, 20, 30 });
	        buff.Get();
			buff.Put(new byte[] { 90, 100 });
	        var result = new byte[4] {20, 30, 90, 100};
	        var i = 0;
	        foreach (var b in buff)
	        {
		        Assert.AreEqual(b, result[i++]);
	        }
        }

        [TestMethod]
        public void CleanBuffer()
        {
			const int count = 50;
			var data = GenerateRandomBytes(count);
			var buff = new CircularBuffer<byte>(100);
			buff.Put(data);
			((ICollection<byte>)buff).Clear();
	        Assert.AreEqual(0, buff.Size);
        }

	    [TestMethod]
	    public void SkipTest()
	    {
		    const int count = 50;
			var data = GenerateRandomBytes(count);
			var buff = new CircularBuffer<byte>(100);
		    buff.Put(data);
			var lastData = new byte[2] { 20, 30 };
			buff.Put(lastData);
			buff.Skip(count);
		    CollectionAssert.AreEqual(buff.Get(2), lastData);
	    }

	    [TestMethod]
	    public void BufferSize()
	    {
		    var data = GenerateRandomBytes(50);
		    var buff = new CircularBuffer<byte>(100);
			var dst = new byte[15];

			buff.Put(data);
		    Assert.AreEqual(50, buff.Size);

		    buff.Get(dst);
		    Assert.AreEqual(35, buff.Size);

			Assert.AreEqual(35, (buff as ICollection).Count);
	    }

		[TestMethod]
	    public void ContainsTest()
	    {
		    var data = new byte[5] { 10, 20, 0, 15, 20 };
			var buff = new CircularBuffer<byte>(10);
			buff.Put(data);
		    Assert.IsTrue(((ICollection<byte>) buff).Contains(20));
			Assert.IsTrue(((ICollection<byte>)buff).Contains(15));
			Assert.IsTrue(((ICollection<byte>)buff).Contains(10));

			Assert.IsFalse(((ICollection<byte>)buff).Contains(18));
			Assert.IsFalse(((ICollection<byte>)buff).Contains(31));
	    }

	    [TestMethod]
	    public void CopyToTest()
	    {
		    const int count = 5;
		    var data = GenerateRandomBytes(count);
		    var buff = new CircularBuffer<byte>(count);
			buff.Put(data);

		    var res = new byte[count];
			buff.CopyTo(res);
			CollectionAssert.AreEqual(data, res);

		    var res1 = new byte[count];
		    buff.CopyTo(res1, 0);
			CollectionAssert.AreEqual(data, res1);
		}

		[TestMethod]
	    public void AddTest()
	    {
			ICollection<byte> buff = new CircularBuffer<byte>(10);
			buff.Add(10);
			buff.Add(40);

			var res = new byte[2];
			(buff as CircularBuffer<Byte>).CopyTo(res, 2, 0);
			CollectionAssert.AreEqual(new Byte[] { 10, 40 }, res);
	    }

        [TestMethod, ExpectedException(typeof(InvalidOperationException))]
        public void OverFlowBuffer()
        {
			const int testedSize = 50;
	        var data = GenerateRandomBytes(testedSize);
	        var buffer = new CircularBuffer<byte>(testedSize - 1);
			buffer.Put(data);
        }

        [TestMethod]
        public void LoadTest()
        {
	        const int numberofCycles = 10000;
	        const int numberOfElements = 5000;
	        var sw = Stopwatch.StartNew();

	        var data = GenerateRandomBytes(numberOfElements);
	        var buffer = new CircularBuffer<byte>(numberOfElements);

	        for (var i = 0; i < numberofCycles; ++i)
	        {
				buffer.Put(data);
		        ((ICollection<byte>)buffer).Clear();
	        }
	        Assert.IsTrue(sw.ElapsedMilliseconds < 1000);
        }
    }
}
