﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Snowflake.Configuration;
using Snowflake.Configuration.Attributes;

namespace Snowflake.Plugin.Emulators.RetroArch.Adapters.Higan.Configuration
{
    [ConfigurationFile("#coreoptions", "retroarch-core-options.cfg", "enabled", "disabled")]
    public interface HiganRetroArchConfiguration : RetroArchConfiguration, IConfigurationCollection<HiganRetroArchConfiguration>
    {
        [SerializableSection("#coreoptions")]
        BsnesCoreConfiguration BsnesCoreConfig { get; set; }
    }
}
