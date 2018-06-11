﻿using System;

namespace SlackAPI
{
    [Flags]
    public enum SlackScope
    {
        Identify = 1,
        Read = 2,
        Post = 4,
        Client = 8,
        Admin = 16,
    }
}
