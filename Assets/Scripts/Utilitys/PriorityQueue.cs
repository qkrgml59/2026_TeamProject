using System.Collections.Generic;
using System;
using UnityEngine;

namespace Utilitys
{
    public class PriorityQueue<T>
    {
        private List<(T item, float priority)> heap = new List<(T item, float priority)>();

        public int Count => heap.Count;

        public void Clear()
        {
            heap.Clear();
        }

        public void Enqueue(T item, float priority)
        {
            heap.Add((item, priority));
            HeapifyUp(heap.Count - 1);
        }

        public T Dequeue()
        {
            if (heap.Count == 0)
                throw new InvalidOperationException("Queue is empty");

            T rootItem = heap[0].item;
            heap[0] = heap[^1];                 // 뒤에서 첫 번째 데이터 선택
            heap.RemoveAt(heap.Count - 1);
            HeapifyDown(0);
            return rootItem;
        }

        private void HeapifyUp(int i)
        {
            while (i > 0)
            {
                int parent = (i - 1) / 2;
                if (heap[i].priority >= heap[parent].priority)
                    break;
                (heap[i], heap[parent]) = (heap[parent], heap[i]);
                i = parent;
            }
        }

        private void HeapifyDown(int i)
        {
            int last = heap.Count - 1;
            while (true)
            {
                int left = 2 * i + 1;
                int right = 2 * i + 2;
                int smallest = i;

                if (left <= last && heap[left].priority < heap[smallest].priority)
                    smallest = left;

                if (right <= last && heap[right].priority < heap[smallest].priority)
                    smallest = right;

                if (smallest == i) break;

                (heap[i], heap[smallest]) = (heap[smallest], heap[i]);
                i = smallest;
            }
        }

        public bool Contains(T item)
        {
            foreach (var h in heap)
                if (Equals(h.item, item))
                    return true;

            return false;
        }
    }
}
