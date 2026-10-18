#if !Mini
using HutongGames.PlayMaker;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

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
        public string hookName;
        public Action action;
        public bool everyFrame = false;
        public MSCL_PMHook(Action action, bool everyFrame = false)
        {
            this.action = action;
            this.everyFrame = everyFrame;
            hookName = action.Method.Name;
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
#if MSC
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
#endif
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
    /// <typeparam name="T">FsmStateAction Action type</typeparam>
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
    /// <returns>FsmEvent</returns>
    public static FsmEvent GetEvent(this PlayMakerFSM fsm, string eventName) => fsm.Fsm.GetEvent(eventName);

    private static FsmEvent GetEvent(Fsm fsm, string name) => fsm.GetEvent(name);


    /// <summary>
    /// Add event to PlayMakerFSM
    /// </summary>
    /// <param name="pm">PlayMakerFSM</param>
    /// <param name="eventName">event name</param>
    /// <returns>FsmEvent</returns>
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
    /// <typeparam name="T">Type of FSM Variable to get.</typeparam>
    /// <param name="pm">PlayMakerFSM from which to get the variable.</param>
    /// <param name="ID">Name of the variable to find.</param>
    /// <returns>The specified variable if it is found, otherwise null.</returns>
    public static T GetVariable<T>(this PlayMakerFSM pm, string ID) where T : class, new()
    {
        pm.InitializeFSM();
        Type type = typeof(T);
        FsmVariables allVars = pm.Fsm.Variables;

        if (type == typeof(FsmFloat)) return allVars.FindFsmFloat(ID) as T;
        if (type == typeof(FsmInt)) return allVars.FindFsmInt(ID) as T;
        if (type == typeof(FsmBool)) return allVars.FindFsmBool(ID) as T;
        if (type == typeof(FsmGameObject)) return allVars.FindFsmGameObject(ID) as T;
        if (type == typeof(FsmString)) return allVars.FindFsmString(ID) as T;
        if (type == typeof(FsmVector2)) return allVars.FindFsmVector2(ID) as T;
        if (type == typeof(FsmVector3)) return allVars.FindFsmVector3(ID) as T;
        if (type == typeof(FsmColor)) return allVars.FindFsmColor(ID) as T;
        if (type == typeof(FsmRect)) return allVars.FindFsmRect(ID) as T;
        if (type == typeof(FsmMaterial)) return allVars.FindFsmMaterial(ID) as T;
        if (type == typeof(FsmTexture)) return allVars.FindFsmTexture(ID) as T;
        if (type == typeof(FsmQuaternion)) return allVars.FindFsmQuaternion(ID) as T;
        if (type == typeof(FsmObject)) return allVars.FindFsmObject(ID) as T;

        ModConsole.Warning($"PlayMakerExtension.GetVariable - Type of <b>{type.Name}</b> is not a NamedVariable, please make sure you are using the correct type (e.g. <b>FsmFloat</b> instead of <b>float</b>)");
        return null;
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
    /// <typeparam name="T">FsmStateAction Action type</typeparam>
    /// <param name="fs">FsmState</param>
    /// <param name="index">index in state array</param>
    /// <returns>FsmStateAction</returns>
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
        if (index < 0 || index >= fs.Actions.Length)
        {
            ModConsole.Error($"PlayMakerExtension.ReplaceAction: index <b>{index}</b> for state <b>{fs.Name}</b> out of range");
            return;
        }
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
        if (index < 0 || index >= fs.Actions.Length + 1)
        {
            ModConsole.Error($"PlayMakerExtension.InsertAction: index <b>{index}</b> for state <b>{fs.Name}</b> out of range");
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

/// <summary>
/// Read DLLs to get native .dll references info
/// </summary>
internal class PeModuleRefs
{
    static byte[] d;
    static int[] rows = new int[64];
    static int heap;

    static int U16(int o) { return BitConverter.ToUInt16(d, o); }
    static int I32(int o) { return BitConverter.ToInt32(d, o); }

    static int secStart, secCount;
    static int Rva(int rva)
    {
        for (int i = 0; i < secCount; i++)
        {
            int s = secStart + i * 40;
            int vs = I32(s + 8), va = I32(s + 12), raw = I32(s + 16), ptr = I32(s + 20);
            if (rva >= va && rva < va + Math.Max(vs, raw)) return rva - va + ptr;
        }
        throw new Exception("RVA not in any section");
    }

    // simple table index size
    static int Idx(int t) { return rows[t] >= 65536 ? 4 : 2; }
    // coded index size
    static int Coded(int bits, params int[] tables)
    {
        int max = 0;
        foreach (int t in tables) max = Math.Max(max, rows[t]);
        return max < (1 << (16 - bits)) ? 2 : 4;
    }

    static int RowSize(int t)
    {
        int S = (heap & 1) != 0 ? 4 : 2;
        int G = (heap & 2) != 0 ? 4 : 2;
        int B = (heap & 4) != 0 ? 4 : 2;
        int TypeDefOrRef = Coded(2, 0x02, 0x01, 0x1B);
        int ResScope = Coded(2, 0x00, 0x1A, 0x23, 0x01);
        int MemberRefParent = Coded(3, 0x02, 0x01, 0x1A, 0x06, 0x1B);
        int HasConstant = Coded(2, 0x04, 0x08, 0x17);
        int HasCustomAttr = Coded(5, 0x06, 0x04, 0x01, 0x02, 0x08, 0x09, 0x0A, 0x00, 0x0E, 0x17, 0x14,
                                     0x11, 0x1A, 0x1B, 0x20, 0x23, 0x26, 0x27, 0x28, 0x2A, 0x2C, 0x2B);
        int CustomAttrType = Coded(3, 0x06, 0x0A);
        int HasFieldMarshal = Coded(1, 0x04, 0x08);
        int HasDeclSecurity = Coded(2, 0x02, 0x06, 0x20);
        int HasSemantics = Coded(1, 0x14, 0x17);
        int MethodDefOrRef = Coded(1, 0x06, 0x0A);

        return t switch
        {
            0x00 => 2 + S + 3 * G,                                    // Module
            0x01 => ResScope + 2 * S,                                 // TypeRef
            0x02 => 4 + 2 * S + TypeDefOrRef + Idx(0x04) + Idx(0x06), // TypeDef
            0x03 => Idx(0x04),                                        // FieldPtr
            0x04 => 2 + S + B,                                        // Field
            0x05 => Idx(0x06),                                        // MethodPtr
            0x06 => 4 + 2 + 2 + S + B + Idx(0x08),                    // MethodDef
            0x07 => Idx(0x08),                                        // ParamPtr
            0x08 => 2 + 2 + S,                                        // Param
            0x09 => Idx(0x02) + TypeDefOrRef,                         // InterfaceImpl
            0x0A => MemberRefParent + S + B,                          // MemberRef
            0x0B => 2 + HasConstant + B,                              // Constant
            0x0C => HasCustomAttr + CustomAttrType + B,               // CustomAttribute
            0x0D => HasFieldMarshal + B,                              // FieldMarshal
            0x0E => 2 + HasDeclSecurity + B,                          // DeclSecurity
            0x0F => 2 + 4 + Idx(0x02),                                // ClassLayout
            0x10 => 4 + Idx(0x04),                                    // FieldLayout
            0x11 => B,                                                // StandAloneSig
            0x12 => Idx(0x02) + Idx(0x14),                            // EventMap
            0x13 => Idx(0x14),                                        // EventPtr
            0x14 => 2 + S + TypeDefOrRef,                             // Event
            0x15 => Idx(0x02) + Idx(0x17),                            // PropertyMap
            0x16 => Idx(0x17),                                        // PropertyPtr
            0x17 => 2 + S + B,                                        // Property
            0x18 => 2 + Idx(0x06) + HasSemantics,                     // MethodSemantics
            0x19 => Idx(0x02) + 2 * MethodDefOrRef,                   // MethodImpl
            _ => throw new Exception("Unexpected table " + t),
        };
    }

    //Reset statics
    static void Reset()
    {
        secCount = 0;
        secStart = 0;
        d = null;
        rows = new int[64];
        heap = 0;

    }
    internal static string[] ReadNativeDlls(string file)
    {
        List<string> result = new List<string>();
        Reset();
        d = File.ReadAllBytes(file);

        // --- PE headers ---
        int pe = I32(0x3C);
        secCount = U16(pe + 6);
        int optSize = U16(pe + 20);
        int opt = pe + 24;
        bool pe64 = U16(opt) == 0x20B;
        secStart = opt + optSize;
        int clrRva = I32(opt + (pe64 ? 112 : 96) + 14 * 8);   // data directory 14 = CLR header
        if (clrRva == 0) { Console.WriteLine("Not a .NET assembly."); return null; }

        // --- CLR header -> metadata root ---
        int cli = Rva(clrRva);
        int md = Rva(I32(cli + 8));
        if (I32(md) != 0x424A5342) { Console.WriteLine("Bad metadata signature."); return null; }
        int p = md + 16 + I32(md + 12);               // skip version string
        int streams = U16(p + 2);
        p += 4;

        int tilde = -1, strings = -1;
        for (int i = 0; i < streams; i++)
        {
            int off = I32(p), q = p + 8;
            StringBuilder sb = new StringBuilder();
            while (d[q] != 0) sb.Append((char)d[q++]);
            p += 8 + ((sb.Length + 1 + 3) & ~3);
            if (sb.ToString() == "#~" || sb.ToString() == "#-") tilde = md + off;
            else if (sb.ToString() == "#Strings") strings = md + off;
        }
        if (tilde < 0 || strings < 0) { Console.WriteLine("Metadata streams not found."); return null; }

        // --- Table stream header ---
        heap = d[tilde + 6];
        ulong valid = BitConverter.ToUInt64(d, tilde + 8);
        p = tilde + 24;
        for (int i = 0; i < 64; i++)
            if ((valid & (1UL << i)) != 0) { rows[i] = I32(p); p += 4; }
        if ((heap & 0x40) != 0) p += 4;               // extra data

        if (rows[0x1A] == 0) { Console.WriteLine("No ModuleRef table (no native references)."); return null; }

        // skip all tables before ModuleRef (0x1A)
        for (int t = 0; t < 0x1A; t++) p += rows[t] * RowSize(t);

        int strIdxSize = (heap & 1) != 0 ? 4 : 2;
        for (int r = 0; r < rows[0x1A]; r++)
        {
            int idx = strIdxSize == 4 ? I32(p) : U16(p);
            p += strIdxSize;
            int s = strings + idx, e = s;
            while (d[e] != 0) e++;
            result.Add(Encoding.UTF8.GetString(d, s, e - s));
        }
        return [.. result];
    }
}


#endif
