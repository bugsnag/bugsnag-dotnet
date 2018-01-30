using System;
using System.Collections.Generic;
using System.Threading;

namespace Bugsnag
{
  public class ThreadQueueTransport : ITransport
  {
    private static ThreadQueueTransport instance = null;

    private static readonly object instanceLock = new object();

    private readonly Queue<WorkItem> _queue;

    private readonly Thread _worker;

    private static readonly object _queueLock = new object();

    private readonly Transport _transport;

    private int _requestCounter;

    private static readonly object _workerLock = new object();

    private ThreadQueueTransport()
    {
      _requestCounter = 0;
      _transport = new Transport();
      _queue = new Queue<WorkItem>();
      lock (_workerLock)
      {
        _worker = new Thread(new ThreadStart(ProcessQueue)) { IsBackground = true };
        _worker.Start();
      }
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
        lock (_workerLock)
        {
          _worker.IsBackground = false;
        }
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
      lock (_workerLock)
      {
        _worker.IsBackground = Interlocked.Decrement(ref _requestCounter) == 0;
      }
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
