#if !Mini
using System;
using System.Collections.Generic;
using System.Linq;


namespace MSCLoader
{
    /// <summary>
    /// Class to scheduling actions to execute at specific moments during game time
    /// </summary>
    public class TimeScheduler : MonoBehaviour
    {
        public static event Action<int> OnTimeSkipped;

        static GameObject timeScheduler;
        static bool schedulerInstantiated = false;

        /// <summary>
        /// Create and init the Action Scheduler (only meant to be called on game scene load)
        /// </summary>
        internal static void StartScheduler()
        {
            if (schedulerInstantiated) return;
            timeScheduler = new GameObject("MSCLoader Time Scheduler");
            timeScheduler.AddComponent<TimeScheduler>();

            schedulerInstantiated = true;
        }

        /// <summary>
        /// Disable and destroy the Action Scheduler (only meant to be called on game scene exit)
        /// </summary>
        internal static void StopScheduler()
        {
            if (!schedulerInstantiated) return;
            GameObject.Destroy(timeScheduler);
            timeScheduler = null;
            ScheduledActions = new List<ScheduledAction>();
            previousMinute = previousHour = default;
            previousDay = default;
            schedulerInstantiated = false;
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

        static int previousMinute;
        static int previousHour;
        static GameTime.Days previousDay;

        void Update()
        {
            if (GameTime.Minute == previousMinute) return;

            for (int i = ScheduledActions.Count - 1; i >= 0; i--)
            {
                ScheduledAction action = ScheduledActions[i];

                if (action.Hour == GameTime.Hour && action.Minute == GameTime.Minute && ((GameTime.Day & action.Day) != 0))
                {
                    action.Action.Invoke();
                    if (action.OneTimeAction) ScheduledActions.Remove(action);
                }
            }

            if ((GameTime.Hour - previousHour == 0 && Math.Abs(GameTime.Minute - previousMinute) > 1) || // same hour, but minutes skipped                                                                     
                GameTime.Hour - previousHour > 1 || GameTime.Hour - previousHour < 0) // more than one hour skipped
            {
                InvokeMissedActions(previousHour, previousMinute, previousDay);
                OnTimeSkipped?.Invoke(GetTimeDifference(previousHour, previousMinute, previousDay));
            }

            previousMinute = GameTime.Minute;
            previousHour = GameTime.Hour;
            previousDay = GameTime.Day;
        }

        static int GetTimeDifference(int previousHour, int previousMinute, GameTime.Days previousDay)
        {
            int currentTotalMinutes = CalcTotalMinutes(GameTime.Hour, GameTime.Minute, GameTime.Day);
            int previousTotalMinutes = CalcTotalMinutes(previousHour, previousMinute, previousDay);

            // week rollover
            if (currentTotalMinutes < previousTotalMinutes) currentTotalMinutes += 10080;  // Adds one full week of minutes

            return currentTotalMinutes - previousTotalMinutes;
        }

        static void InvokeMissedActions(int sinceHour, int sinceMinute, GameTime.Days sinceDay)
        {
            int startMinute = CalcTotalMinutes(sinceHour, sinceMinute, sinceDay);
            int endMinute = CalcTotalMinutes(GameTime.Hour, GameTime.Minute, GameTime.Day);
            if (endMinute < startMinute) endMinute += 10080;

            List<ScheduledAction> missedActions = new List<ScheduledAction>();

            foreach (ScheduledAction action in ScheduledActions) if (ActionMissed(action, sinceDay, sinceHour, sinceMinute)) missedActions.Add(action);

            missedActions = missedActions
            .OrderBy(action => (int)action.Day)
            .ThenBy(action => action.Hour)
            .ThenBy(action => action.Minute)
            .ToList();

            foreach (ScheduledAction action in missedActions)
            {
                action.Action.Invoke();
                if (action.OneTimeAction) ScheduledActions.Remove(action);
            }
        }

        static bool ActionMissed(ScheduledAction action, GameTime.Days sinceDay, int sinceHour, int sinceMinute)
        {
            int currentTotalMinutes = CalcTotalMinutes(GameTime.Hour, GameTime.Minute, GameTime.Day);
            int sinceTotalMinutes = CalcTotalMinutes(sinceHour, sinceMinute, sinceDay);

            for (GameTime.Days day = GameTime.Days.Sunday; day <= GameTime.Days.Saturday; day = (GameTime.Days)((int)day << 1))
            {
                if ((action.Day & day) != 0)
                {
                    int actionTotalMinutes = CalcTotalMinutes(action.Hour, action.Minute, day);

                    // Week rollover
                    if (currentTotalMinutes < sinceTotalMinutes) currentTotalMinutes += 10080;  
                    
                    if (actionTotalMinutes < sinceTotalMinutes) actionTotalMinutes += 10080;

                    if (actionTotalMinutes > sinceTotalMinutes && actionTotalMinutes <= currentTotalMinutes) return true; 
                }
            }

            return false;
        }

        static int CalcTotalMinutes(int hour, int minute, GameTime.Days day = (GameTime.Days)1) => ((int)Math.Log((int)day, 2) * 24 + hour) * 60 + minute;
    }
}
#endif