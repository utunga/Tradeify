using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Offr
{
    public class BackgroundExceptionReceiver:IBackgroundExceptionReceiver
    {
        public void NotifyException(Exception ex)
        {
            throw new NotImplementedException();
        }

        public Exception LastException
        {
            get { throw new NotImplementedException(); }
        }
    }
}
