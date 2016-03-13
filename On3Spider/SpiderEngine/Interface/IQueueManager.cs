﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Abot.Poco;

namespace SpiderEngine.Interface
{
    public interface IQueueManager<T>
    {
        bool Enqueue(T item);
        bool TryDequeue(out T item);
    }
}
