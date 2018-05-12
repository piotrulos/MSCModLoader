using MSCLoader;

namespace $rootnamespace$
{
    public class $safeitemname$ : ConsoleCommand
    {
      	// What the player has to type into the console to execute your commnad
    	public override string Name => "$safeitemname$";

        // The help that's displayed for your command when typing help
        public override string Help => "Command Description";

        // The function that's called when executing command
        public override void Run(string[] args)
        {
            //Do something when command is executed
        }

    }
}
