#if !Mini
using HutongGames.PlayMaker;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MSCLoader.Helper;
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
[Obsolete("This namespace is only designed for ModLoaderPro compatibility pack, DO NOT USE", true)]
public static class ModHelper
{
    [Obsolete("This namespace is only designed for ModLoaderPro compatibility pack, DO NOT USE", true)]
    public static void MakePickable(this GameObject gameObject, bool includeTag = true) => LoadAssets.MakeGameObjectPickable(gameObject);
    [Obsolete("This namespace is only designed for ModLoaderPro compatibility pack, DO NOT USE", true)]
    public static Transform GetTransform(string parentPath, string childPath) => GameObject.Find(parentPath)?.transform.Find(childPath);
    [Obsolete("This namespace is only designed for ModLoaderPro compatibility pack, DO NOT USE", true)]
    public static GameObject GetGameObject(string parentPath, string childPath) => GameObject.Find(parentPath)?.transform.Find(childPath).gameObject;
    [Obsolete("This namespace is only designed for ModLoaderPro compatibility pack, DO NOT USE", true)]
    public static void PlaySound3D(this Transform transform, string type, string variation, float volume = 1f, float pitch = 1f) => MasterAudio.PlaySound3DAndForget(type, transform, variationName: variation, volumePercentage: volume, pitch: pitch);
    [Obsolete("This namespace is only designed for ModLoaderPro compatibility pack, DO NOT USE", true)]
    public static void PlaySound3D(this Vector3 vector3, string type, string variation, float volume = 1f, float pitch = 1f) => MasterAudio.PlaySound3DAtVector3AndForget(type, vector3, variationName: variation, volumePercentage: volume, pitch: pitch);
    [Obsolete("This namespace is only designed for ModLoaderPro compatibility pack, DO NOT USE", true)]
    public static T SelectRandom<T>(this IList<T> list) => list[UnityEngine.Random.Range(0, list.Count)];
    [Obsolete("This namespace is only designed for ModLoaderPro compatibility pack, DO NOT USE", true)]
    public static bool InLayerMask(this LayerMask layerMask, int layer) => layerMask == (layerMask | (1 << layer));
    [Obsolete("This namespace is only designed for ModLoaderPro compatibility pack, DO NOT USE", true)]
    public static void SetParent(this Transform transform, Transform parent, Vector3 position, Vector3 rotation, Vector3 scale, string name = "")
    {
        if (name != "") transform.name = name;

        transform.parent = parent;
        transform.localPosition = position;
        transform.localEulerAngles = rotation;
        transform.localScale = scale;
    }
    [Obsolete("This namespace is only designed for ModLoaderPro compatibility pack, DO NOT USE", true)]
    public static void OpenWebsite(string url) => Application.OpenURL(url);
    [Obsolete("This namespace is only designed for ModLoaderPro compatibility pack, DO NOT USE", true)]
    public static string GetImagesFolder() => $@"{Path.GetFullPath(".")}\Images";
}

[Obsolete("This class requires user to have 'Compatibility References' installed, consider not 'using MSCLoader.Helper;'")]
public static class PlayMakerHelper
{
    [Obsolete("This property requires user to have 'Compatibility References' installed, consider not 'using MSCLoader.Helper;' and check expanded base extensions (usually under similar name)")]
    public static FsmBool FSMGUIUse { get; internal set; }
    [Obsolete("This property requires user to have 'Compatibility References' installed, consider not 'using MSCLoader.Helper;' and check expanded base extensions (usually under similar name)")]
    public static FsmBool FSMGUIAssemble { get; internal set; }
    [Obsolete("This property requires user to have 'Compatibility References' installed, consider not 'using MSCLoader.Helper;' and check expanded base extensions (usually under similar name)")]
    public static FsmBool FSMGUIDisassemble { get; internal set; }
    [Obsolete("This property requires user to have 'Compatibility References' installed, consider not 'using MSCLoader.Helper;' and check expanded base extensions (usually under similar name)")]
    public static FsmBool FSMGUIBuy { get; internal set; }
    [Obsolete("This property requires user to have 'Compatibility References' installed, consider not 'using MSCLoader.Helper;' and check expanded base extensions (usually under similar name)")]
    public static FsmBool FSMGUIDrive { get; internal set; }
    [Obsolete("This property requires user to have 'Compatibility References' installed, consider not 'using MSCLoader.Helper;' and check expanded base extensions (usually under similar name)")]
    public static FsmBool FSMGUIPassenger { get; internal set; }
    [Obsolete("This property requires user to have 'Compatibility References' installed, consider not 'using MSCLoader.Helper;' and check expanded base extensions (usually under similar name)")]
    public static FsmString FSMGUIInteraction { get; internal set; }
    [Obsolete("This property requires user to have 'Compatibility References' installed, consider not 'using MSCLoader.Helper;' and check expanded base extensions (usually under similar name)")]
    public static FsmString FSMGUISubtitle { get; internal set; }
    [Obsolete("This property requires user to have 'Compatibility References' installed, consider not 'using MSCLoader.Helper;' and check expanded base extensions (usually under similar name)")]
    public static bool GUIUse { get => FSMGUIUse.Value; set => FSMGUIUse.Value = value; }
    [Obsolete("This property requires user to have 'Compatibility References' installed, consider not 'using MSCLoader.Helper;' and check expanded base extensions (usually under similar name)")]
    public static bool GUIAssemble { get => FSMGUIAssemble.Value; set => FSMGUIAssemble.Value = value; }
    [Obsolete("This property requires user to have 'Compatibility References' installed, consider not 'using MSCLoader.Helper;' and check expanded base extensions (usually under similar name)")]
    public static bool GUIDisassemble { get => FSMGUIDisassemble.Value; set => FSMGUIDisassemble.Value = value; }
    [Obsolete("This property requires user to have 'Compatibility References' installed, consider not 'using MSCLoader.Helper;' and check expanded base extensions (usually under similar name)")]
    public static bool GUIBuy { get => FSMGUIBuy.Value; set => FSMGUIBuy.Value = value; }
    [Obsolete("This property requires user to have 'Compatibility References' installed, consider not 'using MSCLoader.Helper;' and check expanded base extensions (usually under similar name)")]
    public static bool GUIDrive { get => FSMGUIDrive.Value; set => FSMGUIDrive.Value = value; }
    [Obsolete("This property requires user to have 'Compatibility References' installed, consider not 'using MSCLoader.Helper;' and check expanded base extensions (usually under similar name)")]
    public static bool GUIPassenger { get => FSMGUIPassenger.Value; set => FSMGUIPassenger.Value = value; }
    [Obsolete("This property requires user to have 'Compatibility References' installed, consider not 'using MSCLoader.Helper;' and check expanded base extensions (usually under similar name)")]
    public static string GUIInteraction { get => FSMGUIInteraction.Value; set => FSMGUIInteraction.Value = value; }
    [Obsolete("This property requires user to have 'Compatibility References' installed, consider not 'using MSCLoader.Helper;' and check expanded base extensions (usually under similar name)")]
    public static string GUISubtitle { get => FSMGUISubtitle.Value; set => FSMGUISubtitle.Value = value; }
    [Obsolete("DON'T USE THIS WILL MESS UP REFERENCE")]
    static PlayMakerHelper()
    {
        FSMGUIUse = GetGlobalVariable<FsmBool>("GUIuse");
        FSMGUIAssemble = GetGlobalVariable<FsmBool>("GUIassemble");
        FSMGUIDisassemble = GetGlobalVariable<FsmBool>("GUIdisassemble");
        FSMGUIBuy = GetGlobalVariable<FsmBool>("GUIbuy");
        FSMGUIDrive = GetGlobalVariable<FsmBool>("GUIdrive");
        FSMGUIPassenger = GetGlobalVariable<FsmBool>("GUIpassenger");
        FSMGUIInteraction = GetGlobalVariable<FsmString>("GUIinteraction");
        FSMGUISubtitle = GetGlobalVariable<FsmString>("GUIsubtitle");
    }
    [Obsolete("This namespace is only designed for ModLoaderPro compatibility pack, DO NOT USE", true)]
    public static PlayMakerFSM GetPlayMakerFSM(this GameObject gameObject, string fsmName) => gameObject.GetPlayMaker(fsmName);
    [Obsolete("This namespace is only designed for ModLoaderPro compatibility pack, DO NOT USE", true)]
    public static PlayMakerFSM GetPlayMakerFSM(this Transform transform, string fsmName) => transform.gameObject.GetPlayMakerFSM(fsmName);
    [Obsolete("This namespace is only designed for ModLoaderPro compatibility pack, DO NOT USE", true)]
    public static T GetAction<T>(this FsmState state, int actionIndex) where T : FsmStateAction
    {
        if (state.Actions[actionIndex] is T) return state.Actions[actionIndex] as T;
        else throw new Exception($"GetAction<T>: Action of specified type {typeof(T)} can't be found on index {actionIndex} in state {state.Name} on FSM {state.Fsm.Name} on GameObject {state.Fsm.OwnerName}");
    }
    [Obsolete("This namespace is only designed for ModLoaderPro compatibility pack, DO NOT USE", true)]
    public static T GetAction<T>(this PlayMakerFSM fsm, string stateName, int actionIndex) where T : FsmStateAction => fsm.GetState(stateName).GetAction<T>(actionIndex);
    [Obsolete("This namespace is only designed for ModLoaderPro compatibility pack, DO NOT USE", true)]
    public static T GetAction<T>(this PlayMakerFSM fsm, int stateIndex, int actionIndex) where T : FsmStateAction => fsm.GetState(stateIndex).GetAction<T>(actionIndex);
    [Obsolete("This namespace is only designed for ModLoaderPro compatibility pack, DO NOT USE", true)]
    public static void InsertAction(this FsmState state, int actionIndex, FsmStateAction action)
    {
        List<FsmStateAction> actions = state.Actions.ToList();
        actions.Insert(actionIndex, action);
        state.Actions = actions.ToArray();
    }
    [Obsolete("This namespace is only designed for ModLoaderPro compatibility pack, DO NOT USE", true)]
    public static void InsertAction(this PlayMakerFSM fsm, string stateName, int actionIndex, FsmStateAction action) => fsm.GetState(stateName).InsertAction(actionIndex, action);
    [Obsolete("This namespace is only designed for ModLoaderPro compatibility pack, DO NOT USE", true)]
    public static void AddAction(this FsmState state, FsmStateAction action)
    {
        List<FsmStateAction> actions = state.Actions.ToList();
        actions.Add(action);
        state.Actions = actions.ToArray();
    }
    [Obsolete("This namespace is only designed for ModLoaderPro compatibility pack, DO NOT USE", true)]
    public static void AddAction(this PlayMakerFSM fsm, string stateName, FsmStateAction action) => fsm.GetState(stateName).AddAction(action);
    [Obsolete("This namespace is only designed for ModLoaderPro compatibility pack, DO NOT USE", true)]
    public static void ReplaceAction(this FsmState state, int actionIndex, FsmStateAction action)
    {
        List<FsmStateAction> actions = state.Actions.ToList();
        if (actionIndex < 0 && actionIndex >= actions.Count) throw new ArgumentOutOfRangeException();
        actions[actionIndex] = action;
        state.Actions = actions.ToArray();
    }
    [Obsolete("This namespace is only designed for ModLoaderPro compatibility pack, DO NOT USE", true)]
    public static void ReplaceAction(this PlayMakerFSM fsm, string stateName, int actionIndex, FsmStateAction action) => fsm.GetState(stateName).ReplaceAction(actionIndex, action);
    [Obsolete("This namespace is only designed for ModLoaderPro compatibility pack, DO NOT USE", true)]
    public static void RemoveAction(this FsmState state, int actionIndex)
    {
        List<FsmStateAction> actions = state.Actions.ToList();
        if (actionIndex < 0 && actionIndex >= actions.Count) throw new ArgumentOutOfRangeException();
        actions.RemoveAt(actionIndex);
        state.Actions = actions.ToArray();
    }
    [Obsolete("This namespace is only designed for ModLoaderPro compatibility pack, DO NOT USE", true)]
    public static void RemoveAction(this PlayMakerFSM fsm, string stateName, int actionIndex) => fsm.GetState(stateName).RemoveAction(actionIndex);
    [Obsolete("This namespace is only designed for ModLoaderPro compatibility pack, DO NOT USE", true)]
    public static T GetVariable<T>(this PlayMakerFSM fsm, string name) where T : NamedVariable => fsm.FsmVariables.FindVariable<T>(name);
    [Obsolete("This namespace is only designed for ModLoaderPro compatibility pack, DO NOT USE", true)]
    public static T GetGlobalVariable<T>(string name) where T : NamedVariable => FsmVariables.GlobalVariables.FindVariable<T>(name);
    [Obsolete("This namespace is only designed for ModLoaderPro compatibility pack, DO NOT USE", true)]
    public static T FindVariable<T>(this FsmVariables variables, string name) where T : NamedVariable
    {
        switch (typeof(T))
        {
            case Type _ when typeof(T) == typeof(FsmFloat): return variables.FindFsmFloat(name) as T;
            case Type _ when typeof(T) == typeof(FsmInt): return variables.FindFsmInt(name) as T;
            case Type _ when typeof(T) == typeof(FsmBool): return variables.FindFsmBool(name) as T;
            case Type _ when typeof(T) == typeof(FsmString): return variables.FindFsmString(name) as T;
            case Type _ when typeof(T) == typeof(FsmVector2): return variables.FindFsmVector2(name) as T;
            case Type _ when typeof(T) == typeof(FsmVector3): return variables.FindFsmVector3(name) as T;
            case Type _ when typeof(T) == typeof(FsmRect): return variables.FindFsmRect(name) as T;
            case Type _ when typeof(T) == typeof(FsmQuaternion): return variables.FindFsmQuaternion(name) as T;
            case Type _ when typeof(T) == typeof(FsmColor): return variables.FindFsmColor(name) as T;
            case Type _ when typeof(T) == typeof(FsmGameObject): return variables.FindFsmGameObject(name) as T;
            case Type _ when typeof(T) == typeof(FsmMaterial): return variables.FindFsmMaterial(name) as T;
            case Type _ when typeof(T) == typeof(FsmTexture): return variables.FindFsmTexture(name) as T;
            case Type _ when typeof(T) == typeof(FsmObject): return variables.FindFsmObject(name) as T;
            default: return null;
        }
    }
    [Obsolete("This namespace is only designed for ModLoaderPro compatibility pack, DO NOT USE", true)]
    public static void Initialize(this PlayMakerFSM fsm) => fsm.Fsm.InitData();
}

#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
#endif