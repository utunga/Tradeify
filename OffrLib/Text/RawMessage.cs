using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Offr.Message;
using Offr.Twitter;

namespace Offr.Text
{
    public class RawMessage : IRawMessage
    {
        private readonly IMessagePointer _messagePointer;
        private readonly IUserPointer _createdBy;
        private readonly string _sourceText;
        private  string _timestamp;

        public IMessagePointer Pointer
        {
            get { return _messagePointer; }
        }

        public IUserPointer CreatedBy
        {
            get { return _createdBy; }
        }

        public string Text
        {
            get { return _sourceText; }
        }

        public string Timestamp
        {
            get { return _timestamp; }
        }

        internal void SetTimestamp(string timestamp)
        {
            _timestamp = timestamp;
        }

        public RawMessage(string sourceText, IMessagePointer messagePointer, IUserPointer createdBy, string timestamp)
        {
            _sourceText = sourceText;
            _messagePointer = messagePointer;
            _createdBy = createdBy;
            _timestamp = timestamp;
        }

        public static RawMessage From(JSONStatus status)
        {
            TwitterMessagePointer msgPointer = new TwitterMessagePointer(status.id);
            TwitterUserPointer createdBy = new TwitterUserPointer(status.from_user);
            createdBy.ProfilePicUrl = status.profile_image_url;
            createdBy.ScreenName = status.from_user;

            RawMessage msg = new RawMessage(status.text, msgPointer, createdBy, status.created_at );
            return msg;
        }

        public static IRawMessage From(Status status)
        {
            TwitterMessagePointer msgPointer = new TwitterMessagePointer(status.ID);
            // FIXME xml feed user_id actually doesn't equal the other!
            // see http://code.google.com/p/twitter-api/issues/detail?id=214 for why this is not going to work
            TwitterUserPointer createdBy = new TwitterUserPointer(status.User.ScreenName); 
            RawMessage msg = new RawMessage(status.Text, msgPointer, createdBy, status.CreatedAt);
            return msg;
        }
    }
}
