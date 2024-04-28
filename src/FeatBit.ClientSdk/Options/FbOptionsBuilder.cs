using System;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace FeatBit.Sdk.Client.Options
{
    public class FbOptionsBuilder
    {
        private TimeSpan _startWaitTime;

        private readonly string _secret;
        private DataSyncMode _dataSyncMode;
        private Uri _pollingUri;
        private TimeSpan _pollingInterval;
        private Uri _eventUri;
        private ILoggerFactory _loggerFactory;

        public FbOptionsBuilder() : this(string.Empty)
        {
        }

        public FbOptionsBuilder(string secret)
        {
            _startWaitTime = TimeSpan.FromSeconds(5);
            _secret = secret;

            _dataSyncMode = DataSyncMode.Polling;

            _pollingUri = new Uri("http://localhost:5100");
            _pollingInterval = TimeSpan.FromMinutes(5);

            _eventUri = new Uri("http://localhost:5100");

            _loggerFactory = NullLoggerFactory.Instance;
        }

        public FbOptions Build()
        {
            return new FbOptions(_startWaitTime, _secret, _dataSyncMode, _pollingUri, _pollingInterval, _eventUri,
                _loggerFactory);
        }

        public FbOptionsBuilder StartWaitTime(TimeSpan startWaitTime)
        {
            _startWaitTime = startWaitTime;
            return this;
        }

        public FbOptionsBuilder Polling(Uri pollingUri, TimeSpan? pollingInterval = null)
        {
            _dataSyncMode = DataSyncMode.Polling;

            _pollingUri = pollingUri;
            if (pollingInterval.HasValue)
            {
                _pollingInterval = pollingInterval.Value;
            }

            return this;
        }

        public FbOptionsBuilder Event(Uri eventUri)
        {
            _eventUri = eventUri;
            return this;
        }

        public FbOptionsBuilder LoggerFactory(ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory;
            return this;
        }
    }
}