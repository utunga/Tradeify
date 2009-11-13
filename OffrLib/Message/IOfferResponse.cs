using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Offr.Message;
using Offr.Text;

namespace Offr.Message
{
    public interface IOfferResponse : IMessage
    {
        IMessagePointer InRelationTo { get; }
        IUserPointer Seller { get; }
        IUserPointer Recipient { get; }
    }

    public interface IOfferTaken : IOfferResponse
    {
        string ReasonText { get; }
    }

    public interface IOfferFeedback : IOfferResponse
    {
        string FeedbackText { get; }
        double Rating { get; }
    }

    ///Feedback on the recipient, by the seller
    public interface IOfferRecipientFeedback : IOfferFeedback
    {
    }

    ///Feedback on the 'seller' /'giver', by the recipient
    public interface IOfferSellerFeedback : IOfferFeedback
    {
    }
}
