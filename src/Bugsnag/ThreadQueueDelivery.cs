using Bugsnag.Payload;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace Bugsnag
{
  public class Countdown
  {
    private readonly object _lock = new object();
    private int _counter;
    private readonly ManualResetEvent _resetEvent;

    public Countdown(int initialCount)
    {
      _counter = initialCount;
      _resetEvent = new ManualResetEvent(initialCount == 0);
    }

    internal void AddCount()
    {
      lock (_lock)
      {
        _counter++;
        if (_counter > 0)
        {
          _resetEvent.Reset();
        }
      }
    }

    internal void Signal()
    {
      lock (_lock)
      {
        _counter--;
        if (_counter <= 0)
        {
          _resetEvent.Set();
        }
      }
    }

    internal void Wait(TimeSpan timeout)
    {
      _resetEvent.WaitOne(timeout);
    }
  }

  public class ThreadQueueDelivery : IDelivery
  {
    private static ThreadQueueDelivery instance = null;
    private static readonly object instanceLock = new object();

    private readonly BlockingQueue<IPayload> _queue;

    private readonly Thread _worker;

    private ThreadQueueDelivery()
    {
      _queue = new BlockingQueue<IPayload>();
      _worker = new Thread(new ThreadStart(ProcessQueue)) {
        Name = "Bugsnag Queue",
        IsBackground = true
      };
      _worker.Start();
    }

    internal void Stop()
    {
      _queue.Wait(TimeSpan.FromSeconds(5));
    }

    public static ThreadQueueDelivery Instance
    {
      get
      {
        lock (instanceLock)
        {
          if (instance == null)
          {
            instance = new ThreadQueueDelivery();
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
        }
      }
    }

    private void ReportCallback(IAsyncResult asyncResult)
    {
      if (asyncResult.AsyncState is WebRequest request)
      {
        var response = request.EndSend(asyncResult);
        _queue.Signal();
      }
    }

    public void Send(IPayload payload)
    {
      _queue.Enqueue(payload);
    }

    private class BlockingQueue<T>
    {
      private readonly Countdown _countdown;
      private readonly Queue<T> _queue;
      private readonly object _queueLock;

      public BlockingQueue()
      {
        _countdown = new Countdown(0);
        _queueLock = new object();
        _queue = new Queue<T>();
      }

      public void Enqueue(T item)
      {
        lock (_queueLock)
        {
          _countdown.AddCount();
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

      public void Signal()
      {
        _countdown.Signal();
      }

      public void Wait(TimeSpan timeout)
      {
        _countdown.Wait(timeout);
      }
    }
  }
}
