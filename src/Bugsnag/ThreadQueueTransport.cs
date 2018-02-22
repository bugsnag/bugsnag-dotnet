using Bugsnag.Payload;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace Bugsnag
{
  public class ThreadQueueTransport : ITransport
  {
    private static ThreadQueueTransport instance = null;
    private static readonly object instanceLock = new object();

    private readonly BlockingQueue<IPayload> _queue;

    private readonly Thread _worker;

    private long _activeRequestCounter;

    private ThreadQueueTransport()
    {
      _activeRequestCounter = 0;
      _queue = new BlockingQueue<IPayload>();
      _worker = new Thread(new ThreadStart(ProcessQueue)) { IsBackground = true };
      _worker.Start();
    }

    public static ThreadQueueTransport Instance
    {
      get
      {
        lock (instanceLock)
        {
          if (instance == null)
          {
            instance = new ThreadQueueTransport();
          }

          return instance;
        }
      }
    }

    private void ProcessQueue()
    {
      while (true)
      {
        var payload = _queue.Dequeue();
        _worker.IsBackground = false;
        try
        {
          var serializedPayload = payload.Serialize();
          if (serializedPayload != null)
          {
            var request = new WebRequest();
            request.BeginSend(payload.Endpoint, payload.Proxy, payload.Headers, serializedPayload, ReportCallback, request);
          }
        }
        catch (System.Exception exception)
        {
          Trace.WriteLine(exception);
          _worker.IsBackground = true;
        }
      }
    }

    private void ReportCallback(IAsyncResult asyncResult)
    {
      if (asyncResult.AsyncState is WebRequest request)
      {
        var response = request.EndSend(asyncResult);
        if (response != null)
        {
          var finishedProcessing = Interlocked.Decrement(ref _activeRequestCounter) == 0;
          _worker.IsBackground = finishedProcessing;
        }
      }
    }

    public void Send(IPayload payload)
    {
      Interlocked.Increment(ref _activeRequestCounter);
      _queue.Enqueue(payload);
    }

    private class BlockingQueue<T>
    {
      private readonly Queue<T> _queue;
      private readonly object _queueLock;

      public BlockingQueue()
      {
        _queueLock = new object();
        _queue = new Queue<T>();
      }

      public void Enqueue(T item)
      {
        lock (_queueLock)
        {
          _queue.Enqueue(item);
          Monitor.Pulse(_queueLock);
        }
      }

      public T Dequeue()
      {
        lock (_queueLock)
        {
          while (_queue.Count == 0)
          {
            Monitor.Wait(_queueLock);
          }

          return _queue.Dequeue();
        }
      }
    }
  }
}
