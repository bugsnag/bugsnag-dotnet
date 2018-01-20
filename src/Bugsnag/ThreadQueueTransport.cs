using System;
using System.Collections.Generic;
using System.Threading;

namespace Bugsnag
{
  class ThreadQueueTransport : ITransport
  {
    private static ThreadQueueTransport instance = null;

    private static readonly object instanceLock = new object();

    private readonly Queue<WorkItem> _queue;

    private readonly Thread _worker;

    private readonly object _queueLock;

    private readonly Transport _transport;

    private int _requestCounter;

    private ThreadQueueTransport()
    {
      _requestCounter = 0;
      _transport = new Transport();
      _queue = new Queue<WorkItem>();
      _worker = new Thread(new ThreadStart(ProcessQueue));
      _queueLock = new object();

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
      lock (_queueLock)
      {
        _queue.Enqueue(new WorkItem(endpoint, report));
        Monitor.Pulse(_queueLock);
      }
    }

    private void ProcessQueue()
    {
      while (true)
      {
        WorkItem workItem = null;

        lock (_queueLock)
        {
          while (_queue.Count == 0)
          {
            Monitor.Wait(_queueLock);
          }

          _worker.IsBackground = false;
          Interlocked.Increment(ref _requestCounter);
          workItem = _queue.Dequeue();
        }

        if (workItem != null)
        {
          _transport.BeginSend(workItem.Endpoint, workItem.Report, ReportCallback, workItem);
        }
      }
    }

    private void ReportCallback(IAsyncResult asyncResult)
    {
      _worker.IsBackground = Interlocked.Decrement(ref _requestCounter) == 0;
      var responseCode = _transport.EndSend(asyncResult);
      // don't do anything with the result right now
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
  }
}
