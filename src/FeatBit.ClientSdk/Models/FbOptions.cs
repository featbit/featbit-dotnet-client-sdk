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
        public Uri EventUri { get; set; }

        /// <summary>
        /// The connection timeout. This is the time allowed for the WebSocket client to connect to the server.
        /// </summary>
        /// <remarks>
        /// This value must lower then <see cref="StartWaitTime"/>.
        /// </remarks>
        /// <value>Defaults to 3 seconds</value>
        public TimeSpan ConnectTimeout { get; set; }

        /// <summary>
        /// The close timeout. This is the time allowed for the WebSocket client to perform a graceful shutdown.
        /// </summary>
        /// <value>Defaults to 2 seconds</value>
        public TimeSpan CloseTimeout { get; set; }

        /// <summary>
        /// The frequency at which to send Ping message.
        /// </summary>
        /// <value>Defaults to 15 seconds</value>
        public TimeSpan KeepAliveInterval { get; set; }

        /// <summary>
        /// The connection retry delays.
        /// </summary>
        public TimeSpan[] ReconnectRetryDelays { get; set; }

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

        public int PoollingInterval { get; set; }

        internal FbOptions(
            TimeSpan startWaitTime,
            bool offline,
            string envSecret,
            Uri streamingUri,
            Uri eventUri,
            TimeSpan connectTimeout,
            TimeSpan closeTimeout,
            TimeSpan keepAliveInterval,
            TimeSpan[] reconnectRetryDelays,
            int maxFlushWorker,
            TimeSpan autoFlushInterval,
            TimeSpan flushTimeout,
            int maxEventsInQueue,
            int maxEventPerRequest,
            int maxSendEventAttempts,
            TimeSpan sendEventRetryInterval,
            ILoggerFactory loggerFactory,
            DataSyncMethodEnum dataSyncMethod = DataSyncMethodEnum.Polling,
            int pollingInterval = 1000)
        {
            StartWaitTime = startWaitTime;
            Offline = offline;

            EnvSecret = envSecret;
            StreamingUri = streamingUri;
            EventUri = eventUri;
            ConnectTimeout = connectTimeout;
            CloseTimeout = closeTimeout;
            KeepAliveInterval = keepAliveInterval;
            ReconnectRetryDelays = reconnectRetryDelays;

            MaxFlushWorker = maxFlushWorker;
            AutoFlushInterval = autoFlushInterval;
            FlushTimeout = flushTimeout;
            MaxEventsInQueue = maxEventsInQueue;
            MaxEventPerRequest = maxEventPerRequest;
            MaxSendEventAttempts = maxSendEventAttempts;
            SendEventRetryInterval = sendEventRetryInterval;

            DataSyncMethod = dataSyncMethod;
            PoollingInterval = pollingInterval;

            LoggerFactory = loggerFactory;
        }

        internal FbOptions ShallowCopy()
        {
            var newOptions = new FbOptions(StartWaitTime, Offline, EnvSecret, StreamingUri, EventUri, ConnectTimeout,
                CloseTimeout, KeepAliveInterval, ReconnectRetryDelays, MaxFlushWorker, AutoFlushInterval, FlushTimeout,
                MaxEventsInQueue, MaxEventPerRequest, MaxSendEventAttempts, SendEventRetryInterval,
                LoggerFactory, DataSyncMethod, PoollingInterval);

            return newOptions;
        }
    }
}
