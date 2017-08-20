using MSCLoader;

namespace ExampleMod
{
    public class ExampleCommand : ConsoleCommand
	{
		// What the player has to type into the console to execute your commnad
		public override string Name => "example";

		// The help that's displayed for your command when typing help
		public override string Help => "Just type the command";

		// The function that's called when the command is executed
		public override void Run(string[] args)
		{
			ModConsole.Print("This command works!!!");
		}
	}
}
