#if !Mini
using System;
using System.Collections.Generic;

namespace MSCLoader
{
    /// <summary>
    /// Class to scheduling actions to execute at specific moments during game time
    /// </summary>
    public class TimeScheduler : MonoBehaviour
    {
        static int lastMinute = -1;

        /// <summary>
        /// Init the Action Scheduler (only meant to be called once on game load)
        /// </summary>
        internal static void StartScheduler()
        {
            GameObject timeScheduler = new GameObject("MSCLoader Time Scheduler");
            timeScheduler.AddComponent<TimeScheduler>();
        }

        /// <summary>
        /// Class for all scheduled actions
        /// </summary>
        public class ScheduledAction 
        {
            public int Hour { get; }
            public int Minute { get; }
            public Action Action { get; }
            public bool OneTimeAction { get; }
            public GameTime.Days Day { get; }

            internal ScheduledAction(int hour, int minute, Action action, GameTime.Days day, bool oneTimeAction)
            {
                Hour = hour;
                Minute = minute;
                Action = action;
                Day = day;
                OneTimeAction = oneTimeAction;
            }
        }
        
        /// <summary>
        /// List containing all Scheduled Actions
        /// </summary>
        public static List<ScheduledAction> ScheduledActions { get; private set; } = new List<ScheduledAction>();        

        /// <summary>
        /// Method to schedule an action
        /// </summary>
        /// <param name="hour">Game Hour (0-24)</param>
        /// <param name="minute">Game Minute (0-60)</param>
        /// <param name="action">The action to execute</param>
        /// <param name="oneTimeAction">[Optional] Whether the action is ran only once (false on default)</param>
        public static void ScheduleAction(int hour, int minute, Action action, GameTime.Days day = GameTime.Days.All, bool oneTimeAction = false) 
            => ScheduledActions.Add(new ScheduledAction(hour, minute, action, day, oneTimeAction));
        
        void Update()
        {
            if (GameTime.Minute != lastMinute)
            {
                for (int i = ScheduledActions.Count - 1; i >= 0; i--)
                {
                    ScheduledAction action = ScheduledActions[i];

                    if (action.Hour == GameTime.Hour && action.Minute == GameTime.Minute && (action.Day == GameTime.Day || action.Day == GameTime.Days.All)) // None means ran every day
                    {
                        action.Action.Invoke();
                        if (action.OneTimeAction) ScheduledActions.Remove(action);
                    }
                }
                lastMinute = GameTime.Minute;
            }
        }
    }
}
#endif