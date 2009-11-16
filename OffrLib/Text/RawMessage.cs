using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Offr.Json.Converter;
using Offr.Message;
using Offr.Twitter;

namespace Offr.Text
{
    public class RawMessage : IRawMessage
    {

        private IMessagePointer _messagePointer;
        private readonly IUserPointer _createdBy;
        private readonly string _sourceText;
        private DateTime _timeStampUTC;
        [JsonConverter(typeof(IMessagePointerConverter))]
        public IMessagePointer Pointer
        {
            get { return _messagePointer; }
            set { _messagePointer = value; }
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
            // Substring(0, "Fri, 15 May".Length)
            get { return DateUtils.FriendlyLocalTimeStampFromUTC(_timeStampUTC); }
        }

      
        internal void SetTimestamp(string rfc822TimeStamp)
        {
            _timeStampUTC = DateUtils.UTCDateTimeFromTwitterTimeStamp(rfc822TimeStamp);
        }

        public RawMessage()
        {
        }

        public RawMessage(string sourceText, IMessagePointer messagePointer, IUserPointer createdBy, string timestamp)
        {
            _sourceText = sourceText;
            _messagePointer = messagePointer;
            _createdBy = createdBy;
            SetTimestamp(timestamp);
        }

        public static RawMessage From(TwitterStatus status)
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

        #region Implementation of IComparable<IRawMessage>

        public int CompareTo(IRawMessage otherIRawMessage)
        {
            if (otherIRawMessage is RawMessage)
            {
                RawMessage other = (RawMessage) otherIRawMessage;
                return this._timeStampUTC.CompareTo(other._timeStampUTC);
            }
            else
            {
                throw new NotSupportedException("Don't know how to compare a RawMessage and a " + otherIRawMessage.GetType() );
            }
        }

        #endregion
    }
}
