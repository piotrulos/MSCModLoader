using MSCLoader;
using UnityEngine;

namespace TeleportPlugin
{
	public class TeleportPlugin : Mod
	{
		public override string ID { get { return "Teleport Plugin"; } }
		public override string Name { get { return "Teleport Plugin"; } }
		public override string Author { get { return "Djoe45"; } }
		public override string Version { get { return "1.0.0"; } }

        //KeyBind
        private Keybind tpToCar = new Keybind("Key1", "Teleport To Car", KeyCode.Alpha1, KeyCode.LeftControl);
        private Keybind tpCarToMe = new Keybind("Key2", "Teleport Car Me", KeyCode.Alpha2, KeyCode.LeftControl);
        private Keybind tpToMuscle = new Keybind("Key3", "Teleport To Muscle Car", KeyCode.Alpha3, KeyCode.LeftControl);
        private Keybind tpMuscleToMe = new Keybind("Key4", "Teleport Muscle Car To Me", KeyCode.Alpha4, KeyCode.LeftControl);
        private Keybind tpToTruck = new Keybind("Key5", "Teleport To Truck", KeyCode.Alpha5, KeyCode.LeftControl);
        private Keybind tpTruckToMe = new Keybind("Key6", "Teleport Truck To Me", KeyCode.Alpha6, KeyCode.LeftControl);
        private Keybind tpToTractor = new Keybind("Key7", "Teleport To Tractor", KeyCode.Alpha7, KeyCode.LeftControl);
        private Keybind tpTractorToMe = new Keybind("Key8", "Teleport Tractor To Me", KeyCode.Alpha8, KeyCode.LeftControl);
        private Keybind tpToVan = new Keybind("Key9", "Teleport To Van", KeyCode.Alpha9, KeyCode.LeftControl);
        private Keybind tpVanToMe = new Keybind("Key10", "Teleport Van To Me", KeyCode.Alpha0, KeyCode.LeftControl);

        private Keybind tpToHome = new Keybind("Key11", "Teleport To Home", KeyCode.Alpha1, KeyCode.RightControl);
        private Keybind tpToStore = new Keybind("Key12", "Teleport To Store", KeyCode.Alpha2, KeyCode.RightControl);
        private Keybind tpToRepair = new Keybind("Key13", "Teleport To Repair", KeyCode.Alpha3, KeyCode.RightControl);
        private Keybind tpToDrag = new Keybind("Key14", "Teleport To Drag", KeyCode.Alpha4, KeyCode.RightControl);

        public override void OnLoad()
		{
			Keybind.Add(this, tpToCar);
            Keybind.Add(this, tpCarToMe);
            Keybind.Add(this, tpToMuscle);
            Keybind.Add(this, tpMuscleToMe);
            Keybind.Add(this, tpToTruck);
            Keybind.Add(this, tpTruckToMe);
            Keybind.Add(this, tpToTractor);
            Keybind.Add(this, tpTractorToMe);
            Keybind.Add(this, tpToVan);
            Keybind.Add(this, tpVanToMe);

            Keybind.Add(this, tpToHome);
            Keybind.Add(this, tpToStore);
            Keybind.Add(this, tpToRepair);
            Keybind.Add(this, tpToDrag);

            ModConsole.Print("Teleport Plugin has been loaded!");
        }

		public override void Update()
		{
            if (tpToCar.IsDown()) { TpTo("PLAYER", "SATSUMA(557kg)"); }
            if (tpCarToMe.IsDown()) { TpMe("SATSUMA(557kg)", "PLAYER"); }
            if (tpToMuscle.IsDown()) { TpTo("PLAYER", "FERNDALE"); }
            if (tpMuscleToMe.IsDown()) { TpMe("FERNDALE", "PLAYER"); }
            if (tpToTruck.IsDown()) { TpTo("PLAYER", "GIFU(750/450psi)"); }
            if (tpTruckToMe.IsDown()) { TpMe("GIFU(750/450psi)", "PLAYER"); }
            if (tpToTractor.IsDown()) { TpTo("PLAYER", "KEKMET(350-400psi)"); }
            if (tpTractorToMe.IsDown()) { TpTo("KEKMET(350-400psi)", "PLAYER"); }
            if (tpToVan.IsDown()) { TpTo("PLAYER", "HAYOSIKO(1500kg)"); }
            if (tpVanToMe.IsDown()) { TpTo("HAYOSIKO(1500kg)", "PLAYER"); }

            if (tpToHome.IsDown()) { TpTo("PLAYER", "GraveYardSpawn"); }
            if (tpToStore.IsDown()) { TpTo("PLAYER", "SpawnToStore"); }
            if (tpToRepair.IsDown()) { TpTo("PLAYER", "SpawnToRepair"); }
            if (tpToDrag.IsDown()) { TpTo("PLAYER", "SpawnToDrag"); }
        }

        private void TpTo(string tpObject, string tptoObject)
        {
            ModConsole.Print("Teleportation to:" + tptoObject);
            var posFinder = GameObject.Find(tptoObject);
            Vector3 newPlayerPos = new Vector3(posFinder.transform.position.x + 3, posFinder.transform.position.y, posFinder.transform.position.z);
            GameObject.Find(tpObject).transform.position = newPlayerPos;
        }

        private void TpMe(string tpObject, string tptoObject)
        {
            ModConsole.Print("Teleporte me to:" + tptoObject);
            var posFinder = GameObject.Find(tptoObject);
            Vector3 newCarPos = new Vector3(posFinder.transform.position.x + 3, posFinder.transform.position.y, posFinder.transform.position.z);
            GameObject.Find(tpObject).transform.position = newCarPos;
        }
    }
}
