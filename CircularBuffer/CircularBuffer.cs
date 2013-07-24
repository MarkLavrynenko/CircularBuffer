using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace CircularBuffer
{
    // should we write IEnumerable<T>
    public class CircularBuffer<T> : IEnumerable<T>, ICollection<T>, IEnumerable, ICollection
    {
        private readonly int _capacity;
        private int _size;
        private int _start, _end;
        private T[] _buffer;

        // TODO Serializable attribute
        private object syncRoot;

        public CircularBuffer(int capacity)
        {
            _capacity = capacity;
            _start = _end = capacity;
            _buffer = new T[capacity];
        }

        // TODO constructor from another collection

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


            
        #endregion


        public void Put(T item)
        {
            if (_size == _capacity)
                throw new InvalidOperationException("Buffer is full");

            _buffer[_end++] = item;
            if (_capacity == _end)
                _end = 0;
            _size++;
        }

        public T Get()
        {
            if (0 == _size)
                throw new InvalidOperationException("Buffer is empty");
            --_size;
            T ans = _buffer[_start];
            if (++_start == _capacity)
                _start = 0;

            return ans;
        }   


        private IEnumerator<T> GetEnumerator()
        {
            int index = _start;
            for (int i = 0; i < _start; ++i, ++index)
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
            int index = _start;
            for (int i = 0; i < _size; ++i, ++index)
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
            if (arrayIndex + _size >= array.Length)
                throw new ArgumentException("Array too small");
            int index = _start;
            for (int i = 0; i < _size; ++i, ++_start)
            {
                if (_start == _capacity)
                    _start = 0;
                array[arrayIndex++] = _buffer[index];
            }            
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
            if (0 == _size)
                return false;                
            Get();
            return true;
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        #region ICollection Members

        public int Count { get { return Size; } }

        public bool IsSynchronized { get { return false;  } }

        public object SyncRoot
        {
            get
            {
                if (null == syncRoot)
                {
                    Interlocked.CompareExchange(ref syncRoot, new Object(), null);
                }
                return syncRoot;
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
