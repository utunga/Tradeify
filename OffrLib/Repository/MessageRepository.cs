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
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            //serializer.RegisterConverters(new JavaScriptConverter[] { new MessageListSerializer() });
            List<IMessage> _list = JSONConverter.Deserialize<List<IMessage>>(stringBuilder.ToString(), new IMessageConverter(), new ITagConverter());//= JsonConvert.DeserializeObject<List<IMessage>>(stringBuilder.ToString(),new IMessageConverter()/*,new ILocationConverter()*/,new ITagConverter()/*,new IUserPointerConverter(), new IMessagePointerConverter()*/);
            foreach (IMessage list in _list)
            {
                base.Save(list);
            }
            Console.WriteLine();
            //serializer.Deserialize<List<IMessage>>(stringBuilder.ToString());
            // 'notify' them straight into the MessageProvider (by passing the RawMessage stage)
            Global.Kernel.Get<IMessageProvider>().Notify(_list);

        }

        public IMessage Get(string id)
        {
            return base.Get(id);
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
