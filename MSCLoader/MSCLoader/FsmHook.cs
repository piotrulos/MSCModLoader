using System;
using System.Collections.Generic;
using System.Linq;
#if !Mini
using HutongGames.PlayMaker;
#endif
namespace MSCLoader
{
    /// <summary>
    /// Playmaker hook inject method.
    /// </summary>
    public class FsmHook
    {
        #if !Mini
        private class FsmHookAction : FsmStateAction
        {
            public Action hook;
            public override void OnEnter()
            {
                hook?.Invoke();
                Finish();
            }
        }
#endif
        /// <summary>
        /// Hook to playmaker state
        /// </summary>
        /// <param name="gameObject">GameObject where to hook</param>
        /// <param name="stateName">Name of the state</param>
        /// <param name="hook">Your function to hook</param>
        public static void FsmInject(GameObject gameObject, string stateName, Action hook)
        {
            #if !Mini
            FsmState state = gameObject.GetPlayMakerState(stateName);
            
            if (state != null)
            {
                // inject our hook action to the state machine
                List<FsmStateAction> actions = new List<FsmStateAction>(state.Actions);
                FsmHookAction hookAction = new FsmHookAction
                {
                    hook = hook
                };
                actions.Insert(0, hookAction);
                state.Actions = actions.ToArray();
            }
            else
            {
                ModConsole.Error(string.Format("FsmInject: Cannot find state <b>{0}</b> in GameObject <b>{1}</b>", stateName, gameObject.name));
            }
#endif
        }
    }
}
