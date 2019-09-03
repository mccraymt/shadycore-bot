using System;
using System.Collections.Generic;
namespace ShadyBot.Helpers
{
    public static class Common
    {
        public static readonly List<string> BugTypes = new List<string>()
        {
            "Security", "Crash", "Power",  "Performances", "Usability", "Serious Bug", "Other"
        };
    }
}
