using Snowflake.Configuration;
using Snowflake.Configuration.Attributes;

// autogenerated using generate_retroarch.py
namespace Snowflake.Plugin.Emulators.RetroArch.Configuration.Internal
{
    [ConfigurationSection("log", "Log Options")]
    public interface LogConfiguration : IConfigurationSection<LogConfiguration>
    {
        [ConfigurationOption("log_verbosity", false, DisplayName = "Log Verbosity", Private = true)]
        bool LogVerbosity { get; set; }

        // todo isenum
        [ConfigurationOption("libretro_log_level", 0, DisplayName = "Libretro Log Level", Private = true)]
        int LibretroLogLevel { get; set; }

        [ConfigurationOption("perfcnt_enable", false, DisplayName = "Enable Performance Counter", Private = true)]
        bool PerfcntEnable { get; set; }
    }
}