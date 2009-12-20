using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Offr.Users;

namespace Offr.Twitter
{
    public static class XmlTwitterParser
    {
        public static User ParseUser(string userXML)
        {
            //Convert it to a Status object
            StringReader reader = new StringReader(userXML);
            User user = null;
            try
            {
                user = new System.Xml.Serialization.XmlSerializer(typeof(User)).Deserialize(reader) as User;
            }
            catch (Exception)
            {
                // FIXME should do something with these exceptions
                throw;
            }
            return user;
        }

        public static TwitterStatusXml ParseStatus(string statusXML)
        {
            //Convert it to a Status object
            StringReader reader = new StringReader(statusXML);
            TwitterStatusXml status = null;
            try
            {
                status = new System.Xml.Serialization.XmlSerializer(typeof(TwitterStatusXml)).Deserialize(reader) as TwitterStatusXml;
            }
            catch (Exception)
            {
                // FIXME should do something with these exceptions
                throw;
            }
            return status;
        }
    }
}
