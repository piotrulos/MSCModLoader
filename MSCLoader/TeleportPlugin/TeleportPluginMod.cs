using MSCLoader;
using UnityEngine;

namespace TeleportPlugin
{
	public class TeleportPlugin : Mod
	{
		// The ID of the mod - Should be unique
		public override string ID { get { return "Teleport Plugin"; } }

		// The name of the mod that is displayed
		public override string Name { get { return "Teleport Plugin"; } }
		
		// The name of the author
		public override string Author { get { return "Djoe45"; } }

		// The version of the mod
		public override string Version { get { return "1.0.0"; } }

        // Keybinds
        private Keybind tptocar = new Keybind("KeyID", "Teleport To Car", KeyCode.F2, KeyCode.LeftControl);
        private Keybind tptomuscle = new Keybind("KeyID", "Teleport To Muscle", KeyCode.F3, KeyCode.LeftControl);    
        private Keybind tptotruck = new Keybind("KeyID", "Teleport To Truck", KeyCode.F4, KeyCode.LeftControl);
        private Keybind tptorepairshop = new Keybind("KeyID", "Teleport To Repair Shop", KeyCode.F5, KeyCode.LeftControl);
        private Keybind tptohome = new Keybind("KeyID", "Teleport To Home", KeyCode.F6, KeyCode.LeftControl);
        private Keybind tptostore = new Keybind("KeyID", "Teleport To Store", KeyCode.F7, KeyCode.LeftControl);

        private Keybind tptosatsuna = new Keybind("KeyID", "Teleport My Car To Me", KeyCode.F8, KeyCode.LeftControl);

        // Called when the mod is loaded
        public override void OnLoad()
		{
			// Do your initialization here

			Keybind.Add(this, tptocar);
            Keybind.Add(this, tptomuscle);
            Keybind.Add(this, tptotruck);
            Keybind.Add(this, tptorepairshop);
            Keybind.Add(this, tptohome);
            Keybind.Add(this, tptostore);
            Keybind.Add(this, tptosatsuna);
            ConsoleCommand.Add(new TeleportPluginCommand());
            ModConsole.Print("First Plugin has been loaded!");
        }

		// Called to draw the GUI
		public override void OnGUI()
		{
			// Draw your GUI here
		}

		// Called every tick
		public override void Update()
		{
			// Do your updating here

			if (tptocar.IsDown()){ Tptocar(); }
            if (tptomuscle.IsDown()) { Tptomuscle(); }
            if (tptotruck.IsDown()) { Tptotruck(); }
            if (tptorepairshop.IsDown()) { Tptorepairshop(); }
            if (tptohome.IsDown()) { Tptohome(); }
            if (tptostore.IsDown()) { Tptostore(); }
            if (tptosatsuna.IsDown()) { Tptosatsuma(); }
        }

        public void Tptocar()
        {
            ModConsole.Print("Teleportation to my car!");
            GameObject Player = GameObject.Find("PLAYER");
            Vector3 oldPlayerPos = new Vector3(Player.transform.position.x, Player.transform.position.y, Player.transform.position.z);
            GameObject posFinder = GameObject.Find("SATSUMA(557kg)");
            Vector3 newPlayerPos = new Vector3(posFinder.transform.position.x +2, posFinder.transform.position.y, posFinder.transform.position.z);
            Player.transform.position = newPlayerPos;
        }

        public void Tptomuscle()
        {
            ModConsole.Print("Teleportation to muscle car!");
            GameObject Player = GameObject.Find("PLAYER");
            Vector3 oldPlayerPos = new Vector3(Player.transform.position.x, Player.transform.position.y, Player.transform.position.z);
            GameObject posFinder = GameObject.Find("FERNDALE");
            Vector3 newPlayerPos = new Vector3(posFinder.transform.position.x +2 , posFinder.transform.position.y, posFinder.transform.position.z);
            Player.transform.position = newPlayerPos;
        }

        public void Tptotruck()
        {
            ModConsole.Print("Teleportation to truck!");
            GameObject Player = GameObject.Find("PLAYER");
            Vector3 oldPlayerPos = new Vector3(Player.transform.position.x, Player.transform.position.y, Player.transform.position.z);
            GameObject posFinder = GameObject.Find("GIFU(750/450psi)");
            Vector3 newPlayerPos = new Vector3(posFinder.transform.position.x +3, posFinder.transform.position.y+3, posFinder.transform.position.z);
            Player.transform.position = newPlayerPos;
        }

        public void Tptorepairshop()
        {
            ModConsole.Print("Teleportation to Repair Shop!");
            GameObject Player = GameObject.Find("PLAYER");
            Vector3 oldPlayerPos = new Vector3(Player.transform.position.x, Player.transform.position.y, Player.transform.position.z);
            GameObject posFinder = GameObject.Find("repair_shop_walls");
            Vector3 newPlayerPos = new Vector3(posFinder.transform.position.x - 11, posFinder.transform.position.y + 1, posFinder.transform.position.z);
            Player.transform.position = newPlayerPos;
        }

        public void Tptohome()
        {
            ModConsole.Print("Teleportation to home");
            GameObject Player = GameObject.Find("PLAYER");
            Vector3 oldPlayerPos = new Vector3(Player.transform.position.x, Player.transform.position.y, Player.transform.position.z);
            GameObject posFinder = GameObject.Find("GraveYardSpawn");
            Vector3 newPlayerPos = new Vector3(posFinder.transform.position.x, posFinder.transform.position.y + 1, posFinder.transform.position.z);
            Player.transform.position = newPlayerPos;
        }

        public void Tptostore()
        {
            ModConsole.Print("Teleportation to store");
            GameObject Player = GameObject.Find("PLAYER");
            Vector3 oldPlayerPos = new Vector3(Player.transform.position.x, Player.transform.position.y, Player.transform.position.z);
            GameObject posFinder = GameObject.Find("SpawnToStore");
            Vector3 newPlayerPos = new Vector3(posFinder.transform.position.x, posFinder.transform.position.y +1, posFinder.transform.position.z);
            Player.transform.position = newPlayerPos;
        }

        public void Tptosatsuma()
        {
            ModConsole.Print("Teleportation my car to me!");
            GameObject Car = GameObject.Find("SATSUMA(557kg)");
            Vector3 oldCarPos = new Vector3(Car.transform.position.x, Car.transform.position.y, Car.transform.position.z);
            GameObject posFinder = GameObject.Find("PLAYER");
            Vector3 newCarPos = new Vector3(posFinder.transform.position.x + 2, posFinder.transform.position.y, posFinder.transform.position.z);
            Car.transform.position = newCarPos;
        }
    }
}
