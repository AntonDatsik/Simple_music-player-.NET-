using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Windows.Threading;

namespace Lab7.Model
{
    public class MyObservableCollection<T> : ICollection<T>, INotifyCollectionChanged
    {
        private const int StartCapacity = 16;
        private int Capacity = StartCapacity;
        private T[] array;
        private string Name;
        private int CurrentCount = 0;
        [field: NonSerialized]
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs args)
        {
            var notifyCollectionChangedEventHandler = CollectionChanged;

            if (notifyCollectionChangedEventHandler == null)
                return;

            foreach (var handler in notifyCollectionChangedEventHandler.GetInvocationList())
            {
                var dispatcherObject = handler.Target as DispatcherObject;

                if (dispatcherObject != null && !dispatcherObject.CheckAccess())
                {
                    dispatcherObject.Dispatcher.Invoke(DispatcherPriority.DataBind, handler, this, args);
                }
                else
                    handler(this, args); // note : this does not execute handler in target thread's context
            }
        }

        public int Count
        {
            get
            {
                return CurrentCount;
            }
        }


        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        public MyObservableCollection()
        {
            array = new T[Capacity];
            CurrentCount = 0;
        }
        
        public MyObservableCollection(string name) : this()
        {
            this.Name = name;
        }

        public MyObservableCollection(int capacity)
        {
            if (capacity < 1)
            {
                throw new ArgumentOutOfRangeException();
            }
            array = new T[capacity];
            Capacity = capacity;
            CurrentCount = 0;
        }

        public MyObservableCollection(string name, int capacity) : this(capacity)
        {
            this.Name = name;
        }

        public int Size()
        {
            return Capacity;
        }

        public string GetName()
        {
            return this.Name;
        }

        public void Add(T obj)
        {
            if (CurrentCount == Capacity)
            {
                Array.Resize<T>(ref array, 2 * Capacity);
                Capacity <<= 1;
            }
            else
            {
                array[CurrentCount++] = obj;
            }

            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        public void Remove(int position)
        {
            if (position >= 0 && position < CurrentCount)
            {
                for (int i = position; i < CurrentCount - 1; ++i)
                {
                    array[i] = array[i + 1];
                }
                CurrentCount--;
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));

            }
            else
            {
                throw new IndexOutOfRangeException();
            }
        }

        public bool Remove(T obj)
        {
            lock (this)
            {
                bool res = true;
                try
                {
                    int pos = this.Search(obj);
                    Remove(pos);
                }
                catch (IndexOutOfRangeException)
                {
                    res = false;
                }
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
                return res;
            }
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            if (arrayIndex < CurrentCount)
                for (int i = 0; i < CurrentCount; ++i)
                    array[i] = this.array[i];
            else
                throw new IndexOutOfRangeException();
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        public bool Contains(T item)
        {
            bool result = false;
            for (int i = 0; i < CurrentCount; ++i)
                if (array[i].Equals(item))
                {
                    result = true;
                    break;
                }
            return result;
        }

        public void Clear()
        {
            Array.Resize(ref this.array, StartCapacity);
            for (int i = 0; i < StartCapacity; ++i)
            {
                array[i] = default(T);
            }
            CurrentCount = 0;
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        public T this[int index]
        {
            get
            {
                if (index < CurrentCount && index >= 0) return array[index];
                else
                {
                    throw new IndexOutOfRangeException();
                }
            }
            set
            {
                if (index < CurrentCount && index >= 0) array[index] = value;
                else
                {
                    throw new IndexOutOfRangeException();
                }
            }
        }

        public int Search(T obj)
        {
            int i = 0;
            for (i = 0; i < CurrentCount; ++i)
            {
                if (array[i].Equals(obj)) break;
            }

            if (i == CurrentCount) return -1;
            else
                return i;
        }

        public void Sort()
        {
            if (CurrentCount > 0)
                if (array[0] is IComparable)
                    Array.Sort(array, 0, CurrentCount);
                else
                {
                    throw new Exception("Member don't implemented interface IComparable");
                }
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        public int BinarySearch(T obj)
        {
            return Array.BinarySearch<T>(array, obj);
        }

        public void Sort(IComparer comparer)
        {
            if (CurrentCount > 0)
                if (array[0] is IComparable)
                    Array.Sort(array, 0, CurrentCount, comparer);
                else
                {
                    throw new Exception("Member don't implemented interface IComparable");
                }

            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        private class MyEnumerator<K> : IEnumerator<T>
        {
            int position = -1;
            private MyObservableCollection<T> MyObservableCollection;

            public MyEnumerator(MyObservableCollection<T> list)
            {
                this.MyObservableCollection = list;
            }

            public T Current
            {
                get { return MyObservableCollection.array[position]; }
            }


            object IEnumerator.Current
            {
                get { return Current; }
            }


            public void Reset()
            {
                position = -1;
            }

            public bool MoveNext()
            {
                if (position < MyObservableCollection.CurrentCount - 1)
                {
                    position++;
                    return true;
                }
                else
                {
                    return false;
                }
            }
            public void Dispose()
            { }
        }

        public IEnumerator<T> GetEnumerator()
        {
            return new MyEnumerator<T>(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new MyEnumerator<T>(this);
        }

        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder(Name + "\n");
            for (int i = 0; i < this.CurrentCount; ++i)
            {
                stringBuilder.Append(array[i].ToString() + "\n");
            }

            stringBuilder.Remove(stringBuilder.Length - 1, 1); // delete "/n" in the end
            return stringBuilder.ToString();
        }
        public void ReplaceAll(MyObservableCollection<T> collection)
        {
            this.Clear();
            foreach (T obj in collection)
            {
                this.Add(obj);
            }
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        public void RemoveAt(int index)
        {
            if (!(index >=0 && index < CurrentCount))
            {
                throw new IndexOutOfRangeException();
            }
            else
            {
                for (int i = index; i < CurrentCount - 1; ++i)
                {
                    array[i] = array[i + 1];
                }
                CurrentCount--;
            }
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }
    }
}
