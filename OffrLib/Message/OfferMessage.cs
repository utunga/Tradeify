using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Offr.Json;
using Offr.Json.Converter;
using Offr.Location;
using Offr.Text;

namespace Offr.Message
{
    public class OfferMessage : BaseMarketMessage, IOfferMessage
    {
        #region fields

        #endregion fields

        #region read only properties
        public override MessageType type
        {
            get { return Message.MessageType.offer; }
        }
        #endregion

        public OfferMessage()
            : base()
        {
        }

        #region overrides of abstract base class

        #endregion
        /// <summary>
        /// Set end by - both params must be supplied at same time.
        /// No attempt will be made to parse the 'end by' text
        /// FIXME: ideally these set methods would be internal, not public
        /// </summary>
        /// 
        /// 
        #region Setter Methods

        #endregion Setter Methods

        #region Equals

        #endregion Equals

        #region JSON

        #endregion


    }
}
