using System.Collections.Generic;
using System;
using System.Text;
using System.Buffers.Binary;
using System.Collections;

namespace Prota
{
    // 环形双指针.
    // 左闭右开区间.
    // 使用计数来确定是否满了.
    public class CircleDualPointer
    {
        public readonly int max;

        public int front { get; private set; }
        public int back { get; private set; }
        public int count { get; private set; }
        public bool isEmpty => count == 0;
        public bool isFull => count == max;
        
        public int this[int offset] => Position(front + offset);

        public CircleDualPointer(int max)
        {
            this.max = max;
        }
        
        public CircleDualPointer(int max, int currentCount)
        {
            this.front = 0;
            this.back = currentCount;
            this.count = currentCount;
            this.max = max;
        }
        
        
        public void Reset()
        {
            front = back = count = 0;
        }
        
        public int FrontMovePrev()
        {
            if(isFull) throw new Exception("list is full!");
            front = (front - 1).ModSys(max);
            count++;
            return front;
        }
        
        public int BackMovePrev()
        {
            if(isEmpty) throw new Exception("list is empty!");
            back = (back - 1).ModSys(max);
            count--;
            return back;
        }
        
        public int FrontMoveNext()
        {
            if(isEmpty) throw new Exception("list is empty!");
            front = (front + 1).ModSys(max);
            count--;
            return front;
        }
        
        public int BackMoveNext()
        {
            if(isFull) throw new Exception("list is full!");
            back = (back + 1).ModSys(max);
            count++;
            return back;
        }
        
        public int Position(int i)
        {
            return i.ModSys(max);
        }
        
        
        public static void UnitTest()
        {
            CircleDualPointer g = new CircleDualPointer(6);
            try { g.FrontMoveNext(); } catch { Console.WriteLine("Success!"); }
            try { g.BackMovePrev(); } catch { Console.WriteLine("Success!"); }
            
            g.isEmpty.Assert();
            (!g.isFull).Assert();
            
            g.FrontMovePrev();
            g.BackMoveNext();
            (g.front == 5).Assert();
            (g.back == 1).Assert();
            (g.count == 2).Assert();
            (g[0] == 5).Assert();
            (g[1] == 0).Assert();
            
            g.BackMoveNext();       // 2
            g.BackMoveNext();
            g.BackMoveNext();
            g.BackMoveNext();       // 5, full.
            g.isFull.Assert();
            (!g.isEmpty).Assert();
            
            (g.count == g.max).Assert();
            g.FrontMoveNext();
            
            (g.count == g.max - 1).Assert();
            (g.count == 5).Assert();
            
            g.Reset();
            (g.count == 0).Assert();
            (g.front == 0).Assert();
            (g.back == 0).Assert();
            
            Console.WriteLine("All Done!");
        }
    }




    // 环形数组.
    public class CircleList<T>
    {
        CircleDualPointer pointers;
        
        T[] data;
        
        public CircleList() { }
        
        void Resize()
        {
            if(data == null)
            {
                data = new T[4];
                pointers = new CircleDualPointer(4);
            }
            else
            {
                var oriData = data;
                var oriPointers = pointers;
                data = new T[oriData.Length * 2];
                pointers = new CircleDualPointer(oriPointers.max * 2, oriPointers.count);
                for(int i = 0; i < oriPointers.count; i++)
                {
                    data[i] = oriData[oriPointers[i]];
                }
            }
        }
        
        public T this[int index]
        { 
            get => data[pointers[index]];
            set => data[pointers[index]] = value;
        }

        public int count => pointers.count;

        public bool IsReadOnly => false;


        public void Clear()
        {
            
            pointers = null;
            data = null;
        }
        
        public void Reset() => pointers.Reset();
        
        public void PushFront(T v)
        {
            if(pointers.count == data.Length) Resize();
            pointers.FrontMovePrev();
            data[pointers.front] = v;
        }
        
        public T PopFront()
        {
            var res = data[pointers.front];
            pointers.FrontMoveNext();
            return res;
        }
        
        public void PushBack(T v)
        {
            if(pointers.count == data.Length) Resize();
            data[pointers.front] = v;
            pointers.BackMoveNext();
        }
        
        public T PopBack()
        {
            pointers.BackMovePrev();
            var res = data[pointers.front];
            return res;
        }
    }
}