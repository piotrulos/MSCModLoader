using MSCLoader;
using UnityEngine;

namespace MSCNeon
{
    public class ExampleMod : Mod
    {
        public override string ID { get { return "MSCNeon"; } }
        public override string Name { get { return "MSC Neon Plugin"; } }
        public override string Author { get { return "Djoe45"; } }
        public override string Version { get { return "1.0.0"; } }

        private Keybind redKey = new Keybind("KeyID", "Red Neon", KeyCode.Keypad1, KeyCode.LeftControl);
        private Keybind blueKey = new Keybind("KeyID", "Blue Neon", KeyCode.Keypad2, KeyCode.LeftControl);
        private Keybind greenKey = new Keybind("KeyID", "Green Neon", KeyCode.Keypad3, KeyCode.LeftControl);
        private Keybind cyanKey = new Keybind("KeyID", "Cyan Neon", KeyCode.Keypad4, KeyCode.LeftControl);
        private Keybind magentaKey = new Keybind("KeyID", "Green Neon", KeyCode.Keypad5, KeyCode.LeftControl);
        private Keybind yellowKey = new Keybind("KeyID", "Yellow Neon", KeyCode.Keypad6, KeyCode.LeftControl);
        private Keybind orangeKey = new Keybind("KeyID", "Orange Neon", KeyCode.Keypad7, KeyCode.LeftControl);

        //Variables
        private bool neon = false;
        private Light neonlight;
        private GameObject neoncar;

        public override void OnLoad()
        {
            Keybind.Add(this, redKey);
            Keybind.Add(this, blueKey);
            Keybind.Add(this, greenKey);
            Keybind.Add(this, cyanKey);           
            Keybind.Add(this, magentaKey);
            Keybind.Add(this, yellowKey);
            Keybind.Add(this, orangeKey);

            ModConsole.Print("MSC Neon Plugin has been loaded!");
        }

        public override void Update()
        {
            if (redKey.IsDown()) { carneon(Color.red); };
            if (blueKey.IsDown()) { carneon(Color.blue); };
            if (greenKey.IsDown()) { carneon(Color.green); };
            if (cyanKey.IsDown()) { carneon(Color.cyan); };
            if (magentaKey.IsDown()) { carneon(Color.magenta); };
            if (yellowKey.IsDown()) { carneon(Color.yellow); };
            if (orangeKey.IsDown()) { carneon(new Color32(255, 128, 0, 255)); };
        }

        private void carneon(Color color)
        {
            if (!neon)
            {
                ModConsole.Print("ADD Neon on the Car !");
                GameObject Car = GameObject.Find("SATSUMA(557kg)");
                neoncar = new GameObject();
                neonlight = neoncar.AddComponent<Light>();
                neonlight.transform.SetParent(Car.transform);
                neonlight.transform.position = new Vector3(Car.transform.position.x, Car.transform.position.y - 0.7f, Car.transform.position.z);
                neonlight.transform.Rotate(90, 0, 0);
                neonlight.color = color;
                neonlight.range = 10f;
                neonlight.intensity = 3;
                neonlight.renderMode = LightRenderMode.ForcePixel;
                neon = true;
            }
            else
            {
                GameObject.Destroy(neoncar);
                neon = false;
            }
        }
    }
}
