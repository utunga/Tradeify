using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Offr.Text;

namespace Offr.Message
{
    public interface IMessage : IComparable, Offr.Repository.ITopic
    {
        IRawMessage Source { get; }
        DateTime TimeStamp { get; }
        IUserPointer CreatedBy { get; }
        MessageType MessageType { get; }
        bool IsValid { get; }
        IList<ITag> Tags { get; }
        bool HasTag(ITag tag);
        ReadOnlyCollection<ITag> HashTags { get; }
        ReadOnlyCollection<ITag> CommunityTags { get; }
    }
}