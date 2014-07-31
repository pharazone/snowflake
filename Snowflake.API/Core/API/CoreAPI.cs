﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Snowflake.Information.Platform;
using Snowflake.Information.Game;
namespace Snowflake.Core.API
{
    public static class CoreAPI
    {
        public static IDictionary<string, Platform> GetAllPlatforms()
        {
            return FrontendCore.LoadedCore.LoadedPlatforms;
        }

        public static IList<Game> GetGamesByPlatform(string platformID)
        {
            return FrontendCore.LoadedCore.GameDatabase.GetGamesByPlatform(platformID);
        }
    }
}
