#if !Mini
using HutongGames.PlayMaker;
using System;

namespace MSCLoader
{
    /// <summary>
    /// Class to read the current in-game time and day
    /// </summary> 


    
    public static class GameTime
    {
        /// <summary>
        /// Enum representing the days of the week from sunday to saturday, also containing <b>Week</b>, <b>Weekend</b>, and <b>All</b>.
        /// </summary>       
        [Flags] public enum Days
        {
            Sunday = 1 << 0,
            Monday = 1 << 1,
            Tuesday = 1 << 2,
            Wednesday = 1 << 3,
            Thursday = 1 << 4,
            Friday = 1 << 5,
            Saturday = 1 << 6,

            Week = Monday | Tuesday | Wednesday | Thursday | Friday,
            Weekend = Saturday | Sunday,
            All = Week | Weekend
        }

        static FsmInt fsm_time;
        static FsmFloat fsm_minutes;
        static FsmInt fsm_day;

        static bool initialized = false;

        static bool Initialize()
        {
            if (initialized) return true;

            Transform sun = GameObject.Find("MAP").transform.Find("SUN/Pivot/SUN");
            if (sun == null) return false;

            PlayMakerFSM fsm = sun.GetPlayMaker("Color");

            fsm_time = fsm.GetVariable<FsmInt>("Time");
            fsm_minutes = fsm.GetVariable<FsmFloat>("Minutes");

            fsm_day = PlayMakerGlobals.Instance.Variables.GetFsmInt("GlobalDay");

            initialized = (fsm_time != null && fsm_minutes != null && fsm_day != null);
            return initialized;
        }

        /// <summary>
        /// Current hour from 0 to 24
        /// </summary>
        public static int Hour
        {
            get
            {
                if (!Initialize()) return 0;
                int hour = fsm_time.Value == 24 ? 0 : fsm_time.Value;
                if (fsm_minutes.Value > 60f) hour++;
                return hour;
            }
        }

        /// <summary>
        /// Current minute from 0 to 60
        /// </summary>
        public static int Minute
        {
            get
            {
                if (!Initialize()) return 0;
                return Mathf.Clamp(Mathf.FloorToInt(fsm_minutes.Value) % 60, 0, 59);
            }
        }

        /// <summary>
        /// Current day
        /// </summary>
        public static Days Day
        {
            get
            {
                if (!Initialize()) return 0;
                return (Days)(1 << fsm_day.Value);
            }
        }
    }
}
#endif