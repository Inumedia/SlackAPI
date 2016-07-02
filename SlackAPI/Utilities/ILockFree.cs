using System;

namespace SlackAPI.Utilities
{
    public abstract class ILockFree<T> where T : class
    {
        internal class SingleLinkNode
        {
            public SingleLinkNode Next;
            public T Item;
        }

        public abstract void Push(T pItem);
        public virtual void Push(ILockFree<T> pItems) { T obj; while (pItems.Pop(out obj)) Push(obj); }
        public abstract bool Pop(out T pItem);
        public abstract T Pop();
    }
}
