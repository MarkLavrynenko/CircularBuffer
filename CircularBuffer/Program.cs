using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CircularBuffer
{
    class Program
    {
        public static CircularBuffer<int> buff;
        const int bufferSize = 50;

        static void Main(string[] args)
        {
            buff = new CircularBuffer<int>(bufferSize);
        }
    }
}
