using UnityEngine;

namespace MSCLoader
{
    /// <summary>
    /// Collection of utility functions for interacting with the game.
    /// </summary>
    public class Game : MonoBehaviour {
        void Example() {
            ModConsole.Print(gameObject.name);
        }
    }
}
