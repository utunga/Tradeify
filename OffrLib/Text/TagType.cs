using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Offr.Text
{
    //tag = #[something]
    //type = the currency or type of exchange supported (#free, #NZD, #cash_only etc)
    //loc = a tag depicting location if derived from "l:wellington road, paekakariki:" (or matches a previosuly encounterd location tag, maybe, later)
    //group = a shared tag for a group of people that want to share information (eg #ooooby)
    //msg_type = a tag identifying how the message should be parsed (eg "#offr")
    //text = word from the message (not used currently)
    
    public enum TagType { text, tag, type, loc, group, msg_type}
}