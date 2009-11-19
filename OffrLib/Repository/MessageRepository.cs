using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Offr.Json;
using Offr.Location;
using Offr.Message;
using Offr.Repository;
using Offr.Json.Converter;
using Offr.Text;
using Offr.Twitter;

namespace Offr
{
    public class MessageRepository : BaseRepository<IMessage>, IMessageRepository
    {

        public MessageRepository()
        {
            Initialize();
        }

        public static string InitializeMessagesFilePath
        {
            get;
            set;
        }

        /// <summary>
        /// Loads data from specified JSON file to initialise context to have some running data (good for rebooting webserver without losing recent context, for example)
        /// </summary>
        public void Initialize()
        {

            FileInfo jsonFile;
            if (!(jsonFile = new FileInfo(InitializeMessagesFilePath)).Exists)
            {
                throw new IOException("Cannot find file " + InitializeMessagesFilePath);
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
            //serializer.RegisterConverters(new JavaScriptConverter[] { new MessageListSerializer() });
            List<IMessage> list = JSON.Deserialize<List<IMessage>>(stringBuilder.ToString());
            foreach (IMessage msg in list)
            {
                base.Save(msg);
            }
            Console.WriteLine();
            //serializer.Deserialize<List<IMessage>>(stringBuilder.ToString());
            // 'notify' them straight into the MessageProvider (by passing the RawMessage stage)
            Global.Kernel.Get<IMessageProvider>().Notify(list);

        }
    }
}
