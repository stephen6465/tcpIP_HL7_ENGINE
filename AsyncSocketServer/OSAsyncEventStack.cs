using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;

namespace AsyncSocketServer
{
    /// <summary>
    /// class OSAsyncEventStack
    /// This is a very standard stack implementation.
    /// This one is set up to stack asynchronous socket connections.
    /// It has only two operations: a push onto the stack, and a pop off of it.
    /// </summary>
    sealed class OSAsyncEventStack
    {
        private Stack<SocketAsyncEventArgs> socketstack;

        // This constructor needs to know how many items it will be storing max
        public OSAsyncEventStack(Int32 maxCapacity)
        {
            socketstack = new Stack<SocketAsyncEventArgs>(maxCapacity);
        }

        // Pop an item off of the top of the stack
        public SocketAsyncEventArgs Pop()
        {
            //We are locking the stack, but we could probably use a ConcurrentStack if
            // we wanted to be fancy
            lock (socketstack)
            {
                if (socketstack.Count > 0)
                {
                    return socketstack.Pop();
                }
                else
                {
                    return null;
                }
            }
        }

        // Push an item onto the top of the stack
        public void Push(SocketAsyncEventArgs item)
        {
            if (item == null)
            {
                throw new ArgumentNullException("Cannot add null object to socket stack");
            }

            lock (socketstack)
            {
                socketstack.Push(item);
            }
        }
    }
}
