﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Offr.Text
{
    public interface ITagProvider
    {
        ITag GetTag(String tagString);
    }
}
