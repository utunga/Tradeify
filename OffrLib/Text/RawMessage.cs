using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Offr.Json;
using Offr.Json.Converter;
using Offr.Message;
using Offr.Twitter;

namespace Offr.Text
{
    public class RawMessage : IRawMessage , IEquatable<RawMessage>
    {

        private IUserPointer _createdBy;
        private IMessagePointer _messagePointer;
        private string _sourceText;
        
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

        private DateTime _timeStampUTC;
        public DateTime Timestamp
        {
            get { return _timeStampUTC;  }
        }

        internal void SetTimestamp(string rfc822TimeStamp)
        {
            _timeStampUTC = DateUtils.UTCDateTimeFromTwitterTimeStamp(rfc822TimeStamp);
        }

        internal RawMessage()
        {
        }

        public RawMessage(string sourceText, IMessagePointer messagePointer, IUserPointer createdBy, string timestamp)
        {
            _sourceText = sourceText;
            _messagePointer = messagePointer;
            _createdBy = createdBy;
            SetTimestamp(timestamp);
        }

        public RawMessage(string sourceText, IMessagePointer messagePointer, IUserPointer createdBy, DateTime dateTimeUTC)
        {
            _sourceText = sourceText;
            _messagePointer = messagePointer;
            _createdBy = createdBy;
            _timeStampUTC = dateTimeUTC;
        }

        public bool Equals(RawMessage other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return  Equals(other._messagePointer, _messagePointer) && 
                    Equals(other._createdBy, _createdBy) && 
                    Equals(other._sourceText, _sourceText) && 
                    other._timeStampUTC.Equals(_timeStampUTC);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (RawMessage)) return false;
            return Equals((RawMessage) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int result = (_messagePointer != null ? _messagePointer.GetHashCode() : 0);
                result = (result*397) ^ (_createdBy != null ? _createdBy.GetHashCode() : 0);
                result = (result*397) ^ (_sourceText != null ? _sourceText.GetHashCode() : 0);
                result = (result*397) ^ _timeStampUTC.GetHashCode();
                return result;
            }
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
        
        public static IRawMessage From(string rawText, string id, string screenName, string thumbnail)
        {
            OpenSocialUserPointer userPointer = new OpenSocialUserPointer(screenName);
            userPointer.ProfilePicUrl = thumbnail;
            OpenSocialMessagePointer messagePointer = new OpenSocialMessagePointer(id);
            RawMessage msg= new RawMessage(rawText,messagePointer,userPointer,DateTime.Now);
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
        #region JSON
        public void WriteJson(JsonWriter writer, JsonSerializer serializer)
        {
            JSON.WriteProperty(serializer, writer, "pointer", Pointer);
            JSON.WriteProperty(serializer, writer, "created_by", CreatedBy);
            JSON.WriteProperty(serializer, writer, "text", _sourceText);
            JSON.WriteProperty(serializer, writer, "timestamp_utc", _timeStampUTC);
        }

        public void ReadJson(JsonReader reader, JsonSerializer serializer)
        {
            Pointer=JSON.ReadProperty<TwitterMessagePointer>(serializer, reader, "pointer");
            _createdBy=JSON.ReadProperty<TwitterUserPointer>(serializer, reader, "created_by");
            _sourceText=JSON.ReadProperty<string>(serializer, reader, "text");
            _timeStampUTC = JSON.ReadProperty<DateTime>(serializer, reader, "timestamp_utc");
        }
        #endregion JSON


    }

 
}
