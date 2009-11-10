using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;
using Offr.Message;
using Offr.Repository;

namespace Offr
{
    public class MessageRepository : BaseRepository<IMessage>, IMessageRepository
    {

        public MessageRepository()
        {
            Initialize();
        }

        //FIXME wtf?
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
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            //serializer.RegisterConverters(new JavaScriptConverter[] { new MessageListSerializer() });
            List<IMessage> initialMessages = serializer.Deserialize<List<IMessage>>(stringBuilder.ToString());
            // 'notify' them straight into the MessageProvider (by passing the RawMessage stage)
            Global.Kernel.Get<IMessageProvider>().Notify(initialMessages);

        }

        public IMessage Get(string id)
        {
            return base.Get(id) as IMessage;
        }

        public void Save(IMessage instance)
        {
            base.Save(instance);
        }

        public void Remove(IMessage instance)
        {
            base.Remove(instance);
        }
    }
}
