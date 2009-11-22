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

namespace Offr.Repository
{
    public class MessageRepository : BaseRepository<IMessage>, IMessageRepository
    {

        public MessageRepository()
        {
        }

        public IEnumerable<IMessage> AllMessages()
        {
            return base.GetAll();
        }

        /// <summary>
        /// Loads data from specified JSON file to initialise context to have some running data (good for rebooting webserver without losing recent context, for example)
        /// </summary>
        public void InitializeFromFile(string jsonFilePath)
        {

            FileInfo jsonFile;
            if (!(jsonFile = new FileInfo(jsonFilePath)).Exists)
            {
                throw new IOException("Cannot find file " + jsonFilePath);
            }

            // OK read into a string builder (probably a better way to do this but this will do for now)
            StringBuilder stringBuilder = new StringBuilder();
            using (StreamReader sr = new StreamReader(jsonFile.FullName))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    stringBuilder.AppendLine(line);
                }
            }
            SortedList<string, IMessage> list = JSON.Deserialize<SortedList<string, IMessage>>(stringBuilder.ToString());
            foreach (string key in list.Keys)
            {
                base.Save(list[key]);
            }
        }
    }
}