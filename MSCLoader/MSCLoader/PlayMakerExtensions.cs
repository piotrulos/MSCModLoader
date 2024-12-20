#if !Mini
using HutongGames.PlayMaker;
using System;
using System.Linq;

namespace MSCLoader;

/// <summary>
/// Exception extensions
/// </summary>
public static class ExceptionExtensions
{
    /// <summary>
    /// Get Full Exception messages (including inner exceptions) without stack trace.
    /// </summary>
    /// <param name="ex">Exception</param>
    /// <returns>Full Exception Message</returns>
    public static string GetFullMessage(this Exception ex)
    {
        return ex.InnerException == null
             ? ex.Message
             : ex.Message + " --> " + ex.InnerException.GetFullMessage();
    }
}

/// <summary>
/// My Summer Car specific extensions
/// </summary>
public static class MSCExtensions
{

    /// <summary>
    /// Make this GameObject pickable in game (as long as it has Rigidbody attached)
    /// </summary>
    /// <param name="gameObject">GameObject to make pickable</param>
    public static void MakePickable(this GameObject gameObject) => LoadAssets.MakeGameObjectPickable(gameObject);
    /// <summary>
    /// Make this GameObject pickable in game (as long as it has Rigidbody attached)
    /// </summary>
    /// <param name="transform">Transform part of GameObject to make pickable</param>
    public static void MakePickable(this Transform transform) => LoadAssets.MakeGameObjectPickable(transform.gameObject);

}

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
        if (pm != null && pm.Length > 0)
        {
            for (int i = 0; i < pm.Length; i++)
            {
                if (pm[i].FsmName == playMakerName)
                    return pm[i];
            }
            return null;

        }
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
        if (pm != null && pm.Length > 0)
        {
            for (int i = 0; i < pm.Length; i++)
            {
                for (int j = 0; j < pm[i].FsmStates.Length; j++)
                {
                    if (pm[i].FsmStates[j].Name == stateName)
                        return pm[i].FsmStates[j];
                }
            }
            return null;

        }
        return null;
    }

    private class MSCL_PMHook : FsmStateAction
    {
        public Action action;
        public bool everyFrame = false;

        public MSCL_PMHook(Action action, bool everyFrame = false)
        {
            this.action = action;
            this.everyFrame = everyFrame;
        }

        public override void OnEnter()
        {
            if (!everyFrame)
            {
                action?.Invoke();
                Finish();
            }
        }

        public override void OnUpdate()
        {
            if (everyFrame)
                action?.Invoke();
        }
    }

    /// <summary>
    /// FSM Inject as extension (same as old FsmHook.FsmInject)
    /// </summary>
    /// <param name="gameObject">GameObject where to hook</param>
    /// <param name="stateName">Name of the state</param>
    /// <param name="hook">Your function to hook</param>
    [Obsolete("Please use the other FsmInject extension", true)]
    public static void FsmInject(this GameObject gameObject, string stateName, Action hook)
    {
        PlayMakerFSM[] pm = gameObject.GetComponents<PlayMakerFSM>();
        if (pm != null && pm.Length > 0)
        {
            for (int i = 0; i < pm.Length; i++)
            {
                for (int j = 0; j < pm[i].FsmStates.Length; j++)
                {
                    if (pm[i].FsmStates[j].Name == stateName)
                    {
                        pm[i].FsmInject(stateName, hook, false, 0);
                    }
                }
            }
        }
    }

    /// <summary>
    /// Insert custom Action to the Playmaker
    /// </summary>
    /// <param name="go">Owner gameobject</param>
    /// <param name="playmakerName">Target Playmaker name</param>
    /// <param name="stateName">Target state name</param>
    /// <param name="hook">Action callback</param>
    /// <param name="everyFrame">Execute this function every frame while the state is active</param>
    /// <param name="index">The index where to insert the action</param>
    /// <param name="replace">If true, replaces the action at specified index, otherwise inserts it.</param>
    /// <returns>Whether the injection was successful</returns>
    public static bool FsmInject(this GameObject go, string playmakerName, string stateName, Action hook, bool everyFrame = false, int index = -1, bool replace = false)
    {
        return FsmInject(go.GetPlayMaker(playmakerName), stateName, new MSCL_PMHook(hook, everyFrame), index, replace);
    }
    /// <summary>
    /// Insert custom Action to the Playmaker
    /// </summary>
    /// <param name="tf">Owner transform</param>
    /// <param name="playmakerName">Target Playmaker name</param>
    /// <param name="stateName">Target state name</param>
    /// <param name="hook">Action callback</param>
    /// <param name="everyFrame">Execute this function every frame while the state is active</param>
    /// <param name="index">The index where to insert the action</param>
    /// <param name="replace">If true, replaces the action at specified index, otherwise inserts it.</param>
    /// <returns>Whether the injection was successful</returns>
    public static bool FsmInject(this Transform tf, string playmakerName, string stateName, Action hook, bool everyFrame = false, int index = -1, bool replace = false)
    {
        return FsmInject(tf.GetPlayMaker(playmakerName), stateName, new MSCL_PMHook(hook, everyFrame), index, replace);
    }
    /// <summary>
    /// Insert custom Action to the Playmaker
    /// </summary>
    /// <param name="fsm">Target Playmaker</param>
    /// <param name="stateName">Target state name</param>
    /// <param name="hook">Action callback</param>
    /// <param name="everyFrame">Execute this function every frame while the state is active</param>
    /// <param name="index">The index where to insert the action</param>
    /// <param name="replace">If true, replaces the action at specified index, otherwise inserts it.</param>
    /// <returns>Whether the injection was successful</returns>
    public static bool FsmInject(this PlayMakerFSM fsm, string stateName, Action hook, bool everyFrame = false, int index = -1, bool replace = false)
    {
        return FsmInject(fsm, stateName, new MSCL_PMHook(hook, everyFrame), index, replace);
    }
    /// <summary>
    /// Insert custom Action to the Playmaker
    /// </summary>
    /// <param name="fsm">Target Playmaker</param>
    /// <param name="stateName">Target state name</param>
    /// <param name="action">Custom FsmStateAction</param>
    /// <param name="index">The index where to insert the action</param>
    /// <param name="replace">If true, replaces the action at specified index, otherwise inserts it.</param>
    /// <returns>Whether the injection was successful</returns>
    public static bool FsmInject<T>(this PlayMakerFSM fsm, string stateName, T action, int index = -1, bool replace = false) where T : FsmStateAction
    {
        FsmState fsmState = fsm.GetState(stateName);
        if (fsmState == null) return false;

        FsmStateAction[] a = fsmState.Actions;
        if (index < -1 || index >= a.Length) return false;
        if (replace)
        {
            if (index == -1) return false;
            a[index] = action;
        }
        else
        {
            FsmStateAction[] _a = new FsmStateAction[a.Length + 1];
            if (index == -1)
            {
                _a[_a.Length - 1] = action;
            }

            for (int i = 0, j = 0; i < a.Length; i++, j++)
            {
                if (j == index)
                {
                    _a[j] = action;
                    j++;
                }
                _a[j] = a[i];
            }
            a = _a;
        }
        fsmState.Actions = a;
        return true;
    }

    /// <summary>
    /// Initialize a PlayMakerFSM
    /// </summary>
    /// <param name="pm">FSM to initialize</param>
    public static void InitializeFSM(this PlayMakerFSM pm)
    {
        pm.Fsm.InitData();
    }

    /// <summary>
    /// Get FsmEvent by name
    /// </summary>
    /// <param name="fsm">The target PlaymakerFSM</param>
    /// <param name="eventName">Event name</param>
    /// <returns></returns>
    public static FsmEvent GetEvent(this PlayMakerFSM fsm, string eventName)
    {
        for (int i = 0; i < fsm.FsmEvents.Length; i++)
        {
            if (fsm.FsmEvents[i].Name == eventName)
                return fsm.FsmEvents[i];
        }
        return null;
    }

    private static FsmEvent GetEvent(Fsm fsm, string name)
    {
        for (int i = 0; i < fsm.Events.Length; i++)
        {
            if (fsm.Events[i].Name == name)
                return fsm.Events[i];
        }
        return null;
    }

    /// <summary>
    /// Add event to PlayMakerFSM
    /// </summary>
    /// <param name="pm">PlayMakerFSM</param>
    /// <param name="eventName">event name</param>
    public static FsmEvent AddEvent(this PlayMakerFSM pm, string eventName)
    {
        pm.InitializeFSM();
        FsmEvent[] src = pm.FsmEvents;
        FsmEvent[] vars = new FsmEvent[src.Length + 1];
        src.CopyTo(vars, 0);
        FsmEvent e = new FsmEvent(eventName);
        vars[src.Length] = e;
        pm.Fsm.Events = vars;
        pm.InitializeFSM();
        return e;
    }

    /// <summary>
    /// Add event to PlayMakerFSM
    /// </summary>
    /// <param name="pm">PlayMakerFSM</param>
    /// <param name="eventName">event name</param>
    public static FsmEvent AddEvent(this Fsm pm, string eventName)
    {
        pm.InitData();
        FsmEvent[] src = pm.Events;
        FsmEvent[] vars = new FsmEvent[src.Length + 1];
        src.CopyTo(vars, 0);
        FsmEvent e = new FsmEvent(eventName);
        vars[src.Length] = e;
        pm.Events = vars;
        pm.InitData();
        return e;
    }

    /// <summary>
    /// Adds a GlobalTransition to the PlayMakerFSM
    /// </summary>
    /// <param name="pm">PlayMakerFSM</param>
    /// <param name="eventName">Name of the event</param>
    /// <param name="stateName">Name of the state</param>
    public static void AddGlobalTransition(this PlayMakerFSM pm, string eventName, string stateName)
    {
        AddGlobalTransition(pm, GetEvent(pm, eventName) ?? AddEvent(pm, eventName), stateName);
    }

    /// <summary>
    /// Adds a GlobalTransition to the PlayMakerFSM
    /// </summary>
    /// <param name="pm">PlayMakerFSM</param>
    /// <param name="fsmevent">The event</param>
    /// <param name="stateName">Name of the state</param>
    public static void AddGlobalTransition(this PlayMakerFSM pm, FsmEvent fsmevent, string stateName)
    {
        pm.InitializeFSM();
        FsmTransition[] gt = new FsmTransition[pm.FsmGlobalTransitions.Length + 1];
        pm.FsmGlobalTransitions.CopyTo(gt, 0);
        gt[pm.FsmGlobalTransitions.Length] = new FsmTransition
        {
            FsmEvent = fsmevent,
            ToState = stateName
        };
        pm.Fsm.GlobalTransitions = gt;
        pm.InitializeFSM();
    }

    /// <summary>
    /// Gets a GlobalTransition of the PlayMakerFSM
    /// </summary>
    /// <param name="pm">PlayMakerFSM</param>
    /// <param name="eventName">Name of the event</param>
    /// <returns>FsmTransition</returns>
    public static FsmTransition GetGlobalTransition(this PlayMakerFSM pm, string eventName)
    {
        pm.InitializeFSM();
        for (int i = 0; i < pm.FsmGlobalTransitions.Length; i++)
        {
            if (pm.FsmGlobalTransitions[i].FsmEvent.Name == eventName)
                return pm.FsmGlobalTransitions[i];
        }
        return null;
    }

    /// <summary>
    /// Add FsmFloat variable to PlayMakerFSM
    /// </summary>
    /// <param name="pm">PlayMakerFSM</param>
    /// <param name="fsmVariable">FsmFloat variable</param>
    public static void AddVariable(this PlayMakerFSM pm, FsmFloat fsmVariable)
    {
        pm.InitializeFSM();
        FsmFloat[] src = pm.FsmVariables.FloatVariables;
        FsmFloat[] vars = new FsmFloat[src.Length + 1];
        src.CopyTo(vars, 0);
        vars[src.Length] = fsmVariable;
        pm.FsmVariables.FloatVariables = vars;
        pm.InitializeFSM();
    }
    /// <summary>
    /// Add FsmInt variable to PlayMakerFSM
    /// </summary>
    /// <param name="pm">PlayMakerFSM</param>
    /// <param name="fsmVariable">FsmInt variable</param>
    public static void AddVariable(this PlayMakerFSM pm, FsmInt fsmVariable)
    {
        pm.InitializeFSM();
        FsmInt[] src = pm.FsmVariables.IntVariables;
        FsmInt[] vars = new FsmInt[src.Length + 1];
        src.CopyTo(vars, 0);
        vars[src.Length] = fsmVariable;
        pm.FsmVariables.IntVariables = vars;
        pm.InitializeFSM();
    }

    /// <summary>
    /// Add FsmBool variable to PlayMakerFSM
    /// </summary>
    /// <param name="pm">PlayMakerFSM</param>
    /// <param name="fsmVariable">FsmBool variable</param>
    public static void AddVariable(this PlayMakerFSM pm, FsmBool fsmVariable)
    {
        pm.InitializeFSM();
        FsmBool[] src = pm.FsmVariables.BoolVariables;
        FsmBool[] vars = new FsmBool[src.Length + 1];
        src.CopyTo(vars, 0);
        vars[src.Length] = fsmVariable;
        pm.FsmVariables.BoolVariables = vars;
        pm.InitializeFSM();
    }

    /// <summary>
    /// Add FsmGameObject variable to PlayMakerFSM
    /// </summary>
    /// <param name="pm">PlayMakerFSM</param>
    /// <param name="fsmVariable">FsmGameObject variable</param>
    public static void AddVariable(this PlayMakerFSM pm, FsmGameObject fsmVariable)
    {
        pm.InitializeFSM();
        FsmGameObject[] src = pm.FsmVariables.GameObjectVariables;
        FsmGameObject[] vars = new FsmGameObject[src.Length + 1];
        src.CopyTo(vars, 0);
        vars[src.Length] = fsmVariable;
        pm.FsmVariables.GameObjectVariables = vars;
        pm.InitializeFSM();
    }

    /// <summary>
    /// Add FsmString variable to PlayMakerFSM
    /// </summary>
    /// <param name="pm">PlayMakerFSM</param>
    /// <param name="fsmVariable">FsmString variable</param>
    public static void AddVariable(this PlayMakerFSM pm, FsmString fsmVariable)
    {
        pm.InitializeFSM();
        FsmString[] src = pm.FsmVariables.StringVariables;
        FsmString[] vars = new FsmString[src.Length + 1];
        src.CopyTo(vars, 0);
        vars[src.Length] = fsmVariable;
        pm.FsmVariables.StringVariables = vars;
        pm.InitializeFSM();
    }

    /// <summary>
    /// Add FsmVector2 variable to PlayMakerFSM
    /// </summary>
    /// <param name="pm">PlayMakerFSM</param>
    /// <param name="fsmVariable">FsmVector2 variable</param>
    public static void AddVariable(this PlayMakerFSM pm, FsmVector2 fsmVariable)
    {
        pm.InitializeFSM();
        FsmVector2[] src = pm.FsmVariables.Vector2Variables;
        FsmVector2[] vars = new FsmVector2[src.Length + 1];
        src.CopyTo(vars, 0);
        vars[src.Length] = fsmVariable;
        pm.FsmVariables.Vector2Variables = vars;
        pm.InitializeFSM();
    }

    /// <summary>
    /// Add FsmVector3 variable to PlayMakerFSM
    /// </summary>
    /// <param name="pm">PlayMakerFSM</param>
    /// <param name="fsmVariable">FsmVector3 variable</param>
    public static void AddVariable(this PlayMakerFSM pm, FsmVector3 fsmVariable)
    {
        pm.InitializeFSM();
        FsmVector3[] src = pm.FsmVariables.Vector3Variables;
        FsmVector3[] vars = new FsmVector3[src.Length + 1];
        src.CopyTo(vars, 0);
        vars[src.Length] = fsmVariable;
        pm.FsmVariables.Vector3Variables = vars;
        pm.InitializeFSM();
    }

    /// <summary>
    /// Add FsmColor variable to PlayMakerFSM
    /// </summary>
    /// <param name="pm">PlayMakerFSM</param>
    /// <param name="fsmVariable">FsmColor variable</param>
    public static void AddVariable(this PlayMakerFSM pm, FsmColor fsmVariable)
    {
        pm.InitializeFSM();
        FsmColor[] src = pm.FsmVariables.ColorVariables;
        FsmColor[] vars = new FsmColor[src.Length + 1];
        src.CopyTo(vars, 0);
        vars[src.Length] = fsmVariable;
        pm.FsmVariables.ColorVariables = vars;
        pm.InitializeFSM();
    }

    /// <summary>
    /// Add FsmRect variable to PlayMakerFSM
    /// </summary>
    /// <param name="pm">PlayMakerFSM</param>
    /// <param name="fsmVariable">FsmRect variable</param>
    public static void AddVariable(this PlayMakerFSM pm, FsmRect fsmVariable)
    {
        pm.InitializeFSM();
        FsmRect[] src = pm.FsmVariables.RectVariables;
        FsmRect[] vars = new FsmRect[src.Length + 1];
        src.CopyTo(vars, 0);
        vars[src.Length] = fsmVariable;
        pm.FsmVariables.RectVariables = vars;
        pm.InitializeFSM();
    }

    /// <summary>
    /// Add FsmMaterial variable to PlayMakerFSM
    /// </summary>
    /// <param name="pm">PlayMakerFSM</param>
    /// <param name="fsmVariable">FsmMaterial variable</param>
    public static void AddVariable(this PlayMakerFSM pm, FsmMaterial fsmVariable)
    {
        pm.InitializeFSM();
        FsmMaterial[] src = pm.FsmVariables.MaterialVariables;
        FsmMaterial[] vars = new FsmMaterial[src.Length + 1];
        src.CopyTo(vars, 0);
        vars[src.Length] = fsmVariable;
        pm.FsmVariables.MaterialVariables = vars;
        pm.InitializeFSM();
    }

    /// <summary>
    /// Add FsmTexture variable to PlayMakerFSM
    /// </summary>
    /// <param name="pm">PlayMakerFSM</param>
    /// <param name="fsmVariable">FsmTexture variable</param>
    public static void AddVariable(this PlayMakerFSM pm, FsmTexture fsmVariable)
    {
        pm.InitializeFSM();
        FsmTexture[] src = pm.FsmVariables.TextureVariables;
        FsmTexture[] vars = new FsmTexture[src.Length + 1];
        src.CopyTo(vars, 0);
        vars[src.Length] = fsmVariable;
        pm.FsmVariables.TextureVariables = vars;
        pm.InitializeFSM();
    }

    /// <summary>
    /// Add FsmQuaternion variable to PlayMakerFSM
    /// </summary>
    /// <param name="pm">PlayMakerFSM</param>
    /// <param name="fsmVariable">FsmQuaternion variable</param>
    public static void AddVariable(this PlayMakerFSM pm, FsmQuaternion fsmVariable)
    {
        pm.InitializeFSM();
        FsmQuaternion[] src = pm.FsmVariables.QuaternionVariables;
        FsmQuaternion[] vars = new FsmQuaternion[src.Length + 1];
        src.CopyTo(vars, 0);
        vars[src.Length] = fsmVariable;
        pm.FsmVariables.QuaternionVariables = vars;
        pm.InitializeFSM();
    }

    /// <summary>
    /// Add FsmObject variable to PlayMakerFSM
    /// </summary>
    /// <param name="pm">PlayMakerFSM</param>
    /// <param name="fsmVariable">FsmObject variable</param>
    public static void AddVariable(this PlayMakerFSM pm, FsmObject fsmVariable)
    {
        pm.InitializeFSM();
        FsmObject[] src = pm.FsmVariables.ObjectVariables;
        FsmObject[] vars = new FsmObject[src.Length + 1];
        src.CopyTo(vars, 0);
        vars[src.Length] = fsmVariable;
        pm.FsmVariables.ObjectVariables = vars;
        pm.InitializeFSM();
    }

    /// <summary>
    /// Get a variable of specified type and name.
    /// </summary>
    /// <typeparam name="T">Type of variable to get.</typeparam>
    /// <param name="pm">PlayMakerFSM</param>
    /// <param name="ID">Name of the variable to find.</param>
    /// <returns>PlayMaker variable</returns>
    public static T GetVariable<T>(this PlayMakerFSM pm, string ID) where T : new()
    {
        try
        {
            object var = null;
            pm.Fsm.InitData();
            switch (typeof(T))
            {
                case Type t when t == typeof(FsmFloat): var = (object)pm.Fsm.Variables.FloatVariables.First(x => x.Name == ID); break;
                case Type t when t == typeof(FsmInt): var = (object)pm.Fsm.Variables.IntVariables.First(x => x.Name == ID); break;
                case Type t when t == typeof(FsmBool): var = (object)pm.Fsm.Variables.BoolVariables.First(x => x.Name == ID); break;
                case Type t when t == typeof(FsmGameObject): var = (object)pm.Fsm.Variables.GameObjectVariables.First(x => x.Name == ID); break;
                case Type t when t == typeof(FsmString): var = (object)pm.Fsm.Variables.StringVariables.First(x => x.Name == ID); break;
                case Type t when t == typeof(FsmVector2): var = (object)pm.Fsm.Variables.Vector2Variables.First(x => x.Name == ID); break;
                case Type t when t == typeof(FsmVector3): var = (object)pm.Fsm.Variables.Vector3Variables.First(x => x.Name == ID); break;
                case Type t when t == typeof(FsmColor): var = (object)pm.Fsm.Variables.ColorVariables.First(x => x.Name == ID); break;
                case Type t when t == typeof(FsmRect): var = (object)pm.Fsm.Variables.RectVariables.First(x => x.Name == ID); break;
                case Type t when t == typeof(FsmMaterial): var = (object)pm.Fsm.Variables.MaterialVariables.First(x => x.Name == ID); break;
                case Type t when t == typeof(FsmTexture): var = (object)pm.Fsm.Variables.TextureVariables.First(x => x.Name == ID); break;
                case Type t when t == typeof(FsmQuaternion): var = (object)pm.Fsm.Variables.QuaternionVariables.First(x => x.Name == ID); break;
                case Type t when t == typeof(FsmObject): var = (object)pm.Fsm.Variables.ObjectVariables.First(x => x.Name == ID); break;
            }
            return (T)var;
        }
        catch (Exception ex) { ModConsole.Error(ex.ToString()); return new T(); }
    }

    /// <summary>
    /// Adds an empty FsmState to the PlayMakerFSM
    /// </summary>
    /// <param name="pm">PlayMakerFSM</param>
    /// <param name="stateName">Name of the state</param>
    /// <returns>FsmState</returns>
    public static FsmState AddState(this PlayMakerFSM pm, string stateName)
    {
        FsmState state = GetState(pm, stateName);
        if (state != null) return state;

        pm.InitializeFSM();
        FsmState[] src = pm.FsmStates;
        FsmState[] vars = new FsmState[src.Length + 1];
        src.CopyTo(vars, 0);
        state = new FsmState(pm.Fsm);
        state.Name = stateName;
        vars[src.Length] = state;
        pm.Fsm.States = vars;

        return state;
    }

    /// <summary>
    /// Gets a FsmState from the PlayMakerFSM
    /// </summary>
    /// <param name="pm">PlayMakerFSM</param>
    /// <param name="stateName">Name of the state</param>
    /// <returns>FsmState</returns>
    public static FsmState GetState(this PlayMakerFSM pm, string stateName)
    {
        pm.InitializeFSM();
        for (int i = 0; i < pm.FsmStates.Length; i++)
        {
            if (pm.FsmStates[i].Name == stateName)
                return pm.FsmStates[i];
        }
        return null;
    }

    /// <summary>
    /// Gets a FsmState from the PlayMakerFSM
    /// </summary>
    /// <param name="pm">PlayMakerFSM</param>
    /// <param name="index">State index in array</param>
    /// <returns>FsmState</returns>
    public static FsmState GetState(this PlayMakerFSM pm, int index)
    {
        pm.InitializeFSM();
        return pm.FsmStates[index];
    }

    /// <summary>
    /// Adds a FsmTransition to the FsmState
    /// </summary>
    /// <param name="fs">FsmState</param>
    /// <param name="eventName">Name of the event</param>
    /// <param name="stateName">Name of the state</param>
    public static void AddTransition(this FsmState fs, string eventName, string stateName)
    {
        FsmTransition[] gt = new FsmTransition[fs.Transitions.Length + 1];
        fs.Transitions.CopyTo(gt, 0);
        gt[fs.Transitions.Length] = new FsmTransition
        {
            FsmEvent = GetEvent(fs.Fsm, eventName) ?? AddEvent(fs.Fsm, eventName),
            ToState = stateName
        };
        fs.Transitions = gt;
    }

    /// <summary>
    /// Gets a FsmTransition from the FsmState
    /// </summary>
    /// <param name="fs">FsmState</param>
    /// <param name="eventName">Name of the event</param>
    /// <returns>FsmTransition</returns>
    public static FsmTransition GetTransition(this FsmState fs, string eventName)
    {
        for (int i = 0; i < fs.Transitions.Length; i++)
        {
            if (fs.Transitions[i].FsmEvent.Name == eventName)
                return fs.Transitions[i];
        }
        return null;
    }

    /// <summary>
    /// Adds a FsmStateAction to the FsmState
    /// </summary>
    /// <param name="fs">FsmState</param>
    /// <param name="action">FsmStateAction action</param>
    public static void AddAction(this FsmState fs, FsmStateAction action)
    {
        FsmStateAction[] gt = new FsmStateAction[fs.Actions.Length + 1];
        fs.Actions.CopyTo(gt, 0);
        gt[fs.Actions.Length] = action;
        fs.Actions = gt;
    }

    /// <summary>
    /// Adds multiple FsmStateActions to the FsmState
    /// </summary>
    /// <param name="fs">FsmState</param>
    /// <param name="actions">FsmStateAction collection</param>
    public static void AddActions(this FsmState fs, System.Collections.Generic.ICollection<FsmStateAction> actions)
    {
        FsmStateAction[] gt = new FsmStateAction[fs.Actions.Length + actions.Count];
        fs.Actions.CopyTo(gt, 0);
        actions.CopyTo(gt, fs.Actions.Length);
        fs.Actions = gt;
    }

    /// <summary>
    /// Gets a FsmStateAction from the FsmState
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="fs">FsmState</param>
    /// <param name="index">index in state array</param>
    /// <returns></returns>
    public static T GetAction<T>(this FsmState fs, int index) where T : FsmStateAction
    {
        if (index >= fs.Actions.Length || index < 0) return null;
        return fs.Actions[index] as T;
    }

    /// <summary>
    /// Removes a FsmStateAction from the FsmState
    /// </summary>
    /// <param name="fs">FsmState</param>
    /// <param name="index">index in state array</param>
    public static void RemoveAction(this FsmState fs, int index)
    {
        FsmStateAction[] a = new FsmStateAction[fs.Actions.Length - 1];
        for (int i = 0, j = 0; i < a.Length; i++, j++)
        {
            if (j == index) j++;
            a[i] = fs.Actions[j];
        }
        fs.Actions = a;
    }

    /// <summary>
    /// Replaces a FsmStateAction in the FsmState
    /// </summary>
    /// <param name="fs">FsmState</param>
    /// <param name="index">index in state array</param>
    /// <param name="action">FsmStateAction action</param>
    public static void ReplaceAction(this FsmState fs, int index, FsmStateAction action)
    {
        fs.Actions[index] = action;
    }

    /// <summary>
    /// Inserts a FsmStateAction in the FsmState
    /// </summary>
    /// <param name="fs">FsmState</param>
    /// <param name="index">index in state array</param>
    /// <param name="action">FsmStateAction action</param>
    public static void InsertAction(this FsmState fs, int index, FsmStateAction action)
    {
        FsmStateAction[] _a = new FsmStateAction[fs.Actions.Length + 1];
        if (index < 0 || index >= fs.Actions.Length)
        {
            return;
        }

        for (int i = 0, j = 0; i < fs.Actions.Length; i++, j++)
        {
            if (j == index)
            {
                _a[j] = action;
                j++;
            }
            _a[j] = fs.Actions[i];
        }
        fs.Actions = _a;
    }
}

/// <summary>
/// Extension methods for PlayMakerArrayListProxy and PlayMakerHashTableProxy
/// </summary>
public static class PlayMakerProxyExtensions
{
    /// <summary>
    /// Returns PlayMakerArrayListProxy with specified referenceName
    /// </summary>
    /// <param name="go">current gameObject</param>
    /// <param name="referenceName">Reference name of the PlayMakerArrayListProxy</param>
    /// <returns>PlayMakerArrayListProxy</returns>
    public static PlayMakerArrayListProxy GetArrayListProxy(this GameObject go, string referenceName)
    {
        PlayMakerArrayListProxy[] pmalp = go.GetComponents<PlayMakerArrayListProxy>();

        if (pmalp == null) return null;

        for (int i = 0; i < pmalp.Length; i++)
        {
            if (pmalp[i].referenceName == referenceName) 
                return pmalp[i];
        }

        return null;
    }

    /// <summary>
    /// Returns PlayMakerHashTableProxy with specified referenceName
    /// </summary>
    /// <param name="go">current gameObject</param>
    /// <param name="referenceName">Reference name of the PlayMakerHashTableProxy</param>
    /// <returns>PlayMakerHashTableProxy</returns>
    public static PlayMakerHashTableProxy GetHashTableProxy(this GameObject go, string referenceName)
    {
        PlayMakerHashTableProxy[] pmhtp = go.GetComponents<PlayMakerHashTableProxy>();

        if (pmhtp == null) return null;

        for (int i = 0; i < pmhtp.Length; i++)
        {
            if (pmhtp[i].referenceName == referenceName) 
                return pmhtp[i];
        }

        return null;
    }
}

#endif
