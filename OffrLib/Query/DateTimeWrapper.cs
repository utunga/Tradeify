using System;
using System.Collections.Generic;
using System.Linq;
using Offr.Message;

public class MessageListWrapper
{

    private List<IMessage> IgnoredMessages;
    private bool ignoredSortedFlag=false;
    private List<IMessage> MessagesWithIgnored=null;
    private bool messageSortedFlag=false;
    private List<IMessage> MessagesWithoutIgnored;

    public MessageListWrapper()
    {
        //IgnoredMessages
        MessagesWithoutIgnored = new List<IMessage>();
        IgnoredMessages = new List<IMessage>();

    }
    public List<IMessage> GetIgnoredMessages()
    {
        if (!ignoredSortedFlag)
        {
            IgnoredMessages.Sort((a, b) => b.Timestamp.CompareTo(a.Timestamp));
            ignoredSortedFlag = true;
        }
        return IgnoredMessages;    
    }
    public List<IMessage> GetMessagesWithoutIgnored()
    {
        if (!messageSortedFlag)
        {
            MessagesWithoutIgnored.Sort((a, b) => b.Timestamp.CompareTo(a.Timestamp));
            messageSortedFlag = true;
        }
        return MessagesWithoutIgnored;
    }
    public List<IMessage> GetMessagesWithIgnored()
    {
        if (MessagesWithIgnored == null)
        {
            MessagesWithIgnored = MessagesWithoutIgnored.Concat(IgnoredMessages).ToList();
            MessagesWithIgnored.Sort((a, b) => b.Timestamp.CompareTo(a.Timestamp));
        }
        return MessagesWithoutIgnored;

    }
    public void AddNonIgnored(IMessage message)
    {
        AddMessage(MessagesWithoutIgnored, message);
    }
    public void AddIgnored(IMessage message)
    {
        AddMessage(IgnoredMessages, message);
    }
    private void AddMessage(List<IMessage> list, IMessage message)
    {
        list.Add(message);
    }
}
    /*
     * Not working version that would mean you would only need to sort once for both lists
    private SortedList<DateTimeWrapper, IMessage> IgnoredMessages;
    private List<IMessage> MessagesWithIgnored;
    private SortedList<DateTimeWrapper, IMessage> MessagesWithoutIgnored;


    public MessageListWrapper()
    {
        //IgnoredMessages
        MessagesWithoutIgnored = new SortedList<DateTimeWrapper, IMessage>();
        IgnoredMessages = new SortedList<DateTimeWrapper, IMessage>();

    }

    private struct DateTimeWrapper : IComparable<DateTimeWrapper>
    {
        public DateTime TimeStamp;
        //sort descending
        public int CompareTo(DateTimeWrapper obj)
        {
            return obj.CompareTo(this);
        }
    }
    public void AddNonIgnored(IMessage message)
    {
        AddMessage(MessagesWithoutIgnored, message);
    }
    public void AddIgnored(IMessage message)
    {
        AddMessage(IgnoredMessages, message);
    }
    private void AddMessage(SortedList<DateTimeWrapper, IMessage> list, IMessage message)
    {
        DateTimeWrapper d = new DateTimeWrapper();
        d.TimeStamp = message.Timestamp;
        list[d] = message;
    }
    public IList<IMessage> GetIgnoredMessages()
    {
        return IgnoredMessages.Values.;
    }
    public IList<IMessage> GetMessagesWithoutIgnored()
    {
        return IgnoredMessages.Values;
    }
    
    public List<IMessage> getMessagesWithIgnored()
    {
        if (MessagesWithIgnored == null)
        {
            MessagesWithIgnored = new List<IMessage>();
            IList<DateTimeWrapper> ignoredValues = IgnoredMessages.Keys;
            int ignoredIdx = 0;
            IList<DateTimeWrapper> messageValues = MessagesWithoutIgnored.Keys;
            int messageIdx = 0;
            while (ignoredIdx < IgnoredMessages.Count && messageIdx < MessagesWithoutIgnored.Count)
            {
                DateTimeWrapper ignored = ignoredValues[ignoredIdx];
                DateTimeWrapper message = messageValues[messageIdx];
                int compare = ignored.CompareTo(message);
                if (compare >= 1)
                {
                    MessagesWithIgnored.Add(IgnoredMessages[ignored]);
                    ignoredIdx++;
                }
                else
                {
                    MessagesWithIgnored.Add(MessagesWithoutIgnored[message]);
                    messageIdx++;
                }
            }
            if (ignoredIdx < ignoredValues.Count)
                ConcatLists(IgnoredMessages, ignoredValues, ignoredIdx);
            else ConcatLists(MessagesWithoutIgnored, messageValues, messageIdx);

        }
        return MessagesWithIgnored;
    }
    private void ConcatLists(SortedList<DateTimeWrapper, IMessage> orig, IList<DateTimeWrapper> other, int otherIdx)
    {
        while (otherIdx < other.Count)
        {
            MessagesWithIgnored.Add(orig[other[otherIdx]]);
            otherIdx++;
        }
    }
   
}
  */