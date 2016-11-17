using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
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
