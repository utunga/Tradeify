using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Web.Script.Serialization;
using Ninject.Core;
using Offr.Json;
using Offr.Message;

namespace Offr
{
    public static class Global
    {
        static readonly object[] _syncLock = new object[0];
        static IKernel _ninjectKernel;
      
        public static void Initialize(IModule configuration)
        {
            lock (_syncLock)
            {
                if (_ninjectKernel == null)
                {
                    _ninjectKernel = new StandardKernel(configuration);
                    //FIXME log that it was already initialized
                }
            }
        }

        public static IKernel Kernel
        {
            get 
            {
                if (_ninjectKernel == null)
                {
                    InitializeDefaultConfig();
                }
                return _ninjectKernel;
            }
        }

        private static void InitializeDefaultConfig()
        {
            Initialize(new DefaultNinjectConfig());
        }

        /// <summary>
        /// Loads data from specified JSON file to initialise context to have some running data (good for rebooting webserver without losing recent context, for example)
        /// </summary>
        public static void InitializeWithRecentOffers(string jsonOffersFilePath)
        {
            FileInfo jsonFile;
            if (!(jsonFile =new FileInfo(jsonOffersFilePath)).Exists)
            {
                throw new IOException("Cannot find file " + jsonOffersFilePath);
            }
            // OK read into a string builder (probably a better way)

            StringBuilder stringBuilder = new StringBuilder();
            using (StreamReader sr = new StreamReader(jsonFile.FullName)) 
            {
                string line;
                while ((line = sr.ReadLine()) != null) 
                {
                    stringBuilder.AppendLine(line);// im sure there is an even tighter way to do this, just don't know what it is
                }
            }

            //serialize into memory
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            serializer.RegisterConverters(new JavaScriptConverter[] { new MessageListSerializer() });
            List<IMessage> initialMessages = serializer.Deserialize<List<IMessage>>(stringBuilder.ToString());
            
            // 'notify' them straight into the MessageProvider (by passing the RawMessage stage)
            Global.Kernel.Get<IMessageProvider>().Notify(initialMessages);           
        }
    }
}
