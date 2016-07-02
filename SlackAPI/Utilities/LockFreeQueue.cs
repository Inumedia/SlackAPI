using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace SlackAPI.Utilities
{
    public class LockFreeQueue<T> : ILockFree<T>
        where T : class
    {
        private SingleLinkNode mHead;
        private SingleLinkNode mTail;
        public int Count;
        public T Latest
        {
            get
            {
                return mHead.Next == null ? default(T) : mTail.Item;
            }
        }

        public LockFreeQueue()
        {
            mHead = new SingleLinkNode();
            mTail = mHead;
        }

        private static bool CompareAndExchange(ref SingleLinkNode pLocation, SingleLinkNode pComparand, SingleLinkNode pNewValue)
        {
            return
                pComparand ==
                Interlocked.CompareExchange(ref pLocation, pNewValue, pComparand);
        }

        public T Next { get { return mHead.Next == null ? default(T) : mHead.Next.Item; } }
        public void Unshift(T pItem)
        {
            SingleLinkNode oldHead = null;

            SingleLinkNode newNode = new SingleLinkNode();
            newNode.Item = pItem;

            bool newNodeWasAdded = false;
            while (!newNodeWasAdded)
            {
                oldHead = mHead.Next;
                newNode.Next = oldHead;

                if (mHead.Next == oldHead)
                    newNodeWasAdded = CompareAndExchange(ref mHead.Next, oldHead, newNode);
            }

            CompareAndExchange(ref mHead, oldHead, newNode);
        }
        public override void Push(T pItem)
        {
            SingleLinkNode oldTail = null;
            SingleLinkNode oldTailNext;

            SingleLinkNode newNode = new SingleLinkNode();
            newNode.Item = pItem;

            bool newNodeWasAdded = false;
            while (!newNodeWasAdded)
            {
                oldTail = mTail;
                oldTailNext = oldTail.Next;

                if (mTail == oldTail)
                    if (oldTailNext == null)
                        newNodeWasAdded = CompareAndExchange(ref mTail.Next, null, newNode);
                    else
                        CompareAndExchange(ref mTail, oldTail, oldTailNext);
            }

            CompareAndExchange(ref mTail, oldTail, newNode);
            Interlocked.Increment(ref Count);
        }

        public override bool Pop(out T pItem)
        {
            pItem = default(T);
            SingleLinkNode oldHead = null;

            bool haveAdvancedHead = false;
            while (!haveAdvancedHead)
            {
                oldHead = mHead;
                SingleLinkNode oldTail = mTail;
                SingleLinkNode oldHeadNext = oldHead.Next;

                if (oldHead == mHead)
                {
                    if (oldHead == oldTail)
                    {
                        if (oldHeadNext == null)
                            return false;
                        CompareAndExchange(ref mTail, oldTail, oldHeadNext);
                    }

                    else
                    {
                        pItem = oldHeadNext.Item;
                        haveAdvancedHead =
                          CompareAndExchange(ref mHead, oldHead, oldHeadNext);
                    }
                }
            }
            Interlocked.Decrement(ref Count);
            return true;
        }

        public T Shift()
        {
            T result;
            Shift(out result);
            return result;
        }

        public bool Shift(out T pItem)
        {
            pItem = default(T);
            if (mHead == null)
                return false;
            SingleLinkNode oldHead = null;

            bool haveAdvancedHead = false;
            while (!haveAdvancedHead)
            {
                oldHead = mHead;
                if (oldHead != null)
                {
                    SingleLinkNode oldHeadNext = oldHead.Next;
                    if (CompareAndExchange(ref mHead, oldHead, oldHeadNext))
                    {
                        pItem = oldHead.Item;
                        return true;
                    }
                }
            }
            return false;
        }

        public override T Pop()
        {
            T result;
            Pop(out result);
            return result;
        }

        public override string ToString()
        {
            return String.Format("Item count: {0}", Count);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return new LockFreeEnumerator(this);
        }

        /// <summary>
        /// Does *not* provide any kind of stateful guarantee.  Should only be used in cases where we know that the queue is not volatile.
        /// </summary>
        internal class LockFreeEnumerator : IEnumerator<T>
        {
            LockFreeQueue<T> parent;
            SingleLinkNode currentNode;

            T IEnumerator<T>.Current
            {
                get
                {
                    return currentNode.Item;
                }
            }
            object IEnumerator.Current
            {
                get
                {
                    return currentNode.Item;
                }
            }

            public bool MoveNext()
            {
                if (currentNode == null)
                    currentNode = parent.mHead.Next;
                else
                    currentNode = currentNode.Next;
                return currentNode != null;
            }

            public LockFreeEnumerator(LockFreeQueue<T> list)
            {
                parent = list;
            }

            public void Dispose()
            {
                parent = null;
                currentNode = null;
            }

            public void Reset()
            {
                currentNode = parent.mHead.Next;
            }
        }
    }
}
