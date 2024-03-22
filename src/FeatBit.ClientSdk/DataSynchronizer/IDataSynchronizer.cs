using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FeatBit.ClientSdk
{
    public interface IDataSynchronizer
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

        /// <summary>
        /// Stop the data synchronizer and dispose all resources.
        /// </summary>
        /// <returns>The <c>Task</c></returns>
        Task StopAsync();
    }
}
