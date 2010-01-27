using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Offr.Demo;
using Offr.Text;

namespace Offr.Repository
{
    public class IgnoredUserRepository : BaseRepository<IUserPointer>, IUserPointerRepository, IPersistedRepository
    {
        public IgnoredUserRepository()
        {
            //for now
            base.Save(new OpenSocialUserPointer("ooooby", "Dummy","http://s3.amazonaws.com/twitter_production/profile_images/228862942/YinD_ContactSheet-003_normal.jpg",""));
        }

    }
}
