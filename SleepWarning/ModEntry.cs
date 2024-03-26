using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

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

        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            this.Config = Helper.ReadConfig<SleepWarningConfig>();
            helper.Events.GameLoop.TimeChanged += this.OnTimeChanged;
        }


        /*********
        ** Private methods
        *********/
        /// <summary>
        /// Plays the sleep warning sound and repeats it the specified amount of times.
        /// </summary>
        /// <param name="repeat">The number of times to repeat.</param>
        private void PlayWarningSound(int repeat)
        {
            for (int i = 0; i < repeat; i++)
            {
                DelayedAction.playSoundAfterDelay(Config.WarningSound, 500*i, null, null, 1, true);
            }
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
    }
}
