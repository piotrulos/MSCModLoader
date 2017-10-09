using UnityEngine;
using UnityEngine.UI;

namespace MSCLoader
{
    #pragma warning disable CS1591
    public class ConsoleView : MonoBehaviour
    {

        public ConsoleController console;

        public GameObject viewContainer; //Container for console view, should be a child of this GameObject
        public Text logTextArea;
        public InputField inputField;

        void Start()
        {
            if (console != null)
            {
                console.visibilityChanged += onVisibilityChanged;
                console.logChanged += onLogChanged;
            }
            updateLogStr(console.log);
        }

        ~ConsoleView()
        {
            console.visibilityChanged -= onVisibilityChanged;
            console.logChanged -= onLogChanged;
        }

        public void toggleVisibility()
        {
            if(viewContainer.activeSelf)
            {
                viewContainer.transform.GetChild(4).gameObject.GetComponent<ConsoleUIResizer>().SaveConsoleSize();
            }
            setVisibility(!viewContainer.activeSelf);
        }

        public void setVisibility(bool visible)
        {
            viewContainer.SetActive(visible);
        }

        void onVisibilityChanged(bool visible)
        {
            setVisibility(visible);
        }

        void onLogChanged(string[] newLog)
        {
            updateLogStr(newLog);
        }

        void updateLogStr(string[] newLog)
        {
            if (newLog == null)
            {
                logTextArea.text = "";
            }
            else
            {
                logTextArea.text = string.Join("\n", newLog);
            }
        }

        // Event that should be called by anything wanting to submit the current input to the console.
        public void runCommand()
        {
            console.runCommandString(inputField.text);
            inputField.text = "";
        }

    }
    #pragma warning restore CS1591
}