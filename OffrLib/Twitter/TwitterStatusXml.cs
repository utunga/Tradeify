using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Offr.Users;

namespace Offr.Twitter
{
    [System.Xml.Serialization.XmlRoot(ElementName="status", Namespace = "")]
    public class TwitterStatusXml
    {

        [System.Xml.Serialization.XmlElement( "favorited" )]
        public bool Favorited
        {
            get;
            set;
        }

        [System.Xml.Serialization.XmlElement( "id" )]
        public long ID
        {
            get;
            set;
        }

        [System.Xml.Serialization.XmlElement( "in_reply_to_screen_name" )]
        public string InReplyToScreenName
        {
            get;
            set;
        }

        [System.Xml.Serialization.XmlElement( "in_reply_to_status_id" )]
        public string InReplyToStatusID
        {
            get;
            set;
        }

        [System.Xml.Serialization.XmlElement( "in_reply_to_user_id" )]
        public string InReplyToUserID
        {
            get;
            set;
        }

        [System.Xml.Serialization.XmlElement( "source" )]
        public string Source
        {
            get;
            set;
        }

        [System.Xml.Serialization.XmlElement( "text" )]
        public string Text
        {
            get;
            set;
        }

        [System.Xml.Serialization.XmlElement( "truncated" )]
        public bool Truncated
        {
            get;
            set;
        }

        [System.Xml.Serialization.XmlElement( "created_at" )]
        public string CreatedAt
        {
            get;
            set;
        }

        [System.Xml.Serialization.XmlElement( "user" )]
        public User User
        {
            get;
            set;
        }

    }
}