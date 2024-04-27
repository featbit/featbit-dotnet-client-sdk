using System;
using System.Threading.Tasks;
using FeatBit.ClientSdk.Model;

namespace FeatBit.ClientSdk.DataSynchronizer
{
    public interface IDataSynchronizer : IDisposable
    {
        /// <summary>
        /// Indicates whether the data synchronizer has finished initializing.
        /// </summary>
        public bool Initialized { get; }

        /// <summary>
        /// Starts the data synchronizer. This is called once from the <see cref="FbClient"/> constructor.
        /// </summary>
        /// <returns>a <c>Task</c> which is completed once the data synchronizer has finished starting up</returns>
        Task<bool> StartAsync();
    }
}