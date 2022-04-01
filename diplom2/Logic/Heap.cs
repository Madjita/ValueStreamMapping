using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


namespace diplom2.Logic
{

    public class Heap<T> : IComparable
    {
        private Comparer<T> _comparer; // optional comparator

        private List<T> items = new List<T>();
        public int Count => items.Count;

        public object locker = new object();

        public T Peek()
        {
            if (Count > 0)
            {
                return items[0];
            }
            else
            {
                return default(T);
            }

        }

        public void Insert(T item)
        {
           // lock (locker)
          //  {
                items.Add(item);

                var currentIndex = Count - 1;
                var parentIndex = GetParentIndex(currentIndex);


                var compare = _comparer.Compare(items[parentIndex], items[currentIndex]);

                if (compare == 0)
                {
                    return;
                }

                while (currentIndex > 0 && compare > 0)
                {
                    Swap(currentIndex, parentIndex);

                    currentIndex = parentIndex;
                    parentIndex = GetParentIndex(currentIndex);
                }

                for (int i = Count; i >= 0; i--)
                {
                    Sort(i);
                }
          //  }

        }

        public void Delete(T item)
        {
            if (item is SimulationOrderProductItem data)
            {
                var t = items.Find(o =>
                {
                    if(o is SimulationOrderProductItem d)
                    {
                        return d.getOrderProductItem().Id == data.getOrderProductItem().Id;
                    }
                    return false;
                });



                items.Remove(t);
            }
            
        }

        public List<T> ToList()
        {
            return items;
        }

        public List<T> ToListCopy()
        {
            var list = items.GetRange(0, Count);
            return list;
        }

        public T GetMax()
        {
            lock (locker)
            {
                var result = items[0];
                //items[0] = items[Count - 1];
                //items.RemoveAt(Count - 1);
                items.RemoveAt(0);
                //Sort(0);
                return result;
            }
          
        }

        private void Sort(int curentIndex)
        {
            int leftIndex;
            int rightIndex;
            int maxIndex = curentIndex;




            while (curentIndex < Count)
            {
                Console.WriteLine("SORT");

                leftIndex = 2 * curentIndex + 1;
                rightIndex = 2 * curentIndex + 2;

                if (Count <= leftIndex || Count <= rightIndex)
                {
                    return;
                }

                var left = _comparer.Compare(items[leftIndex], items[maxIndex]);

                if(left > 0)//if (_comparer.Compare(items[leftIndex], items[maxIndex]) > 0) //items[leftIndex] > items[maxIndex]
                {
                    maxIndex = leftIndex;
                }


                if (Count <= leftIndex || Count <= rightIndex)
                {
                    return;
                }

                var right = _comparer.Compare(items[rightIndex], items[maxIndex]);

                if(right > 0)//if (_comparer.Compare(items[rightIndex], items[maxIndex]) > 0) //items[rightIndex] > items[maxIndex]
                {
                    maxIndex = rightIndex;
                }


                if (Count <= leftIndex || Count <= rightIndex)
                {
                    return;
                }

                if (maxIndex == curentIndex)
                {
                    break;
                }


                Swap(curentIndex, maxIndex);
                curentIndex = maxIndex;

            }


        }

        private void Swap(int currentIndex, int parentIndex)
        {
            var temp = items[currentIndex];
            items[currentIndex] = items[parentIndex];
            items[parentIndex] = temp;
        }

        private static int GetParentIndex(int currentIndex)
        {
            return (currentIndex - 1) / 2;
        }

        public Heap(Comparer<T> comparer)
        {
            _comparer = comparer;
        }

        public IEnumerable<T> GetEnumerator()
        {
            while (Count > 0)
            {
                yield return GetMax();
            }
        }

        public int CompareTo(object obj)
        {
            throw new NotImplementedException();
        }


        public T GetIndex(int index)
        {
            var temp = items[index];
            items.RemoveAt(index);
            Sort(0);
            return temp;
        }

        /*
        public static implicit operator Heap<T>(Heap<SimulationOrder> v)
        {
            throw new NotImplementedException();
        }*/
    }
}

