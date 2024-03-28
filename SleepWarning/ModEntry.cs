using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using GenericModConfigMenu;
using System;
using static System.Collections.Specialized.BitVector32;

namespace SleepWarning
{
    /// <summary>The mod entry point.</summary>
    internal class ModEntry : Mod
    {
        /*********
        ** Properties
        *********/
        /// <summary>The mod configuration from the player.</summary>
        private SleepWarningConfig Config;

        /// <summary> Pretty sound names used by Generic Mod Config Menu. </summary>
        private readonly Dictionary<string, string> SoundNames = new()
            {
                { "crystal", "Ding (Default)" },
                { "cameraNoise", "Camera Click" },
                { "cat", "Cat" },
                { "cluck", "Chicken" },
                { "cow", "Cow" },
                { "dog_bark", "Dog" },
                { "goat", "Goat" },
                { "parrot", "Parrot" },
                { "pig", "Pig" },
                { "select", "Select (UI)" },
                { "whistle", "Whistle" }
            };

        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            this.Config = Helper.ReadConfig<SleepWarningConfig>();
            helper.Events.GameLoop.GameLaunched += this.OnGameLaunched;
            helper.Events.GameLoop.TimeChanged += this.OnTimeChanged;
        }


        /*********
        ** Private methods
        *********/
        private void PlayDelayedSound(string sound, int delay = 0)
        {
            DelayedAction.playSoundAfterDelay(sound, delay, null, null, 1, true);
        }
        /// <summary>
        /// Plays the sleep warning sound and repeats it the specified amount of times.
        /// </summary>
        /// <param name="repeat">The number of times to repeat.</param>
        private void PlayWarningSound(int repeat, string sound="")
        {
            for (int i = 0; i < repeat; i++)
                PlayDelayedSound(Config.WarningSound, 500 * i);
        }

        /// <inheritdoc cref="IGameLoopEvents.TimeChanged"/>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnTimeChanged(object? sender, TimeChangedEventArgs e) 
        {
            if (Game1.timeOfDay < Config.FirstWarnTime)
                return;

            if (Config.ThirdWarnTime > -1 && Game1.timeOfDay == Config.ThirdWarnTime)
            {
                PlayWarningSound(3);
            }
            else if (Config.SecondWarnTime > -1 && Game1.timeOfDay == Config.SecondWarnTime)
            {
                PlayWarningSound(2);
            }
            else if (Config.FirstWarnTime > -1 && Game1.timeOfDay == Config.FirstWarnTime)
            {
                PlayWarningSound(1);
            }
        }

        private string FormatTimeValue(int time)
        {
            if (time == -1)
                return "Disable";
            string min_str = (time % 2 == 0) ? "00" : "30";
            int hours = (time / 2) + 6;
            string am_pm = (hours > 11 && hours < 24) ? " PM" : " AM";
            string hour_str = (hours % 12 == 0) ? "12" : (hours % 12).ToString();
            string result = hour_str + ":" + min_str + am_pm;
            return result;
        }

        private string FormatSoundValue(string sound)
        {
            if (SoundNames.ContainsKey(sound))
                return SoundNames[sound];

            return sound;
        }
            

        private int ConvertTimeValue(int time)
        {
            if (time == -1)
                return -1;
            int minutes = (time % 2 == 0) ? 0 : 30;
            int hours = (time / 2) * 100 + 600;
            // Monitor.Log("In: " + time + " Out:" + (hours + minutes));
            return hours + minutes;
        }

        private int ReverseConvertTime(int time)
        {
            if (time == -1)
                return -1;
            int hours = Math.Clamp(time / 100, 6, 26) - 6;
            int minutes = Math.Min(time/10 % 10, 5);

            return (hours * 2 + (minutes >= 3 ? 1 : 0));
        }

        private string SetSoundValue(string sound)
        {
            PlayDelayedSound(sound, 1000);
            return sound;
        }

        private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
        {
            // Add support for Generic Mod Config Menu, if installed
            var configMenu = this.Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (configMenu is null)
                return;

            configMenu.Register(
                mod: this.ModManifest,
                reset: () => this.Config = new SleepWarningConfig(),
                save: () => this.Helper.WriteConfig(this.Config) 
                );
            configMenu.AddNumberOption(
                mod: this.ModManifest,
                name: () => "First Warn Time",
                getValue: () => ReverseConvertTime(this.Config.FirstWarnTime),
                setValue: value => this.Config.FirstWarnTime = ConvertTimeValue(value),
                min: -1,
                max: 40,
                formatValue: FormatTimeValue 
                );
            configMenu.AddNumberOption(
                mod: this.ModManifest,
                name: () => "Second Warn Time",
                getValue: () => ReverseConvertTime(this.Config.SecondWarnTime),
                setValue: value => this.Config.SecondWarnTime = ConvertTimeValue(value),
                min: -1,
                max: 40,
                formatValue: FormatTimeValue 
                );
            configMenu.AddNumberOption(
                mod: this.ModManifest,
                name: () => "Third Warn Time",
                getValue: () => ReverseConvertTime(this.Config.ThirdWarnTime),
                setValue: value => this.Config.ThirdWarnTime = ConvertTimeValue(value),
                min: -1,
                max: 40,
                formatValue: FormatTimeValue 
                );
            configMenu.AddTextOption(
                mod: this.ModManifest,
                name: () => "Warning Sound",
                getValue: () => this.Config.WarningSound,
                setValue: value => this.Config.WarningSound = SetSoundValue(value),
                allowedValues: new string[] { "cameraNoise", "cat", "cluck", "cow", "crystal", "Duck", "dog_bark", "goat", "parrot", "pig", "select", "whistle" },
                formatAllowedValue: FormatSoundValue,
                fieldId: "glimmerDev.SleepWarning.WarnSound"
                );
        }
    }
}
