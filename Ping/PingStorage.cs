using System;
using System.Collections.Generic;

namespace SupportTool.Ping
{
    class PingStorage
    {
        private LimitedQueue<Tuple<ushort, long>> PingResults;
        private string Host;

        /// <summary>
        /// Creates a storage for ping results.
        /// </summary>
        /// <param name="pingTarget">IP or Host to ping</param>
        /// <param name="timeInToKeep">Amount of minutes to keep the results</param>
        /// <param name="pingFrequency">Ping each X seconds</param>
        public PingStorage(string pingTarget, ushort timeInToKeep, byte pingFrequency)
        {
            timeInToKeep = 0 != timeInToKeep ? timeInToKeep : (ushort)1;
            pingFrequency = 0 != pingFrequency ? pingFrequency : (byte)1;
            PingResults = new LimitedQueue<Tuple<ushort, long>>(timeInToKeep * (60 / pingFrequency));
            Host = pingTarget;
        }

        public PingResult ping()
        {
            PingResult result = Pinger.PingHosts(Host);
            
            PingResults.Enqueue(Tuple.Create((ushort) result.AveragePing, DateTime.Today.Ticks));

            return result;
        }

        public int Count { get { return PingResults.Count; } }
    }

    class LimitedQueue<T> : Queue<T>
    {
        public int Limit { get; set; }

        public LimitedQueue(int limit) : base(limit)
        {
            Limit = limit;
        }

        public new void Enqueue(T item)
        {
            while (Count >= Limit)
            {
                Dequeue();
            }
            base.Enqueue(item);
        }
    }
}
