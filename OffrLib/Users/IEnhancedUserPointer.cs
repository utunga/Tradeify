using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Offr.Text;

namespace Offr.Users
{
    public interface IEnhancedUserPointer : IUserPointer
    {
        string ProfilePicUrl { get; set; }
        string ScreenName { get; }
        string MoreInfoUrl { get; }
    }
}
