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

    private readonly Dictionary<IAsyncResult, bool> _inflight;

    private readonly object _inflightLock;

    private readonly Transport _transport;

    private ThreadQueueTransport()
    {
      _transport = new Transport();
      _queue = new Queue<WorkItem>();
      _worker = new Thread(new ThreadStart(ProcessQueue));
      _queueLock = new object();
      _inflight = new Dictionary<IAsyncResult, bool>();
      _inflightLock = new object();
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
      }
    }

    private void ProcessQueue()
    {
      WorkItem workItem = null;

      lock (_queueLock)
      {
        if (_queue.Peek() != null)
        {
          workItem = _queue.Dequeue();
        }
      }

      if (workItem != null)
      {
        lock (_inflightLock)
        {
          var asyncResult = _transport.BeginSend(workItem.Endpoint, workItem.Report, ReportCallback, workItem);
          _inflight.Add(asyncResult, true);
        }
      }
    }

    private void ReportCallback(IAsyncResult asyncResult)
    {
      var state = (WorkItem)asyncResult.AsyncState;
      var responseCode = _transport.EndSend(asyncResult);
      // don't do anything with the result right now
      lock (_inflightLock)
      {
        _inflight.Remove(asyncResult);
      }
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
