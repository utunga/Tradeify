using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Offr.Text;

namespace Offr.Twitter
{
    public class TwitterRawMessage : RawMessage, IRawMessage
    {
        public TwitterRawMessage(TwitterStatus status)
        {
            base.Pointer = new TwitterMessagePointer(status.id);
            base.CreatedBy = new TwitterUserPointer(status.from_user, status.profile_image_url);
            base.Text = status.text;
            base.Timestamp = DateUtils.UTCDateTimeFromTwitterTimeStamp(status.created_at); 
        }

        // FIXME see http://code.google.com/p/twitter-api/issues/detail?id=214 
        // for why this is not going to work
        public TwitterRawMessage(TwitterStatusXml xmlStatus)
        {
            base.Pointer = new TwitterMessagePointer(xmlStatus.ID);
            base.CreatedBy = new TwitterUserPointer(xmlStatus.User.ScreenName, xmlStatus.User.ProfileImageUrl);
            base.Text = xmlStatus.Text;
            base.Timestamp = DateUtils.UTCDateTimeFromTwitterTimeStamp(xmlStatus.CreatedAt);        
        }
    }
}
