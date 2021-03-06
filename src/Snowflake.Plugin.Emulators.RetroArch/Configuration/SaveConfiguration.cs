using Snowflake.Configuration;
using Snowflake.Configuration.Attributes;

// autogenerated using generate_retroarch.py
namespace Snowflake.Plugin.Emulators.RetroArch.Configuration
{
    [ConfigurationSection("save", "Save Options")]
    public interface SaveConfiguration : IConfigurationSection<SaveConfiguration>
    {
        [ConfigurationOption("autosave_interval", 10, DisplayName = "Autosave Interval (seconds)", Increment = 10)]
        int AutosaveInterval { get; set; }

        [ConfigurationOption("block_sram_overwrite", false, DisplayName = "Block SRAM Overwrite when loading savegames")
        ]
        bool BlockSramOverwrite { get; set; }

        [ConfigurationOption("savestate_auto_index", false, DisplayName = "Automatically increment savestate index")]
        bool SavestateAutoIndex { get; set; }

        [ConfigurationOption("savestate_auto_load", false, DisplayName = "Load state on start")]
        bool SavestateAutoLoad { get; set; }

        [ConfigurationOption("savestate_auto_save", false, DisplayName = "Auto-save state on close")]
        bool SavestateAutoSave { get; set; }

        [ConfigurationOption("~flag_quick_resume", false, DisplayName = "Enable quick resume", Simple = true,
            Flag = true,
            Description =
                "Restores your game state from where you left off. Overrides savestate_auto_save and savestate_auto_load")]
        bool FlagQuickResume { get; set; }

        [ConfigurationOption("state_slot", 0, DisplayName = "Default save state slot")]
        int StateSlot { get; set; }
    }
}