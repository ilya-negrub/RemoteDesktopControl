using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientRdc.Helpers
{
    //Если очередь пуста поток будет ожидать добавления элемента в очередь
    public class QueueWaitOfDequeue<T> : IEnumerable<T>
    {
        System.Threading.Semaphore sem = new System.Threading.Semaphore(0, 1);
        private bool isLock = false;

        public int Count => queue.Count;
        private Queue<T> queue = new Queue<T>();

        public T Dequeue()
        {
            while (queue.Count == 0)
            {
                isLock = true;
                sem.WaitOne();
                isLock = false;
            }

            return queue.Dequeue();
        }

        public void Enqueue(T item)
        {
            queue.Enqueue(item);
            if (isLock)
                sem.Release();
        }

        public T Peek() => queue.Peek();

        public void Clear() => queue.Clear();
        public void Clear(Func<Queue<T>, bool> func)
        {
            if (func?.Invoke(queue) == true)
                queue.Clear();
        }

        public IEnumerator<T> GetEnumerator()
        {
            return queue.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
