#if !Mini
using HutongGames.PlayMaker;
using System;

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
[Obsolete("Obsolete", true)]
public class FsmHook
{
    /// <summary>
    /// Hook to playmaker state
    /// </summary>
    /// <param name="gameObject">GameObject where to hook</param>
    /// <param name="stateName">Name of the state</param>
    /// <param name="hook">Your function to hook</param>
    [Obsolete("Please use the other FsmInject override.", true)]
    public static void FsmInject(GameObject gameObject, string stateName, Action hook)
    {
        gameObject.FsmInject(stateName, hook);
    }
}
#endif