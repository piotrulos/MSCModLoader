#if !Mini
using HutongGames.PlayMaker;
using System;
using System.Collections.Generic;

namespace MSCLoader;

//Fix shitty cursor lock
internal class SetMouseCursorFix : FsmStateAction
{
    public FsmBool hideCursor;

    public FsmBool lockCursor;

    public override void Reset()
    {
        //cursorTexture = null;
        hideCursor = false;
        lockCursor = false;
    }
    public SetMouseCursorFix(bool locked)
    {
        hideCursor = locked;
        lockCursor = locked;
    }
    public override void OnEnter()
    {
        Cursor.visible = !hideCursor.Value;
        Cursor.SetCursor(null, new Vector2(0, 0), CursorMode.Auto);
        if (lockCursor.Value)
            Cursor.lockState = CursorLockMode.Locked;
        else
            Cursor.lockState = CursorLockMode.None;
        Finish();
    }
}

/// <summary>
/// Playmaker hook inject method.
/// </summary>
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

    /// <summary>
    /// Hook to playmaker state
    /// </summary>
    /// <param name="gameObject">GameObject where to hook</param>
    /// <param name="stateName">Name of the state</param>
    /// <param name="hook">Your function to hook</param>
    public static void FsmInject(GameObject gameObject, string stateName, Action hook)
    {
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
            ModConsole.Error($"FsmInject: Cannot find state <b>{stateName}</b> in GameObject <b>{gameObject.name}</b>");
        }
    }
}
#endif