using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FillItUp
{
    // Two and three member Tuples

    struct Tuple<T, U> : IEquatable<Tuple<T, U>>
    {
        readonly T first;
        readonly U second;

        public Tuple(T first, U second)
        {
            this.first = first;
            this.second = second;
        }

        public T First { get { return first; } }
        public U Second { get { return second; } }

        public override int GetHashCode()
        {
            return first.GetHashCode() ^ second.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }
            return Equals((Tuple<T, U>)obj);
        }

        public bool Equals(Tuple<T, U> other)
        {
            return other.first.Equals(first) && other.second.Equals(second);
        }
    }

    struct Tuple<T, U, W> : IEquatable<Tuple<T, U, W>>
    {
        readonly T first;
        readonly U second;
        readonly W third;

        public Tuple(T first, U second, W third)
        {
            this.first = first;
            this.second = second;
            this.third = third;
        }

        public T First { get { return first; } }
        public U Second { get { return second; } }
        public W Third { get { return third; } }

        public override int GetHashCode()
        {
            return first.GetHashCode() ^ second.GetHashCode() ^ third.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }
            return Equals((Tuple<T, U, W>)obj);
        }

        public bool Equals(Tuple<T, U, W> other)
        {
            return other.first.Equals(first) && other.second.Equals(second) && other.third.Equals(third);
        }
    }
}
