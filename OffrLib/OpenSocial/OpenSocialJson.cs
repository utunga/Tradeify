using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Offr.OpenSocial
{
    #region examplejson
    /*
     {

    * Message: "b;a"
    *
      -
      User: {
          o
            -
            fields_: {
                +
                  -
                  photos: [
                      #
                        -
                        {
                            * linkText: "Joav"
                            * primary: true
                            * value: http://api.ning.com:80/files/ojJR0x7XjgKkCg6JW0BbUO1R3DYiSYEwoD49ysquHWI_/455779645.png?crop=1%3A1
                            * type: "thumbnail"
                        }
                  ]
                + id: "0asph7yumi8p0"
                + ning.admin: true
                + ning.creator: true
                + profileUrl: http://tradeify.ning.com/profile/Joav
                + isViewer: true
                +
                  -
                  urls: [
                      #
                        -
                        {
                            *
                              -
                              fields_: {
                                  o linkText: "View Joav's page on tradeify"
                                  o primary: true
                                  o value: http://tradeify.ning.com/profile/Joav
                                  o type: "profile"
                                  o address: http://tradeify.ning.com/profile/Joav
                              }
                        }
                  ]
                + thumbnailUrl: http://api.ning.com:80/files/ojJR0x7XjgKkCg6JW0BbUO1R3DYiSYEwoD49ysquHWI_/455779645.png?crop=1%3A1
                +
                  -
                  name: {
                      #
                        -
                        fields_: {
                            * formatted: "Joav"
                            * unstructured: "Joav"
                        }
                  }
                + isOwner: true
                + displayName: "Joav"
            }
          o isOwner_: true
          o isViewer_: true
      }

}*/
    #endregion examplejson
    public class OpenSocialJson
    {
        public string Message{get;set;}
        public UserType User { get; set; }
        public class UserType
        {
            public FieldType fields_ { get; set; }
            public class FieldType
            {
                public string displayName { get; set; }
                public string thumbnailUrl { get; set; }
            }
        }
    }
}
