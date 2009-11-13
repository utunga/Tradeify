using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Offr.Users
{
    [System.Xml.Serialization.XmlRoot(ElementName = "user", Namespace = "")] 
    public class User
    {
        [System.Xml.Serialization.XmlElement( "created_at" )]
        public string CreatedAt
        {
            get;
            set;
        }
        [System.Xml.Serialization.XmlElement( "description" )]
        public string Description
        {
            get;
            set;
        }

        [System.Xml.Serialization.XmlElement( "favourites_count" )]
        public int FavouritesCount //damn brits
        {
            get;
            set;
        }

        [System.Xml.Serialization.XmlElement( "followers_count" )]
        public int FollowersCount
        {
            get;
            set;
        }

        [System.Xml.Serialization.XmlElement( "following" )]
        public string Following
        {
            get;
            set;
        }

        [System.Xml.Serialization.XmlElement( "friends_count" )]
        public int FriendsCount
        {
            get;
            set;
        }

        [System.Xml.Serialization.XmlElement( "location" )]
        public string Location
        {
            get;
            set;
        }

        [System.Xml.Serialization.XmlElement( "name" )]
        public string Name
        {
            get;
            set;
        }

        [System.Xml.Serialization.XmlElement( "notifications" )]
        public string Notifications
        {
            get;
            set;
        }

        [System.Xml.Serialization.XmlElement( "profile_background_color" )]
        public string ProfileBackgroundColor
        {
            get;
            set;
        }

        [System.Xml.Serialization.XmlElement( "profile_background_image_url" )]
        public string ProfileBackgroundImageUrl
        {
            get;
            set;
        }

        [System.Xml.Serialization.XmlElement( "profile_background_tile" )]
        public bool ProfileBackgroundTile
        {
            get;
            set;
        }

        [System.Xml.Serialization.XmlElement( "profile_image_url" )]
        public string ProfileImageUrl
        {
            get;
            set;
        }

        [System.Xml.Serialization.XmlElement( "profile_link_color" )]
        public string ProfileLinkColor
        {
            get;
            set;
        }

        [System.Xml.Serialization.XmlElement( "profile_sidebar_border_color" )]
        public string ProfileSidebarBorderColor
        {
            get;
            set;
        }

        [System.Xml.Serialization.XmlElement( "profile_sidebar_fill_color" )]
        public string ProfileSidebarFillColor
        {
            get;
            set;
        }

        [System.Xml.Serialization.XmlElement( "profile_text_color" )]
        public string ProfileTextColor
        {
            get;
            set;
        }
        [System.Xml.Serialization.XmlElement("protected")]
        public bool Protected
        {
            get;
            set;
        }

        [System.Xml.Serialization.XmlElement( "screen_name" )]
        public string ScreenName
        {
            get;
            set;
        }

        [System.Xml.Serialization.XmlElement( "statuses_count" )]
        public int StatusesCount
        {
            get;
            set;
        }

        [System.Xml.Serialization.XmlElement( "time_zone" )]
        public string TimeZone
        {
            get;
            set;
        }

        [System.Xml.Serialization.XmlElement( "url" )]
        public string Url
        {
            get;
            set;
        }

        [System.Xml.Serialization.XmlElement( "utc_offset" )]
        public string UtcOffset
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


    }
}