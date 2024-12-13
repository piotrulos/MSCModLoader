#if !Mini
using UnityEngine.Events;
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
namespace MSCLoader;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
[System.Obsolete("=> ModUI", true)]
public class ModPrompt
{
    [System.Obsolete("=> ModUI.ShowYesNoMessage", true)]
    public static ModPrompt CreateYesNoPrompt(string message, string title, UnityAction onYes, UnityAction onNo = null, UnityAction onPromptClose = null)
    {
        ModUI.ShowYesNoMessage(message, title, delegate { onYes?.Invoke(); });
        return null;
    }
}
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
#endif