using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading;

namespace CircularBuffer
{
    public class CircularBuffer<T> : IEnumerable<T>, ICollection<T>, IEnumerable, ICollection
    {
        private readonly int _capacity;
        private int _size;
        private int _start, _end;
        private T[] _buffer;

        [NonSerialized]
        private object _syncRoot;

        public CircularBuffer(int capacity)
        {
			if (capacity <= 0)
				throw new ArgumentException("Capacity can't be less than zero");
            _capacity = capacity;
            _start = _end = 0;
            _buffer = new T[capacity];
        }

	    public CircularBuffer(ICollection<T> other)
	    {
		    _size = _capacity = other.Count;
		    _start = 0;
		    _end = _capacity - 1;
			_buffer = new T[_capacity];
			other.CopyTo(_buffer, 0);
	    }

        #region Public properties
        // TODO make read-write
        public int Capacity
        {
            get { return _capacity; }
        }

        public int Size
        {
            get { return _size; }
        }

	    public T this[int index]
	    {
		    get
		    {
			    return At(index);
		    }
	    }

        #endregion

	    public void Put(T[] items)
	    {
		    foreach (var t in items)
		    {
			    Put(t);
		    }
	    }

	    public void Put(T item)
        {
            if (_size == _capacity)
                throw new InvalidOperationException("Buffer is full");

            _buffer[_end++] = item;
            if (_capacity == _end)
                _end = 0;
            _size++;
        }

	    public void Skip(int amount)
	    {
			if (amount > _size)
				throw new ArgumentOutOfRangeException("Can't skip too many elements");
		    _start += amount;
		    if (_start >= _capacity)
			    _start -= _capacity;
		    _size -= amount;
	    }

	    public void SetAt(int index, T item)
	    {
			if (index >= _size)
				throw new ArgumentOutOfRangeException("Index is too big");
		    index += _start;
		    if (index >= _capacity)
			    index -= _capacity;
		    _buffer[index] = item;
	    }

		public T At(int index)
		{
			if (index >= _size)
				throw new ArgumentOutOfRangeException("Index is too big");
			index += _start;
			if (index >= _capacity)
				index -= _capacity;
			return _buffer[index];
		}

		// return index of item
		// return size when item not found
	    public int Find(T item)
	    {
		    return this.TakeWhile(t => !t.Equals(item)).Count();
	    }

	    #region Get Methods

	    public T[] Get(int amount)
	    {
		    var ans = new T[amount];
		    Get(ans);
		    return ans;
	    }

	    public int Get(T[] dst)
	    {
		    var amount = dst.Length;
		    var realCount = Math.Min(amount, Size);
		    for (var i = 0; i < realCount; ++i, ++_start)
		    {
			    if (_start == _capacity)
				    _start = 0;
			    dst[i] = _buffer[_start];
		    }
		    _size -= realCount;
		    return realCount;
	    }

        public T Get()
        {
            if (0 == _size)
                throw new InvalidOperationException("Buffer is empty");
            --_size;
            var ans = _buffer[_start];
            if (++_start == _capacity)
                _start = 0;
            return ans;
        }

		#endregion

		#region CopyTo methods

	    public void CopyTo(T[] dst)
	    {
		    CopyTo(dst, dst.Length);
	    }

	    public void CopyTo(T[] dst, int amount)
	    {
		    CopyTo(dst, amount, 0);
	    }

	    public void CopyTo(T[] dst, int amount, int arrayIndex)
	    {
			if (arrayIndex < 0)
				throw new IndexOutOfRangeException("Array index can't be less than zero");
			if (arrayIndex + _size > dst.Length)
				throw new ArgumentException("Array too small");
			var index = _start;
			for (var i = 0; i < _size; ++i, ++index)
			{
				if (index == _capacity)
					index = 0;
				dst[arrayIndex++] = _buffer[index];
			}            
	    }

		#endregion
		
        private IEnumerator<T> GetEnumerator()
        {
            var index = _start;
            for (var i = 0; i < _start; ++i, ++index)
            {
                if (_capacity == index)
                    index = 0;
                yield return _buffer[index];
            }
        }

        #region ICollection<T> Members

        void ICollection<T>.Add(T item)
        {
            Put(item);
        }

        void ICollection<T>.Clear()
        {
            _size = 0;
            _start = 0;
            _end = 0;
        }

        bool ICollection<T>.Contains(T item)
        {
            var comparer = EqualityComparer<T>.Default;
            var index = _start;
            for (var i = 0; i < _size; ++i, ++index)
            {
                if (index == _capacity)
                    index = 0;
                // TODO nulls
                if (comparer.Equals(item, _buffer[index]))
                    return true;
            }
            return false;
        }

        void ICollection<T>.CopyTo(T[] array, int arrayIndex)
        {
            CopyTo(array, Size, arrayIndex);
        }

        int ICollection<T>.Count
        {
            get { return Size; }
        }

        bool ICollection<T>.IsReadOnly
        {
            get { return false; }
        }

        // remove first element of the buffer
        bool ICollection<T>.Remove(T item)
        {
	        var ind = Find(item);
            if (ind == _size)
                return false;

			for (var i = 1; i < ind; ++i)
				SetAt(i, At(i-1));
	        --_size;
	        ++_start;
	        if (_start == _capacity)
		        _start = 0;
			return true;
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        #region ICollection Members

        public int Count { get { return Size; } }

        public bool IsSynchronized { get { return false; } }

        public object SyncRoot
        {
            get
            {
                if (null == _syncRoot)
                {
                    Interlocked.CompareExchange(ref _syncRoot, new Object(), null);
                }
                return _syncRoot;
            }
        }

        public void CopyTo(Array array, int index)
        {
            CopyTo((T[])array, index);   
        }
        #endregion

        #region IEnumerable Members
        IEnumerator IEnumerable.GetEnumerator()
        {
            return (IEnumerator)GetEnumerator();
        }
        #endregion
    }
}
