using System;
using System.Collections.Generic;
using System.Linq;
using HutongGames.PlayMaker;
using UnityEngine;

/* Usage:
// save game event
GameHook.InjectStateHook(GameObject.Find("ITEMS"), "Save game", Save);
*/


namespace MSCLoader
{
    public class FsmHook
    {
        private class FsmHookAction : FsmStateAction
        {
            public Action hook;
            public override void OnEnter()
            {
                hook?.Invoke();
                Finish();
            }
        }

        public static void FsmInject(GameObject gameObject, string stateName, Action hook)
        {
            FsmState state = GetStateFromGameObject(gameObject, stateName);
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
                ModConsole.Error(string.Format("Cannot find state <b>{0}</b> in GameObject <b>{1}</b>", stateName, gameObject.name));
            }
        }

        private static FsmState GetStateFromGameObject(GameObject obj, string stateName)
        {
            PlayMakerFSM[] comps = obj.GetComponents<PlayMakerFSM>();
            foreach (PlayMakerFSM playMakerFsm in comps)
            {
                FsmState state = playMakerFsm.FsmStates.FirstOrDefault(x => x.Name == stateName);
                if (state != null)
                    return state;
            }
            return null;
        }
    }
}
