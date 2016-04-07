using DevelopmentUtilities.Utilities;
using Spectrum.API;
using Spectrum.API.Configuration;
using Spectrum.API.FileSystem;
using Spectrum.API.Interfaces.Plugins;
using Spectrum.API.Interfaces.Systems;

namespace DevelopmentUtilities
{
    public class Entry : IPlugin
    {
        public string FriendlyName => "Development Utilities";
        public string Author => "Ciastex";
        public string Contact => "ciastexx@live.com";
        public APILevel CompatibleAPILevel => APILevel.RadioWave;

        private PluginData PluginData { get; set; }
        private Settings Settings { get; set; }

        private SceneDumper SceneDumper { get; set; }

        public void Initialize(IManager manager)
        {
            Settings = new Settings(typeof(Entry));
            ValidateSettings();

            PluginData = new PluginData(typeof(Entry));

            SceneDumper = new SceneDumper(PluginData);

            manager.Hotkeys.Bind(Settings["SceneDumperBriefHotkey"], () =>
            {
                SceneDumper.DumpCurrentScene(false);
            });

            manager.Hotkeys.Bind(Settings["SceneDumperDetailedHotkey"], () =>
            {
                SceneDumper.DumpCurrentScene(true);
            });
        }

        public void Shutdown()
        {

        }

        private void ValidateSettings()
        {
            if (Settings["SceneDumperBriefHotkey"] == string.Empty)
            {
                Settings["SceneDumperBriefHotkey"] = "LeftControl+Alpha0";
            }

            if (Settings["SceneDumperDetailedHotkey"] == string.Empty)
            {
                Settings["SceneDumperDetailedHotkey"] = "LeftControl+Alpha9";
            }
            Settings.Save();
        }
    }
}
