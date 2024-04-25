using System;
using Microsoft.Extensions.Logging;

namespace FeatBit.ClientSdk.Options
{
    public sealed class FbOptions
    {
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
        /// <value>Defaults to <see cref="NullLoggerFactory.Instance"/>.</value>
        public ILoggerFactory LoggerFactory { get; set; }

        internal FbOptions(
            string secret,
            DataSyncMode dataSyncMode,
            Uri pollingUri,
            TimeSpan pollingInterval,
            Uri eventUri,
            ILoggerFactory loggerFactory)
        {
            Secret = secret;

            DataSyncMode = dataSyncMode;

            PollingUri = pollingUri;
            PollingInterval = pollingInterval;

            EventUri = eventUri;

            LoggerFactory = loggerFactory;
        }

        internal FbOptions ShallowCopy()
        {
            var newOptions = new FbOptions(Secret, DataSyncMode, PollingUri, PollingInterval, EventUri, LoggerFactory);

            return newOptions;
        }
    }
}