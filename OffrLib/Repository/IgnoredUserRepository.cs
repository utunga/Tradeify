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
    public class IgnoredUserRepository : BaseRepository<IUserPointer>, IIgnoredUserRepository, IPersistedRepository
    {
        public void InitializeFromFile()
        {
            /*
            Save(new OpenSocialUserPointer("ooooby", "just_a_test",
             "http://s3.amazonaws.com/twitter_production/profile_images/228862942/YinD_ContactSheet-003_normal.jpg",
             ""));
             */
            Save(new OpenSocialUserPointer("ooooby", "Dummy","http://s3.amazonaws.com/twitter_production/profile_images/228862942/YinD_ContactSheet-003_normal.jpg",""));
        }

    }
}
