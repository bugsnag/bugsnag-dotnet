using System;
using System.Collections.Generic;
using System.Threading;

namespace Bugsnag
{
  public class ThreadQueueTransport : ITransport
  {
    private static ThreadQueueTransport instance = null;
    private static readonly object instanceLock = new object();

    private readonly BlockingQueue<WorkItem> _queue;

    private readonly Thread _worker;

    private readonly Transport _transport;

    private long _activeRequestCounter;

    private ThreadQueueTransport()
    {
      _activeRequestCounter = 0;
      _transport = new Transport();
      _queue = new BlockingQueue<WorkItem>();
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

    public void Send(Uri endpoint, byte[] report)
    {
      Interlocked.Increment(ref _activeRequestCounter);
      _queue.Enqueue(new WorkItem(endpoint, report));
    }

    private void ProcessQueue()
    {
      while (true)
      {
        var workItem = _queue.Dequeue();
        _worker.IsBackground = false;
        _transport.BeginSend(workItem.Endpoint, workItem.Report, ReportCallback, workItem);
      }
    }

    private void ReportCallback(IAsyncResult asyncResult)
    {
      _transport.EndSend(asyncResult);
      var finishedProcessing = Interlocked.Decrement(ref _activeRequestCounter) == 0;
      _worker.IsBackground = finishedProcessing;
    }

    private class WorkItem
    {
      private readonly Uri _endpoint;
      private readonly byte[] _report;

      public WorkItem(Uri endpoint, byte[] report)
      {
        _endpoint = endpoint;
        _report = report;
      }

      public Uri Endpoint { get { return _endpoint; } }
      public byte[] Report { get { return _report; } }
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
