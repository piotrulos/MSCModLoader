#if !Mini
using HutongGames.PlayMaker;
using System;
using System.Linq;

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
        public static void FsmInject(this GameObject gameObject, string stateName, Action hook)
        {
            FsmHook.FsmInject(gameObject, stateName, hook);
        }

  
        //------------------ BrennFuchS Playmaker Extensions ---------------------//       
        
        /// <summary>
        /// Initialize a PlayMakerFSM
        /// </summary>
        /// <param name="pm">FSM to initialize</param>
        public static void InitializeFSM(this PlayMakerFSM pm)
        {
            try { pm.Fsm.InitData(); }
            catch (Exception ex) { ModConsole.Error(ex.ToString()); }
        }

        /// <summary>
        /// Adds a GlobalTransition to the PlayMakerFSM
        /// </summary>
        /// <param name="pm">PlayMakerFSM</param>
        /// <param name="eventName">Name of the event</param>
        /// <param name="stateName">Name of the state</param>
        public static void AddGlobalTransition(this PlayMakerFSM pm, string eventName, string stateName)
        {
            try
            {
                pm.Fsm.InitData();
                var gT = pm.Fsm.GlobalTransitions.ToList();
                gT.Add(new FsmTransition { FsmEvent = pm.FsmEvents.First(x => x.Name == eventName), ToState = stateName });
                pm.Fsm.GlobalTransitions = gT.ToArray();
                pm.Fsm.InitData();
            }
            catch (Exception ex) { ModConsole.Error(ex.ToString()); }
        }

        /// <summary>
        /// Gets a GlobalTransition of the PlayMakerFSM
        /// </summary>
        /// <param name="pm">PlayMakerFSM</param>
        /// <param name="eventName">Name of the event</param>
        /// <returns>FsmTransition</returns>
        public static FsmTransition GetGlobalTransition(this PlayMakerFSM pm, string eventName)
        {
            try
            {
                pm.Fsm.InitData();
                var gT = pm.Fsm.GlobalTransitions.First(x => x.EventName == eventName);
                if (gT != null) return gT;
                else return null;
            }
            catch (Exception ex) { ModConsole.Error(ex.ToString()); return null; }
        }

        /// <summary>
        /// Add FsmFloat variable to PlayMakerFSM
        /// </summary>
        /// <param name="pm">PlayMakerFSM</param>
        /// <param name="fsmVariable">FsmFloat variable</param>
        public static void AddVariable(this PlayMakerFSM pm, FsmFloat fsmVariable)
        {
            try
            {
                var vars = pm.Fsm.Variables.FloatVariables.ToList();
                vars.Add(fsmVariable);
                pm.Fsm.Variables.FloatVariables = vars.ToArray();
            }
            catch (Exception ex) { ModConsole.Error(ex.ToString()); }
        }
        /// <summary>
        /// Add FsmInt variable to PlayMakerFSM
        /// </summary>
        /// <param name="pm">PlayMakerFSM</param>
        /// <param name="fsmVariable">FsmInt variable</param>
        public static void AddVariable(this PlayMakerFSM pm, FsmInt fsmVariable)
        {
            try
            {
                var vars = pm.Fsm.Variables.IntVariables.ToList();
                vars.Add(fsmVariable);
                pm.Fsm.Variables.IntVariables = vars.ToArray();
            }
            catch (Exception ex) { ModConsole.Error(ex.ToString()); }
        }

        /// <summary>
        /// Add FsmBool variable to PlayMakerFSM
        /// </summary>
        /// <param name="pm">PlayMakerFSM</param>
        /// <param name="fsmVariable">FsmBool variable</param>
        public static void AddVariable(this PlayMakerFSM pm, FsmBool fsmVariable)
        {
            try
            {
                var vars = pm.Fsm.Variables.BoolVariables.ToList();
                vars.Add(fsmVariable);
                pm.Fsm.Variables.BoolVariables = vars.ToArray();
            }
            catch (Exception ex) { ModConsole.Error(ex.ToString()); }
        }

        /// <summary>
        /// Add FsmGameObject variable to PlayMakerFSM
        /// </summary>
        /// <param name="pm">PlayMakerFSM</param>
        /// <param name="fsmVariable">FsmGameObject variable</param>
        public static void AddVariable(this PlayMakerFSM pm, FsmGameObject fsmVariable)
        {
            try
            {
                var vars = pm.Fsm.Variables.GameObjectVariables.ToList();
                vars.Add(fsmVariable);
                pm.Fsm.Variables.GameObjectVariables = vars.ToArray();
            }
            catch (Exception ex) { ModConsole.Error(ex.ToString()); }
        }

        /// <summary>
        /// Add FsmString variable to PlayMakerFSM
        /// </summary>
        /// <param name="pm">PlayMakerFSM</param>
        /// <param name="fsmVariable">FsmString variable</param>
        public static void AddVariable(this PlayMakerFSM pm, FsmString fsmVariable)
        {
            try
            {
                var vars = pm.Fsm.Variables.StringVariables.ToList();
                vars.Add(fsmVariable);
                pm.Fsm.Variables.StringVariables = vars.ToArray();
            }
            catch (Exception ex) { ModConsole.Error(ex.ToString()); }
        }

        /// <summary>
        /// Add FsmVector2 variable to PlayMakerFSM
        /// </summary>
        /// <param name="pm">PlayMakerFSM</param>
        /// <param name="fsmVariable">FsmVector2 variable</param>
        public static void AddVariable(this PlayMakerFSM pm, FsmVector2 fsmVariable)
        {
            try
            {
                var vars = pm.Fsm.Variables.Vector2Variables.ToList();
                vars.Add(fsmVariable);
                pm.Fsm.Variables.Vector2Variables = vars.ToArray();
            }
            catch (Exception ex) { ModConsole.Error(ex.ToString()); }
        }

        /// <summary>
        /// Add FsmVector3 variable to PlayMakerFSM
        /// </summary>
        /// <param name="pm">PlayMakerFSM</param>
        /// <param name="fsmVariable">FsmVector3 variable</param>
        public static void AddVariable(this PlayMakerFSM pm, FsmVector3 fsmVariable)
        {
            try
            {
                var vars = pm.Fsm.Variables.Vector3Variables.ToList();
                vars.Add(fsmVariable);
                pm.Fsm.Variables.Vector3Variables = vars.ToArray();
            }
            catch (Exception ex) { ModConsole.Error(ex.ToString()); }
        }

        /// <summary>
        /// Add FsmColor variable to PlayMakerFSM
        /// </summary>
        /// <param name="pm">PlayMakerFSM</param>
        /// <param name="fsmVariable">FsmColor variable</param>
        public static void AddVariable(this PlayMakerFSM pm, FsmColor fsmVariable)
        {
            try
            {
                var vars = pm.Fsm.Variables.ColorVariables.ToList();
                vars.Add(fsmVariable);
                pm.Fsm.Variables.ColorVariables = vars.ToArray();
            }
            catch (Exception ex) { ModConsole.Error(ex.ToString()); }
        }

        /// <summary>
        /// Add FsmRect variable to PlayMakerFSM
        /// </summary>
        /// <param name="pm">PlayMakerFSM</param>
        /// <param name="fsmVariable">FsmRect variable</param>
        public static void AddVariable(this PlayMakerFSM pm, FsmRect fsmVariable)
        {
            try
            {
                var vars = pm.Fsm.Variables.RectVariables.ToList();
                vars.Add(fsmVariable);
                pm.Fsm.Variables.RectVariables = vars.ToArray();
            }
            catch (Exception ex) { ModConsole.Error(ex.ToString()); }
        }

        /// <summary>
        /// Add FsmMaterial variable to PlayMakerFSM
        /// </summary>
        /// <param name="pm">PlayMakerFSM</param>
        /// <param name="fsmVariable">FsmMaterial variable</param>
        public static void AddVariable(this PlayMakerFSM pm, FsmMaterial fsmVariable)
        {
            try
            {
                var vars = pm.Fsm.Variables.MaterialVariables.ToList();
                vars.Add(fsmVariable);
                pm.Fsm.Variables.MaterialVariables = vars.ToArray();
            }
            catch (Exception ex) { ModConsole.Error(ex.ToString()); }
        }

        /// <summary>
        /// Add FsmTexture variable to PlayMakerFSM
        /// </summary>
        /// <param name="pm">PlayMakerFSM</param>
        /// <param name="fsmVariable">FsmTexture variable</param>
        public static void AddVariable(this PlayMakerFSM pm, FsmTexture fsmVariable)
        {
            try
            {
                var vars = pm.Fsm.Variables.TextureVariables.ToList();
                vars.Add(fsmVariable);
                pm.Fsm.Variables.TextureVariables = vars.ToArray();
            }
            catch (Exception ex) { ModConsole.Error(ex.ToString()); }
        }

        /// <summary>
        /// Add FsmQuaternion variable to PlayMakerFSM
        /// </summary>
        /// <param name="pm">PlayMakerFSM</param>
        /// <param name="fsmVariable">FsmQuaternion variable</param>
        public static void AddVariable(this PlayMakerFSM pm, FsmQuaternion fsmVariable)
        {
            try
            {
                var vars = pm.Fsm.Variables.QuaternionVariables.ToList();
                vars.Add(fsmVariable);
                pm.Fsm.Variables.QuaternionVariables = vars.ToArray();
            }
            catch (Exception ex) { ModConsole.Error(ex.ToString()); }
        }

        /// <summary>
        /// Add FsmObject variable to PlayMakerFSM
        /// </summary>
        /// <param name="pm">PlayMakerFSM</param>
        /// <param name="fsmVariable">FsmObject variable</param>
        public static void AddVariable(this PlayMakerFSM pm, FsmObject fsmVariable)
        {
            try
            {
                var vars = pm.Fsm.Variables.ObjectVariables.ToList();
                vars.Add(fsmVariable);
                pm.Fsm.Variables.ObjectVariables = vars.ToArray();
            }
            catch (Exception ex) { ModConsole.Error(ex.ToString()); }
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
            FsmState fs = null;

            try
            {
                pm.Fsm.InitData();
                var fss = pm.FsmStates.ToList();
                if (fss.First(x => x.Name == stateName) == null)
                {
                    fs = new FsmState(pm.Fsm);
                    fs.Name = stateName;
                    fss.Add(fs);
                }
                return fs;
            }
            catch (Exception ex)
            {
                ModConsole.Error(ex.ToString());
                return null;
            }
        }

        /// <summary>
        /// Gets a FsmState from the PlayMakerFSM
        /// </summary>
        /// <param name="pm">PlayMakerFSM</param>
        /// <param name="stateName">Name of the state</param>
        /// <returns>FsmState</returns>
        public static FsmState GetState(this PlayMakerFSM pm, string stateName)
        {
            try
            {
                pm.Fsm.InitData();
                return pm.FsmStates.First(x => x.Name == stateName);
            }
            catch (Exception ex)
            {
                ModConsole.Error(ex.ToString());
                return null;
            }
        }

        /// <summary>
        /// Gets a FsmState from the PlayMakerFSM
        /// </summary>
        /// <param name="pm">PlayMakerFSM</param>
        /// <param name="index">State index in array</param>
        /// <returns>FsmState</returns>
        public static FsmState GetState(this PlayMakerFSM pm, int index)
        {
            try
            {
                pm.Fsm.InitData();
                return pm.FsmStates[index];
            }
            catch (Exception ex)
            {
                ModConsole.Error(ex.ToString());
                return null;
            }
        }

        /// <summary>
        /// Adds a FsmTransition to the FsmState
        /// </summary>
        /// <param name="fs">FsmState</param>
        /// <param name="eventName">Name of the event</param>
        /// <param name="stateName">Name of the state</param>
        public static void AddTransition(this FsmState fs, string eventName, string stateName)
        {
            try
            {
                fs.Fsm.InitData();
                var t = fs.Transitions.ToList();
                if (t.First(x => x.EventName == eventName) == null) t.Add(new FsmTransition { FsmEvent = fs.Fsm.Events.First(x => x.Name == eventName), ToState = stateName });
                else ModConsole.Error($"Transition for {eventName} in State {fs.Name} exists already!");
                fs.Fsm.GlobalTransitions = t.ToArray();
                fs.Fsm.InitData();
            }
            catch (Exception ex) { ModConsole.Error(ex.ToString()); }
        }

        /// <summary>
        /// Gets a FsmTransition from the FsmState
        /// </summary>
        /// <param name="fs">FsmState</param>
        /// <param name="eventName">Name of the event</param>
        /// <returns>FsmTransition</returns>
        public static FsmTransition GetTransition(this FsmState fs, string eventName)
        {
            try
            {
                if (fs.Transitions.First(x => x.EventName == eventName) != null) return fs.Transitions.First(x => x.EventName == eventName);
                else { ModConsole.Error($"Transition for {eventName} in State {fs.Name} doesn't exist!"); return null; }
            }
            catch (Exception ex) { ModConsole.Error(ex.ToString()); return null; }
        }

        /// <summary>
        /// Adds a FsmStateAction to the FsmState
        /// </summary>
        /// <param name="fs">FsmState</param>
        /// <param name="action">FsmStateAction action</param>
        public static void AddAction(this FsmState fs, FsmStateAction action)
        {
            try
            {
                var a = fs.Actions.ToList();
                a.Add(action);
                fs.Actions = a.ToArray();
            }
            catch (Exception ex) { ModConsole.Error(ex.ToString()); }
        }

        /// <summary>
        /// Adds multiple FsmStateActions to the FsmState
        /// </summary>
        /// <param name="fs">FsmState</param>
        /// <param name="actions">FsmStateAction collection</param>
        public static void AddActions(this FsmState fs, System.Collections.Generic.ICollection<FsmStateAction> actions)
        {
            for (int i = 0; i < actions.Count; i++) fs.AddAction(actions.ToArray()[i]);
        }

        /// <summary>
        /// Gets a FsmStateAction from the FsmState
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fs">FsmState</param>
        /// <param name="index">index in state array</param>
        /// <returns></returns>
        public static T GetAction<T>(this FsmState fs, int index) where T : class, new()
        {
            try { return (T)((object)fs.Actions[index]); }
            catch (Exception ex) { ModConsole.Error(ex.ToString()); return new T(); }
        }

        /// <summary>
        /// Removes a FsmStateAction from the FsmState
        /// </summary>
        /// <param name="fs">FsmState</param>
        /// <param name="index">index in state array</param>
        public static void RemoveAction(this FsmState fs, int index)
        {
            try
            {
                var a = fs.Actions.ToList();
                a.RemoveAt(index);
                fs.Actions = a.ToArray();
            }
            catch (Exception ex) { ModConsole.Error(ex.ToString()); }
        }

        /// <summary>
        /// Replaces a FsmStateAction in the FsmState
        /// </summary>
        /// <param name="fs">FsmState</param>
        /// <param name="index">index in state array</param>
        /// <param name="action">FsmStateAction action</param>
        public static void ReplaceAction(this FsmState fs, int index, FsmStateAction action)
        {
            try
            {
                var a = fs.Actions.ToList();
                fs.RemoveAction(index);
                a.Insert(index, action);
                fs.Actions = a.ToArray();
            }
            catch (Exception ex) { ModConsole.Error(ex.ToString()); }
        }

        /// <summary>
        /// Inserts a FsmStateAction in the FsmState
        /// </summary>
        /// <param name="fs">FsmState</param>
        /// <param name="index">index in state array</param>
        /// <param name="action">FsmStateAction action</param>
        public static void InsertAction(this FsmState fs, int index, FsmStateAction action)
        {
            try
            {
                var a = fs.Actions.ToList();
                a.Insert(index, action);
                fs.Actions = a.ToArray();
            }
            catch (Exception ex) { ModConsole.Error(ex.ToString()); }
        }
    }
}
#endif