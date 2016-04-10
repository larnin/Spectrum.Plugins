using Spectrum.API;
using Spectrum.API.Configuration;
using Spectrum.API.Game;
using Spectrum.API.Game.Vehicle;
using Spectrum.API.Interfaces.Plugins;
using Spectrum.API.Interfaces.Systems;

namespace MusicSwitcher
{
    public class Entry : IPlugin
    {
        public string FriendlyName => "Music Switcher";
        public string Author => "Ciastex";
        public string Contact => "ciastexx@live.com";
        public APILevel CompatibleAPILevel => APILevel.MicroWave;

        private Settings _settings;

        public void Initialize(IManager manager)
        {
            _settings = new Settings(typeof(Entry));
            ValidateSettings();

            manager.Hotkeys.Bind(_settings["RepeatToggleHotkey"], () =>
            {
                if (Audio.CustomMusicEnabled)
                {
                    Audio.RepeatCustomMusic = !Audio.RepeatCustomMusic;

                    var phrase = Audio.RepeatCustomMusic ? "repeat on" : "repeat off";
                    LocalVehicle.Screen.SetTimeBarText(phrase, "#0077DD", 1.0f);
                }
                else
                {
                    LocalVehicle.Screen.WriteText("Custom music is not enabled. Please enable custom music and try again.");
                }
            });

            manager.Hotkeys.Bind(_settings["ShuffleToggleHotkey"], () =>
            {
                if (Audio.CustomMusicEnabled)
                {
                    Audio.ShuffleCustomMusic = !Audio.ShuffleCustomMusic;

                    var phrase = Audio.ShuffleCustomMusic ? "shuffle on" : "shuffle off";
                    LocalVehicle.Screen.SetTimeBarText(phrase, "#0077DD", 1.0f);
                }
                else
                {
                    LocalVehicle.Screen.WriteText("Custom music is not enabled. Please enable custom music and try again.");
                }
            });

            manager.Hotkeys.Bind(_settings["NextMusicTrackHotkey"], () =>
            {
                if (Audio.CustomMusicEnabled)
                {
                    Audio.NextCustomSong();
                }
                else
                {
                    LocalVehicle.Screen.WriteText("Custom music is not enabled. Please enable custom music and try again.");
                }
            });

            manager.Hotkeys.Bind(_settings["PrevMusicTrackHotkey"], () =>
            {
                if (Audio.CustomMusicEnabled)
                {
                    Audio.PreviousCustomSong();
                }
                else
                {
                    LocalVehicle.Screen.WriteText("Custom music is not enabled. Please enable custom music and try again.");
                }
            });

            manager.Hotkeys.Bind(_settings["ToggleMP3MusicHotkey"], () =>
            {
                Audio.CustomMusicEnabled = !Audio.CustomMusicEnabled;

                var phrase = Audio.CustomMusicEnabled ? "music on" : "music off";
                LocalVehicle.Screen.SetTimeBarText(phrase, "#0077DD", 1f);
            });

            Audio.CustomMusicChanged += (sender, args) =>
            {
                if (Audio.CustomMusicEnabled)
                {
                    LocalVehicle.Screen.WriteText($"Now playing:\n{args.NewTrackName.Substring(0, args.NewTrackName.Length - 4)}");
                }
            };
        }

        public void Shutdown()
        {
            
        }

        private void ValidateSettings()
        {
            if (_settings["RepeatToggleHotkey"] == string.Empty)
            {
                _settings["RepeatToggleHotkey"] = "X+Backslash";
            }

            if (_settings["ShuffleToggleHotkey"] == string.Empty)
            {
                _settings["ShuffleToggleHotkey"] = "X+Quote";
            }

            if (_settings["NextMusicTrackHotkey"] == string.Empty)
            {
                _settings["NextMusicTrackHotkey"] = "X+RightBracket";
            }

            if (_settings["PrevMusicTrackHotkey"] == string.Empty)
            {
                _settings["PrevMusicTrackHotkey"] = "X+LeftBracket";
            }

            if (_settings["ToggleMP3MusicHotkey"] == string.Empty)
            {
                _settings["ToggleMP3MusicHotkey"] = "X+Semicolon";
            }
            _settings.Save();
        }
    }
}
