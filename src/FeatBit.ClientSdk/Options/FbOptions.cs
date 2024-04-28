using System;
using FeatBit.Sdk.Client.Model;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace FeatBit.Sdk.Client.Options
{
    public sealed class FbOptions
    {
        /// <summary>
        /// How long the client constructor will block awaiting a successful connection to FeatBit.
        /// </summary>
        /// <value>Defaults to 5 seconds</value>
        public TimeSpan StartWaitTime { get; set; }

        /// <summary>
        /// Whether or not this client is offline. If true, no calls to FeatBit will be made.
        /// </summary>
        /// <value>Defaults to <c>false</c></value>
        public bool Offline { get; set; }

        /// <summary>
        /// Feature flags to be used as initial data before the first data synchronization with FeatBit.
        /// </summary>
        /// <value>Defaults to an empty array.</value>
        public FeatureFlag[] Bootstrap { get; set; }

        /// <summary>
        /// The SDK secret for your FeatBit environment.
        /// </summary>
        public string Secret { get; set; }

        /// <summary>
        /// The data sync mode for the SDK.
        /// </summary>
        /// <value>Defaults to <see cref="DataSyncMode.Polling"/>.</value>
        public DataSyncMode DataSyncMode { get; set; }

        /// <summary>
        /// The base URI of the polling service
        /// </summary>
        /// <value>Defaults to http://localhost:5100.</value>
        public Uri PollingUri { get; set; }

        /// <summary>
        /// The polling interval in milliseconds.
        /// </summary>
        /// <value>Defaults to 0.</value>
        public TimeSpan PollingInterval { get; set; }

        /// <summary>
        /// The base URI of the event service
        /// </summary>
        /// <value>Defaults to http://localhost:5100.</value>
        public Uri EventUri { get; set; }

        /// <summary>
        /// The logger factory used by FbClient.
        /// </summary>
        /// <value>Defaults to <see cref="NullLoggerFactory.Instance"/></value>
        public ILoggerFactory LoggerFactory { get; set; }

        internal FbOptions(
            TimeSpan startWaitTime,
            bool offline,
            FeatureFlag[] bootstrap,
            string secret,
            DataSyncMode dataSyncMode,
            Uri pollingUri,
            TimeSpan pollingInterval,
            Uri eventUri,
            ILoggerFactory loggerFactory)
        {
            StartWaitTime = startWaitTime;
            Offline = offline;
            Bootstrap = bootstrap;
            Secret = secret;

            DataSyncMode = dataSyncMode;

            PollingUri = pollingUri;
            PollingInterval = pollingInterval;

            EventUri = eventUri;

            LoggerFactory = loggerFactory;
        }
    }
}