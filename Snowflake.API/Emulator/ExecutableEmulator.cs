﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Snowflake.Plugin.Interface;
using Snowflake.Plugin;
using System.Reflection;

namespace Snowflake.Plugin.Emulator
{
    public abstract class ExecutableEmulator : BasePlugin, IEmulator
    {
        public ExecutableEmulator(Assembly pluginAssembly):base(pluginAssembly)
        {
        }
        public abstract void Run(string uuid);


   }
}

