using System;
using System.Collections.Generic;

namespace ORM_1_21_.Extensions
{
    internal class Set<TElement>
    {
        internal struct Slot
        {
            internal int HashCode;

            internal TElement Value;

            internal int Next;
        }

        private int[] _buckets;

        private Slot[] _slots;

        private int _count;

        private int _freeList;

        private readonly IEqualityComparer<TElement> _comparer;

        public Set()
            : this(null)
        {
        }

        public Set(IEqualityComparer<TElement> comparer)
        {
            if (comparer == null)
            {
                comparer = EqualityComparer<TElement>.Default;
            }
            this._comparer = comparer;
            _buckets = new int[7];
            _slots = new Slot[7];
            _freeList = -1;
        }

        public bool Add(TElement value)
        {
            return !Find(value, add: true);
        }

        public bool Contains(TElement value)
        {
            return Find(value, add: false);
        }

        public bool Remove(TElement value)
        {
            int num = InternalGetHashCode(value);
            int num2 = num % _buckets.Length;
            int num3 = -1;
            for (int num4 = _buckets[num2] - 1; num4 >= 0; num4 = _slots[num4].Next)
            {
                if (_slots[num4].HashCode == num && _comparer.Equals(_slots[num4].Value, value))
                {
                    if (num3 < 0)
                    {
                        _buckets[num2] = _slots[num4].Next + 1;
                    }
                    else
                    {
                        _slots[num3].Next = _slots[num4].Next;
                    }
                    _slots[num4].HashCode = -1;
                    _slots[num4].Value = default(TElement);
                    _slots[num4].Next = _freeList;
                    _freeList = num4;
                    return true;
                }
                num3 = num4;
            }
            return false;
        }

        private bool Find(TElement value, bool add)
        {
            int num = InternalGetHashCode(value);
            for (int num2 = _buckets[num % _buckets.Length] - 1; num2 >= 0; num2 = _slots[num2].Next)
            {
                if (_slots[num2].HashCode == num && _comparer.Equals(_slots[num2].Value, value))
                {
                    return true;
                }
            }
            if (add)
            {
                int num3;
                if (_freeList >= 0)
                {
                    num3 = _freeList;
                    _freeList = _slots[num3].Next;
                }
                else
                {
                    if (_count == _slots.Length)
                    {
                        Resize();
                    }
                    num3 = _count;
                    _count++;
                }
                int num4 = num % _buckets.Length;
                _slots[num3].HashCode = num;
                _slots[num3].Value = value;
                _slots[num3].Next = _buckets[num4] - 1;
                _buckets[num4] = num3 + 1;
            }
            return false;
        }

        private void Resize()
        {
            int num = checked(_count * 2 + 1);
            int[] array = new int[num];
            Slot[] array2 = new Slot[num];
            Array.Copy(_slots, 0, array2, 0, _count);
            for (var i = 0; i < _count; i++)
            {
                int num2 = array2[i].HashCode % num;
                array2[i].Next = array[num2] - 1;
                array[num2] = i + 1;
            }
            _buckets = array;
            _slots = array2;
        }

        internal int InternalGetHashCode(TElement value)
        {
            if (value != null)
            {
                return _comparer.GetHashCode(value) & 0x7FFFFFFF;
            }
            return 0;
        }
    }

    internal class MySet<TElement>
    {
        private readonly HashSet<object> _hashSet = new HashSet<object>();

        public bool Add(TElement value)
        {
            var v = AttributesOfClass<TElement>.GetValuePrimaryKey(value);
            if (_hashSet.Contains(v)) return false;
            _hashSet.Add(v);
            return true;
        }

    }
}