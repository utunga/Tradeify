using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Offr
{
    public interface IBackgroundExceptionReceiver
    {
        /// <summary>
        /// Background threads call this to notify 'someone' that they had an exception
        /// </summary>
        void NotifyException(Exception ex);

        /// <summary>
        /// Last exception notified to us
        /// </summary>
        Exception LastException
        {
            get;
        }
    }
}
