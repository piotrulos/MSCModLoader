using HutongGames.PlayMaker;
using System;
using System.Linq;
using UnityEngine;

namespace MSCLoader
{

    /// <summary>
    /// Exception extensions
    /// </summary>
    public static class ExceptionExtensions
    {
        /// <summary>
        /// Get Full Exception messages (including inner exceptions) without stack trace.
        /// </summary>
        /// <param name="ex">Exception</param>
        /// <returns></returns>
        public static string GetFullMessage(this Exception ex)
        {
            return ex.InnerException == null
                 ? ex.Message
                 : ex.Message + " --> " + ex.InnerException.GetFullMessage();
        }
    }

    //TODO: Expand
    /// <summary>
    /// PlayMaker extensions for Unity API
    /// </summary>
    public static class PlayMakerExtensions
    {
        /// <summary>
        /// Get PlayMaker from this object by Name
        /// </summary>
        /// <param name="obj">this game object</param>
        /// <param name="playMakerName">Name of the PlayMaker</param>
        /// <returns>PlayMakerFSM</returns>
        public static PlayMakerFSM GetPlayMaker(this GameObject obj, string playMakerName)
        {
            PlayMakerFSM[] pm = obj.GetComponents<PlayMakerFSM>();
            if (pm != null)
            {
                PlayMakerFSM fsm = pm.FirstOrDefault(x => x.FsmName == playMakerName);
                if (fsm != null)
                    return fsm;
                else
                {
                    ModConsole.Error(string.Format("GetPlayMaker: Cannot find <b>{0}</b> in GameObject <b>{1}</b>", playMakerName, obj.name));
                    return null;
                }

            }
            ModConsole.Error(string.Format("GetPlayMaker: Cannot find any Playmakers in GameObject <b>{0}</b>", obj.name));
            return null;
        }

        /// <summary>
        /// Get PlayMaker from this object by Name
        /// </summary>
        /// <param name="t">this transform</param>
        /// <param name="playMakerName">Name of the PlayMaker</param>
        /// <returns>PlayMakerFSM</returns>
        public static PlayMakerFSM GetPlayMaker(this Transform t, string playMakerName)
        {
            //Just a redirect if used as Transform.
            return t.gameObject.GetPlayMaker(playMakerName);
        }

        /// <summary>
        /// Get PlayMaker state from this object by Name
        /// </summary>
        /// <param name="obj">this game object</param>
        /// <param name="stateName">Name of the PlayMaker state</param>
        /// <returns>FsmState</returns>
        public static FsmState GetPlayMakerState(this GameObject obj, string stateName)
        {
            PlayMakerFSM[] pm = obj.GetComponents<PlayMakerFSM>();
            if (pm != null)
            {
                for (int i = 0; i < pm.Length; i++)
                {
                    FsmState state = pm[i].FsmStates.FirstOrDefault(x => x.Name == stateName);
                    if (state != null)
                        return state;
                }
                ModConsole.Error(string.Format("GetPlayMakerState: Cannot find <b>{0}</b> state in GameObject <b>{1}</b>", stateName, obj.name));
                return null;
            }
            ModConsole.Error(string.Format("GetPlayMakerState: Cannot find any Playmakers in GameObject <b>{0}</b>", obj.name));
            return null;
        }
        /// <summary>
        /// FSM Inject as extension (same as old FsmHook.FsmInject)
        /// </summary>
        /// <param name="gameObject">GameObject where to hook</param>
        /// <param name="stateName">Name of the state</param>
        /// <param name="hook">Your function to hook</param>
        /// <example><code lang="C#" title="Fsm Inject as Extension">GameObject.Find("Some game object").FsmInject("state name", function);</code></example>

        public static void FsmInject(this GameObject gameObject, string stateName, Action hook)
        {
            FsmHook.FsmInject(gameObject, stateName, hook);
        }

    }
}
