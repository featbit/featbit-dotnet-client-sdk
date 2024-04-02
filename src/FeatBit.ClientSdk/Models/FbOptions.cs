using FeatBit.ClientSdk.Enums;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace FeatBit.ClientSdk
{
    public sealed class FbOptions
    {
        /// <summary>
        /// How long the client constructor will block awaiting a successful connection to FeatBit.
        /// </summary>
        /// <remarks>
        /// This value must greater than <see cref="ConnectTimeout"/>.
        /// </remarks>
        /// <value>Defaults to 5 seconds</value>
        public TimeSpan StartWaitTime { get; set; }

        /// <summary>
        /// Whether or not this client is offline. If true, no calls to FeatBit will be made.
        /// </summary>
        /// <value>Defaults to <c>false</c></value>
        public bool Offline { get; set; }

        /// <summary>
        /// The SDK key for your FeatBit environment.
        /// </summary>
        public string EnvSecret { get; set; }

        /// <summary>
        /// The base URI of the streaming service
        /// </summary>
        /// <value>Defaults to ws://localhost:5100</value>
        public Uri StreamingUri { get; set; }

        /// <summary>
        /// The base URI of the event service
        /// </summary>
        /// <value>Defaults to http://localhost:5100</value>
        public Uri EvalUri { get; set; }

        /// <summary>
        /// The event flush timeout.
        /// </summary>
        /// <value>Defaults to 5 seconds</value>
        public TimeSpan FlushTimeout { get; set; }

        /// <summary>
        /// The maximum number of flush workers.
        /// </summary>
        /// <value>Defaults to <c>Math.Min(Math.Max(Environment.ProcessorCount / 2, 1), 4)</c></value>
        public int MaxFlushWorker { get; set; }

        /// <summary>
        /// The time interval between each flush operation.
        /// </summary>
        /// <value>Defaults to 5 seconds</value>
        public TimeSpan AutoFlushInterval { get; set; }

        /// <summary>
        /// The maximum number of events in queue.
        /// </summary>
        /// <value>Defaults to 10_000</value>
        public int MaxEventsInQueue { get; set; }

        /// <summary>
        /// The maximum number of events per request. 
        /// </summary>
        /// <value>Defaults to 50</value>
        public int MaxEventPerRequest { get; set; }

        /// <summary>
        /// The maximum number of attempts to send an event before giving up.
        /// </summary>
        /// <value>Defaults to 2</value>
        public int MaxSendEventAttempts { get; set; }

        /// <summary>
        /// The time interval between each retry attempt to send an event.
        /// </summary>
        /// <value>Defaults to 200 milliseconds</value>
        public TimeSpan SendEventRetryInterval { get; set; }

        /// <summary>
        /// The logger factory used by FbClient.
        /// </summary>
        /// <value>Defaults to <see cref="NullLoggerFactory.Instance"/></value>
        public ILoggerFactory LoggerFactory { get; set; }

        public DataSyncMethodEnum DataSyncMethod { get; set; }

        public long PoollingInterval { get; set; }

        internal FbOptions(
            bool offline,
            string envSecret,
            Uri evalUri,
            int maxFlushWorker,
            TimeSpan autoFlushInterval,
            TimeSpan flushTimeout,
            int maxEventsInQueue,
            int maxEventPerRequest,
            int maxSendEventAttempts,
            TimeSpan sendEventRetryInterval,
            ILoggerFactory loggerFactory,
            long pollingInterval)
        {
            Offline = offline;

            EnvSecret = envSecret;
            EvalUri = evalUri;

            MaxFlushWorker = maxFlushWorker;
            AutoFlushInterval = autoFlushInterval;
            FlushTimeout = flushTimeout;
            MaxEventsInQueue = maxEventsInQueue;
            MaxEventPerRequest = maxEventPerRequest;
            MaxSendEventAttempts = maxSendEventAttempts;
            SendEventRetryInterval = sendEventRetryInterval;

            if(pollingInterval > 0)
            {
                DataSyncMethod = DataSyncMethodEnum.Polling;
                PoollingInterval = pollingInterval;
            }

            LoggerFactory = loggerFactory;
        }

        internal FbOptions ShallowCopy()
        {
            var newOptions = new FbOptions(Offline, EnvSecret, EvalUri, 
                MaxFlushWorker, AutoFlushInterval, FlushTimeout,
                MaxEventsInQueue, MaxEventPerRequest, MaxSendEventAttempts, SendEventRetryInterval,
                LoggerFactory, PoollingInterval);

            return newOptions;
        }
    }
}
