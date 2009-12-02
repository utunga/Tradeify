using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Offr
{
    public class BackgroundExceptionReceiver:IBackgroundExceptionReceiver
    {
        private Exception _lastException = null;
        public void NotifyException(Exception ex)
        {
            _lastException = ex;
        }

        public Exception LastException
        {
            get { return _lastException; }
        }
    }
}
