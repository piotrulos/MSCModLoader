using HutongGames.PlayMaker;
using System;
using System.Collections.Generic;

namespace MSCLoader
{
    /// <summary>
    /// Class to scheduling actions to execute at specific moments during game time
    /// </summary>
    public class TimeScheduler : MonoBehaviour
    {
        private static int lastMinute = -1;

        internal static void StartScheduler()
        {
            GameObject timeScheduler = new GameObject("MSCLoader Time Scheduler");
            timeScheduler.AddComponent<TimeScheduler>();
        }

        private class ScheduledAction 
        {
            public int Hour { get; }
            public int Minute { get; }
            public Action Action { get; }
            public bool OneTimeAction { get; }

            internal ScheduledAction(int hour, int minute, Action action, bool oneTimeAction)
            {
                Hour = hour;
                Minute = minute;
                Action = action;
                OneTimeAction = oneTimeAction;
            }
        }

        private static List<ScheduledAction> scheduledActions = new List<ScheduledAction>();

        /// <summary>
        /// Method to schedule an action
        /// </summary>
        /// <param name="hour">Game Hour (0-24)</param>
        /// <param name="minute">Game Minute (0-60)</param>
        /// <param name="action">The action to execute</param>
        /// <param name="oneTimeAction">[Optional] Whether the action is ran only once (false on default)</param>
        public static void ScheduleAction(int hour, int minute, Action action, bool oneTimeAction = false) => scheduledActions.Add(new ScheduledAction(hour, minute, action, oneTimeAction));
        
        private void Update()
        {
            int hour = MSCTime.hours;
            int minute = MSCTime.minutes;

            if (minute != lastMinute)
            {
                for (int i = scheduledActions.Count - 1; i >= 0; i--)
                {
                    ScheduledAction action = scheduledActions[i];

                    if (action.Hour == hour && action.Minute == minute)
                    {
                        action.Action.Invoke();
                        if (action.OneTimeAction) scheduledActions.Remove(action);
                    }
                }
                lastMinute = minute;
            }
        }
    }

    internal static class MSCTime
    {
        static FsmInt fsm_time;
        static FsmFloat fsm_minutes;
        static bool initialized = false;

        static bool Initialize()
        {
            if (initialized) return true;

            Transform sun = GameObject.Find("MAP").transform.Find("SUN/Pivot/SUN");
            if (sun == null) return false;

            PlayMakerFSM fsm = sun.GetPlayMaker("Color");
            fsm_time = fsm.GetVariable<FsmInt>("Time");
            fsm_minutes = fsm.GetVariable<FsmFloat>("Minutes");

            initialized = (fsm_time != null && fsm_minutes != null);
            return initialized;
        }

        public static int hours
        {
            get
            {
                if (!Initialize()) return 0;
                int hour = fsm_time.Value == 24 ? 0 : fsm_time.Value;
                if (fsm_minutes.Value > 60f) hour++;
                return hour;
            }
        }

        public static int minutes
        {
            get
            {
                if (!Initialize()) return 0;
                return Mathf.Clamp(Mathf.FloorToInt(fsm_minutes.Value) % 60, 0, 59);
            }
        }
    }

}