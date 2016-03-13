using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abot.Poco;
using SpiderEngine.Interface;

namespace SpiderEngine.Engine
{
    public class QueueManager<T> : IQueueManager<T>
    {
        private readonly ConcurrentQueue<T> _queue;

        public bool Enqueue(T item)
        {
            throw new NotImplementedException();
        }

        public bool TryDequeue(out T item)
        {
            throw new NotImplementedException();
        }
    }
}
